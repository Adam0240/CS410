namespace ConsoleApp_121_FinalProjectShell.Core.Persistence;

public class GameSaveState
{
    public int PlayerRoomId { get; set; }
    //Save State Edit 11
    public List<int> PlayerBacktrackRoomIds { get; set; } = [];
    public List<string> PlayerInventory { get; set; } = [];
    public int PlayerCarryWeight { get; set; }

    public int ProtagonistRoomId { get; set; }
    public int ProtagonistStepCounter { get; set; }

    public int FollowerRoomId { get; set; }
    public bool FollowerIsFollowing { get; set; }
    public List<string> FollowerInventory { get; set; } = [];

    public bool SwampCleared { get; set; }
    public bool ForgePrepared { get; set; }
    public bool SwordPlaced { get; set; }
    public bool GateOpen { get; set; }
    public bool ToldProtagGate { get; set; }
    public bool ToldProtagSword { get; set; }

    // roomId -> item names currently in that room
    public Dictionary<int, List<string>> RoomItems { get; set; } = [];
}
