using System;
using System.IO;
using Xunit;
using ConsoleApp_121_FinalProjectShell;

public class ParserTests
{
    [Fact]
    public void GetCommand_PrintsPromptAndParsesTwoWords()
    {
        // Arrange
        var parser = new Parser();

        var input = new StringReader("walk north\n");
        var output = new StringWriter();

        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        Command cmd = parser.getCommand();

        // Assert
        Assert.Equal("> ", output.ToString());                  // prompt printed
        Assert.Equal(CommandWord.GO, cmd.GetCommandWord());     // "walk" -> GO
        Assert.Equal("north", cmd.GetSecondWord());             // second token
    }

    [Fact]
    public void GetCommand_WithOneWord_SecondWordIsNull()
    {
        // Arrange
        var parser = new Parser();

        var input = new StringReader("help\n");
        var output = new StringWriter();

        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        Command cmd = parser.getCommand();

        // Assert
        Assert.Equal("> ", output.ToString());
        Assert.Equal(CommandWord.HELP, cmd.GetCommandWord());
        Assert.Null(cmd.GetSecondWord());
    }

    [Fact]
    public void GetCommand_IgnoresWordsAfterSecond()
    {
        // Arrange
        var parser = new Parser();

        var input = new StringReader("walk north extra ignored\n");
        var output = new StringWriter();

        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        Command cmd = parser.getCommand();

        // Assert
        Assert.Equal("> ", output.ToString());
        Assert.Equal(CommandWord.GO, cmd.GetCommandWord());
        Assert.Equal("north", cmd.GetSecondWord()); // confirms only 2nd word is kept
    }

    [Fact]
    public void GetCommand_WithEmptyLine_ReturnsUnknownAndNullSecondWord()
    {
        // Arrange
        var parser = new Parser();

        var input = new StringReader("\n");
        var output = new StringWriter();

        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        Command cmd = parser.getCommand();

        // Assert
        Assert.Equal("> ", output.ToString());

        // Empty line => first token == "" => GetCommandWord("") => UNKNOWN
        Assert.Equal(CommandWord.UNKNOWN, cmd.GetCommandWord());
        Assert.Null(cmd.GetSecondWord());
    }

    [Fact]
    public void GetCommand_WhenReadLineReturnsNull_ThrowsNullReferenceException_CurrentBehavior()
    {
        // Arrange
        var parser = new Parser();

        // StringReader with empty content causes ReadLine() to return null immediately
        var input = new StringReader(string.Empty);
        var output = new StringWriter();

        Console.SetIn(input);
        Console.SetOut(output);

        // Act + Assert
        // Current behavior: inputLine is null, then inputLine.Split(' ') throws.
        Assert.Throws<NullReferenceException>(() => parser.getCommand());
    }

    [Fact]
    public void ShowCommands_WritesKnownCommandsToConsole()
    {
        // Arrange
        var parser = new Parser();
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        parser.showCommands();

        // Assert
        var text = output.ToString();

        // These strings come from CommandWords.ShowAll() output
        Assert.Contains("walk", text);
        Assert.Contains("help", text);
        Assert.Contains("EndGame", text);
    }
}
