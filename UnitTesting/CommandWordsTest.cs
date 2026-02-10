using System;
using System.IO;
using ConsoleApp_121_FinalProjectShell;
using Xunit;

public class CommandWordsTests
{
    [Fact]
    public void IsCommand_WithValidCommands_ReturnsTrue()
    {
        var cw = new CommandWords();

        Assert.True(cw.IsCommand("walk"));
        Assert.True(cw.IsCommand("help"));
        Assert.True(cw.IsCommand("EndGame"));
        Assert.True(cw.IsCommand("back"));
        Assert.True(cw.IsCommand("look"));
        Assert.True(cw.IsCommand("take"));
        Assert.True(cw.IsCommand("drop"));
        Assert.True(cw.IsCommand("items"));
        Assert.True(cw.IsCommand("use"));
        Assert.True(cw.IsCommand("talk"));
        Assert.True(cw.IsCommand("sleep"));
    }

    [Fact]
    public void IsCommand_WithInvalidCommand_ReturnsFalse()
    {
        var cw = new CommandWords();

        Assert.False(cw.IsCommand("quit")); // note: dictionary uses "EndGame", not "quit"
        Assert.False(cw.IsCommand("xyz"));
        Assert.False(cw.IsCommand(""));
    }

    [Fact]
    public void IsCommand_WithNull_ThrowsArgumentNullException_CurrentBehavior()
    {
        var cw = new CommandWords();

        // Current behavior due to Dictionary.ContainsKey(null)
        Assert.Throws<ArgumentNullException>(() => cw.IsCommand(null));
    }

    [Fact]
    public void GetCommandWord_WithValidCommands_ReturnsExpectedEnum()
    {
        var cw = new CommandWords();

        Assert.Equal(CommandWord.GO, cw.GetCommandWord("walk"));
        Assert.Equal(CommandWord.HELP, cw.GetCommandWord("help"));
        Assert.Equal(CommandWord.QUIT, cw.GetCommandWord("EndGame"));
        Assert.Equal(CommandWord.BACK, cw.GetCommandWord("back"));
        Assert.Equal(CommandWord.LOOK, cw.GetCommandWord("look"));
        Assert.Equal(CommandWord.TAKE, cw.GetCommandWord("take"));
        Assert.Equal(CommandWord.DROP, cw.GetCommandWord("drop"));
        Assert.Equal(CommandWord.ITEMS, cw.GetCommandWord("items"));
        Assert.Equal(CommandWord.USE, cw.GetCommandWord("use"));
        Assert.Equal(CommandWord.TALK, cw.GetCommandWord("talk"));
        Assert.Equal(CommandWord.SLEEP, cw.GetCommandWord("sleep"));
    }

    [Fact]
    public void GetCommandWord_WithUnknownCommand_ReturnsUnknown()
    {
        var cw = new CommandWords();

        Assert.Equal(CommandWord.UNKNOWN, cw.GetCommandWord("quit")); // not in dictionary
        Assert.Equal(CommandWord.UNKNOWN, cw.GetCommandWord("xyz"));
        Assert.Equal(CommandWord.UNKNOWN, cw.GetCommandWord(""));
    }

    [Fact]
    public void GetCommandWord_WithNull_ThrowsArgumentNullException_CurrentBehavior()
    {
        var cw = new CommandWords();

        // Current behavior due to Dictionary.ContainsKey(null)
        Assert.Throws<ArgumentNullException>(() => cw.GetCommandWord(null));
    }

    [Fact]
    public void ShowAll_WritesAllCommandsToConsole()
    {
        var cw = new CommandWords();

        var originalOut = Console.Out;
        try
        {
            using var sw = new StringWriter();
            Console.SetOut(sw);

            cw.ShowAll();

            var output = sw.ToString();

            // Must contain all command keys (order not guaranteed)
            Assert.Contains("walk", output);
            Assert.Contains("help", output);
            Assert.Contains("EndGame", output);
            Assert.Contains("back", output);
            Assert.Contains("look", output);
            Assert.Contains("take", output);
            Assert.Contains("drop", output);
            Assert.Contains("items", output);
            Assert.Contains("use", output);
            Assert.Contains("talk", output);
            Assert.Contains("sleep", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }
}
