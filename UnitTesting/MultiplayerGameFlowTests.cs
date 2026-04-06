// Multiplayer summary: verifies shared-world sync, presence text, and snapshot initialization for the multiplayer MVP.
using ConsoleApp_121_FinalProjectShell.Commands;
using ConsoleApp_121_FinalProjectShell;
using ConsoleApp_121_FinalProjectShell.Core;
using Xunit;

namespace UnitTesting;

public class MultiplayerGameFlowTests
{
    [Fact]
    public void PresenceLineAppearsWhenPlayersShareRoom()
    {
        Game game = new(true);
        game.ConfigureAsHost();
        game.SetPlayerConnected(2, true);
        game.GetPlayer(2).setCurrentRoom(game.GetPlayer(1).GetCurrentRoom());

        string roomText = game.GetLocationInfoText(1);

        Assert.Contains("Player 2 is here as well.", roomText);
    }

    [Fact]
    public void PresenceLineDoesNotAppearWhenPlayersAreInDifferentRooms()
    {
        Game game = new(true);
        game.ConfigureAsHost();
        game.SetPlayerConnected(2, true);
        game.GetPlayer(2).setCurrentRoom(game.GetAllRooms().Single(room => room.GetId() == 5));

        string roomText = game.GetLocationInfoText(1);

        Assert.DoesNotContain("Player 2 is here as well.", roomText);
    }

    [Fact]
    public void PlayerOneTakeRemovesItemFromPlayerTwoView()
    {
        Game game = new(true);
        game.ConfigureAsHost();
        game.SetPlayerConnected(2, true);

        Room battleground = game.GetAllRooms().Single(room => room.GetId() == 2);
        game.GetPlayer(1).setCurrentRoom(battleground);
        game.GetPlayer(2).setCurrentRoom(battleground);

        game.ExecuteAuthoritativeCommand(1, new Command(CommandWord.TAKE, "axe"));

        Assert.False(battleground.hasItemByName("axe"));
        Assert.DoesNotContain("AXE", game.GetLocationInfoText(2));
    }

    [Fact]
    public void PlayerOneDropAddsItemToPlayerTwoView()
    {
        Game game = new(true);
        game.ConfigureAsHost();
        game.SetPlayerConnected(2, true);

        Room battleground = game.GetAllRooms().Single(room => room.GetId() == 2);
        game.GetPlayer(1).setCurrentRoom(battleground);
        game.GetPlayer(2).setCurrentRoom(battleground);
        game.ExecuteAuthoritativeCommand(1, new Command(CommandWord.TAKE, "axe"));

        game.ExecuteAuthoritativeCommand(1, new Command(CommandWord.DROP, "axe"));

        Assert.True(battleground.hasItemByName("axe"));
        Assert.Contains("AXE", game.GetLocationInfoText(2));
    }

    [Fact]
    public void UsingItemThatChangesWorldUpdatesOtherPlayerView()
    {
        Game game = new(true);
        game.ConfigureAsHost();
        game.SetPlayerConnected(2, true);

        Room swamp = game.GetAllRooms().Single(room => room.GetId() == 1);
        Room battleground = game.GetAllRooms().Single(room => room.GetId() == 2);
        game.GetPlayer(1).setCurrentRoom(battleground);
        game.ExecuteAuthoritativeCommand(1, new Command(CommandWord.TAKE, "axe"));
        game.GetPlayer(1).setCurrentRoom(swamp);
        game.GetPlayer(2).setCurrentRoom(swamp);

        game.ExecuteAuthoritativeCommand(1, new Command(CommandWord.USE, "axe"));

        Assert.True(game.GetProgress().SwampCleared);
        Assert.Contains("hidden path", game.GetLocationInfoText(2), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void InvalidMovementStillReturnsNoPath()
    {
        Game game = new(true);
        game.ConfigureAsHost();
        game.SetPlayerConnected(2, true);

        CommandExecutionResult result = game.ExecuteAuthoritativeCommand(2, new Command(CommandWord.GO, "west"));

        Assert.Contains("There is no path!", result.OutputText);
    }

    [Fact]
    public void InvalidItemActionStillReturnsError()
    {
        Game game = new(true);
        game.ConfigureAsHost();
        game.SetPlayerConnected(2, true);

        CommandExecutionResult result = game.ExecuteAuthoritativeCommand(2, new Command(CommandWord.TAKE, "axe"));

        Assert.Contains("There isn't anything like that around.", result.OutputText);
    }

    [Fact]
    public void HostAuthoritativeTakeDoesNotDuplicateUniqueItem()
    {
        Game game = new(true);
        game.ConfigureAsHost();
        game.SetPlayerConnected(2, true);

        Room battleground = game.GetAllRooms().Single(room => room.GetId() == 2);
        game.GetPlayer(1).setCurrentRoom(battleground);
        game.GetPlayer(2).setCurrentRoom(battleground);

        game.ExecuteAuthoritativeCommand(1, new Command(CommandWord.TAKE, "axe"));
        CommandExecutionResult result = game.ExecuteAuthoritativeCommand(2, new Command(CommandWord.TAKE, "axe"));

        Assert.True(game.GetPlayer(1).hasItemByName("axe"));
        Assert.False(game.GetPlayer(2).hasItemByName("axe"));
        Assert.Contains("There isn't anything like that around.", result.OutputText);
    }

    [Fact]
    public void JoiningClientCanBeInitializedFromHostSnapshot()
    {
        Game hostGame = new(true);
        hostGame.ConfigureAsHost();
        hostGame.SetPlayerConnected(2, true);

        Room battleground = hostGame.GetAllRooms().Single(room => room.GetId() == 2);
        hostGame.GetPlayer(1).setCurrentRoom(battleground);
        hostGame.ExecuteAuthoritativeCommand(1, new Command(CommandWord.TAKE, "axe"));
        hostGame.GetPlayer(2).setCurrentRoom(hostGame.GetAllRooms().Single(room => room.GetId() == 5));

        MultiplayerSessionState snapshot = hostGame.CaptureMultiplayerState();

        Game clientGame = new(true);
        clientGame.ConfigureAsClient();
        clientGame.ApplyMultiplayerState(snapshot);

        Assert.True(clientGame.GetPlayer(1).hasItemByName("axe"));
        Assert.Equal(5, clientGame.GetPlayer(2).GetCurrentRoom().GetId());
        Assert.False(clientGame.GetAllRooms().Single(room => room.GetId() == 2).hasItemByName("axe"));
    }
}
