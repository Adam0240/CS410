//Transcribed Command.java file - Adam Abbadusky

namespace ConsoleApp_121_FinalProjectShell;

public class Command
{
    private CommandWord _commandWord;
    private string _argument;

    /// <summary>
    /// Create a command object. First and second word must be supplied, but
    /// either one (or both) can be null.
    /// </summary>
    /// <param name="commandWord">
    /// The first word of the command. Null if the command was not recognised.
    /// </param>
    /// <param name="argument">The second word of the command.</param>
    public Command(CommandWord commandWord, string argument)
    {
        this._commandWord = commandWord;
        this._argument = argument;
    }

    /// <summary>
    /// Return the command word (the first word) of this command. If the
    /// command was not understood, the result is null.
    /// </summary>
    /// <returns>The command word.</returns>
    public CommandWord GetCommandWord()
    {
        return _commandWord;
    }

    /// <summary>
    /// Return the second word of this command. Returns null if there was no
    /// second word.
    /// </summary>
    /// <returns>The second word.</returns>
    public string GetSecondWord()
    {
        return _argument;
    }

    /// <summary>
    /// Return true if this command was not understood.
    /// </summary>
    public bool IsUnknown()
    {
        return (_commandWord == CommandWord.UNKNOWN);
    }

    /// <summary>
    /// Return true if the command has a second word.
    /// </summary>
    public bool HasSecondWord()
    {
        return _argument != null;
    }
}