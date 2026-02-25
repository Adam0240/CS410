using ConsoleApp_121_FinalProjectShell;
using ConsoleApp_121_FinalProjectShell.Items;
using System;
using Xunit;

public class RoomTests
{
    // Helper: reset static progression flags so tests don't interfere with each other
    private void ResetClearCons()
    {
        bool[] flags = Room.getClearCons();
        for (int i = 0; i < flags.Length; i++)
        {
            Room.setClearCon(i, false);
        }
    }

    // Helper item for tests (Item is abstract now, so we must create a concrete item via the factory)
    private Item CreateTestItem(string name, string desc, int weight = 1, int id = 3)
    {
        // id just needs to map to a valid item type in your factory.
        // 3 is "ore" in your game mapping, but the actual behavior doesn't matter for Room inventory tests.
        return ItemFactory.Create(name, desc, weight, id);
    }

    [Fact]
    public void Constructor_InitializesRoomCorrectly()
    {
        // Arrange
        var room = new Room("Test room", 99);

        // Act
        var id = room.getID();
        var description = room.getLongDesc();

        // Assert
        Assert.Equal(99, id);
        Assert.Contains("Test room", description);
    }

    [Fact]
    public void SetExit_AddsExitSuccessfully()
    {
        // Arrange
        var roomA = new Room("Room A", 1);
        var roomB = new Room("Room B", 2);

        // Act
        roomA.setExit("north", roomB);

        // Assert
        Assert.True(roomA.getExits().ContainsKey("north"));
        Assert.Equal(roomB, roomA.getExits()["north"]);
    }

    [Fact]
    public void GetExitString_ShowsAllExits_WhenNoRestrictions()
    {
        // Arrange
        ResetClearCons();

        var room = new Room("Test", 1);
        room.setExit("north", new Room("North", 2));
        room.setExit("south", new Room("South", 3));

        // Act
        var exitString = room.getExitString();

        // Assert
        Assert.Contains("north", exitString);
        Assert.Contains("south", exitString);
    }

    [Fact]
    public void GroveExit_Hidden_WhenSwampNotCleared()
    {
        // Arrange
        ResetClearCons();
        Room.setClearCon(0, false); // swampCleared = false

        var room = new Room("Swamp", 1);
        room.setExit("grove", new Room("Grove", 8));

        // Act
        var exitString = room.getExitString();

        // Assert
        Assert.DoesNotContain("grove", exitString);
    }

    [Fact]
    public void GroveExit_Shown_WhenSwampCleared()
    {
        // Arrange
        ResetClearCons();
        Room.setClearCon(0, true); // swampCleared = true

        var room = new Room("Swamp", 1);
        room.setExit("grove", new Room("Grove", 8));

        // Act
        var exitString = room.getExitString();

        // Assert
        Assert.Contains("grove", exitString);
    }

    [Fact]
    public void AddItem_ItemAppearsInRoom()
    {
        // Arrange
        var room = new Room("Test", 1);
        var item = CreateTestItem("Key", "a rusty key");

        // Act
        room.addItem(item);

        // Assert
        // Avoid room.hasItem(item) in case your Room class doesn't support that overload
        Assert.True(room.hasItemByName("key"));
        Assert.NotNull(room.getItemByName("key"));
    }

    [Fact]
    public void RemoveItemByName_RemovesCorrectItem()
    {
        // Arrange
        var room = new Room("Test", 1);
        var item = CreateTestItem("Key", "a rusty key");
        room.addItem(item);

        // Act
        room.removeItemByName("key");

        // Assert
        Assert.False(room.hasItemByName("key"));
        Assert.Null(room.getItemByName("key"));
    }

    [Fact]
    public void GetItemByName_IsCaseInsensitive()
    {
        // Arrange
        var room = new Room("Test", 1);
        var item = CreateTestItem("Sword", "a sharp sword");
        room.addItem(item);

        // Act
        var found = room.getItemByName("sWoRd");

        // Assert
        Assert.NotNull(found);
        Assert.Equal(item, found);
    }

    [Fact]
    public void GetDescription_Changes_WhenForgePrepared()
    {
        // Arrange
        ResetClearCons();
        Room.setClearCon(1, true); // forgePrepared = true

        var room = new Room("Old forge", 4);

        // Act
        var description = room.getDescription();

        // Assert
        Assert.Contains("forge", description, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void GetDescription_Changes_WhenGateOpen()
    {
        // Arrange
        ResetClearCons();
        Room.setClearCon(3, true); // gateOpen = true

        var room = new Room("Castle Gate", 6);

        // Act
        var description = room.getDescription();

        // Assert
        Assert.Contains("hole", description, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void GetClearCons_ReturnsCorrectLength()
    {
        // Act
        var flags = Room.getClearCons();

        // Assert
        Assert.Equal(6, flags.Length);
    }

    [Fact]
    public void SetClearCon_UpdatesCorrectFlag()
    {
        // Arrange
        ResetClearCons();

        // Act
        Room.setClearCon(2, true); // swordPlaced
        var flags = Room.getClearCons();

        // Assert
        Assert.True(flags[2]);
    }

    [Fact]
    public void GetRandomExit_ReturnsValidExit()
    {
        // Arrange
        var room = new Room("Test", 1);
        room.setExit("north", new Room("North", 2));
        room.setExit("south", new Room("South", 3));

        // Act
        var exit = room.getRandomExit();

        // Assert
        Assert.NotNull(exit);
        Assert.Contains(exit, room.getExits().Keys);
    }
}
