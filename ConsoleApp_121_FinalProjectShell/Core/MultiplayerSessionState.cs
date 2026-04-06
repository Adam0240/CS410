// Multiplayer summary: stores the authoritative shared-world snapshot and command result payloads used for sync.
using System.Collections.Generic;

namespace ConsoleApp_121_FinalProjectShell.Core;

public sealed class MultiplayerSessionState
{
    public Dictionary<int, MultiplayerPlayerState> Players { get; set; } = [];
    public Dictionary<int, List<string>> RoomItems { get; set; } = [];
    public bool SwampCleared { get; set; }
    public bool ForgePrepared { get; set; }
    public bool SwordPlaced { get; set; }
    public bool GateOpen { get; set; }
    public bool ToldProtagGate { get; set; }
    public bool ToldProtagSword { get; set; }
    public int ProtagonistRoomId { get; set; }
    public int ProtagonistStepCounter { get; set; }
    public int FollowerRoomId { get; set; }
    public bool FollowerIsFollowing { get; set; }
    public List<string> FollowerInventory { get; set; } = [];
}

public sealed class MultiplayerPlayerState
{
    public int PlayerId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public int CurrentRoomId { get; set; }
    public List<int> BacktrackRoomIds { get; set; } = [];
    public List<string> Inventory { get; set; } = [];
    public int CarryWeight { get; set; }
    public bool IsConnected { get; set; }
}

public sealed class CommandExecutionResult
{
    public bool ShouldQuit { get; init; }
    public string OutputText { get; init; } = string.Empty;
    public string RoomText { get; init; } = string.Empty;
    public MultiplayerSessionState Snapshot { get; init; } = new();
}

public sealed class LocalCommandExecutionResult
{
    public bool ShouldQuit { get; init; }
    public MultiplayerSessionState Snapshot { get; init; } = new();
}
