using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

//ADDED FOR TESTING:
//methods and properties need to be exposed to the testing project to enable unit testing on them
//Delete this comment in Sprint 5, demonstrates pull requst process
[assembly: InternalsVisibleTo("UnitTesting")]

namespace ConsoleApp_121_FinalProjectShell;

/**
 *  This class is the main class of the "World of Zuul" application. 
 *  "World of Zuul" is a very simple, text based adventure game.  Users 
 *  can walk around some scenery. That's all. It should really be extended 
 *  to make it more interesting!
 * 
 *  To play this game, create an instance of this class and call the "play"
 *  method.
 * 
 *  This main class creates and initialises all the others: it creates all
 *  rooms, creates the parser and starts the game.  It also evaluates and
 *  executes the commands that the parser returns.
 * 
 * @author  Michael Kölling and David J. Barnes
 * @version 2016.02.29
 */

public class Game
{
    private Parser parser;
    private Player player;
    private Player protag;
    public static Random random = new Random();

    // CHANGED: ArrayList is non-generic and indexing requires items to exist.
    // We keep ArrayList (to avoid refactoring logic), but we will populate it with Add()
    // instead of invalid index assignment.
    private ArrayList givenItems;

    //ADDED FOR TESTING:
    //enables testing of certain things
    private bool isTestInstance;
    //holds all the rooms created in the constructor for easier testing
    internal List<Room> allRooms;
    //hold all the items created in the constructor for easier testing
    internal List<Item> allItems;

    /*
     * Create the game and initialise its internal map.
     * Also initialises givenItems (used to hold items given by puzzles),
     * and random (a public instance of Random to be used by any object)
     *
     * ADDITION:
     * Now takes a boolean determining whether to perform extra functions
     * to enable easier testing
     * 
     */
    public Game(bool isTestInstance)
    {
        givenItems = new ArrayList();
        random = new Random();
        parser = new Parser();
        player = new Player(false);
        protag = new Player(true);

        this.isTestInstance = isTestInstance;

        //Loop 1
        if (!isTestInstance)
        {
            createRooms();
            return;
        }

        allRooms = new List<Room>();
        allItems = new List<Item>();
        createRooms();
    }



