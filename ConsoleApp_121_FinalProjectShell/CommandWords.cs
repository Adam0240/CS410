//Transcribed CommandWords.java file - Adam Abbadusky
namespace ConsoleApp_121_FinalProjectShell;

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
            { "walk", CommandWord.GO }, //adds to dictionary with 'add' instead of 'put' - fixed, now uses collection initializer
            { "help", CommandWord.HELP },
            { "EndGame", CommandWord.QUIT }, //possible bug doesn't match "quit"
            { "back", CommandWord.BACK },
            { "look", CommandWord.LOOK },
            { "take", CommandWord.TAKE },
            { "drop", CommandWord.DROP },
            { "items", CommandWord.ITEMS },
            { "use", CommandWord.USE },
            { "talk", CommandWord.TALK },
            { "sleep", CommandWord.SLEEP }
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
        return _validCommands.ContainsKey(commandWord) ? _validCommands[commandWord] : CommandWord.UNKNOWN;
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