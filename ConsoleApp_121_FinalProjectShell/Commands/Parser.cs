//Transcribed Parser.java file - Adam Abbadusky

// The parser has a set of known command words. It checks user input against
// the known commands, and if the input is not one of the known commands, it
// returns a command object that is marked as an unknown command.

using System;

namespace ConsoleApp_121_FinalProjectShell.Commands;

public class Parser 
{
    private readonly CommandWords _commands;  // holds all valid command words

    /**
     * Create a parser to read from the terminal window.
     */
    public Parser() 
    {
        _commands = new CommandWords();
    }

    /**
     * @return The next command from the user.
     */
    public Command GetCommand() 
    {
        Console.Write("> ");     // print prompt

        string? inputLine = Console.ReadLine();

        // Multiplayer Change 1:
        // Route console input through the shared parser helper so network-delivered commands
        // can be parsed by the exact same logic.
        return ParseCommand(inputLine);
    }

    public Command ParseCommand(string? inputLine)
    {
        string? word1 = null;
        string? word2 = null;

        if (string.IsNullOrWhiteSpace(inputLine))
        {
            return new Command(CommandWord.UNKNOWN, null);
        }

        string[] tokenizer = inputLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (tokenizer.Length > 0)
        {
            word1 = tokenizer[0];
            if (tokenizer.Length >= 2)
            {
                word2 = tokenizer[1];
            }
        }

        if (word1 == null)
        {
            return new Command(CommandWord.UNKNOWN, null);
        }

        return new Command(_commands.GetCommandWord(word1), word2);
    }

    /**
     * Get a singular word as input from the player. Used for selections in submenus.
     */
    public static string GetSingleCommand()
    {
        string? word = null;

        Console.Write("> ");

        string? inputLine = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(inputLine))
        {
            return string.Empty;
        }

        string[] tokenizer = inputLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (tokenizer.Length > 0)
        {
            word = tokenizer[0];
        }

        if (word == null)
        {
            return string.Empty;
        }

        return word.ToLower();
    }

    /**
     * Print out a list of valid command words.
     */
    public void ShowCommands()
    {
        _commands.ShowAll();
    }
}
