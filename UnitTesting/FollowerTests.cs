

using ConsoleApp_121_FinalProjectShell.Commands;
using ConsoleApp_121_FinalProjectShell.Core;
using ConsoleApp_121_FinalProjectShell.Items;
using ConsoleApp_121_FinalProjectShell.People;
using FluentAssertions;
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

        follower.IsFollowing().Should().BeFalse();
        follower.itemsText().ToLower().Should().Contain("nothing");
        follower.GetName().Should().Be("companion");
    }

    [Fact]
    public void Follow_SetsFollowingToTrue()
    {
        var follower = new Follower();

        follower.Follow();

        follower.IsFollowing().Should().BeTrue();
    }

    [Fact]
    public void Stay_SetsFollowingToFalse()
    {
        var follower = new Follower();

        follower.Follow();
        follower.Stay();

        follower.IsFollowing().Should().BeFalse();
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

        follower.hasItemByName("ring").Should().BeTrue();
        follower.getItemByName("ring").Should().NotBeNull();
        follower.GetCurrentWeight().Should().Be(2);
        follower.itemsText().Should().Contain("ring");
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

        follower.hasItemByName("ring").Should().BeFalse();
        follower.itemsText().Should().NotContain("ring");
        follower.GetCurrentWeight().Should().Be(0);
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

        result.Should().BeFalse();
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

        result.Should().BeTrue();
        player.hasItemByName("ring").Should().BeFalse();
        follower.hasItemByName("ring").Should().BeTrue();
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

        result.Should().BeTrue();
        player.hasItemByName("ring").Should().BeTrue();
        follower.hasItemByName("ring").Should().BeFalse();
    }

    [Fact]
    public void GetRandomIdleText_ReturnsFallback_WhenNoIdleTextExists()
    {
        var follower = new Follower(name: "Old Mule");

        string result = follower.GetRandomIdleText();

        result.Should().Contain("Old Mule");
    }

    [Fact]
    public void AddIdleText_StoresText_AndRandomIdleTextReturnsOneOfThem()
    {
        var follower = new Follower();
        follower.AddIdleText("Your mule brays softly.");
        follower.AddIdleText("Your mule stamps the ground.");

        var allText = follower.GetAllIdleText();
        string randomText = follower.GetRandomIdleText();

        allText.Count.Should().Be(2);
        allText.Should().Contain(randomText);
    }

    [Fact]
    public void FollowingFollower_MovesWithPlayer_WhenPlayerMoves()
    {
        Player player = _game.GetPlayer();
        Follower follower = _game.GetFollower();

        follower.setCurrentRoom(player.getCurrentRoom());
        follower.Follow();

        _game.GoTo(new Command(CommandWord.GO, "north"));

        follower.getCurrentRoom().Should().Be(player.getCurrentRoom());
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

        follower.getCurrentRoom().Should().NotBe(player.getCurrentRoom());
        follower.getCurrentRoom().Should().Be(originalRoom);
    }
}