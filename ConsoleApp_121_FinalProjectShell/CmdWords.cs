using System.Collections;

namespace ConsoleApp_121_FinalProjectShell;

/**
 * This class is part of the "World of Zuul" application.
 * "World of Zuul" is a very simple, text based adventure game.
 *
 * This class holds an enumeration of all command words known to the game.
 * It is used to recognise commands as they are typed in.
 *
 * @author  Michael Kölling and David J. Barnes
 * @version 2016.02.29
 */

public class CmdWords
{
    // A mapping between a command word and the CommandWord
    // associated with it.
    private Hashtable validCommands;

    /**
     * Constructor - initialise the command words.
     */
    public CmdWords()
    {
        validCommands = new Hashtable();
        foreach (CommandWord command in Enum.GetValues<CommandWord>()) {
            if(command != CommandWord.UNKNOWN) {
                validCommands.Add(command.ToString().ToLower(), command);
            }
        }
    }

    /**
     * Find the CommandWord associated with a command word.
     * @param commandWord The word to look up.
     * @return The CommandWord correspondng to commandWord, or UNKNOWN
     *         if it is not a valid command word.
     */
    public CommandWord getCommandWord(String commandWord)
    {
        CommandWord command = (CommandWord) validCommands[commandWord];
        if(command != null) {
            return command;
        }
        else {
            return CommandWord.UNKNOWN;
        }
    }
    
    /**
     * Check whether a given String is a valid command word.
     * @return true if it is, false if it isn't.
     */
    public bool isCommand(String aString)
    {
        return validCommands.ContainsKey(aString);
    }

    /**
     * Print all valid commands to System.out.
     */
    public void showAll() 
    {
        foreach(String command in validCommands.Keys) {
            Console.Write(command + "  ");
        }
        Console.WriteLine();
    }
}