    /**
     * Create all the rooms and link their exits together.
     * In addition, creates and places all items in their respective places.
     */
    internal void createRooms()
    {
        Room hub, swamp, battleGr, rocky, lava, graves, castleGate, castleTown, altarGrove;
        Item axe, ring, hammer, ore, hilt, sword;

        // create the rooms
        hub = new Room("This campsite, used by travelers passing through, right now houses only you. \nA fitting place to rest when the job is done.", 0);
        swamp = new Room("Your boots catch in the stiff and stinking muck of the swamp. \nA large fallen log sits to one side.", 1);
        battleGr = new Room("The site of a once great battle stands silent, unusable weapons dotting the landscape.", 2);
        rocky = new Room("Standing at the remains of a collapsed quarry, you can see veins of ore within the stone.", 3);
        lava = new Room("Lava flows through channels dug into the rock around a vacant smith's shop, \nthe tools of which seem strangely lacking.", 4);
        graves = new Room("You stand in the graveyard where resurrected heroes first wake.", 5);
        castleGate = new Room("The wooden castle gate stands tall, imposing, and completely shut.", 6);
        castleTown = new Room("Standing in the deserted square of the castle's town, \nyou think at one point it must have been bustling with activity.", 7);
        altarGrove = new Room("Sunlight filters through the treetops into the solitary grove. \nA derelict altar stands at its center.", 8);


        //create the items
        axe = new Item("axe", "a battered war AXE", 55, 0);
        ring = new Item("ring", "a shining RING with a knight's insignia", 2, 1);
        hammer = new Item("hammer", "a standard issue craft HAMMER with a flat head", 34, 2);
        ore = new Item("ore", "a chunk of unrefined ORE", 46, 3);
        hilt = new Item("hilt", "a HILT of an old sword", 17, 4);
        sword = new Item("sword", "a sharp SWORD with a regal gleam", 22, 5);

        //TESTING ONLY
        //add the rooms and items to their respective testing lists if this is a testing instance
        //Loop 2
        if (isTestInstance)
        {
            foreach (var room in new[] { hub, swamp, battleGr, rocky, lava, graves, castleGate, castleTown, altarGrove })
            {
                allRooms.Add(room);
            }

            foreach (var item in new[] { axe, ring, hammer, ore, hilt, sword })
            {
                allItems.Add(item);
            }
        }

        // initialise room exits
        hub.setExit("north", castleTown);
        hub.setExit("cave", graves);
        hub.setExit("east", rocky);

        castleTown.setExit("south", hub);
        castleTown.setExit("north", castleGate);
        castleTown.setExit("east", battleGr);
        castleTown.setExit("northwest", swamp);

        swamp.setExit("southeast", castleTown);
        swamp.setExit("grove", altarGrove);

        battleGr.setExit("west", castleTown);
        battleGr.setExit("south", rocky);

        rocky.setExit("north", battleGr);
        rocky.setExit("west", hub);
        rocky.setExit("south", lava);

        lava.setExit("north", rocky);
        lava.setExit("slideward", graves);

        graves.setExit("exit", hub);

        castleGate.setExit("south", castleTown);

        altarGrove.setExit("swampward", swamp);

        //initialize items in rooms
        battleGr.addItem(axe);
        castleGate.addItem(ring);
        graves.addItem(hammer);
        altarGrove.addItem(hilt);

        // CHANGED: ArrayList does not support assigning to [0] / [1] unless elements exist.
        // The original code attempted:
        //   givenItems[0] = ore;
        //   givenItems-[1] = sword;   (also had a syntax error)
        // We populate in order with Add() so givenItems[0] and givenItems[1] still work later.
        givenItems.Add(ore);
        givenItems.Add(sword);

        player.setCurrentRoom(hub);
        // start game in the game hub
        protag.setCurrentRoom(graves);
        //protagonist starts in the obligatory area
    }

    /**
    *  Main play routine.  Loops until end of play.
    */
    public void play()
    {
        printWelcome();

        Command command;
        //Loop 3
        while (!processCommand(command = parser.getCommand()))
        {
            // keep looping until processCommand returns true (finished)
        }

        Console.WriteLine("Play again, if you'd like.");
    }
    //no need to test, very simple

    /**
     * Print out the opening message for the player.
     */
    private void printWelcome()
    {
        Console.WriteLine();
        Console.WriteLine("Welcome to Messed Up NPC with a Creepy Laugh!");
        Console.WriteLine("Help the dumb protagonist have the slimmest chance to survive.");
        Console.WriteLine("Type 'help' if you need help.");
        Console.WriteLine();
        printLocationInfo(player.getCurrentRoom());
    }
    //no need to test, just prints

    /**
     * Given a command, process (that is: execute) the command.
     * @param command The command to be processed.
     * @return true If the command ends the game, false otherwise.
     */
    internal bool processCommand(Command command)
    {
        CommandWord commandWord = command.GetCommandWord();

        //loop 4
        switch (commandWord)
        {
            case CommandWord.HELP:
                printHelp();
                protagMove();
                return false;

            case CommandWord.GO:
                goTo(command);
                printLocationInfo(player.getCurrentRoom());
                protagMove();
                return false;

            case CommandWord.QUIT:
                bool quitRequested = quit(command);
                protagMove();
                return quitRequested;

            case CommandWord.BACK:
                backTo();
                printLocationInfo(player.getCurrentRoom());
                protagMove();
                return false;

            case CommandWord.LOOK:
                printLocationInfo(player.getCurrentRoom());
                protagMove();
                return false;

            case CommandWord.TAKE:
                take(command);
                protagMove();
                return false;

            case CommandWord.DROP:
                drop(command);
                protagMove();
                return false;

            case CommandWord.ITEMS:
                itemsPrint();
                protagMove();
                return false;

            case CommandWord.USE:
                bool useRequestedQuit = use(command);
                protagMove();
                return useRequestedQuit;

            case CommandWord.TALK:
                talk();
                protagMove();
                return false;

            case CommandWord.SLEEP:
                bool sleepRequestedQuit = sleep();
                protagMove();
                return sleepRequestedQuit;

            case CommandWord.UNKNOWN:
            default:
                Console.WriteLine("I don't know what you mean...");
                return false;
        }
    }


