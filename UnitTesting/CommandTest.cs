using ConsoleApp_121_FinalProjectShell;
using Xunit;

public class CommandTests
{
    [Fact]
    public void Constructor_SetsCommandWord()
    {
        var command = new Command(CommandWord.GO, "north");

        Assert.Equal(CommandWord.GO, command.GetCommandWord());
    }

    [Fact]
    public void Constructor_SetsSecondWord()
    {
        var command = new Command(CommandWord.GO, "north");

        Assert.Equal("north", command.GetSecondWord());
    }

    [Fact]
    public void IsUnknown_ReturnsTrue_WhenCommandIsUnknown()
    {
        var command = new Command(CommandWord.UNKNOWN, null);

        Assert.True(command.IsUnknown());
    }

    [Fact]
    public void IsUnknown_ReturnsFalse_WhenCommandIsKnown()
    {
        var command = new Command(CommandWord.HELP, null);

        Assert.False(command.IsUnknown());
    }

    [Fact]
    public void HasSecondWord_ReturnsTrue_WhenSecondWordExists()
    {
        var command = new Command(CommandWord.GO, "north");

        Assert.True(command.HasSecondWord());
    }

    [Fact]
    public void HasSecondWord_ReturnsFalse_WhenSecondWordIsNull()
    {
        var command = new Command(CommandWord.GO, null);

        Assert.False(command.HasSecondWord());
    }
}
