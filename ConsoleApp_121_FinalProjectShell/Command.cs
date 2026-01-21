//Transcribed Command.java file - Adam Abbadusky

namespace ConsoleApp_121_FinalProjectShell;

public class Command
{
    private CommandWord commandWord;
    private string secondWord;

    /// <summary>
    /// Create a command object. First and second word must be supplied, but
    /// either one (or both) can be null.
    /// </summary>
    /// <param name="firstWord">
    /// The first word of the command. Null if the command was not recognised.
    /// </param>
    /// <param name="secondWord">The second word of the command.</param>
    public Command(CommandWord firstWord, string secondWord)
    {
        this.commandWord = firstWord;
        this.secondWord = secondWord;
    }

    /// <summary>
    /// Return the command word (the first word) of this command. If the
    /// command was not understood, the result is null.
    /// </summary>
    /// <returns>The command word.</returns>
    public CommandWord GetCommandWord()
    {
        return commandWord;
    }

    /// <summary>
    /// Return the second word of this command. Returns null if there was no
    /// second word.
    /// </summary>
    /// <returns>The second word.</returns>
    public string GetSecondWord()
    {
        return secondWord;
    }

    /// <summary>
    /// Return true if this command was not understood.
    /// </summary>
    public bool IsUnknown()
    {
        return (commandWord == CommandWord.UNKNOWN);
    }

    /// <summary>
    /// Return true if the command has a second word.
    /// </summary>
    public bool HasSecondWord()
    {
        return (secondWord != null);
    }
}