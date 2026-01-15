import java.util.*;
/**
 * This class is the main class of the "FUGwACL Adventure" application. 
 * "FUGwACL Adventure" is a slightly less simple, text based adventure game.  
 * 
 * Created as a silly reference to the recurring character archetype in From Software's
 * series of third-person action-rpgs that began with Demon's Souls.
 * 
 * This Class contains most scripting that runs the appplication.
 * 
 * @author  Michael Kölling, David J. Barnes, and Christian Byrne
 * @version 2023-12-03
 */

public class Game 
{
    private Parser parser;
    private Player player;
    private Player protag;
    public static Random random;

    private ArrayList<Item> givenItems;
    /**
     * Create the game and initialise its internal map.
     * Also initializes givenItems (used to hold items given by puzzles),
     * and random (a public instance of Random to be used by any object)
     */
    public Game() 
    {
        givenItems = new ArrayList<>();
        random = new Random();
        parser = new Parser();
        player = new Player(false);
        protag = new Player(true);
        createRooms();
    }

    /**
     * Create all the rooms and link their exits together.
     * In addition, creates and places all items in their respective places.
     */
    private void createRooms()
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
        lava .setExit("slideward", graves);

        graves.setExit("exit", hub);

        castleGate.setExit("south", castleTown);

        altarGrove.setExit("swampward", swamp);

        //initialize items in rooms
        battleGr.addItem(axe);
        castleGate.addItem(ring);
        graves.addItem(hammer);
        altarGrove.addItem(hilt);

        //initialize items to be given when conditions are met
        givenItems.add(0, ore);
        givenItems.add(1, sword);

