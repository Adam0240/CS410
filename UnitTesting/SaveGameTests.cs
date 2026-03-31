using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bogus;
using Moq;
using Xunit;

using ConsoleApp_121_FinalProjectShell;
using ConsoleApp_121_FinalProjectShell.Commands;
using ConsoleApp_121_FinalProjectShell.Core;
using ConsoleApp_121_FinalProjectShell.Core.Persistence;

namespace UnitTesting;

public class SaveGameTests
{
    // Tests that Game constructor calls Initialize() on the repository once.
    [Fact]
    public void Constructor_CallsInitialize_Once()
    {
        var repoMock = new Mock<IGameSaveRepository>();

        Game game = new(false, repoMock.Object);

        repoMock.Verify(r => r.Initialize(), Times.Once);
    }

    // Tests that the save command captures current game state and writes JSON to the repository.
    [Fact]
    public void SaveCommand_PersistsCurrentGameState()
    {
        var repoMock = new Mock<IGameSaveRepository>();
        string? savedJson = null;

        repoMock
            .Setup(r => r.SaveJson(It.IsAny<string>()))
            .Callback<string>(json => savedJson = json);

        Game game = new(false, repoMock.Object);

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

        GameSaveState? savedState = savedJson is null
            ? null
            : GameSaveSerializer.FromJson(savedJson);

        repoMock.Verify(r => r.Initialize(), Times.Once);
        repoMock.Verify(r => r.SaveJson(It.IsAny<string>()), Times.Once);

        Assert.NotNull(savedJson);
        Assert.NotNull(savedState);
        Assert.Equal(4, savedState!.PlayerRoomId);
        Assert.Contains("ore", savedState.PlayerInventory);
        Assert.True(savedState.ForgePrepared);
        Assert.True(savedState.GateOpen);
        Assert.True(savedState.ToldProtagGate);
        Assert.Equal("Game saved.\n", NormalizeOutput(output));
    }

    // Tests that the load command asks the repository for saved JSON once.
    [Fact]
    public void LoadCommand_CallsLoadJson_Once()
    {
        var repoMock = new Mock<IGameSaveRepository>();
        repoMock.Setup(r => r.LoadJson()).Returns((string?)null);

        Game game = new(false, repoMock.Object);

        game.ProcessCommand(new Command(CommandWord.LOAD, null));

        repoMock.Verify(r => r.LoadJson(), Times.Once);
    }

    // Tests that the load command restores player position, inventories, and progress flags from saved data.
    [Fact]
    public void LoadCommand_RestoresSavedGameState()
    {
        var repoMock = new Mock<IGameSaveRepository>();

        repoMock.Setup(r => r.LoadJson())
            .Returns(GameSaveSerializer.ToJson(new GameSaveState
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
                RoomItems = new Dictionary<int, List<string>>
                {
                    [2] = new() { "axe" },
                    [8] = new() { "hilt" }
                }
            }));

        Game game = new(false, repoMock.Object);

        game.GetPlayer().setCurrentRoom(game.allRooms.Single(room => room.GetId() == 0));
        game.GetProgress().GateOpen = false;
        game.GetProgress().ForgePrepared = false;
        game.GetPlayer().ClearInventory();
        game.GetFollower().ClearInventory();

        StringWriter output = new();
        Console.SetOut(output);

        game.ProcessCommand(new Command(CommandWord.LOAD, null));

        repoMock.Verify(r => r.LoadJson(), Times.Once);

        Assert.Equal(4, game.GetPlayer().GetCurrentRoom().GetId());
        Assert.True(game.GetPlayer().hasItemByName("ore"));
        Assert.True(game.GetFollower().hasItemByName("ring"));
        Assert.True(game.GetProgress().GateOpen);
        Assert.True(game.GetProgress().ForgePrepared);
        Assert.False(game.GetFollower().IsFollowing());
        Assert.Contains("Game loaded.\n", NormalizeOutput(output));
    }

    // Tests that delete command asks the repository to delete the save once.
    [Fact]
    public void DeleteCommand_CallsDeleteSave_Once()
    {
        var repoMock = new Mock<IGameSaveRepository>();
        repoMock.Setup(r => r.DeleteSave()).Returns(true);

        Game game = new(false, repoMock.Object);

        StringWriter output = new();
        Console.SetOut(output);

        game.ProcessCommand(new Command(CommandWord.DELETE, null));

        repoMock.Verify(r => r.DeleteSave(), Times.Once);
        Assert.Equal("Save deleted.\n", NormalizeOutput(output));
    }

    // Bogus
    
    [Fact]
    public void LoadCommand_WithGeneratedStates_RestoresKeyInvariants()
    {
        Randomizer.Seed = new Random(410);

        var stateFaker = new Faker<GameSaveState>()
            .RuleFor(s => s.PlayerRoomId, f => f.PickRandom(new[] { 0, 3, 4, 6, 8 }))
            .RuleFor(s => s.PlayerInventory, f => new List<string> { "ore" })
            .RuleFor(s => s.PlayerCarryWeight, _ => 100)
            .RuleFor(s => s.ProtagonistRoomId, _ => 5)
            .RuleFor(s => s.ProtagonistStepCounter, f => f.Random.Int(0, 7))
            .RuleFor(s => s.FollowerRoomId, f => f.PickRandom(new[] { 5, 6 }))
            .RuleFor(s => s.FollowerIsFollowing, f => f.Random.Bool())
            .RuleFor(s => s.FollowerInventory, f => new List<string> { "ring" })
            .RuleFor(s => s.SwampCleared, f => f.Random.Bool())
            .RuleFor(s => s.ForgePrepared, f => f.Random.Bool())
            .RuleFor(s => s.SwordPlaced, f => f.Random.Bool())
            .RuleFor(s => s.GateOpen, f => f.Random.Bool())
            .RuleFor(s => s.ToldProtagGate, f => f.Random.Bool())
            .RuleFor(s => s.ToldProtagSword, f => f.Random.Bool())
            .RuleFor(s => s.RoomItems, _ => new Dictionary<int, List<string>>
            {
                [2] = new() { "axe" },
                [8] = new() { "hilt" }
            });

        for (int i = 0; i < 3; i++)
        {
            GameSaveState generatedState = stateFaker.Generate();

            var repoMock = new Mock<IGameSaveRepository>();
            repoMock.Setup(r => r.LoadJson())
                .Returns(GameSaveSerializer.ToJson(generatedState));

            Game game = new(false, repoMock.Object);

            game.GetPlayer().ClearInventory();
            game.GetFollower().ClearInventory();
            game.GetProgress().GateOpen = false;
            game.GetProgress().ForgePrepared = false;

            game.ProcessCommand(new Command(CommandWord.LOAD, null));

            Assert.Equal(generatedState.PlayerRoomId, game.GetPlayer().GetCurrentRoom().GetId());
            Assert.Equal(generatedState.GateOpen, game.GetProgress().GateOpen);
            Assert.Equal(generatedState.ForgePrepared, game.GetProgress().ForgePrepared);
            Assert.Equal(generatedState.FollowerIsFollowing, game.GetFollower().IsFollowing());

            if (generatedState.PlayerInventory.Contains("ore"))
            {
                Assert.True(game.GetPlayer().hasItemByName("ore"));
            }

            if (generatedState.FollowerInventory.Contains("ring"))
            {
                Assert.True(game.GetFollower().hasItemByName("ring"));
            }
        }
    }

    private static string NormalizeOutput(StringWriter writer) =>
        writer.ToString().Replace("\r\n", "\n");
}