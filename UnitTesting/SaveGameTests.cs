using System;
using System.IO;
using System.Linq;
using ConsoleApp_121_FinalProjectShell;
using ConsoleApp_121_FinalProjectShell.Commands;
using ConsoleApp_121_FinalProjectShell.Core;
using ConsoleApp_121_FinalProjectShell.Core.Persistence;
using Xunit;

namespace UnitTesting;

public class SaveGameTests
{
    // Tests that the save command captures the current game state and writes it to the repository.
    [Fact]
    public void SaveCommand_PersistsCurrentGameState()
    {
        InMemoryGameSaveRepository repository = new();
        Game game = new(false, repository);

        Room rocky = game.allRooms.Single(room => room.GetId() == 3);
        Room lava = game.allRooms.Single(room => room.GetId() == 4);

        game.GetPlayer().setCurrentRoom(rocky);
        game.HammerUse();
        game.Take(new Command(CommandWord.TAKE, "ore"));
        game.GetPlayer().setCurrentRoom(lava);
        game.HammerUse();
        game.GetProgress().GateOpen = true;
        game.GetProgress().ToldProtagGate = true;

        StringWriter output = new();
        Console.SetOut(output);

        game.ProcessCommand(new Command(CommandWord.SAVE, null));

        GameSaveState? savedState = GameSaveSerializer.FromJson(repository.StoredJson!);

        Assert.True(repository.InitializeCalled);
        Assert.NotNull(repository.StoredJson);
        Assert.NotNull(savedState);
        Assert.Equal(4, savedState!.PlayerRoomId);
        Assert.Contains("ore", savedState.PlayerInventory);
        Assert.True(savedState.ForgePrepared);
        Assert.True(savedState.GateOpen);
        Assert.True(savedState.ToldProtagGate);
        Assert.Equal("Game saved.\n", NormalizeOutput(output));
    }

    // Tests that the load command restores player position, inventories, and progress flags from saved data.
    [Fact]
    public void LoadCommand_RestoresSavedGameState()
    {
        InMemoryGameSaveRepository repository = new();
        Game game = new(false, repository);

        repository.StoredJson = GameSaveSerializer.ToJson(new GameSaveState
        {
            PlayerRoomId = 4,
            PlayerInventory = ["ore"],
            PlayerCarryWeight = 100,
            ProtagonistRoomId = 5,
            ProtagonistStepCounter = 3,
            FollowerRoomId = 6,
            FollowerIsFollowing = false,
            FollowerInventory = ["ring"],
            SwampCleared = true,
            ForgePrepared = true,
            SwordPlaced = false,
            GateOpen = true,
            ToldProtagGate = true,
            ToldProtagSword = false,
            RoomItems =
            {
                [2] = ["axe"],
                [8] = ["hilt"]
            }
        });

        game.GetPlayer().setCurrentRoom(game.allRooms.Single(room => room.GetId() == 0));
        game.GetProgress().GateOpen = false;
        game.GetProgress().ForgePrepared = false;
        game.GetPlayer().ClearInventory();
        game.GetFollower().ClearInventory();

        StringWriter output = new();
        Console.SetOut(output);

        game.ProcessCommand(new Command(CommandWord.LOAD, null));

        Assert.Equal(4, game.GetPlayer().GetCurrentRoom().GetId());
        Assert.True(game.GetPlayer().hasItemByName("ore"));
        Assert.True(game.GetFollower().hasItemByName("ring"));
        Assert.True(game.GetProgress().GateOpen);
        Assert.True(game.GetProgress().ForgePrepared);
        Assert.False(game.GetFollower().IsFollowing());
        Assert.Contains("Game loaded.\n", NormalizeOutput(output));
    }

    // Tests that the delete command removes the saved data from the repository and reports success.
    [Fact]
    public void DeleteCommand_RemovesExistingSave()
    {
        InMemoryGameSaveRepository repository = new()
        {
            StoredJson = "{}"
        };
        Game game = new(false, repository);

        StringWriter output = new();
        Console.SetOut(output);

        game.ProcessCommand(new Command(CommandWord.DELETE, null));

        Assert.Null(repository.StoredJson);
        Assert.Equal("Save deleted.\n", NormalizeOutput(output));
    }

    private static string NormalizeOutput(StringWriter writer) =>
        writer.ToString().Replace("\r\n", "\n");

    private sealed class InMemoryGameSaveRepository : IGameSaveRepository
    {
        public bool InitializeCalled { get; private set; }
        public string? StoredJson { get; set; }

        public void Initialize()
        {
            InitializeCalled = true;
        }

        public void SaveJson(string saveJson)
        {
            StoredJson = saveJson;
        }

        public string? LoadJson()
        {
            return StoredJson;
        }

        public bool DeleteSave()
        {
            bool hadSave = StoredJson is not null;
            StoredJson = null;
            return hadSave;
        }
    }
}