    //basic functionality methods
    /**
     * Print out some help information.
     * Informs the player their goals and lists the available commands.
     */
    private void printHelp()
    {
        Console.WriteLine("Assist the protagonist in progressing through the beginning areas. \nThey will need to be able to obtain a weapon and have a way into the castle.");
        Console.WriteLine();
        Console.WriteLine("Your command words are:");
        parser.showCommands();
    }
    //no need to test, just prints and parser command is tested on its own


    /**
    * Prints the information of a location. Uses the longDesc from Room,
    * and accounts for the presence of the protagonist
    */
    //loop 5
    internal void printLocationInfo(Room currentRoom)
    {
        if (protag.getCurrentRoom() == currentRoom)
        {
            Console.WriteLine(currentRoom.getLongDesc() +
                "\nThe protagonist is here, bumbling about the area.");
            return;
        }

        Console.WriteLine(currentRoom.getLongDesc());
    }



    /** 
     * "Quit" was entered. Check the rest of the command to see
     * whether we really quit the game.
     * @return true, if this command quits the game, false otherwise.
     */
    internal bool quit(Command command)
    {
        //loop 6
        if (command.HasSecondWord())
        {
            Console.WriteLine("Quit what?");
            return false;
        }

        return true;  // signal that we want to quit
    }


    //inventory methods
    /**
     * Tries to move a given item from the current room into the player's inventory
     * Calls weightCheck to make sure the player has the allowance to do so
     */
    internal void take(Command command)
    {
        //loop 6
        if (!command.HasSecondWord())
        {
            Console.WriteLine("Take what?");
            return;
        }

        string itemName = command.GetSecondWord();
        Item tempItem = player.getCurrentRoom().getItemByName(itemName);

        //loop 7
        if (tempItem == null)
        {
            Console.WriteLine("There isn't anything like that around.");
            return;
        }
        //loop 8
        if (!player.weightCheck(tempItem.getWeight()))
        {
            Console.WriteLine("That's too heavy to carry right now.");
            return;
        }

        player.addItem(tempItem);
        player.getCurrentRoom().removeItemByName(itemName);
        Console.WriteLine("Picked up the " + tempItem.getName() + "!");

        // BUG FIX: string comparison should use .Equals()
        //loop 9
        if (tempItem.getName().Equals("hammer") && player.getCurrentRoom().getID() == 4)
        {
            Room.setClearCon(1, false);
            Console.WriteLine("The forge's tool set is once again incomplete.");
        }
    }


    /**
     * Similar to take(), but lacks a weight check
     */
    internal void drop(Command command)
    {
        //loop 10
        if (!command.HasSecondWord())
        {
            Console.WriteLine("Drop what?");
            return;
        }

        string itemName = command.GetSecondWord();
        Item tempItem = player.getItemByName(itemName);

        //loop 11
        if (tempItem == null)
        {
            Console.WriteLine("You don't have anything like that.");
            return;
        }

        player.getCurrentRoom().addItem(tempItem);
        player.removeItemByName(itemName);
        Console.WriteLine("Dropped the " + tempItem.getName() + "!");
    }


    private void itemsPrint()
    {
        Console.WriteLine(player.itemsText());
    }
    //Just calls a Player method, testing should be done over there


