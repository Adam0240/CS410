// Multiplayer summary: adds host-authoritative player/session state, snapshot sync, and player-specific command execution helpers.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ConsoleApp_121_FinalProjectShell.Commands;
using ConsoleApp_121_FinalProjectShell.Items;
using ConsoleApp_121_FinalProjectShell.People;

namespace ConsoleApp_121_FinalProjectShell.Core;

public partial class Game
{
    private readonly Dictionary<int, Player> players = [];
    private readonly Dictionary<int, bool> playerConnections = [];
    private readonly object commandSync = new();
    private bool multiplayerEnabled;
    private int localPlayerId = 1;
    private int activePlayerId = 1;

    internal int LocalPlayerId => localPlayerId;
    internal bool IsMultiplayerActive => multiplayerEnabled;

    internal void InitializePlayerRegistry()
    {
        players.Clear();
        players[1] = player;
        playerConnections.Clear();
        playerConnections[1] = true;
        localPlayerId = 1;
        activePlayerId = 1;
        multiplayerEnabled = false;
    }

    internal void ConfigureAsHost()
    {
        multiplayerEnabled = true;
        localPlayerId = 1;
        EnsurePlayerExists(2);
        playerConnections[1] = true;
    }

    internal void ConfigureAsClient()
    {
        multiplayerEnabled = true;
        localPlayerId = 2;
        EnsurePlayerExists(2);
    }

    internal void EnsurePlayerExists(int playerId)
    {
        if (players.ContainsKey(playerId))
        {
            return;
        }

        Room startRoom = allRooms.First(r => r.GetId() == 0);
        players[playerId] = new Player(startRoom, playerId, $"Player {playerId}");
        playerConnections[playerId] = false;
    }

    internal void SetPlayerConnected(int playerId, bool isConnected)
    {
        EnsurePlayerExists(playerId);
        playerConnections[playerId] = isConnected;
    }

    internal string GetLocationInfoText(int viewerPlayerId)
    {
        Player viewer = GetPlayer(viewerPlayerId);
        return GetLocationInfoText(viewerPlayerId, viewer.GetCurrentRoom());
    }

    internal string GetLocationInfoText(int viewerPlayerId, Room room)
    {
        return RoomTextService.GetLongDescription(room, progress, GetPresenceLines(viewerPlayerId, room));
    }

    private IEnumerable<string> GetPresenceLines(int viewerPlayerId, Room room)
    {
        foreach ((int otherPlayerId, Player otherPlayer) in players)
        {
            if (otherPlayerId == viewerPlayerId)
            {
                continue;
            }

            if (otherPlayer.GetCurrentRoom() == room && playerConnections.GetValueOrDefault(otherPlayerId))
            {
                yield return $"{otherPlayer.DisplayName} is here as well.";
            }
        }

        if (oldHorseFollower.getCurrentRoom() == room)
        {
            yield return oldHorseFollower.GetRandomIdleText();
        }

        if (protag.getCurrentRoom() == room)
        {
            yield return "The protagonist is here, bumbling about the area.";
        }
    }

    internal Command ParseCommandText(string commandText)
    {
        return parser.ParseCommand(commandText);
    }

    internal CommandExecutionResult ExecuteAuthoritativeCommand(int playerId, Command command)
    {
        lock (commandSync)
        {
            int previousActivePlayerId = activePlayerId;
            activePlayerId = playerId;
            StringWriter capture = new();
            TextWriter originalOut = Console.Out;

            try
            {
                Console.SetOut(capture);
                bool shouldQuit = ProcessCommand(command);

                return new CommandExecutionResult
                {
                    ShouldQuit = shouldQuit,
                    OutputText = NormalizeLineEndings(capture.ToString()),
                    RoomText = GetLocationInfoText(playerId),
                    Snapshot = CaptureMultiplayerState()
                };
            }
            finally
            {
                Console.SetOut(originalOut);
                activePlayerId = previousActivePlayerId;
            }
        }
    }

    internal LocalCommandExecutionResult ExecuteLocalAuthoritativeCommand(int playerId, Command command)
    {
        lock (commandSync)
        {
            int previousActivePlayerId = activePlayerId;
            activePlayerId = playerId;

            try
            {
                bool shouldQuit = ProcessCommand(command);
                return new LocalCommandExecutionResult
                {
                    ShouldQuit = shouldQuit,
                    Snapshot = CaptureMultiplayerState()
                };
            }
            finally
            {
                activePlayerId = previousActivePlayerId;
            }
        }
    }

