using ConsoleApp_121_FinalProjectShell.Commands;
using ConsoleApp_121_FinalProjectShell.Core;

namespace ConsoleApp_121_FinalProjectShell.People;

/// <summary>
/// Manages traits and actions unique to the Protagonist character, a unique kind of NPC tied to progression.
/// They are the bumbling fool the player needs to set up to have the best possible chance for success.
///</summary>
public class Protagonist : Character
{
    private bool _isCacheClear;
    private List<string> DialogueCache;
    private int _protagStepsCount = Game.random.Next(6);

    public Protagonist()
    {
        _isCacheClear = false;
        DialogueCache = new List<string>();
    }
    public Protagonist(Game game)
    {
        _isCacheClear = false;
        DialogueCache = new List<string>();
        if (game != null)
        {
            game.PlayerMovement += ClearCache;
        }
    }
    //Moves the protagonist through a random room exit. Very simple function.
    public bool protagSteps(Command command)
    {
        if (_protagStepsCount >= 10)
        {
            _protagStepsCount -= 10;
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

    void ClearCache(object? sender, Command command)
    {
        //clear the protag cache when the player moves
        DialogueCache = new List<string>();
        _isCacheClear = true;
    }

    public bool isCacheClear()
    {
        return _isCacheClear;
    }

    public string GetNextDialogue()
    {
        if (DialogueCache.Count > 0)
        {
            int index = Game.random.Next(DialogueCache.Count);
            string result = DialogueCache[index];
            DialogueCache.RemoveAt(index);
            return result;
        }
        return "It would seem he has nothing left to say right now.";
    }
    public void SetDialogueCache(List<string> list)
    {
        DialogueCache = list;
        _isCacheClear = false;
    }
    
    


}