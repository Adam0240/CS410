using ConsoleApp_121_FinalProjectShell;
using ConsoleApp_121_FinalProjectShell.Core;
using Xunit;

public class RoomTextServiceTests
{
    [Fact]
    public void GetDescription_ReturnsBaseDescription_WhenNoProgressApplies()
    {
        var room = new Room("Old forge", 4);
        var progress = new GameProgress();

        var description = RoomTextService.GetDescription(room, progress);

        Assert.Equal("Old forge", description);
    }

    [Fact]
    public void GetDescription_Changes_WhenForgePrepared()
    {
        var room = new Room("Old forge", 4);
        var progress = new GameProgress
        {
            ForgePrepared = true
        };

        var description = RoomTextService.GetDescription(room, progress);

        Assert.Contains("forge", description, System.StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void GetDescription_Changes_WhenGateOpen()
    {
        var room = new Room("Castle Gate", 6);
        var progress = new GameProgress
        {
            GateOpen = true
        };

        var description = RoomTextService.GetDescription(room, progress);

        Assert.Contains("hole", description, System.StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void GetDescription_Changes_WhenSwampCleared()
    {
        var room = new Room("Swamp", 1);
        var progress = new GameProgress
        {
            SwampCleared = true
        };

        var description = RoomTextService.GetDescription(room, progress);

        Assert.Contains("hidden path", description, System.StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void GetDescription_Changes_WhenSwordPlaced()
    {
        var room = new Room("Grove", 8);
        var progress = new GameProgress
        {
            SwordPlaced = true
        };

        var description = RoomTextService.GetDescription(room, progress);

        Assert.Contains("altar", description, System.StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void GetExitString_HidesGrove_WhenSwampNotCleared()
    {
        var room = new Room("Swamp", 1);
        room.setExit("north", new Room("North", 2));
        room.setExit("grove", new Room("Grove", 8));

        var progress = new GameProgress
        {
            SwampCleared = false
        };

        var exitString = RoomTextService.GetExitString(room, progress);

        Assert.Contains("north", exitString);
        Assert.DoesNotContain("grove", exitString);
    }

    [Fact]
    public void GetExitString_ShowsGrove_WhenSwampCleared()
    {
        var room = new Room("Swamp", 1);
        room.setExit("north", new Room("North", 2));
        room.setExit("grove", new Room("Grove", 8));

        var progress = new GameProgress
        {
            SwampCleared = true
        };

        var exitString = RoomTextService.GetExitString(room, progress);

        Assert.Contains("north", exitString);
        Assert.Contains("grove", exitString);
    }
}
