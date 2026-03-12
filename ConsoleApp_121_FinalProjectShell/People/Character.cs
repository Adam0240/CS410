using ConsoleApp_121_FinalProjectShell.Commands;

namespace ConsoleApp_121_FinalProjectShell.People;

public abstract class Character
{
    private Room _currentRoom = null!;
    private readonly Stack<Room> _lastRooms;

    protected Character()
    {
        _lastRooms = new Stack<Room>();
    }

    protected Character(Room startroom)
    {
        _lastRooms = new Stack<Room>();
        _currentRoom = startroom;
    }

    public Room getCurrentRoom() { return _currentRoom; }

    public void setCurrentRoom(Room room) { _currentRoom = room; }

    public Stack<Room> getLastRooms() { return _lastRooms; }

    //Save State Edit 9
    internal List<int> GetLastRoomIds()
    {
        return _lastRooms.Select(room => room.GetId()).ToList();
    }

    //Save State Edit 10
    internal void RestoreLastRooms(IEnumerable<Room> roomsInPopOrder)
    {
        _lastRooms.Clear();

        foreach (Room room in roomsInPopOrder.Reverse())
        {
            _lastRooms.Push(room);
        }
    }

    public int goRoom(Command command)
    {
        if (!command.HasSecondWord())
            return 0;

        string direction = command.GetSecondWord()!;

        if (!_currentRoom.GetExits().TryGetValue(direction, out Room? nextRoom) || nextRoom == null)
            return -1;

        _lastRooms.Push(_currentRoom);
        _currentRoom = nextRoom;

        return direction.Equals("slide", StringComparison.OrdinalIgnoreCase) ? 2 : 1;
    }

    public int back()
    {
        if (_lastRooms.Count == 0)
            return 0;

        _currentRoom = _lastRooms.Pop();
        return _lastRooms.Count + 1;
    }
}
