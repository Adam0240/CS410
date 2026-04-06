using ConsoleApp_121_FinalProjectShell;
using Xunit;
using ConsoleApp_121_FinalProjectShell.Commands;
using ConsoleApp_121_FinalProjectShell.Core;
using Bogus;
using FluentAssertions;


namespace ConsoleApp_121_FinalProjectShell.Tests;

public class CommandTests
{
    //bogus parameters and commands
    private static readonly int Seed = 970;
    private static readonly Faker Faker = new("en");

    public CommandTests()
    {
        Randomizer.Seed = new Random(Seed);
    }
    private static CommandWord RandomCommandWord()
    {
        CommandWord[] all = (CommandWord[])Enum.GetValues(typeof(CommandWord));
        
        return Faker.PickRandom(all);
    }

    private static string RandomSecondWordFor(CommandWord cw)
    {
        return cw == CommandWord.QUIT
            ? string.Empty
            : Faker.Lorem.Word();
    }
    
    [Fact]
    public void Constructor_SetsCommandWord()
    {
        var command = new Command(CommandWord.GO, RandomSecondWordFor(CommandWord.GO));

        
        command.GetCommandWord().Should().Be(CommandWord.GO);
    }

    [Fact]
    public void Constructor_SetsSecondWord()
    {
        var command = new Command(RandomCommandWord(), "north");

        command.GetSecondWord().Should().Be("north");
    }

    [Fact]
    public void IsUnknown_ReturnsTrue_WhenCommandIsUnknown()
    {
        var command = new Command(CommandWord.UNKNOWN, RandomSecondWordFor(CommandWord.UNKNOWN));

        command.IsUnknown().Should().BeTrue();
    }

    [Fact]
    public void IsUnknown_ReturnsFalse_WhenCommandIsKnown()
    {
        var command = new Command(CommandWord.HELP, RandomSecondWordFor(CommandWord.HELP));

        command.IsUnknown().Should().BeFalse();
    }

    [Fact]
    public void HasSecondWord_ReturnsTrue_WhenSecondWordExists()
    {
        var command = new Command(RandomCommandWord(), RandomSecondWordFor(CommandWord.GO));

        command.HasSecondWord().Should().BeTrue();
    }

    [Fact]
    public void HasSecondWord_ReturnsFalse_WhenSecondWordIsNull()
    {
        var command = new Command(CommandWord.GO, null);

        command.HasSecondWord().Should().BeFalse();
    }
}
