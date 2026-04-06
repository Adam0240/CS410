// Multiplayer summary: defines the wire-format message envelope and message types exchanged between host and client.
using System.Text.Json.Serialization;
using ConsoleApp_121_FinalProjectShell.Core;

namespace ConsoleApp_121_FinalProjectShell.Networking;

public sealed class NetworkMessage
{
    public NetworkMessageType Type { get; set; }
    public int? PlayerId { get; set; }
    public string? PlayerName { get; set; }
    public string? Text { get; set; }
    public MultiplayerSessionState? Snapshot { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum NetworkMessageType
{
    JoinRequest,
    JoinAccepted,
    PlayerConnected,
    PlayerDisconnected,
    CommandSubmission,
    StateUpdate,
    RoomUpdate,
    SystemMessage,
    FullSnapshotSync
}
