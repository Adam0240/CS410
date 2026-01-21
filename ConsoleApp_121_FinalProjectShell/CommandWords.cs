//Transcribed CommandWords.java file - Adam Abbadusky

using System.Collections.Generic;       //changed from import java.util.HashMap

public class CommandWords
{
    // a Dictionary that holds strings of the command words
    private Dictionary<string, CommandWord> validCommands;

    /// <summary>
    /// Constructor - initialise the command words.
    /// </summary>
    public CommandWords()
    {
        validCommands = new Dictionary<string, CommandWord>();      //changed from private HashMap<String, CommandWord> validCommands;

        validCommands.Add("walk", CommandWord.GO);          //adds to dictionary with 'add' instead of 'put'
        validCommands.Add("help", CommandWord.HELP);
        validCommands.Add("EndGame", CommandWord.QUIT);     //possible bug doesn't match "quit"
        validCommands.Add("back", CommandWord.BACK);
        validCommands.Add("look", CommandWord.LOOK);
        validCommands.Add("take", CommandWord.TAKE);
        validCommands.Add("drop", CommandWord.DROP);
        validCommands.Add("items", CommandWord.ITEMS);
        validCommands.Add("use", CommandWord.USE);
        validCommands.Add("talk", CommandWord.TALK);
        validCommands.Add("sleep", CommandWord.SLEEP);
    }

    /// <summary>
    /// Check whether a given string is a valid command word.
    /// </summary>
    /// <returns>
    /// True if a given string is a valid command, false if it isn't.
    /// </returns>
    public bool IsCommand(string aString)           //bug will throw an exception if aString is null, difference with Java hashmaps vs c# dictionary
    {
        return validCommands.ContainsKey(aString);
    }


    /// <summary>
    /// Get the CommandWord associated with a given command string.
    /// </summary>
    public CommandWord GetCommandWord(string commandWord)
    {
        // Bug:  Dictionary does not allow null keys and this will throw an exception if commandWord is null. 
        if (validCommands.ContainsKey(commandWord))
        {
            return validCommands[commandWord];
        }
        else
        {
            return CommandWord.UNKNOWN;
        }
    }

    /// <summary>
    /// Print all valid commands to the console.
    /// </summary>
    public void ShowAll()
    {
        foreach (string command in validCommands.Keys)
        {
            Console.Write(command + "  ");
        }
        Console.WriteLine();
    }

}