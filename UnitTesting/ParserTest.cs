using System;
using System.Collections.Generic; //Bogus Change 1: Added for generated test data collections
using System.IO;
using Bogus; //Bogus Change 2: Added Bogus
using Bogus.DataSets; //Bogus Change 3: Added Bogus datasets
using ConsoleApp_121_FinalProjectShell.Commands;
using Xunit;

namespace UnitTesting;

public class ParserTests
{
    // Console is global process-wide state. Lock around any SetIn/SetOut usage.
    private static readonly object ConsoleLock = new object();

    //Bogus Change 4: Deterministic seed for repeatable generated parser input
    private static readonly int Seed = 410;

    //Bogus Change 5: Shared Faker instance for generating randomized command lines
    private static readonly Faker Faker = new("en");

    //Bogus Change 6: Generator for random alphabetic tokens (safe parser words)
    private static string RandomWord(int min = 3, int max = 10)
    {
        int len = Faker.Random.Int(min, max);
        return Faker.Random.String2(len, "abcdefghijklmnopqrstuvwxyz");
    }

    //Bogus Change 7: Generate "extra token" command lines to verify parser ignores words after second token
    public static IEnumerable<object[]> ExtraTokenInputs()
    {
        Randomizer.Seed = new Random(Seed);

        for (int i = 0; i < 20; i++)
        {
            // use alias "walk" so expected command word is always GO
            string secondWord = RandomWord();
            string extra1 = RandomWord();
            string extra2 = RandomWord();

            string input = $"walk {secondWord} {extra1} {extra2}\n";
            yield return new object[] { input, secondWord };
        }
    }

    //Bogus Change 8: Generate whitespace-heavy inputs to validate robust tokenization
    public static IEnumerable<object[]> WhitespaceInputs()
    {
        Randomizer.Seed = new Random(Seed + 1);

        for (int i = 0; i < 15; i++)
        {
            string secondWord = RandomWord();
            int spacesA = Faker.Random.Int(1, 4);
            int spacesB = Faker.Random.Int(2, 6);

            string input = $"{new string(' ', spacesA)}walk{new string(' ', spacesB)}{secondWord}\n";
            yield return new object[] { input, secondWord };
        }
    }

    [Fact]
    public void GetCommand_PrintsPromptAndParsesTwoWords()
    {
        lock (ConsoleLock)
        {
            var originalIn = Console.In;
            var originalOut = Console.Out;

            try
            {
                var parser = new Parser();

                var input = new StringReader("walk north\n");
                var output = new StringWriter();

                Console.SetIn(input);
                Console.SetOut(output);

                Command cmd = parser.GetCommand();

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
                var parser = new Parser();

                var input = new StringReader("help\n");
                var output = new StringWriter();

                Console.SetIn(input);
                Console.SetOut(output);

                Command cmd = parser.GetCommand();

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
                var parser = new Parser();

                var input = new StringReader("walk north extra ignored\n");
                var output = new StringWriter();

                Console.SetIn(input);
                Console.SetOut(output);

                Command cmd = parser.GetCommand();

                Assert.Equal("> ", output.ToString());
                Assert.Equal(CommandWord.GO, cmd.GetCommandWord());
                Assert.Equal("north", cmd.GetSecondWord());
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
                var parser = new Parser();

                var input = new StringReader("\n");
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
                var parser = new Parser();
                var output = new StringWriter();
                Console.SetOut(output);

                parser.ShowCommands();

                var text = output.ToString();

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

    //Bogus Change 9: Added generated test to validate "ignore extra tokens" across many randomized cases
    [Theory]
    [MemberData(nameof(ExtraTokenInputs))]
    public void GetCommand_IgnoresAllTokensAfterSecond_BogusGenerated(string inputLine, string expectedSecondWord)
    {
        lock (ConsoleLock)
        {
            var originalIn = Console.In;
            var originalOut = Console.Out;

            try
            {
                var parser = new Parser();
                var input = new StringReader(inputLine);
                var output = new StringWriter();

                Console.SetIn(input);
                Console.SetOut(output);

                Command cmd = parser.GetCommand();

                Assert.Equal("> ", output.ToString());
                Assert.Equal(CommandWord.GO, cmd.GetCommandWord());
                Assert.Equal(expectedSecondWord, cmd.GetSecondWord());
            }
            finally
            {
                Console.SetIn(originalIn);
                Console.SetOut(originalOut);
            }
        }
    }

    //Bogus Change 10: Added whitespace-randomization test to ensure parser handles variable spacing
    [Theory]
    [MemberData(nameof(WhitespaceInputs))]
    public void GetCommand_HandlesVariableWhitespace_BogusGenerated(string inputLine, string expectedSecondWord)
    {
        lock (ConsoleLock)
        {
            var originalIn = Console.In;
            var originalOut = Console.Out;

            try
            {
                var parser = new Parser();
                var input = new StringReader(inputLine);
                var output = new StringWriter();

                Console.SetIn(input);
                Console.SetOut(output);

                Command cmd = parser.GetCommand();

                Assert.Equal("> ", output.ToString());
                Assert.Equal(CommandWord.GO, cmd.GetCommandWord());
                Assert.Equal(expectedSecondWord, cmd.GetSecondWord());
            }
            finally
            {
                Console.SetIn(originalIn);
                Console.SetOut(originalOut);
            }
        }
    }

    //Bogus Change 11: Added generated invalid-first-word test to harden UNKNOWN path
    [Fact]
    public void GetCommand_WithGeneratedUnknownFirstWord_ReturnsUnknown()
    {
        lock (ConsoleLock)
        {
            var originalIn = Console.In;
            var originalOut = Console.Out;

            try
            {
                var parser = new Parser();

                // generate non-command token unlikely to exist in command vocabulary
                string unknownWord = $"zzz{RandomWord(6, 10)}";
                string inputLine = $"{unknownWord} {RandomWord()}\n";

                var input = new StringReader(inputLine);
                var output = new StringWriter();

                Console.SetIn(input);
                Console.SetOut(output);

                Command cmd = parser.GetCommand();

                Assert.Equal("> ", output.ToString());
                Assert.Equal(CommandWord.UNKNOWN, cmd.GetCommandWord());
                // second token is still parsed, but command word should remain UNKNOWN
                Assert.NotNull(cmd.GetSecondWord());
            }
            finally
            {
                Console.SetIn(originalIn);
                Console.SetOut(originalOut);
            }
        }
    }

    //Bogus Change 12: Added randomized coverage for Parser.GetSingleCommand()
    [Fact]
    public void GetSingleCommand_ReturnsFirstTokenLowercase_BogusGenerated()
    {
        lock (ConsoleLock)
        {
            var originalIn = Console.In;
            var originalOut = Console.Out;

            try
            {
                string rawFirst = Faker.Random.String2(6, "ABCdefXYZ").ToUpperInvariant();
                string second = RandomWord();
                string inputText = $"{rawFirst} {second}\n";

                var input = new StringReader(inputText);
                var output = new StringWriter();

                Console.SetIn(input);
                Console.SetOut(output);

                string result = Parser.GetSingleCommand();

                Assert.Equal("> ", output.ToString());
                Assert.Equal(rawFirst.ToLowerInvariant(), result);
            }
            finally
            {
                Console.SetIn(originalIn);
                Console.SetOut(originalOut);
            }
        }
    }
}