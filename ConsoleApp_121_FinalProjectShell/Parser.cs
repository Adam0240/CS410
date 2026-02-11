//Transcribed Parser.java file - Adam Abbadusky

// The parser has a set of known command words. It checks user input against
// the known commands, and if the input is not one of the known commands, it
// returns a command object that is marked as an unknown command.

using System;

namespace ConsoleApp_121_FinalProjectShell;

public class Parser 
{
    private CommandWords _commands;  // holds all valid command words

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
    public Command getCommand() 
    {
        String word1 = null;
        String word2 = null;

        Console.Write("> ");     // print prompt

        var inputLine = Console.ReadLine(); // will hold the full input line

        // Find up to two words on the line.
        string[] tokenizer = inputLine.Split(' ');
        if(tokenizer.Length > 0) {
            word1 = tokenizer[0];      // get first word
            if(tokenizer.Length >= 2) {
                word2 = tokenizer[1];      // get second word
                // note: we just ignore the rest of the input line.
            }
        }

        //bug: may cause error if word1 is null
        return new Command(_commands.GetCommandWord(word1), word2);
    }

    /**
     * Print out a list of valid command words.
     */
    public void showCommands()
    {
        _commands.ShowAll();
    }
}