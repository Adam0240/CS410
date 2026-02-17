using ConsoleApp_121_FinalProjectShell.Core;

namespace ConsoleApp_121_FinalProjectShell.Commands;

public interface ICommandAction
{
    // Returns true only when the game should quit (same meaning as processCommand return value)
    bool Execute(Game game, Command command);
}
