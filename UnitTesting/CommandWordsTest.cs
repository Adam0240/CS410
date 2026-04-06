using System;
using System.Collections.Concurrent;
using System.IO;
using Bogus;
using ConsoleApp_121_FinalProjectShell;
using ConsoleApp_121_FinalProjectShell.Commands;
using FluentAssertions;
using Xunit;

namespace UnitTesting
{
    public class CommandWordsTests
    {
        //bogus parameters and commands
        private static readonly int Seed = 685;
        private static readonly Faker Faker = new("en");
        public CommandWordsTests()
        {
            Randomizer.Seed = new Random(Seed);
        }
        private string RandomWord()
        {
            return Faker.Lorem.Sentence(); //generates nonsense strings
        }
        
        [Fact]
        public void IsCommand_WithValidCommands_ReturnsTrue()
        {
            var cw = new CommandWords();

            cw.IsCommand("go").Should().BeTrue();
            cw.IsCommand("walk").Should().BeTrue();
            cw.IsCommand("move").Should().BeTrue();
            cw.IsCommand("help").Should().BeTrue();
            cw.IsCommand("quit").Should().BeTrue();
            cw.IsCommand("back").Should().BeTrue();
            cw.IsCommand("look").Should().BeTrue();
            cw.IsCommand("take").Should().BeTrue();
            cw.IsCommand("drop").Should().BeTrue();
            cw.IsCommand("items").Should().BeTrue();
            cw.IsCommand("use").Should().BeTrue();
            cw.IsCommand("talk").Should().BeTrue();
            cw.IsCommand("sleep").Should().BeTrue();
            cw.IsCommand("trade").Should().BeTrue();
            cw.IsCommand("follow").Should().BeTrue();
            cw.IsCommand("stay").Should().BeTrue();
        }

        [Fact]
        public void IsCommand_WithInvalidCommand_ReturnsFalse()
        {
            var cw = new CommandWords();
            
            //check blank string first, then ten random strings just to make sure
            cw.IsCommand("").Should().BeFalse();
            
            cw.IsCommand(RandomWord()).Should().BeFalse();
            cw.IsCommand(RandomWord()).Should().BeFalse();
            cw.IsCommand(RandomWord()).Should().BeFalse();
            cw.IsCommand(RandomWord()).Should().BeFalse();
            cw.IsCommand(RandomWord()).Should().BeFalse();
            cw.IsCommand(RandomWord()).Should().BeFalse();
            cw.IsCommand(RandomWord()).Should().BeFalse();
            cw.IsCommand(RandomWord()).Should().BeFalse();
            cw.IsCommand(RandomWord()).Should().BeFalse();
            cw.IsCommand(RandomWord()).Should().BeFalse();
        }

        [Fact]
        public void IsCommand_WithEmptyString_ReturnsFalse()
        {
            var cw = new CommandWords();

            cw.IsCommand(string.Empty).Should().BeFalse();
        }

        [Fact]
        public void GetCommandWord_WithValidCommands_ReturnsExpectedEnum()
        {
            var cw = new CommandWords();

            cw.GetCommandWord("go").Should().Be(CommandWord.GO);
            cw.GetCommandWord("walk").Should().Be(CommandWord.GO);
            cw.GetCommandWord("move").Should().Be(CommandWord.GO);
            cw.GetCommandWord("help").Should().Be(CommandWord.HELP);
            cw.GetCommandWord("quit").Should().Be(CommandWord.QUIT);
            cw.GetCommandWord("back").Should().Be(CommandWord.BACK);
            cw.GetCommandWord("look").Should().Be(CommandWord.LOOK);
            cw.GetCommandWord("take").Should().Be(CommandWord.TAKE);
            cw.GetCommandWord("drop").Should().Be(CommandWord.DROP);
            cw.GetCommandWord("items").Should().Be(CommandWord.ITEMS);
            cw.GetCommandWord("use").Should().Be(CommandWord.USE);
            cw.GetCommandWord("talk").Should().Be(CommandWord.TALK);
            cw.GetCommandWord("sleep").Should().Be(CommandWord.SLEEP);
            cw.GetCommandWord("trade").Should().Be(CommandWord.TRADE);
            cw.GetCommandWord("follow").Should().Be(CommandWord.FOLLOW);
            cw.GetCommandWord("stay").Should().Be(CommandWord.STAY);
        }

        [Fact]
        public void GetCommandWord_WithUnknownCommand_ReturnsUnknown()
        {
            var cw = new CommandWords();
            
            cw.GetCommandWord("").Should().Be(CommandWord.UNKNOWN); 
            cw.GetCommandWord(RandomWord()).Should().Be(CommandWord.UNKNOWN); 
            cw.GetCommandWord(RandomWord()).Should().Be(CommandWord.UNKNOWN); 
            cw.GetCommandWord(RandomWord()).Should().Be(CommandWord.UNKNOWN); 
            cw.GetCommandWord(RandomWord()).Should().Be(CommandWord.UNKNOWN); 
            cw.GetCommandWord(RandomWord()).Should().Be(CommandWord.UNKNOWN); 
            cw.GetCommandWord(RandomWord()).Should().Be(CommandWord.UNKNOWN); 
            cw.GetCommandWord(RandomWord()).Should().Be(CommandWord.UNKNOWN); 
            cw.GetCommandWord(RandomWord()).Should().Be(CommandWord.UNKNOWN); 
            cw.GetCommandWord(RandomWord()).Should().Be(CommandWord.UNKNOWN); 
            cw.GetCommandWord(RandomWord()).Should().Be(CommandWord.UNKNOWN); 
        }

        [Fact]
        public void GetCommandWord_WithEmptyString_ReturnsUnknown()
        {
            var cw = new CommandWords();

            cw.GetCommandWord(string.Empty).Should().Be(CommandWord.UNKNOWN);
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
                output.Should().Contain("go");
                output.Should().Contain("walk");
                output.Should().Contain("move");
                output.Should().Contain("help");
                output.Should().Contain("quit");
                output.Should().Contain("back");
                output.Should().Contain("look");
                output.Should().Contain("take");
                output.Should().Contain("drop");
                output.Should().Contain("items");
                output.Should().Contain("use");
                output.Should().Contain("talk");
                output.Should().Contain("sleep");
                output.Should().Contain("trade");
                output.Should().Contain("follow");
                output.Should().Contain("stay");
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }
    }
}
