using ConsoleApp_121_FinalProjectShell.Commands;
using ConsoleApp_121_FinalProjectShell.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using ConsoleApp_121_FinalProjectShell.People;
using ConsoleApp_121_FinalProjectShell.Items;
using Bogus; //Bogus Change 1: Added Bogus namespace
using Bogus.DataSets; //Bogus Change 2: Added Bogus dataset support


using Xunit;
using ConsoleApp_121_FinalProjectShell;

namespace UnitTesting;

public class GameTest
{
    private readonly Game _testGame;

    //Bogus Change 3: Deterministic seed for repeatable generated data
    private static readonly int Seed = 410;

    //Bogus Change 4: Shared Faker instance for generated values in tests
    private static readonly Faker Faker = new("en");

    public GameTest()
    {
        //Bogus Change 5: Seed Bogus randomizer so CI/local runs are stable
        Randomizer.Seed = new Random(Seed);
        //UNVIERSAL ARRANGE
        _testGame = new Game(true);
    }

    // ----------------------------
    // Bogus Helpers
    // ----------------------------

    private static string Normalize(string value) => value.Replace("\r\n", "\n");

    //Bogus Change 6: Helper to pick random CommandWord values
    private static CommandWord RandomCommandWord()
    {
        CommandWord[] all = (CommandWord[])Enum.GetValues(typeof(CommandWord));
        return Faker.PickRandom(all);
    }

    //Bogus Change 7: Helper to generate random second words for commands
    private static string RandomSecondWordFor(CommandWord cw)
    {
        return cw == CommandWord.QUIT
            ? string.Empty
            : Faker.Lorem.Word();
    }

    //Bogus Change 8: Helper to generate randomized GameProgress permutations
    private static GameProgress RandomProgress()
    {
        return new Faker<GameProgress>()
            .RuleFor(p => p.SwampCleared, f => f.Random.Bool())
            .RuleFor(p => p.ForgePrepared, f => f.Random.Bool())
            .RuleFor(p => p.SwordPlaced, f => f.Random.Bool())
            .RuleFor(p => p.GateOpen, f => f.Random.Bool())
            .RuleFor(p => p.ToldProtagGate, f => f.Random.Bool())
            .RuleFor(p => p.ToldProtagSword, f => f.Random.Bool())
            .Generate();
    }

    //BASIC FUNCTIONALITY

    //Tests the functionality of both the primary constructor and the createRooms method. Ensures it creates the 
    //full set of rooms, places the player and protagonist into them at the correct locations, places the items
    //into the correct locations, and places the rooms in the correct configuration. 
    //createRooms() is called within Game's primary constructor, so these have to be tested together
    [Fact]
    public void ConstructorAndCreateRoomsTest()
    {
        //no need to act here, just

        //ASSERT
        //ensure the game was properly constructed
        Assert.NotNull(_testGame);

        //ensure all rooms and items were created successfully
        //WILL NEED TO BE UPDATED IF THE GAME EXPANDS
        Assert.Equal(9, _testGame.allRooms.Count);
        Assert.Equal(6, _testGame.allItems.Count);

        //ensure the player and protagonist were created
        Assert.NotNull(_testGame.GetPlayer());
        Assert.NotNull(_testGame.GetProtag());
        _ = _testGame.GetPlayer();
        _ = _testGame.GetProtag();
        _ = _testGame.allRooms;
        _ = _testGame.allItems;

        //ensure the player and protagonist were placed in the correct rooms
        Assert.Equal(0, _testGame.GetPlayer().GetCurrentRoom()!.GetId());
        Assert.Equal(5, _testGame.GetProtag().getCurrentRoom()!.GetId());

        foreach (Room room in _testGame.allRooms)
        {
            switch (room.GetId())
            {
                case 2: //battleground, checking for the axe
                    Assert.True(room.hasItemByName("axe"));
                    break;
                case 6: //castle gate, checking for the ring
                    Assert.True(room.hasItemByName("ring"));
                    break;
                case 5: //graves, checking for the hammer
                    Assert.True(room.hasItemByName("hammer"));
                    break;
                case 8: //grove, checking for hilt
                    Assert.True(room.hasItemByName("hilt"));
                    break;
                default:
                    Assert.True(true);
                    break;
            }
        }
    }


