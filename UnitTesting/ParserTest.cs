using System;
using System.IO;
using Xunit;
using ConsoleApp_121_FinalProjectShell;

public class ParserTests
{
    // Console is global process-wide state. Lock around any SetIn/SetOut usage.
    private static readonly object ConsoleLock = new object();

    [Fact]
    public void GetCommand_PrintsPromptAndParsesTwoWords()
    {
        lock (ConsoleLock)
        {
            // Arrange
            var originalIn = Console.In;
            var originalOut = Console.Out;

            try
            {
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
            finally
            {
                Console.SetIn(originalIn);
                Console.SetOut(originalOut);
            }
        }
    }

    [Fact]
    public void GetCommand_WithOneWord_SecondWordIsNull()
    {
        lock (ConsoleLock)
        {
            var originalIn = Console.In;
            var originalOut = Console.Out;

            try
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
            finally
            {
                Console.SetIn(originalIn);
                Console.SetOut(originalOut);
            }
        }
    }

    [Fact]
    public void GetCommand_IgnoresWordsAfterSecond()
    {
        lock (ConsoleLock)
        {
            var originalIn = Console.In;
            var originalOut = Console.Out;

            try
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
            finally
            {
                Console.SetIn(originalIn);
                Console.SetOut(originalOut);
            }
        }
    }

    [Fact]
    public void GetCommand_WithEmptyLine_ReturnsUnknownAndNullSecondWord()
    {
        lock (ConsoleLock)
        {
            var originalIn = Console.In;
            var originalOut = Console.Out;

            try
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
            finally
            {
                Console.SetIn(originalIn);
                Console.SetOut(originalOut);
            }
        }
    }

    [Fact]
    public void GetCommand_WhenReadLineReturnsNull_ThrowsNullReferenceException_CurrentBehavior()
    {
        lock (ConsoleLock)
        {
            var originalIn = Console.In;
            var originalOut = Console.Out;

            try
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
            finally
            {
                Console.SetIn(originalIn);
                Console.SetOut(originalOut);
            }
        }
    }

    [Fact]
    public void ShowCommands_WritesKnownCommandsToConsole()
    {
        lock (ConsoleLock)
        {
            var originalOut = Console.Out;

            try
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
            finally
            {
                Console.SetOut(originalOut);
            }
        }
    }
}
