using ConsoleApp_121_FinalProjectShell.Commands;

namespace ConsoleApp_121_FinalProjectShell.People;

/// <summary>
/// Base class for any entity in the game, creatures, NPCs, the player, the protagonist, etc.
/// Characters by baseline can be placed in a Room and move between them via exits in each
/// Class is abstract because a Character should always have a purpose, Player and Protag are already defined,
/// but this leaves room for types implementing enemies or informative NPCs to be added.
/// </summary>
public abstract class Character
{
    //Basic location-storing parameters, necessary for every character
    //Gives them the ability to move around
    private Room _currentRoom;
    private Stack<Room> _lastRooms;

    //Basic constructor.
    protected Character()
    {
        _lastRooms = new Stack<Room>();
    }
    //Overloaded constructor to allow for providing an initial location.
    protected Character(Room startroom)
    {
        _lastRooms = new Stack<Room>();
        _currentRoom = startroom;
    }

    //Accessors and mutators
    public Room getCurrentRoom() { return _currentRoom; }
    
    public void setCurrentRoom(Room room) { _currentRoom = room; }
    
    public Stack<Room> getLastRooms() { return _lastRooms; }
    
    
    /// <summary>
    /// Moves the character to the listed destination, and returns output corresponding to the result and validity of
    /// the given exit. 
    /// </summary>
    /// <param name="command">The command to use to interpret where to go.</param>
    /// <returns>Returns 0 if there's no listed destination, -1 if the given exit doesn't exist, 1 for a valid exit, and 2
    /// for the special slide exit. </returns>
    public int goRoom(Command command)
    {
        if (!command.HasSecondWord())
            return 0;

        string direction = command.GetSecondWord();

        if (!_currentRoom.getExits().TryGetValue(direction, out Room nextRoom) || nextRoom == null)
            return -1;

        _lastRooms.Push(_currentRoom);
        _currentRoom = nextRoom;

        return direction.Equals("slide", StringComparison.OrdinalIgnoreCase) ? 2 : 1;
    }
    
    /// <summary>
    /// Backtracks through the list of rooms visited by the Character in order. Currently only used by the Player but
    /// should still be part of the functionality of Character.
    /// </summary>
    /// <returns>0 if the character has no rooms to backtrack to, 1 otherwise. </returns>
    public int back()
    {
        if (_lastRooms.Count == 0)
            return 0;

        _currentRoom = _lastRooms.Pop();
        return _lastRooms.Count + 1;
    }
    
}