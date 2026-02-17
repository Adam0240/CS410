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
        game.printHelp();
        return false;
    }
}

public sealed class GoCommandAction : CommandActionBase
{
    public override bool Execute(Game game, Command command)
    {
        game.goTo(command);
        game.printLocationInfo(game.getPlayer().getCurrentRoom());
        return false;
    }
}

public sealed class QuitCommandAction : CommandActionBase
{
    public override bool Execute(Game game, Command command) => game.quit(command);
}

public sealed class BackCommandAction : CommandActionBase
{
    public override bool Execute(Game game, Command command)
    {
        game.backTo();
        game.printLocationInfo(game.getPlayer().getCurrentRoom());
        return false;
    }
}

public sealed class LookCommandAction : CommandActionBase
{
    public override bool Execute(Game game, Command command)
    {
        game.printLocationInfo(game.getPlayer().getCurrentRoom());
        return false;
    }
}

public sealed class TakeCommandAction : CommandActionBase
{
    public override bool Execute(Game game, Command command)
    {
        game.take(command);
        return false;
    }
}

public sealed class DropCommandAction : CommandActionBase
{
    public override bool Execute(Game game, Command command)
    {
        game.drop(command);
        return false;
    }
}

public sealed class ItemsCommandAction : CommandActionBase
{
    public override bool Execute(Game game, Command command)
    {
        game.itemsPrint();
        return false;
    }
}

public sealed class UseCommandAction : CommandActionBase
{
    public override bool Execute(Game game, Command command) => game.use(command);
}

public sealed class TalkCommandAction : CommandActionBase
{
    public override bool Execute(Game game, Command command)
    {
        game.talk();
        return false;
    }
}

public sealed class SleepCommandAction : CommandActionBase
{
    public override bool Execute(Game game, Command command) => game.sleep();
}

public sealed class UnknownCommandAction : CommandActionBase
{
    public override bool Execute(Game game, Command command)
    {
        Console.WriteLine("I don't know what you mean...");
        return false;
    }
}
