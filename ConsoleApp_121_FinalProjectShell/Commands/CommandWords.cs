//Transcribed CommandWords.java file - Adam Abbadusky

using System;
using System.Collections.Generic;

namespace ConsoleApp_121_FinalProjectShell.Commands;

public class CommandWords 
{
    // a Dictionary that holds strings of the command words
    private readonly Dictionary<string, CommandWord> _validCommands;

    /// <summary>
    /// Constructor - initialise the command words.
    /// </summary>
    public CommandWords()
    {
        _validCommands = new Dictionary<string, CommandWord>
        {
            { "go", CommandWord.GO },
            { "walk", CommandWord.GO },
            { "move", CommandWord.GO },
            { "help", CommandWord.HELP },
            { "quit", CommandWord.QUIT },
            { "back", CommandWord.BACK },
            { "look", CommandWord.LOOK },
            { "take", CommandWord.TAKE },
            { "drop", CommandWord.DROP },
            { "items", CommandWord.ITEMS },
            { "use", CommandWord.USE },
            { "talk", CommandWord.TALK },
            { "sleep", CommandWord.SLEEP },
            { "follow", CommandWord.FOLLOW },
            { "stay", CommandWord.STAY},
            { "trade", CommandWord.TRADE}, 
            //Save State Edit 3
            { "save", CommandWord.SAVE },
            //Save State Edit 4
            { "load", CommandWord.LOAD },
            //Save State Edit 26
            { "delete", CommandWord.DELETE },
        };
    }

    /// <summary>
    /// Check whether a given string is a valid command word.
    /// </summary>
    /// <returns>
    /// True if a given string is a valid command, false if it isn't.
    /// </returns>
    public bool IsCommand(string aString) //bug will throw an exception if aString is null, difference with Java hashmaps vs c# dictionary
    {
        return _validCommands.ContainsKey(aString);
    }


    /// <summary>
    /// Get the CommandWord associated with a given command string.
    /// </summary>
    public CommandWord GetCommandWord(string commandWord)
    {
        // Bug:  Dictionary does not allow null keys and this will throw an exception if commandWord is null. 
        return _validCommands.GetValueOrDefault(commandWord, CommandWord.UNKNOWN);
    }

    /// <summary>
    /// Print all valid commands to the console.
    /// </summary>
    public void ShowAll()
    {
        foreach (string command in _validCommands.Keys)
        {
            Console.Write(command + "  ");
        }
        Console.WriteLine();
    }

}
