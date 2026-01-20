namespace ConsoleApp_121_FinalProjectShell;

/**
 * This class is part of the "World of Zuul" application.
 * "World of Zuul" is a very simple, text based adventure game.
 *
 * This parser reads user input and tries to interpret it as an "Adventure"
 * command. Every time it is called it reads a line from the terminal and
 * tries to interpret the line as a two-word command. It returns the command
 * as an object of class Command.
 *
 * The parser has a set of known command words. It checks user input against
 * the known commands, and if the input is not one of the known commands, it
 * returns a command object that is marked as an unknown command.
 *
 * @author  Michael Kölling and David J. Barnes
 * @version 2016.02.29
 */
public class Parser 
{
    private CmdWords commands;  // holds all valid command words

    /**
     * Create a parser to read from the terminal window.
     */
    public Parser() 
    {
        commands = new CmdWords();
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

        return new Command(commands.getCommandWord(word1), word2);
    }

    /**
     * Print out a list of valid command words.
     */
    public void showCommands()
    {
        commands.showAll();
    }
}