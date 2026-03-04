using System.Collections.Generic;
using ConsoleApp_121_FinalProjectShell.Core;

namespace ConsoleApp_121_FinalProjectShell.Commands;

public static class CommandActionRegistry
{
    public static Dictionary<CommandWord, ICommandAction> CreateDefault()
    {
        return new Dictionary<CommandWord, ICommandAction>
        {
            { CommandWord.HELP, new HelpCommandAction() },
            { CommandWord.GO, new GoCommandAction() },
            { CommandWord.QUIT, new QuitCommandAction() },
            { CommandWord.BACK, new BackCommandAction() },
            { CommandWord.LOOK, new LookCommandAction() },
            { CommandWord.TAKE, new TakeCommandAction() },
            { CommandWord.DROP, new DropCommandAction() },
            { CommandWord.ITEMS, new ItemsCommandAction() },
            { CommandWord.USE, new UseCommandAction() },
            { CommandWord.TALK, new TalkCommandAction() },
            { CommandWord.SLEEP, new SleepCommandAction() },
            { CommandWord.UNKNOWN, new UnknownCommandAction() },
            { CommandWord.FOLLOW, new FollowCommandAction() },
            { CommandWord.STAY, new StayCommandAction() },
            { CommandWord.TRADE, new TradeCommandAction() },
            
        };
    }
}
