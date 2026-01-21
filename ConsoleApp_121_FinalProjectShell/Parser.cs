//Transcribed Parser.java file - Adam Abbadusky

/// <summary>
/// The parser has a set of known command words. It checks user input against
/// the known commands, and if the input is not one of the known commands, it
/// returns a command object that is marked as an unknown command.
/// </summary>

namespace ConsoleApp_121_FinalProjectShell;

public class Parser 
{
    private CommandWords commands;  // holds all valid command words
    //private Scanner reader;       //not needed in c#

    /**
     * Create a parser to read from the terminal window.
     */
    public Parser() 
    {
        commands = new CommandWords();
        //scanner not needed in c#
        //private Scanner reader;
    }

    /**
     * @return The next command from the user.
     */
    public Command getCommand() 
    {
        String inputLine;   // will hold the full input line
        String word1 = null;
        String word2 = null;

        Console.Write("> ");     // print prompt

        inputLine = Console.ReadLine();

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
        return new Command(commands.GetCommandWord(word1), word2);
    }

    /**
     * Print out a list of valid command words.
     */
    public void showCommands()
    {
        commands.ShowAll();
    }
}