    internal MultiplayerSessionState CaptureMultiplayerState()
    {
        MultiplayerSessionState snapshot = new()
        {
            SwampCleared = progress.SwampCleared,
            ForgePrepared = progress.ForgePrepared,
            SwordPlaced = progress.SwordPlaced,
            GateOpen = progress.GateOpen,
            ToldProtagGate = progress.ToldProtagGate,
            ToldProtagSword = progress.ToldProtagSword,
            ProtagonistRoomId = protag.getCurrentRoom().GetId(),
            ProtagonistStepCounter = protag.getProtagStepsCount(),
            FollowerRoomId = oldHorseFollower.getCurrentRoom().GetId(),
            FollowerIsFollowing = oldHorseFollower.IsFollowing(),
            FollowerInventory = oldHorseFollower.GetInventoryItemNames()
        };

        foreach (Room room in allRooms)
        {
            snapshot.RoomItems[room.GetId()] = room.GetItemNames();
        }

        foreach ((int playerId, Player currentPlayer) in players)
        {
            snapshot.Players[playerId] = new MultiplayerPlayerState
            {
                PlayerId = playerId,
                DisplayName = currentPlayer.DisplayName,
                CurrentRoomId = currentPlayer.GetCurrentRoom().GetId(),
                BacktrackRoomIds = currentPlayer.GetLastRoomIds(),
                Inventory = currentPlayer.GetInventoryItemNames(),
                CarryWeight = currentPlayer.getCarryWeight(),
                IsConnected = playerConnections.GetValueOrDefault(playerId)
            };
        }

        return snapshot;
    }

    internal void ApplyMultiplayerState(MultiplayerSessionState snapshot)
    {
        Dictionary<int, Room> roomsById = allRooms.ToDictionary(room => room.GetId());

        progress.SwampCleared = snapshot.SwampCleared;
        progress.ForgePrepared = snapshot.ForgePrepared;
        progress.SwordPlaced = snapshot.SwordPlaced;
        progress.GateOpen = snapshot.GateOpen;
        progress.ToldProtagGate = snapshot.ToldProtagGate;
        progress.ToldProtagSword = snapshot.ToldProtagSword;

        if (roomsById.TryGetValue(snapshot.ProtagonistRoomId, out Room? protagRoom))
        {
            protag.setCurrentRoom(protagRoom);
        }

        protag.setProtagStepsCount(snapshot.ProtagonistStepCounter);

        if (roomsById.TryGetValue(snapshot.FollowerRoomId, out Room? followerRoom))
        {
            oldHorseFollower.setCurrentRoom(followerRoom);
        }

        oldHorseFollower.ClearInventory();
        foreach (string itemName in snapshot.FollowerInventory)
        {
            oldHorseFollower.addItem(CreateItemByName(itemName));
        }

        if (snapshot.FollowerIsFollowing)
        {
            oldHorseFollower.Follow();
        }
        else
        {
            oldHorseFollower.Stay();
        }

        foreach (Room room in allRooms)
        {
            room.ClearItems();
        }

        foreach ((int roomId, List<string> itemNames) in snapshot.RoomItems)
        {
            if (!roomsById.TryGetValue(roomId, out Room? room))
            {
                continue;
            }

            foreach (string itemName in itemNames)
            {
                room.addItem(CreateItemByName(itemName));
            }
        }

        foreach ((int playerId, MultiplayerPlayerState playerState) in snapshot.Players)
        {
            EnsurePlayerExists(playerId);
            Player currentPlayer = players[playerId];

            if (roomsById.TryGetValue(playerState.CurrentRoomId, out Room? currentRoom))
            {
                currentPlayer.setCurrentRoom(currentRoom);
            }

            currentPlayer.RestoreLastRooms(
                playerState.BacktrackRoomIds
                    .Where(roomsById.ContainsKey)
                    .Select(roomId => roomsById[roomId]));

            currentPlayer.ClearInventory();
            foreach (string itemName in playerState.Inventory)
            {
                currentPlayer.addItem(CreateItemByName(itemName));
            }

            currentPlayer.setCarryWeight(playerState.CarryWeight);
            playerConnections[playerId] = playerState.IsConnected;
        }
    }

    internal void PrintLocationInfoForPlayer(int playerId)
    {
        Console.WriteLine(GetLocationInfoText(playerId));
    }

    internal bool IsSaveLoadBlocked()
    {
        if (!multiplayerEnabled)
        {
            return false;
        }

        Console.WriteLine("Save/load is unavailable during multiplayer sessions.");
        return true;
    }

    internal string NormalizeLineEndings(string text)
    {
        return text.Replace("\r\n", "\n");
    }

    private static Item CreateItemByName(string itemName)
    {
        return itemName.ToLowerInvariant() switch
        {
            "axe" => ItemFactory.Create("axe", "a battered war AXE", 55, 0),
            "ring" => ItemFactory.Create("ring", "a shining RING with a knight's insignia", 2, 1),
            "hammer" => ItemFactory.Create("hammer", "a standard issue craft HAMMER with a flat head", 34, 2),
            "ore" => ItemFactory.Create("ore", "a chunk of unrefined ORE", 46, 3),
            "hilt" => ItemFactory.Create("hilt", "a HILT of an old sword", 17, 4),
            "sword" => ItemFactory.Create("sword", "a sharp SWORD with a regal gleam", 22, 5),
            _ => throw new InvalidOperationException($"Unknown item name: {itemName}")
        };
    }
}
