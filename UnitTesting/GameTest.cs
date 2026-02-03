using System.Reflection;

namespace ConsoleApp_121_FinalProjectShell.Tests;

public class GameTest
{
    private Game _testGame;
    public GameTest()
    {
        //UNVIERSAL ARRANGE
        _testGame = new Game(true);
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
        Assert.True(_testGame.allRooms.Count == 9);
        Assert.True(_testGame.allItems.Count == 6);

        //ensure the player and protagonist were created
        Assert.NotNull(_testGame.getPlayer());
        Assert.NotNull(_testGame.getProtag());

        Player player = _testGame.getPlayer();
        Player protage = _testGame.getProtag();
        List<Room> rooms = _testGame.allRooms;
        List<Item> items = _testGame.allItems;

        //ensure the player and protagonist were placed in the correct rooms
        Assert.True(_testGame.getPlayer().getCurrentRoom().getID() == 0);
        Assert.True(_testGame.getProtag().getCurrentRoom().getID() == 5);

        foreach (Room room in _testGame.allRooms)
        {
            switch (room.getID())
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
        //ARRANGE
        Room room1 = _testGame.allRooms[2];
        Room room2 = _testGame.allRooms[5];
        StringWriter stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        //ACT
        _testGame.printLocationInfo(room1);
        _testGame.printLocationInfo(room2);

        //ASSERT
        var output = stringWriter.ToString().Replace("\r\n", "\n");
        Assert.Equal($"{room1.getLongDesc()}\n{room2.getLongDesc()}\nThe protagonist is here, bumbling about the area.\n", output);
    }

    [Fact]
    public void quitTest()
    {
        //ARRANGE 
        Command commandfalse = new Command(CommandWord.QUIT, "thang");
        Command commandtrue = new Command(CommandWord.QUIT, null);

        //ACT
        bool commandfalseResult = _testGame.quit(commandfalse);
        bool commandtrueResult = _testGame.quit(commandtrue);

        //ASSERT
        Assert.False(commandfalseResult);
        Assert.True(commandtrueResult);
    }

    // NOTE: This test currently reveals a bug or design mismatch in processCommand/protagonist step updates.
    // It is being tracked to be fixed in Sprint 4.
    [Fact]
    public void protagMoveTest()
    {
        // ARRANGE
        StringWriter stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        Player protag = _testGame.getProtag();

        // Grab known rooms from the test instance (IDs from Game.createRooms)
        Room hub = _testGame.allRooms.Single(r => r.getID() == 0);
        Room graves = _testGame.allRooms.Single(r => r.getID() == 5);

        // Reflection helper to set protagStepsCount deterministically
        FieldInfo? stepsField = typeof(Player).GetField(
            "protagStepsCount",
            BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.NotNull(stepsField);

        foreach (CommandWord commandWord in (CommandWord[])Enum.GetValues(typeof(CommandWord)))
        {
            // Reset protag to a room with exactly one exit so movement is deterministic (graves -> hub)
            protag.setCurrentRoom(graves);

            // Force protag to move on the next protagSteps() call (>= 8 triggers a move)
            stepsField!.SetValue(protag, 8);

            int initialSteps = protag.getProtagStepsCount();
            Room initialRoom = protag.getCurrentRoom();

            // ACT
            string? secondWord = (commandWord == CommandWord.QUIT) ? null : "foobar";
            _testGame.processCommand(new Command(commandWord, secondWord));

            int afterSteps = protag.getProtagStepsCount();
            Room afterRoom = protag.getCurrentRoom();

            // ASSERT
            if (commandWord == CommandWord.UNKNOWN)
            {
                // After your fix, UNKNOWN should NOT trigger protagMove()
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
        Player player = _testGame.getPlayer();
        List<Room> rooms = _testGame.allRooms;
        StringWriter stringWriter = new StringWriter();
        int weight1;
        int weight2;
        int weight3;

        //ACT

        player.setCurrentRoom(rooms[8]);
        _testGame.take(new Command(CommandWord.TAKE, "hilt"));
        weight1 = player.getCurrentWeight();

        player.setCurrentRoom(rooms[6]);
        _testGame.take(new Command(CommandWord.TAKE, "ring"));
        weight2 = player.getCurrentWeight();

        player.setCurrentRoom(rooms[5]);
        _testGame.take(new Command(CommandWord.TAKE, "hammer"));
        weight3 = player.getCurrentWeight();

        player.setCurrentRoom(rooms[2]);
        _testGame.take(new Command(CommandWord.TAKE, "axe"));

        //We have to set this here to ensure we're not capturing output from the other actions, which
        //can be tested in other ways.
        Console.SetOut(stringWriter);
        _testGame.take(new Command(CommandWord.TAKE, "foobar"));
        _testGame.take(new Command(CommandWord.TAKE, null));

        //
        //ASSERT
        Assert.Equal(0, rooms[8].getItemsCount());
        Assert.Equal(0, rooms[6].getItemsCount());
        Assert.Equal(0, rooms[5].getItemsCount());
        Assert.Equal(1, rooms[2].getItemsCount());

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
        Player player = _testGame.getPlayer();
        List<Room> rooms = _testGame.allRooms;
        int weight1;
        int roomInvCount;
        StringWriter stringWriter = new StringWriter();

        //ACT
        player.setCurrentRoom(rooms[2]);
        _testGame.take(new Command(CommandWord.TAKE, "axe"));
        weight1 = player.getCurrentWeight();
        roomInvCount = rooms[2].getItemsCount();
        _testGame.drop(new Command(CommandWord.DROP, "axe"));

        Console.SetOut(stringWriter);
        _testGame.drop(new Command(CommandWord.DROP, null));
        _testGame.drop(new Command(CommandWord.DROP, "somethang"));

        //ASSERT
        var output = stringWriter.ToString().Replace("\r\n", "\n");
        Assert.True(weight1 == 55 && player.getCurrentWeight() == 0);
        Assert.True(roomInvCount == 0 && rooms[2].getItemsCount() == 1);

        Assert.Equal("Drop what?\nYou don't have anything like that.\n", output);

    }

    //the individual "[item]Use" methods are what actually controls the item functionality, so for this method we're
    //just testing that use() selects the right option based on the command (missing second word, invalid second word,
    //and valid second word)
    //in addition, this also tests axeUse() as we need to test the use case for a valid item, and it reduces
    //the number of tests we have to write
    [Fact]
    private void useTest()
    {
        //ARRANGE
        Player player = _testGame.getPlayer();
        player.addItem(_testGame.allItems[0]);
        player.setCurrentRoom(_testGame.allRooms[6]);
        StringWriter stringWriter = new StringWriter();

        //ACT
        _testGame.use(new Command(CommandWord.USE, "axe"));
        Console.SetOut(stringWriter);
        _testGame.use(new Command(CommandWord.USE, null));
        _testGame.use(new Command(CommandWord.USE, "nothingburger"));
        player.setCurrentRoom(_testGame.allRooms[4]);
        _testGame.use(new Command(CommandWord.USE, "axe"));

        //ASSERT
        Assert.True(Room.getClearCons()[3]);
        var output = stringWriter.ToString().Replace("\r\n", "\n");
        Assert.Equal("Use what?\nYou don't have an item like that.\nNothing to do with that here.\n", output);

        player.setCurrentRoom(_testGame.getProtag().getCurrentRoom());
        Assert.True(_testGame.use(new Command(CommandWord.USE, "axe")));
    }

    //this is similar to useTest, it just tests to ensure that itemSwitch accounts for the correct things
    //like use test, we also test one of the item use methods for the same reasons, in this case ringUse
    [Fact]
    private void itemSwitchTest()
    {
        //ARRANGE
        Player player = _testGame.getPlayer();
        StringWriter stringWriter = new StringWriter();

        //ACT
        Console.SetOut(stringWriter);
        _testGame.itemSwitch(1);
        _testGame.itemSwitch(10);

        //ASSERT
        var output = stringWriter.ToString().Replace("\r\n", "\n");
        Assert.True(player.getCarryWeight() == 150);
        Assert.Equal("By equipping the ring, your maximum carryable weight has increased.\nSomething has gone terribly wrong.\n", output);
    }

    //Bug identified to be fixed in sprint 4
    [Fact]
    private void hammerUseTest()
    {
        //ARRANGE
        Player player = _testGame.getPlayer();
        List<Room> rooms = _testGame.allRooms;
        StringWriter stringWriter = new StringWriter();
        int roomItems1;
        int roomItems2;
        player.setCurrentRoom(rooms[3]);

        //ACT
        _testGame.hammerUse();
        roomItems1 = player.getCurrentRoom().getItemsCount();
        _testGame.take(new Command(CommandWord.TAKE, "ore"));
        _testGame.hammerUse();
        roomItems2 = player.getCurrentRoom().getItemsCount();

        player.setCurrentRoom(rooms[4]);
        _testGame.hammerUse();

        player.setCurrentRoom(rooms[0]);
        Console.SetOut(stringWriter);
        _testGame.hammerUse();

        //ASSERT
        //make sure error dialogue is displayed when there's nothing to do
        var output = stringWriter.ToString().Replace("\r\n", "\n");
        Assert.Equal("Nothing to do with that here.\n", output);

        //ensure the forge is prepared when the hammer is used in the room
        Assert.True(rooms[4].getItemsCount() == 1 && !player.hasItemByName("hammer") && Room.getClearCons()[1]);

        //ensure that the ore can be obtained AND that it won't be re-placed on use again
        //(currently fails, change this comment once that bug is fixed)
        Assert.True(player.hasItemByName("ore") && rooms[3].getItemsCount() == 0);


    }

    [Fact]
    private void hiltUseTest()
    {
        //ARRANGE
        Player player = _testGame.getPlayer();
        List<Room> rooms = _testGame.allRooms;
        List<Item> items = _testGame.allItems;
        player.setCurrentRoom(rooms[0]);
        player.addItem(items[3]);
        player.addItem(items[4]);
        player.addItem(items[5]);
        StringWriter stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        //ACT
        _testGame.hiltUse();
        var output = stringWriter.ToString().Replace("\r\n", "\n");

        player.setCurrentRoom(rooms[4]);
        _testGame.hammerUse();
        _testGame.hiltUse();

        //ASSERT
        Assert.Equal("Can't do anything with that right now.\n", output);
        Assert.False(player.hasItemByName("hammer"));
        Assert.False(player.hasItemByName("ore"));
        Assert.False(player.hasItemByName("hilt"));
        Assert.True(player.hasItemByName("sword"));

    }

    [Fact]
    private void swordUseTest()
    {
        //ARRANGE
        Player player = _testGame.getPlayer();
        Player protag = _testGame.getProtag();
        List<Room> rooms = _testGame.allRooms;
        List<Item> items = _testGame.allItems;
        player.setCurrentRoom(rooms[0]);
        player.addItem(items[5]);
        bool protagdead;
        StringWriter stringWriter = new StringWriter();

        //ACT
        Console.SetOut(stringWriter);
        _testGame.swordUse();
        var output = stringWriter.ToString().Replace("\r\n", "\n");

        player.setCurrentRoom(protag.getCurrentRoom());
        protagdead = _testGame.swordUse();

        player.setCurrentRoom(rooms[8]);
        _testGame.swordUse();

        //ASSERT
        Assert.True(protagdead);
        Assert.True(Room.getClearCons()[2]);

    }

    //BASIC COMMANDS (private methods accessed via processCommand)

    [Fact]
    public void talkTest()
    {
        //ARRANGE
        Player player = _testGame.getPlayer();
        Player protag = _testGame.getProtag();
        List<Room> rooms = _testGame.allRooms;

        //Reset progression flags in case other tests ran first
        bool[] cons = Room.getClearCons();
        for (int i = 0; i < cons.Length; i++)
        {
            Room.setClearCon(i, false);
        }

        StringWriter stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        //ACT
        //1) Not in same room as protag
        player.setCurrentRoom(rooms[0]);
        _testGame.processCommand(new Command(CommandWord.TALK, null));

        //2) Same room, weapon-location dialogue (clearCon[2] -> clearCon[5])
        player.setCurrentRoom(protag.getCurrentRoom());
        Room.setClearCon(2, true);
        Room.setClearCon(5, false);
        _testGame.processCommand(new Command(CommandWord.TALK, null));

        //3) Same room, way-forward dialogue (clearCon[3] -> clearCon[4])
        Room.setClearCon(3, true);
        Room.setClearCon(4, false);
        _testGame.processCommand(new Command(CommandWord.TALK, null));

        //4) Same room, nothing left to say
        _testGame.processCommand(new Command(CommandWord.TALK, null));

        //ASSERT
        var output = stringWriter.ToString().Replace("\r\n", "\n");

        // Some builds print a prompt like "> " that gets captured in tests. Strip it.
        while (output.StartsWith("> "))
        {
            output = output.Substring(2);
        }
        output = output.Replace("\n> ", "\n");

        Assert.Equal(
            "There's no-one to talk to!\n" +
            "You inform the protagonist of the location of a weapon.\n" +
            "You inform the protagonist of a way forward.\n" +
            "Nothing to say to the protagonist right now.\n",
            output);

        Assert.True(Room.getClearCons()[4]);
        Assert.True(Room.getClearCons()[5]);
    }

    [Fact]
    public void sleepTest()
    {
        //ARRANGE
        Player player = _testGame.getPlayer();
        List<Room> rooms = _testGame.allRooms;

        //Reset progression flags in case other tests ran first
        bool[] cons = Room.getClearCons();
        for (int i = 0; i < cons.Length; i++)
        {
            Room.setClearCon(i, false);
        }

        StringWriter stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        //ACT
        //1) Sleeping outside the start room should fail
        player.setCurrentRoom(rooms[2]);
        bool badRoomResult = _testGame.processCommand(new Command(CommandWord.SLEEP, null));

        //2) Sleeping in the start room without progression flags should fail
        player.setCurrentRoom(rooms[0]);
        bool notDoneResult = _testGame.processCommand(new Command(CommandWord.SLEEP, null));

        //3) Sleeping in the start room with progression flags should end the game
        Room.setClearCon(4, true);
        Room.setClearCon(5, true);
        bool doneResult = _testGame.processCommand(new Command(CommandWord.SLEEP, null));

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
        Player player = _testGame.getPlayer();
        Player protag = _testGame.getProtag();
        StringWriter stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        //ACT
        player.setCurrentRoom(protag.getCurrentRoom());
        bool result = _testGame.swordUse(); //calls protagKill() when in the same room

        //ASSERT
        Assert.True(result);

        var output = stringWriter.ToString().Replace("\r\n", "\n");
        Assert.Equal(
            "In a single mighty blow, you strike down the oblivious protagonist.\n" +
            "With this character's death the thread of prophecy... et cetera.\n",
            output);
    }
}
