using System;
using ConsoleApp_121_FinalProjectShell.Core;

namespace ConsoleApp_121_FinalProjectShell.Commands;

public abstract class CommandActionBase : ICommandAction
{
    public abstract bool Execute(Game game, Command command);
}

public sealed class HelpCommandAction : CommandActionBase
{
    public override bool Execute(Game game, Command command)
    {
        game.PrintHelp();
        return false;
    }
}

public sealed class GoCommandAction : CommandActionBase
{
    public override bool Execute(Game game, Command command)
    {
        game.GoTo(command);
        game.PrintLocationInfo(game.GetPlayer().getCurrentRoom());
        return false;
    }
}

public sealed class QuitCommandAction : CommandActionBase
{
    public override bool Execute(Game game, Command command) => game.Quit(command);
}

public sealed class BackCommandAction : CommandActionBase
{
    public override bool Execute(Game game, Command command)
    {
        game.BackTo();
        game.PrintLocationInfo(game.GetPlayer().getCurrentRoom());
        return false;
    }
}

public sealed class LookCommandAction : CommandActionBase
{
    public override bool Execute(Game game, Command command)
    {
        game.PrintLocationInfo(game.GetPlayer().getCurrentRoom());
        return false;
    }
}

public sealed class TakeCommandAction : CommandActionBase
{
    public override bool Execute(Game game, Command command)
    {
        game.Take(command);
        return false;
    }
}

public sealed class DropCommandAction : CommandActionBase
{
    public override bool Execute(Game game, Command command)
    {
        game.Drop(command);
        return false;
    }
}

public sealed class ItemsCommandAction : CommandActionBase
{
    public override bool Execute(Game game, Command command)
    {
        game.ItemsPrint();
        return false;
    }
}

public sealed class UseCommandAction : CommandActionBase
{
    public override bool Execute(Game game, Command command) => game.Use(command);
}

public sealed class TalkCommandAction : CommandActionBase
{
    public override bool Execute(Game game, Command command)
    {
        game.Talk();
        return false;
    }
}

public sealed class SleepCommandAction : CommandActionBase
{
    public override bool Execute(Game game, Command command) => game.Sleep();
}

public sealed class UnknownCommandAction : CommandActionBase
{
    public override bool Execute(Game game, Command command)
    {
        Console.WriteLine("I don't know what you mean...");
        return false;
    }
}
