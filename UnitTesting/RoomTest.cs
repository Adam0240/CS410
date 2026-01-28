using ConsoleApp_121_FinalProjectShell;
using Xunit;

public class RoomTests
{
    // Helper item for tests
    private Item CreateTestItem(string name, string desc)
    {
        return new Item(name, desc, 1, 10);
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
        Assert.True(room.hasItem(item));
        Assert.True(room.hasItemByName("key"));
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
        // Arrange
        // (static flags already exist)

        // Act
        var flags = Room.getClearCons();

        // Assert
        Assert.Equal(6, flags.Length);
    }

    [Fact]
    public void SetClearCon_UpdatesCorrectFlag()
    {
        // Arrange
        Room.setClearCon(2, true); // swordPlaced

        // Act
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
        Assert.Contains(exit, room.getExits().Keys);
    }
}