    //methods for moving the player
    /**
     * Calls player's goRoom() method to determine what should be printed
     */
    private void goTo(Command command)
    {
        int result = player.goRoom(command);

        //successful move no action needed
        if (result == 1)
        {
            return;
        }

        //loop 12
        switch (result)
        {
            case 0:
                Console.WriteLine("Go where?");
                break;
            case -1:
                Console.WriteLine("There is no path!");
                break;
            case 2:
                Console.WriteLine("Woohoo!");
                break;
            default:
                Console.WriteLine("Something has gone terribly wrong.");
                break;
        }
    }


    /**
     * Attempts to move the player to the last room they were in.
     */
    private void backTo()
    {
        //loop 13
        if (player.back() != 0)
        {
            return;
        }

        Console.WriteLine("You haven't gone anywhere!");
    }
    //the movement logic for the game is all handled by the Player class, these methods just
    //call it when needed, and write the corresponding lines
    //could theoretically bundle the messages into the Player class as the return type,
    //and remove these methods all-together

    /**
     * Methods for item functionality
     *
     * I copied much of the code for use(), take(), and drop() from Player's goRoom()
     * as they have similar requirements in terms of what format command they parse
     */
    internal bool use(Command command)
    {
        //loop 14
        if (!command.HasSecondWord())
        {
            Console.WriteLine("Use what?");
            return false;
        }

        string item = command.GetSecondWord();

        //loop 15
        if (!player.hasItemByName(item))
        {
            Console.WriteLine("You don't have an item like that.");
            return false;
        }

        return itemSwitch(player.getItemByName(item).getID());
    }



    /**
     * Switch statement that determines what methods to run based on the ID of the item stated
     * The default case should never be triggered unless you add a new item and don't add a case for it
     */
    internal bool itemSwitch(int ID)
    {
        //loop 16
        switch (ID)
        {
            case 0:
                return axeUse();

            case 1:
                ringUse();
                return false;

            case 2:
                hammerUse();
                return false;

            case 3:
                Console.WriteLine("There's nothing to do with this on its own.");
                return false;

            case 4:
                hiltUse();
                return false;

            case 5:
                return swordUse();

            default:
                Console.WriteLine("Something has gone terribly wrong.");
                return false;
        }
    }


    //methods that determine what happens when a particular item is used
    private bool axeUse()
    {
        //loop 17
        if (player.getCurrentRoom().getID() == 1 && !Room.getClearCons()[0])
        {
            Room.setClearCon(0, true);
            Console.WriteLine("You chop the large log into several more easily navigable pieces.");
            Console.WriteLine("Beyond where it stood is revealed the entrance to a hidden grove.");
            return false;
        }

        //loop 18
        if (player.getCurrentRoom().getID() == 6 && !Room.getClearCons()[3])
        {
            Room.setClearCon(3, true);
            Console.WriteLine("Utilizing the hefty weight of the axe, you smash a hole through the wooden gate.");
            return false;
        }

        //loop 19
        if (player.getCurrentRoom() == protag.getCurrentRoom())
        {
            return protagKill();
        }

        Console.WriteLine("Nothing to do with that here.");
        return false;
    }


    private void ringUse()
    {
        player.setCarryWeight(150);
        player.removeItemByName("ring");
        Console.WriteLine("By equipping the ring, your maximum carryable weight has increased.");
    }

    internal void hammerUse()
    {
        Room currentRoom = player.getCurrentRoom();
        int roomId = currentRoom.getID();

        //loop 20
        switch (roomId)
        {
            case 3:
                // prevent spawning ore repeatedly in the quarry
                if (player.hasItemByName("ore") || currentRoom.hasItemByName("ore"))
                {
                    Console.WriteLine("Nothing to do with that here.");
                    break;
                }

                currentRoom.addItem((Item)givenItems[0]);
                Console.WriteLine("A chunk of ore falls to the ground as you break it free from the surrounding rock.");
                break;

            case 4:
                Item hammer = player.getItemByName("hammer");
                currentRoom.addItem(hammer);
                player.removeItemByName("hammer");

                Room.setClearCon(1, true);
                Console.WriteLine("You place the hammer with the set of forge tools, completing the set.");
                break;

            default:
                Console.WriteLine("Nothing to do with that here.");
                break;
        }
    }


