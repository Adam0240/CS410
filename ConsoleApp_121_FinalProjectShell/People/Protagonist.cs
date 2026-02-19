using ConsoleApp_121_FinalProjectShell.Commands;
using ConsoleApp_121_FinalProjectShell.Core;

namespace ConsoleApp_121_FinalProjectShell.People;

public class Protagonist : Character
{
    private int protagStepsCount;

    public Protagonist() : base()
    {
        protagStepsCount = Game.random.Next(6);
    }
    
    
    //moves the protagonist. called after every command.
    public bool protagSteps(Command command)
    {
        if (protagStepsCount >= 8)
        {
            protagStepsCount -= 8;
            goRoom(command);
            return true;
        }
        
        protagStepsCount += Game.random.Next(4);
        return false;
    }
    
    //Method added for testing
    public int getProtagStepsCount()
    {
        return protagStepsCount;
    }
    
}