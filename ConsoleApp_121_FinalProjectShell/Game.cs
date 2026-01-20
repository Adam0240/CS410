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
    private Room currentRoom;

    /**
     * Create the game and initialise its internal map.
     */
    public Game() 
    {
        createRooms();
        parser = new Parser();
    }

    /**
     * Create all the rooms and link their exits together.
     */
    private void createRooms()
    {
        Room office;
        Room livingRoom;

        // create the rooms
        office = new Room("office");
        livingRoom = new Room("living room");

        // initialise room exits
        office.setExit("east", livingRoom);
        livingRoom.setExit("west", office);

        currentRoom = office;  // start game outside
    }

    /**
     *  Main play routine.  Loops until end of play.
     */
    public void play() 
    {            
        printWelcome();

        // Enter the main command loop.  Here we repeatedly read commands and
        // execute them until the game is over.

        bool finished = false; //notifies program the user wishes to end the game

        Command command = parser.getCommand();
        finished = processCommand(command);

        Console.WriteLine("Thank you for playing.  Good bye.");
    }

    /**
     * Print out the opening message for the player.
     */
    private void printWelcome()
    {
        Console.WriteLine(currentRoom.getIdentifier());
    }

    /**
     * Given a command, process (that is: execute) the command.
     * @param command The command to be processed.
     * @return true If the command ends the game, false otherwise.
     */
    private bool processCommand(Command command) 
    {
        bool wantToQuit = false;

        CommandWord commandWord = command.getCommandWord();

        switch (commandWord) {
            case CommandWord.UNKNOWN:
                Console.WriteLine("I don't know what you mean...");
                break;
            case CommandWord.HELP:
                printHelp();
                break;
            case CommandWord.WALK:
                goRoom(command);
                break;
            case CommandWord.QUIT:
                wantToQuit = quit(command);
                break;
        }
        return wantToQuit;
    }

    // implementations of user commands:

    /**
     * Print out some help information.
     * Here we print a message and a list of the command words.
     */
    private void printHelp() 
    {
        Console.WriteLine("Your command words are:");
        parser.showCommands();
    }

    /** 
     * Try to go in one direction. If there is an exit, enter the new
     * room, otherwise print an error message.
     */
    private void goRoom(Command command) 
    {
        if(!command.hasSecondWord()) {
            // if there is no second word, we don't know where to go...
            Console.WriteLine("Go where?");
            return;
        }

        String direction = command.getSecondWord();

        // Try to leave current room.
        Room nextRoom = currentRoom.getExit(direction);

        if (nextRoom == null) {
            Console.WriteLine("There is no door!");
        }
        else {
            currentRoom = nextRoom;
            Console.WriteLine(currentRoom.getIdentifier());
        }
    }

    /** 
     * "Quit" was entered. Check the rest of the command to see
     * whether we really quit the game.
     * @return true, if this command quits the game, false otherwise.
     */
    private bool quit(Command command) 
    {
        if(command.hasSecondWord()) {
            Console.WriteLine("Quit what?");
            return false;
        }
        else {
            return true;  // signal that we want to quit
        }
    }
}