        player.setCurrentRoom(hub);
        // start game in the game hub
        protag.setCurrentRoom(graves);
        //protagonist starts in the obligatory area
    }

    /**
     *  Main play routine.  Loops until end of play.bu
     */
    public void play() 
    {            
        printWelcome();

        // Enter the main command loop.  Here we repeatedly read commands and
        // execute them until the game is over.

        boolean finished = false;
        while (! finished) {
            Command command = parser.getCommand();
            finished = processCommand(command);
        }

        System.out.println("Play again, if you'd like.");
    }

    /**
     * Print out the opening message for the player.
     */
    private void printWelcome()
    {
        System.out.println();
        System.out.println("Welcome to Messed Up NPC with a Creepy Laugh!");
        System.out.println("Help the dumb protagonist have the slimmest chance to survive.");
        System.out.println("Type 'help' if you need help.");
        System.out.println();
        printLocationInfo(player.getCurrentRoom());
    }

    /**
     * Given a command, process (that is: execute) the command.
     * @param command The command to be processed.
     * @return true If the command ends the game, false otherwise.
     */
    private boolean processCommand(Command command) 
    {
        boolean wantToQuit = false;

        CommandWord commandWord = command.getCommandWord();
        switch(commandWord)
        {
            case HELP:
                printHelp();
                break;
            case GO:
                goTo(command);
                printLocationInfo(player.getCurrentRoom());
                break;
            case QUIT:
                wantToQuit = quit(command);
                break;
            case BACK:
                backTo();
                printLocationInfo(player.getCurrentRoom());
                break;
            case LOOK:
                printLocationInfo(player.getCurrentRoom());
                break;
            case TAKE:
                take(command);
                break;
            case DROP:
                drop(command);
                break;
            case ITEMS:
                itemsPrint();
                break;
            case USE:
                wantToQuit = use(command);
                break;
            case TALK:
                talk();
                break;
            case SLEEP:
                wantToQuit = sleep();
                break;
            case UNKNOWN:
                System.out.println("I don't know what you mean...");
                break;
        }
        //initiates the protagonist's move routine after every command
// BUG 1: The protagonist moves even when the player types an invalid command.
// Example: typing "asdf" prints "I don't know what you mean..." but protagMove()
// still runs and the protagonist changes rooms anyway
        protagMove();
        return wantToQuit;
    }

    //basic functionality methods
    /**
     * Print out some help information.
     * Informs the player their goals and lists the available commands.
     */
    private void printHelp() 
    {
        System.out.println("Assist the protagonist in progressing through the beginning areas. \nThey will need to be able to obtain a weapon and have a way into the castle.");
        System.out.println();
        System.out.println("Your command words are:");
        parser.showCommands();   
    }

    /**
     * Prints the information of a location. Uses the longDesc from Room, 
     * and accounts for the presence of the protagonist
     */
    private void printLocationInfo(Room currentRoom)
    {
        if (protag.getCurrentRoom() == currentRoom)
        {
            System.out.println(currentRoom.getLongDesc() + "\nThe protagonist is here, bumbling about the area.");
        } else { System.out.println(currentRoom.getLongDesc()); }
    }

    /** 
     * "Quit" was entered. Check the rest of the command to see
     * whether we really quit the game.
     * @return true, if this command quits the game, false otherwise.
     */
    private boolean quit(Command command) 
    {
        if(command.hasSecondWord()) {
            System.out.println("Quit what?");
            return false;
        }
        else {
            return true;  // signal that we want to quit
        }
    }

    //inventory methods
    /**
     * Tries to move a given item from the current room into the player's inventory
     * Calls weightCheck to make sure the player has the allowance to do so
     */
    private void take(Command command)
    {
        if(!command.hasSecondWord()) {
            // if there is no second word, we don't know what to take...
            System.out.println("Take what?");
            return;
        }

        String itemName = command.getSecondWord();
        Item tempItem = player.getCurrentRoom().getItemByName(itemName);
        if (tempItem != null)
        {
            if (player.weightCheck(tempItem.getWeight()))
            {
                player.addItem(tempItem);
                player.getCurrentRoom().removeItemByName(itemName);
                System.out.println("Picked up the " + tempItem.getName() + "!");
                // BUG 2: This compares strings using == instead of .equals().
                if (tempItem.getName() == "hammer" && player.getCurrentRoom().getID() == 4)
                {
                    Room.setClearCon(1, false);
                    System.out.println("The forge's tool set is once again incomplete.");
                }
            } else
            {
                System.out.println("That's too heavy to carry right now.");
            }
        }
        else
        {
            System.out.println("There isn't anything like that around.");
        }
    }

    /**
     * Similar to take(), but lacks a weight check
     */
    private void drop(Command command)
    {
        if(!command.hasSecondWord()) {
            // if there is no second word, we don't know what to take...
            System.out.println("Drop what?");
            return;
        }

        String itemName = command.getSecondWord();
        Item tempItem = player.getItemByName(itemName);
        if (tempItem != null)
        {
            player.getCurrentRoom().addItem(tempItem);
            player.removeItemByName(itemName);
            System.out.println("Dropped the " + tempItem.getName() + "!");
        } else {System.out.println("You don't have anything like that.");}
    }

    private void itemsPrint()
    {
        System.out.println(player.itemsText());
    }

    //methods for moving the player
    /**
     * Calls player's goRoom() method to determine what should be printed
     */
    private void goTo(Command command)
    {
        switch (player.goRoom(command))
        {
            case 0:
                System.out.println("Go where?");
                break;
            case -1:
                System.out.println("There is no path!");
                break;
            case 1:
                break;
            case 2:
                System.out.println("Woohoo!");
                break;
            default:
                System.out.println("Something has gone terribly wrong.");
                break;
        }
    }

    /**
     * Attempts to move the player to the last room they were in.
     */
    private void backTo()
    {
        if (player.back() == 0)
        {
            System.out.println("You haven't gone anywhere!");
        }
    }

    
    /**
     * Methods for item functionality
     * 
     * I copied much of the code for use(), take(), and drop() from Player's goRoom() 
     * as they have similar requirements in terms of what format command they parse
     */
    private boolean use(Command command)
    {
        if(!command.hasSecondWord()) {
            System.out.println("Use what?");
            return false;
        }

        String item = command.getSecondWord();

        if (player.hasItemByName(item))
        {
            return itemSwitch(player.getItemByName(item).getID());
        }
        else 
        {
            System.out.println("You don't have an item like that.");
            return false;
        }
    }

    /**
     * Switch statement that determines what methods to run based on the ID of the item stated
     * The default case should never be triggered unless you add a new item and don't add a case for it
     */
    private boolean itemSwitch(int ID)
    {
        boolean quitBool = false;

        switch (ID)
        {
            case 0:
                quitBool = axeUse();
                break;
            case 1:
                ringUse();
                break;
            case 2:
                hammerUse();
                break;
            case 3:
                // oreUse() was not necessary
                System.out.println("There's nothing to do with this on its own.");
                break;
            case 4:
                hiltUse();
                break;
            case 5:
                quitBool = swordUse();
                break;
            default:
                System.out.println("Something has gone terribly wrong.");
                break;
        }
        return quitBool;
    }

    //methods that determine what happens when a particular item is used
    private boolean axeUse()
    {
        boolean quitAxe = false;
        if (player.getCurrentRoom().getID() == 1 && !Room.getClearCons()[0])
        {
            Room.setClearCon(0, true);
            System.out.println("You chop the large log into several more easily navigable pieces.");
            System.out.println("Beyond where it stood is revealed the entrance to a hidden grove.");
        } else if(player.getCurrentRoom().getID() == 6 && !Room.getClearCons()[3])
        {
            Room.setClearCon(3, true);
            System.out.println("Utilizing the hefty weight of the axe, you smash a hole through the wooden gate.");
        } else if (player.getCurrentRoom() == protag.getCurrentRoom())
        {
            quitAxe = protagKill();
        } else
        {
            System.out.println("Nothing to do with that here.");
        }  
        return quitAxe;
    }

    private void ringUse()
    {
        player.setCarryWeight(150);
        player.removeItemByName("ring");
        System.out.println("By equipping the ring, your maximum carryable weight has increased.");
    }

    private void hammerUse()
    {
        switch (player.getCurrentRoom().getID())
        {
            case 3:
                // BUG 3: Using the hammer in the quarry can be done over and over.
                // Each use adds the same ore item again, so the game keeps spawning ore.
                // It looks like only one ore exists because they all reference the same item,
                // but the room state is still being duplicated.
                player.getCurrentRoom().addItem(givenItems.get(0));
                System.out.println("A chunk of ore falls to the ground as you break it free from the surrounding rock.");
                break;
            case 4:
                player.getCurrentRoom().addItem(player.getItemByName("hammer"));
                player.removeItemByName("hammer");
                Room.setClearCon(1, true);
                System.out.println("You place the hammer with the set of forge tools, completing the set.");
                break;
            default:
                System.out.println("Nothing to do with that here.");
                break;
        }
    }

    private void hiltUse()
    {
        if (player.hasItemByName("ore") && player.getCurrentRoom().getID() == 4 && Room.getClearCons()[1])
        {
            player.removeItemByName("ore");
            player.removeItemByName("hilt");
            player.getCurrentRoom().addItem(givenItems.get(1));
            System.out.println("Forged the hilt into a new sword!");
        } else 
        {
            System.out.println("Can't do anything with that right now.");
        }
    }

    private boolean swordUse()
    {
        if (player.getCurrentRoom().getID() == 8)
        {
            player.removeItemByName("sword");
            Room.setClearCon(2, true);
            System.out.println("You place the sword within the altar, now only to be obtained by a true hero.");
        }
        else if (player.getCurrentRoom() == protag.getCurrentRoom())
        {
            return protagKill();
        } else 
        {
            System.out.println("Nothing to do with that here.");
        }
        return false;
    }

    /**
     * talk() triggers progression flags for the end of the game when conditions are met
     * sleep() ends the game if those progression flags are true
     */
    private void talk()
    {
        if (player.getCurrentRoom() == protag.getCurrentRoom())
        {
            if (Room.getClearCons()[2] && !Room.getClearCons()[5])
            {
                Room.setClearCon(5, true);
                System.out.println("You inform the protagonist of the location of a weapon.");
            } else if(Room.getClearCons()[3] && !Room.getClearCons()[4])
            {
                Room.setClearCon(4, true);
                System.out.println("You inform the protagonist of a way forward.");
            } else
            {
                System.out.println("Nothing to say to the protagonist right now.");
            }
        } else {System.out.println("There's no-one to talk to!");}
    }

    private boolean sleep()
    {
        boolean quitSleep = false;
        if (player.getCurrentRoom().getID() == 0)
        {
            if (Room.getClearCons()[4] && Room.getClearCons()[5])
            {
                System.out.println("You lay your head down to sleep, your (likely fruitless) endeavors complete.");
                quitSleep = true;
            } else {System.out.println("You've not finished all that you need to!");}
        } else {System.out.println("This is a terrible place to sleep.");}
        return quitSleep;
    }

    /**
     * called when using the axe or the sword in the same room as the protagonist without 
     * anything else to do with it there
     * 
     * ends the game
     */
    private boolean protagKill()
    {
        System.out.println("In a single mighty blow, you strike down the oblivious protagonist.");
        System.out.println("With this character's death the thread of prophecy... et cetera."); 
        return true;
    }

    /**
     * called after every recognized command
     * initiates protagSteps() in Player
     */
    private void protagMove()
    {
        //Bug 4 GetRandomExit may return null if the room has no exits. 
        Command command = new Command(CommandWord.GO, protag.getCurrentRoom().getRandomExit());
        protag.protagSteps(command);
    }

    public static void main(String[] args) {
    Game game = new Game();
    game.play();
}

}
