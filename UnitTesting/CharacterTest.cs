using ConsoleApp_121_FinalProjectShell;
using ConsoleApp_121_FinalProjectShell.Commands;
using ConsoleApp_121_FinalProjectShell.People;
using Xunit;


public class CharacterTests
{
     //Helper Methods
    private Player CreatePlayer()
    {
        return new Player();
    }
    private Protagonist CreateProtagonist()
    {
        return new Protagonist();
    }

    private Room CreateRoom(string name = "Room", int id = 0)
    {
        return new Room(name, id);
    }

    private Room CreateRoomWithExit(string direction, Room destination)
    {
        var room = CreateRoom("Start", 0);
        room.setExit(direction, destination);
        return room;
    }

    private Item? CreateItem(string name, int weight, int id = 0)
    {
        // Item is abstract now, so we create a concrete item through the factory.
        // ID doesn't matter for Player inventory/weight tests, but must map to a valid item type.
        return ItemFactory.Create(name, "test item", weight, id);
    }

    private Command CreateGoCommand(string direction)
    {
        return new Command(CommandWord.GO, direction);
    }

  

    [Fact]
    public void Player_StartsWithEmptyInventory()
    {
        // Arrange
        var player = CreatePlayer();

        // Act
        string itemsText = player.itemsText();

        // Assert
        Assert.Contains("nothing", itemsText);
        Assert.Equal(0, player.getCurrentWeight());
    }

    [Fact]
    public void AddItem_IncreasesCurrentWeight()
    {
        // Arrange
        var player = CreatePlayer();
        var item = CreateItem("Key", 10);

        // Act
        player.addItem(item);

        // Assert
        Assert.Equal(10, player.getCurrentWeight());
        Assert.True(player.hasItemByName("Key"));
    }

    [Fact]
    public void RemoveItem_DecreasesCurrentWeight()
    {
        // Arrange
        var player = CreatePlayer();
        var item = CreateItem("Coin", 5);
        player.addItem(item);

        // Act
        player.removeItemByName("Coin");

        // Assert
        Assert.Equal(0, player.getCurrentWeight());
        Assert.False(player.hasItemByName("Coin"));
    }

    [Fact]
    public void WeightCheck_ReturnsFalse_WhenOverLimit()
    {
        // Arrange
        var player = CreatePlayer();
        player.setCarryWeight(5);

        // Act
        bool canCarry = player.weightCheck(10);

        // Assert
        Assert.False(canCarry);
    }

    [Fact]
    public void GoRoom_ReturnsZero_WhenNoSecondWord()
    {
        // Arrange
        var player = CreatePlayer();
        var command = new Command(CommandWord.GO, null);

        // Act
        int result = player.goRoom(command);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void GoRoom_ReturnsMinusOne_WhenExitDoesNotExist()
    {
        // Arrange
        var player = CreatePlayer();
        var room = CreateRoom();
        player.setCurrentRoom(room);
        var command = CreateGoCommand("north");

        // Act
        int result = player.goRoom(command);

        // Assert
        Assert.Equal(-1, result);
    }

    [Fact]
    public void GoRoom_MovesPlayer_WhenExitExists()
    {
        // Arrange
        var destination = CreateRoom("End", 1);
        var start = CreateRoomWithExit("north", destination);
        var player = CreatePlayer();
        player.setCurrentRoom(start);
        var command = CreateGoCommand("north");

        // Act
        int result = player.goRoom(command);

        // Assert
        Assert.Equal(1, result);
        Assert.Equal(destination, player.getCurrentRoom());
    }

    [Fact]
    public void Back_MovesPlayerToPreviousRoom()
    {
        // Arrange
        var room1 = CreateRoom("Room1", 1);
        var room2 = CreateRoom("Room2", 2);
        var player = CreatePlayer();
        player.setCurrentRoom(room1);

        player.getLastRooms().Push(room1);
        player.setCurrentRoom(room2);

        // Act
        player.back();

        // Assert
        Assert.Equal(room1, player.getCurrentRoom());
    }

    [Fact]
    public void ProtagSteps_ReturnsFalse_WhenStepsNotReached()
    {
        // Arrange
        var protag = CreateProtagonist();
        var command = CreateGoCommand("north");

        // Act
        bool moved = protag.protagSteps(command);

        // Assert
        Assert.False(moved);
    }
}