using ConsoleApp_121_FinalProjectShell.Commands;
using ConsoleApp_121_FinalProjectShell.Core;

namespace ConsoleApp_121_FinalProjectShell.People;

/// <summary>
/// Manages traits and actions unique to the Protagonist character, a unique kind of NPC tied to progression.
/// They are the bumbling fool the player needs to set up to have the best possible chance for success.
///</summary>
public class Protagonist : Character
{
    private int _protagStepsCount = Game.random.Next(6);


    //Moves the protagonist through a random room exit. Very simple function.
    public bool protagSteps(Command command)
    {
        if (_protagStepsCount >= 8)
        {
            _protagStepsCount -= 8;
            goRoom(command);
            return true;
        }
        
        _protagStepsCount += Game.random.Next(4);
        return false;
    }
    
    //Method needed for testing purposes. 
    public int getProtagStepsCount()
    {
        return _protagStepsCount;
    }

    //savestate
    internal void setProtagStepsCount(int count)
    {
        _protagStepsCount = count;
    }

}