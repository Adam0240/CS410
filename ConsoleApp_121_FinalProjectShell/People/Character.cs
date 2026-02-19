using ConsoleApp_121_FinalProjectShell.Commands;

namespace ConsoleApp_121_FinalProjectShell.People;

public abstract class Character
{
    public Room currentRoom;
    public Stack<Room> lastRooms;

    protected Character()
    {
        lastRooms = new Stack<Room>();
    }

    
    public Room getCurrentRoom() { return currentRoom; }
    public void setCurrentRoom(Room room) { currentRoom = room; }
    public Stack<Room> getLastRooms() { return lastRooms; }
    
    
    public int goRoom(Command command)
    {
        if (!command.HasSecondWord())
            return 0;

        string direction = command.GetSecondWord();

        if (!currentRoom.getExits().TryGetValue(direction, out Room nextRoom) || nextRoom == null)
            return -1;

        lastRooms.Push(currentRoom);
        currentRoom = nextRoom;

        return direction.Equals("slide", StringComparison.OrdinalIgnoreCase) ? 2 : 1;
    }
    
    /**
     * currently only used by the player, but functionality may be needed later
     */
    public int back()
    {
        if (lastRooms.Count == 0)
            return 0;

        currentRoom = lastRooms.Pop();
        return lastRooms.Count + 1;
    }
    
}