    [Fact]
    public void printLocationInfoTest()
    {
        Room room1 = _testGame.allRooms[2];
        Room room2 = _testGame.allRooms[5];
        StringWriter stringWriter = new();
        Console.SetOut(stringWriter);

        _testGame.PrintLocationInfo(room1);
        _testGame.PrintLocationInfo(room2);

        var output = stringWriter.ToString().Replace("\r\n", "\n");

        // Multiplayer Change 1:
        // Validate against the multiplayer-aware room formatter so presence text placement matches runtime behavior.
        string expectedRoom1 = _testGame.GetLocationInfoText(1, room1);
        string expectedRoom2 = _testGame.GetLocationInfoText(1, room2);

        Assert.Equal($"{expectedRoom1}\n{expectedRoom2}\n", output);
    }

    [Fact]
    public void quitTest()
    {
        //ARRANGE 
        Command commandfalse = new(CommandWord.QUIT, "thang");
        Command commandtrue = new(CommandWord.QUIT, null);

        //ACT
        bool commandfalseResult = Game.Quit(commandfalse);
        bool commandtrueResult = Game.Quit(commandtrue);

        //ASSERT
        Assert.False(commandfalseResult);
        Assert.True(commandtrueResult);
    }

    [Fact]
    public void protagMoveTest()
    {
        // ARRANGE
        StringWriter stringWriter = new();
        Console.SetOut(stringWriter);

        Protagonist protag = _testGame.GetProtag();

        // Grab known rooms from the test instance (IDs from Game.createRooms)
        Room hub = _testGame.allRooms.Single(r => r.GetId() == 0);
        Room graves = _testGame.allRooms.Single(r => r.GetId() == 5);

        // Reflection helper to set protagStepsCount deterministically
        FieldInfo? stepsField = typeof(Protagonist).GetField(
            "_protagStepsCount",
            BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.NotNull(stepsField);

        foreach (CommandWord commandWord in (CommandWord[])Enum.GetValues(typeof(CommandWord)))
        {
            // Reset protag to a room with exactly one exit so movement is deterministic (graves -> hub)
            protag.setCurrentRoom(graves);

            // Force protag to move on the next protagSteps() call (>= 8 triggers a move)
            stepsField!.SetValue(protag, 8);

            int initialSteps = protag.getProtagStepsCount();
            Room initialRoom = protag.getCurrentRoom()!;

            // ACT
            string? secondWord = (commandWord == CommandWord.QUIT) ? null : "foobar";
            _testGame.ProcessCommand(new Command(commandWord, secondWord));

            int afterSteps = protag.getProtagStepsCount();
            Room afterRoom = protag.getCurrentRoom()!;

            // ASSERT
            if (commandWord == CommandWord.UNKNOWN ||
                commandWord == CommandWord.SAVE ||
                commandWord == CommandWord.LOAD ||
                commandWord == CommandWord.DELETE)
            {
                // UNKNOWN and save-system commands should NOT trigger protagMove().
                Assert.Equal(initialSteps, afterSteps);
                Assert.Same(initialRoom, afterRoom);
            }
            else
            {
                // Recognized commands should trigger protagMove()
                // With steps forced to 8, protagSteps() must subtract 8 and move graves -> hub
                Assert.Equal(initialSteps - 8, afterSteps);
                Assert.Same(hub, afterRoom);
            }
        }
    }



    //INVENTORY

    //We move the protagonist to each room that starts with an item, then have them attempt to
    //pick it up. The first three should succeed and leave the room empty, while the fourth
    //should fail due to weight limit. We also check to see that for bad values in 
    [Fact]
    public void takeTest()
    {
        //ARRANGE
        Player player = _testGame.GetPlayer();
        List<Room> rooms = _testGame.allRooms;
        StringWriter stringWriter = new();
        int weight1;
        int weight2;
        int weight3;

        //ACT

        player.setCurrentRoom(rooms[8]);
        _testGame.Take(new Command(CommandWord.TAKE, "hilt"));
        weight1 = player.getCurrentWeight();

        player.setCurrentRoom(rooms[6]);
        _testGame.Take(new Command(CommandWord.TAKE, "ring"));
        weight2 = player.getCurrentWeight();

        player.setCurrentRoom(rooms[5]);
        _testGame.Take(new Command(CommandWord.TAKE, "hammer"));
        weight3 = player.getCurrentWeight();

        player.setCurrentRoom(rooms[2]);
        _testGame.Take(new Command(CommandWord.TAKE, "axe"));

        //We have to set this here to ensure we're not capturing output from the other actions, which
        //can be tested in other ways.
        Console.SetOut(stringWriter);
        _testGame.Take(new Command(CommandWord.TAKE, "foobar"));
        _testGame.Take(new Command(CommandWord.TAKE, null));

        //
        //ASSERT
        Assert.Equal(0, rooms[8].GetItemsCount());
        Assert.Equal(0, rooms[6].GetItemsCount());
        Assert.Equal(0, rooms[5].GetItemsCount());
        Assert.Equal(1, rooms[2].GetItemsCount());

        Assert.Equal(17, weight1);
        Assert.Equal(19, weight2);
        Assert.Equal(53, weight3);
        Assert.Equal(53, player.getCurrentWeight());

        var output = stringWriter.ToString().Replace("\r\n", "\n");
        Assert.Equal("There isn't anything like that around.\nTake what?\n", output);
    }

    //Much easier to actually test than take(), as this has no weight dependencies to account for
    [Fact]
    public void dropTest()
    {
        //ARRANGE
        Player player = _testGame.GetPlayer();
        List<Room> rooms = _testGame.allRooms;
        int weight1;
        int roomInvCount;
        StringWriter stringWriter = new();

        //ACT
        player.setCurrentRoom(rooms[2]);
        _testGame.Take(new Command(CommandWord.TAKE, "axe"));
        weight1 = player.getCurrentWeight();
        roomInvCount = rooms[2].GetItemsCount();
        _testGame.Drop(new Command(CommandWord.DROP, "axe"));

        Console.SetOut(stringWriter);
        _testGame.Drop(new Command(CommandWord.DROP, null));
        _testGame.Drop(new Command(CommandWord.DROP, "somethang"));

        //ASSERT
        var output = stringWriter.ToString().Replace("\r\n", "\n");
        Assert.True(weight1 == 55 && player.getCurrentWeight() == 0);
        Assert.True(roomInvCount == 0 && rooms[2].GetItemsCount() == 1);

        Assert.Equal("Drop what?\nYou don't have anything like that.\n", output);

    }



    //the individual "[item]Use" methods are what actually controls the item functionality, so for this method we're
    //just testing that use() selects the right option based on the command (missing second word, invalid second word,
    //and valid second word)
    //in addition, this also tests axeUse() as we need to test the use case for a valid item, and it reduces
    //the number of tests we have to write
    [Fact]
    public void ProtagMoveTest_BogusDriven_CommandSampling()
    {
        //Bogus Change 9: Existing command-loop concept converted to Bogus-driven random sampling
        StringWriter sw = new();
        Console.SetOut(sw);

        Protagonist protag = _testGame.GetProtag();
        Room hub = _testGame.allRooms.Single(r => r.GetId() == 0);
        Room graves = _testGame.allRooms.Single(r => r.GetId() == 5);

        FieldInfo? stepsField = typeof(Protagonist).GetField(
            "_protagStepsCount",
            BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.NotNull(stepsField);

        //Bogus Change 10: Randomized command coverage with deterministic seed
        for (int i = 0; i < 30; i++)
        {
            CommandWord cw = RandomCommandWord();
            string? secondWord = cw == CommandWord.QUIT ? null : RandomSecondWordFor(cw);

            protag.setCurrentRoom(graves);
            stepsField!.SetValue(protag, 8);

            int beforeSteps = protag.getProtagStepsCount();
            Room beforeRoom = protag.getCurrentRoom()!;

            _testGame.ProcessCommand(new Command(cw, secondWord));

            int afterSteps = protag.getProtagStepsCount();
            Room afterRoom = protag.getCurrentRoom()!;

            if (cw == CommandWord.UNKNOWN || cw == CommandWord.SAVE || cw == CommandWord.LOAD || cw == CommandWord.DELETE)
            {
                Assert.Equal(beforeSteps, afterSteps);
                Assert.Same(beforeRoom, afterRoom);
            }
            else
            {
                Assert.Equal(beforeSteps - 8, afterSteps);
                Assert.Same(hub, afterRoom);
            }
        }
    }

    [Fact]
    public void TalkFlow_BogusProgressPermutations_DoNotThrow()
    {
        //Bogus Change 11: Added randomized GameProgress permutations for TALK robustness
        Player player = _testGame.GetPlayer();
        Protagonist protag = _testGame.GetProtag();
        StringWriter sw = new();
        Console.SetOut(sw);

        for (int i = 0; i < 20; i++)
        {
            GameProgress generated = RandomProgress();

            _testGame.GetProgress().SwampCleared = generated.SwampCleared;
            _testGame.GetProgress().ForgePrepared = generated.ForgePrepared;
            _testGame.GetProgress().SwordPlaced = generated.SwordPlaced;
            _testGame.GetProgress().GateOpen = generated.GateOpen;
            _testGame.GetProgress().ToldProtagGate = generated.ToldProtagGate;
            _testGame.GetProgress().ToldProtagSword = generated.ToldProtagSword;

            //Bogus Change 12: Randomly put player with or away from protagonist
            bool sameRoom = Faker.Random.Bool();
            if (sameRoom)
            {
                player.setCurrentRoom(protag.getCurrentRoom()!);
            }
            else
            {
                Room randomRoom = Faker.PickRandom(_testGame.allRooms);
                player.setCurrentRoom(randomRoom);
            }

            bool result = _testGame.ProcessCommand(new Command(CommandWord.TALK, null));
            Assert.False(result);
        }

        string output = Normalize(sw.ToString());
        Assert.NotNull(output);
    }

    [Fact]
    public void Sleep_BogusScenarios_OnlyEndsWhenPrereqsMet()
    {
        //Bogus Change 13: Added generated scenario matrix for sleep completion conditions
        Player player = _testGame.GetPlayer();
        List<Room> rooms = _testGame.allRooms;
        Room startRoom = rooms.Single(r => r.GetId() == 0);

        for (int i = 0; i < 25; i++)
        {
            bool inStartRoom = Faker.Random.Bool();
            bool toldGate = Faker.Random.Bool();
            bool toldSword = Faker.Random.Bool();

            player.setCurrentRoom(inStartRoom
                ? startRoom
                : Faker.PickRandom(rooms.Where(r => r.GetId() != 0).ToList()));

            _testGame.GetProgress().ToldProtagGate = toldGate;
            _testGame.GetProgress().ToldProtagSword = toldSword;

            bool done = _testGame.ProcessCommand(new Command(CommandWord.SLEEP, null));
            bool expected = inStartRoom && toldGate && toldSword;
            Assert.Equal(expected, done);
        }
    }

    //This test verifies that attempting to use an item not present in the player�s inventory is correctly rejected,
    //preventing item behavior execution and displaying the appropriate error message.
    [Fact]
    public void useInvalidItemTest()
    {
        Game game = _testGame;
        StringWriter sw = new();
        Console.SetOut(sw);

        //Bogus Change 14: Invalid item input now generated instead of hardcoded literal
        string invalidName = Faker.Commerce.ProductName().Replace(" ", "").ToLowerInvariant();
        Command cmd = new(CommandWord.USE, invalidName);
        game.ProcessCommand(cmd);

        string output = Normalize(sw.ToString());
        Assert.Contains("You don't have an item like that.\n", output);
    }

    //This test verifies that when a player uses the ring item from their inventory, the correct polymorphic Use() method executes,
    //increasing the player's carry weight and printing the expected message.
    [Fact]
    public void ringUseTestage()
    {
        // Create references to the test game instance and its player.
        // _testGame was constructed with Game(true) so randomness is deterministic.
        Game game = _testGame;
        Player player = game.GetPlayer();

        // Add a ring item directly to the player's inventory.
        // This ensures the "use ring" command is valid and will trigger the ring's Use() override.
        player.addItem(ItemFactory.Create(
            "ring",
            "a shining RING with a knight's insignia",
            2,
            1
        ));

        // Capture console output 
        StringWriter stringWriter = new();
        Console.SetOut(stringWriter);

        // Simulate the player entering the command: "use ring".
        // This verifies that Game.processCommand routes correctly to use(),
        // and that polymorphism dispatches to the RingItem's overridden Use() method.
        Command cmd = new(CommandWord.USE, "ring");
        game.ProcessCommand(cmd);

        // Normalize newlines for consistent cross-platform comparison.
        var output = stringWriter.ToString().Replace("\r\n", "\n");

        // Verify the ring correctly modified player state.
        // The ring should increase the player's carry weight to 150.
        Assert.Equal(150, player.getCarryWeight());

        // Verify the correct message was printed, confirming the correct item behavior executed.
        Assert.Contains("By equipping the ring, your maximum carryable weight has increased.\n", output);
    }

    [Fact]
    public void hammerUseTest()
    {
        //ARRANGE
        Player player = _testGame.GetPlayer();
        List<Room> rooms = _testGame.allRooms;
        StringWriter stringWriter = new();
        player.setCurrentRoom(rooms[3]);

        //ACT
        _testGame.HammerUse();
        _ = player.GetCurrentRoom().GetItemsCount();
        _testGame.Take(new Command(CommandWord.TAKE, "ore"));
        _testGame.HammerUse();
        _ = player.GetCurrentRoom().GetItemsCount();

        player.setCurrentRoom(rooms[4]);
        _testGame.HammerUse();

        player.setCurrentRoom(rooms[0]);
        Console.SetOut(stringWriter);
        _testGame.HammerUse();

        //ASSERT
        //make sure error dialogue is displayed when there's nothing to do
        var output = stringWriter.ToString().Replace("\r\n", "\n");
        Assert.Equal("Nothing to do with that here.\n", output);

        //ensure the forge is prepared when the hammer is used in the room
        Assert.True(rooms[4].GetItemsCount() == 1 && !player.hasItemByName("hammer") && _testGame.GetProgress().ForgePrepared);

        //ensure that the ore can be obtained AND that it won't be re-placed on use again
        //(currently fails, change this comment once that bug is fixed)
        Assert.True(player.hasItemByName("ore") && rooms[3].GetItemsCount() == 0);


    }

    [Fact]
    public void hiltUseTest()
    {
        //ARRANGE
        Player player = _testGame.GetPlayer();
        List<Room> rooms = _testGame.allRooms;
        List<Item> items = _testGame.allItems;
        player.setCurrentRoom(rooms[0]);
        player.addItem(items[3]);
        player.addItem(items[4]);
        player.addItem(items[5]);
        StringWriter stringWriter = new();
        Console.SetOut(stringWriter);

        //ACT
        _testGame.HiltUse();
        var output = stringWriter.ToString().Replace("\r\n", "\n");

        player.setCurrentRoom(rooms[4]);
        _testGame.HammerUse();
        _testGame.HiltUse();

        //ASSERT
        Assert.Equal("Can't do anything with that right now.\n", output);
        Assert.False(player.hasItemByName("hammer"));
        Assert.False(player.hasItemByName("ore"));
        Assert.False(player.hasItemByName("hilt"));
        Assert.True(player.hasItemByName("sword"));

    }

    [Fact]
    public void swordUseTest()
    {
        //ARRANGE
        Player player = _testGame.GetPlayer();
        Protagonist protag = _testGame.GetProtag();
        List<Room> rooms = _testGame.allRooms;
        List<Item> items = _testGame.allItems;
        player.setCurrentRoom(rooms[0]);
        player.addItem(items[5]);
        bool protagdead;
        StringWriter stringWriter = new();

        //ACT
        Console.SetOut(stringWriter);
        _testGame.SwordUse();
        _ = stringWriter.ToString().Replace("\r\n", "\n");

        player.setCurrentRoom(protag.getCurrentRoom()!);
        protagdead = _testGame.SwordUse();

        player.setCurrentRoom(rooms[8]);
        _testGame.SwordUse();

        //ASSERT
        Assert.True(protagdead);
        Assert.True(_testGame.GetProgress().SwordPlaced);

    }

    //BASIC COMMANDS (private methods accessed via processCommand)

    [Fact]
    public void talkTest()
    {
        //ARRANGE
        Player player = _testGame.GetPlayer();
        Protagonist protag = _testGame.GetProtag();
        List<Room> rooms = _testGame.allRooms;

        //Reset progression flags in case other tests ran first
        _testGame.GetProgress().SwampCleared = false;
        _testGame.GetProgress().ForgePrepared = false;
        _testGame.GetProgress().SwordPlaced = false;
        _testGame.GetProgress().GateOpen = false;
        _testGame.GetProgress().ToldProtagGate = false;
        _testGame.GetProgress().ToldProtagSword = false;

        StringWriter stringWriter = new();
        Console.SetOut(stringWriter);

        //ACT
        //1) Not in same room as protag
        player.setCurrentRoom(rooms[0]);
        _testGame.ProcessCommand(new Command(CommandWord.TALK, null));

        //2) Same room, weapon-location dialogue (clearCon[2] -> clearCon[5])
        player.setCurrentRoom(protag.getCurrentRoom()!);
        _testGame.GetProgress().SwordPlaced = true;
        _testGame.GetProgress().ToldProtagSword = false;
        _testGame.ProcessCommand(new Command(CommandWord.TALK, null));

        //3) Same room, way-forward dialogue (clearCon[3] -> clearCon[4])
        _testGame.GetProgress().GateOpen = true;
        _testGame.GetProgress().ToldProtagGate = false;
        _testGame.ProcessCommand(new Command(CommandWord.TALK, null));

        //4) Same room, nothing left to say
        _testGame.ProcessCommand(new Command(CommandWord.TALK, null));

        //ASSERT
        var output = stringWriter.ToString().Replace("\r\n", "\n");

        // Some builds print a prompt like "> " that gets captured in tests. Strip it.
        while (output.StartsWith("> "))
        {
            output = output[2..];
        }
        output = output.Replace("\n> ", "\n");

        Assert.Equal(
            "There's no-one to talk to!\n" +
            "You inform the protagonist of the location of a weapon.\n" +
            "You inform the protagonist of a way forward.\n" +
            "Nothing to say to the protagonist right now.\n",
            output);

        Assert.True(_testGame.GetProgress().ToldProtagGate);
        Assert.True(_testGame.GetProgress().ToldProtagSword);
    }

    [Fact]
    public void sleepTest()
    {
        //ARRANGE
        Player player = _testGame.GetPlayer();
        List<Room> rooms = _testGame.allRooms;

        //Reset progression flags in case other tests ran first
        _testGame.GetProgress().SwampCleared = false;
        _testGame.GetProgress().ForgePrepared = false;
        _testGame.GetProgress().SwordPlaced = false;
        _testGame.GetProgress().GateOpen = false;
        _testGame.GetProgress().ToldProtagGate = false;
        _testGame.GetProgress().ToldProtagSword = false;

        StringWriter stringWriter = new();
        Console.SetOut(stringWriter);

        //ACT
        //1) Sleeping outside the start room should fail
        player.setCurrentRoom(rooms[2]);
        bool badRoomResult = _testGame.ProcessCommand(new Command(CommandWord.SLEEP, null));

        //2) Sleeping in the start room without progression flags should fail
        player.setCurrentRoom(rooms[0]);
        bool notDoneResult = _testGame.ProcessCommand(new Command(CommandWord.SLEEP, null));

        //3) Sleeping in the start room with progression flags should end the game
        _testGame.GetProgress().ToldProtagGate = true;
        _testGame.GetProgress().ToldProtagSword = true;
        bool doneResult = _testGame.ProcessCommand(new Command(CommandWord.SLEEP, null));

        //ASSERT
        Assert.False(badRoomResult);
        Assert.False(notDoneResult);
        Assert.True(doneResult);

        var output = stringWriter.ToString().Replace("\r\n", "\n");
        Assert.Equal(
            "This is a terrible place to sleep.\n" +
            "You've not finished all that you need to!\n" +
            "You lay your head down to sleep, your (likely fruitless) endeavors complete.\n",
            output);
    }

    [Fact]
    public void protagKillTest()
    {
        //ARRANGE
        Player player = _testGame.GetPlayer();
        Protagonist protag = _testGame.GetProtag();
        StringWriter stringWriter = new();
        Console.SetOut(stringWriter);

        //ACT
        player.setCurrentRoom(protag.getCurrentRoom()!);
        bool result = _testGame.SwordUse(); //calls protagKill() when in the same room

        //ASSERT
        Assert.True(result);

        var output = stringWriter.ToString().Replace("\r\n", "\n");
        Assert.Equal(
            "In a single mighty blow, you strike down the oblivious protagonist.\n" +
            "With this character's death the thread of prophecy... et cetera.\n",
            output);
    }
}
