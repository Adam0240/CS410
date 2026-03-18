

using ConsoleApp_121_FinalProjectShell.Commands;
using ConsoleApp_121_FinalProjectShell.Core;
using ConsoleApp_121_FinalProjectShell.Items;
using ConsoleApp_121_FinalProjectShell.People;
using Xunit;

namespace UnitTesting;

public class FollowerTests
{
    private readonly Game _game;

    public FollowerTests()
    {
        _game = new Game(true);
    }

    [Fact]
    public void Constructor_DefaultFollower_IsNotFollowing_AndHasEmptyInventory()
    {
        var follower = new Follower();

        Assert.False(follower.IsFollowing());
        Assert.Contains("nothing", follower.itemsText(), System.StringComparison.OrdinalIgnoreCase);
        Assert.Equal("companion", follower.GetName());
    }

    [Fact]
    public void Follow_SetsFollowingToTrue()
    {
        var follower = new Follower();

        follower.Follow();

        Assert.True(follower.IsFollowing());
    }

    [Fact]
    public void Stay_SetsFollowingToFalse()
    {
        var follower = new Follower();

        follower.Follow();
        follower.Stay();

        Assert.False(follower.IsFollowing());
    }

    [Fact]
    public void AddItem_IncreasesInventoryText_AndWeight()
    {
        var follower = new Follower();
        Item ring = ItemFactory.Create(
            "ring",
            "a shining RING with a knight's insignia",
            2,
            1
        );

        follower.addItem(ring);

        Assert.True(follower.hasItemByName("ring"));
        Assert.NotNull(follower.getItemByName("ring"));
        Assert.Equal(2, follower.GetCurrentWeight());
        Assert.Contains("ring", follower.itemsText(), System.StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void RemoveItemByName_RemovesItem_AndDecreasesWeight()
    {
        var follower = new Follower();
        Item ring = ItemFactory.Create(
            "ring",
            "a shining RING with a knight's insignia",
            2,
            1
        );

        follower.addItem(ring);
        follower.removeItemByName("ring");

        Assert.False(follower.hasItemByName("ring"));
        Assert.Null(follower.getItemByName("ring"));
        Assert.Equal(0, follower.GetCurrentWeight());
    }

    [Fact]
    public void IsValidItem_ReturnsFalse_WhenItemWouldExceedCarryWeight()
    {
        var follower = new Follower(carryWeight: 10);
        Item axe = ItemFactory.Create(
            "axe",
            "a battered war AXE",
            55,
            0
        );

        bool result = follower.isValidItem(axe);

        Assert.False(result);
    }

    [Fact]
    public void ReceiveFromPlayer_MovesItemFromPlayerToFollower()
    {
        Player player = _game.GetPlayer();
        Follower follower = _game.GetFollower();

        Item ring = ItemFactory.Create(
            "ring",
            "a shining RING with a knight's insignia",
            2,
            1
        );

        player.addItem(ring);

        bool result = follower.ReceiveFromPlayer(player, "ring");

        Assert.True(result);
        Assert.False(player.hasItemByName("ring"));
        Assert.True(follower.hasItemByName("ring"));
    }

    [Fact]
    public void GiveToPlayer_MovesItemFromFollowerToPlayer()
    {
        Player player = _game.GetPlayer();
        Follower follower = _game.GetFollower();

        Item ring = ItemFactory.Create(
            "ring",
            "a shining RING with a knight's insignia",
            2,
            1
        );

        follower.addItem(ring);

        bool result = follower.GiveToPlayer(player, "ring");

        Assert.True(result);
        Assert.True(player.hasItemByName("ring"));
        Assert.False(follower.hasItemByName("ring"));
    }

    [Fact]
    public void GetRandomIdleText_ReturnsFallback_WhenNoIdleTextExists()
    {
        var follower = new Follower(name: "Old Mule");

        string result = follower.GetRandomIdleText();

        Assert.Contains("Old Mule", result);
    }

    [Fact]
    public void AddIdleText_StoresText_AndRandomIdleTextReturnsOneOfThem()
    {
        var follower = new Follower();
        follower.AddIdleText("Your mule brays softly.");
        follower.AddIdleText("Your mule stamps the ground.");

        var allText = follower.GetAllIdleText();
        string randomText = follower.GetRandomIdleText();

        Assert.Equal(2, allText.Count);
        Assert.Contains(randomText, allText);
    }

    [Fact]
    public void FollowingFollower_MovesWithPlayer_WhenPlayerMoves()
    {
        Player player = _game.GetPlayer();
        Follower follower = _game.GetFollower();

        follower.setCurrentRoom(player.getCurrentRoom());
        follower.Follow();

        _game.GoTo(new Command(CommandWord.GO, "north"));

        Assert.Equal(player.getCurrentRoom(), follower.getCurrentRoom());
    }

    [Fact]
    public void StayingFollower_DoesNotMoveWithPlayer_WhenPlayerMoves()
    {
        Player player = _game.GetPlayer();
        Follower follower = _game.GetFollower();

        var originalRoom = player.getCurrentRoom();
        follower.setCurrentRoom(originalRoom);
        follower.Stay();

        _game.GoTo(new Command(CommandWord.GO, "north"));

        Assert.NotEqual(player.getCurrentRoom(), follower.getCurrentRoom());
        Assert.Equal(originalRoom, follower.getCurrentRoom());
    }
}