    internal void hiltUse()
    {
        //loop 21
        if (!player.hasItemByName("ore") ||
            player.getCurrentRoom().getID() != 4 ||
            !Room.getClearCons()[1])
        {
            Console.WriteLine("Can't do anything with that right now.");
            return;
        }

        player.removeItemByName("ore");
        player.removeItemByName("hilt");

        // givenItems is an ArrayList, so index access returns object
        player.getCurrentRoom().addItem((Item)givenItems[1]);

        Console.WriteLine("Forged the hilt into a new sword!");
    }


    internal bool swordUse()
    {
        //loop 22
        if (player.getCurrentRoom().getID() == 8)
        {
            player.removeItemByName("sword");
            Room.setClearCon(2, true);
            Console.WriteLine(
                "You place the sword within the altar, now only to be obtained by a true hero.");
            return false;
        }

        //loop 23
        if (player.getCurrentRoom() == protag.getCurrentRoom())
        {
            return protagKill();
        }

        Console.WriteLine("Nothing to do with that here.");
        return false;
    }


    /*
     * talk() triggers progression flags for the end of the game when conditions are met
     * sleep() ends the game if those progression flags are true
     */

    private void talk()
    {
        //loop 24
        if (player.getCurrentRoom() != protag.getCurrentRoom())
        {
            Console.WriteLine("There's no-one to talk to!");
            return;
        }

        //loop 25
        if (Room.getClearCons()[2] && !Room.getClearCons()[5])
        {
            Room.setClearCon(5, true);
            Console.WriteLine("You inform the protagonist of the location of a weapon.");
            return;
        }

        //loop 26
        if (Room.getClearCons()[3] && !Room.getClearCons()[4])
        {
            Room.setClearCon(4, true);
            Console.WriteLine("You inform the protagonist of a way forward.");
            return;
        }

        Console.WriteLine("Nothing to say to the protagonist right now.");
    }


    private bool sleep()
    {
        //loop 27
        if (player.getCurrentRoom().getID() != 0)
        {
            Console.WriteLine("This is a terrible place to sleep.");
            return false;
        }

        //loop 28
        if (!Room.getClearCons()[4] || !Room.getClearCons()[5])
        {
            Console.WriteLine("You've not finished all that you need to!");
            return false;
        }

        Console.WriteLine(
            "You lay your head down to sleep, your (likely fruitless) endeavors complete.");
        return true;
    }


    /*
     * called when using the axe or the sword in the same room as the protagonist without 
     * anything else to do with it there
     * 
     * ends the game
     */

    private bool protagKill()
    {
        Console.WriteLine("In a single mighty blow, you strike down the oblivious protagonist.");
        Console.WriteLine("With this character's death the thread of prophecy... et cetera.");
        return true;
    }

    /*
     * called after every recognized command
     * initiates protagSteps() in Player
     */
    private void protagMove()
    {
        //loop 29
        Room current = protag.getCurrentRoom();
        if (current == null)
        {
            return;
        }

        // If there are no exits, don't try to move (prevents crash)
        var exits = current.getExits();
        //loop 30
        if (exits == null || exits.Count == 0)
        {
            return;
        }

        string direction = current.getRandomExit();
        //loop 31
        if (string.IsNullOrWhiteSpace(direction))
        {
            return;
        }

        Command command = new Command(CommandWord.GO, direction);
        protag.protagSteps(command);
    }



    /*
     * The following are a set of internal accessor methods used for testing various parts of this class.
     * Do not modify.
     */
    internal Player getPlayer() { return player; }
    internal Player getProtag() { return protag; }
}
