using System;
using System.IO;
using Xunit;
using ConsoleApp_121_FinalProjectShell.Commands;

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
                Command cmd = parser.GetCommand();

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
                Command cmd = parser.GetCommand();

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
                Command cmd = parser.GetCommand();

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
                Command cmd = parser.GetCommand();

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
    public void GetCommand_WhenReadLineReturnsNull_ReturnsUnknownCommand()
    {
        lock (ConsoleLock)
        {
            var originalIn = Console.In;
            var originalOut = Console.Out;

            try
            {
                var parser = new Parser();

                var input = new StringReader(string.Empty);
                var output = new StringWriter();

                Console.SetIn(input);
                Console.SetOut(output);

                Command cmd = parser.GetCommand();

                Assert.Equal("> ", output.ToString());
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
                parser.ShowCommands();

                // Assert
                var text = output.ToString();

                // These strings come from CommandWords.ShowAll() output
                Assert.Contains("go", text);
                Assert.Contains("walk", text);
                Assert.Contains("move", text);
                Assert.Contains("help", text);
                Assert.Contains("quit", text);
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }
    }
}
