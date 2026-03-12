using ConsoleApp_121_FinalProjectShell.Core;
using ConsoleApp_121_FinalProjectShell.Items;

namespace ConsoleApp_121_FinalProjectShell.Core.Persistence;


public static class GameStateMapper
{
    public static GameSaveState Capture(Game game)
    {
        var player = game.GetPlayer();
        var protag = game.GetProtag();
        var follower = game.GetFollower();
        var progress = game.GetProgress();

        var state = new GameSaveState
        {
            PlayerRoomId = player.GetCurrentRoom().GetId(),
            //Save State Edit 12
            PlayerBacktrackRoomIds = player.GetLastRoomIds(),
            PlayerInventory = player.GetInventoryItemNames(),
            PlayerCarryWeight = player.getCarryWeight(),

            ProtagonistRoomId = protag.getCurrentRoom().GetId(),
            ProtagonistStepCounter = protag.getProtagStepsCount(),

            FollowerRoomId = follower.getCurrentRoom().GetId(),
            FollowerIsFollowing = follower.IsFollowing(),
            FollowerInventory = follower.GetInventoryItemNames(),

            SwampCleared = progress.SwampCleared,
            ForgePrepared = progress.ForgePrepared,
            SwordPlaced = progress.SwordPlaced,
            GateOpen = progress.GateOpen,
            ToldProtagGate = progress.ToldProtagGate,
            ToldProtagSword = progress.ToldProtagSword
        };

        foreach (var room in game.GetAllRooms())
        {
            state.RoomItems[room.GetId()] = room.GetItemNames();
        }

        return state;
    }

    public static void Apply(Game game, GameSaveState state)
    {
        var player = game.GetPlayer();
        var protag = game.GetProtag();
        var follower = game.GetFollower();
        var progress = game.GetProgress();

        var roomsById = game.GetAllRooms().ToDictionary(r => r.GetId());

        // 1) positions
        if (roomsById.TryGetValue(state.PlayerRoomId, out var playerRoom))
            player.setCurrentRoom(playerRoom);

        //Save State Edit 13
        player.RestoreLastRooms(
            state.PlayerBacktrackRoomIds
                .Where(roomsById.ContainsKey)
                .Select(roomId => roomsById[roomId]));

        if (roomsById.TryGetValue(state.ProtagonistRoomId, out var protagRoom))
            protag.setCurrentRoom(protagRoom);

        if (roomsById.TryGetValue(state.FollowerRoomId, out var followerRoom))
            follower.setCurrentRoom(followerRoom);

        // 2) progress flags
        progress.SwampCleared = state.SwampCleared;
        progress.ForgePrepared = state.ForgePrepared;
        progress.SwordPlaced = state.SwordPlaced;
        progress.GateOpen = state.GateOpen;
        progress.ToldProtagGate = state.ToldProtagGate;
        progress.ToldProtagSword = state.ToldProtagSword;

        // 3) room items
        foreach (var room in game.GetAllRooms())
            room.ClearItems();

        foreach (var (roomId, itemNames) in state.RoomItems)
        {
            if (!roomsById.TryGetValue(roomId, out var room))
                continue;

            foreach (var itemName in itemNames)
                room.addItem(CreateItemByName(itemName));
        }

        // 4) player inventory + carry
        player.ClearInventory();
        foreach (var itemName in state.PlayerInventory)
            player.addItem(CreateItemByName(itemName));
        player.setCarryWeight(state.PlayerCarryWeight);

        // 5) follower inventory + follow state
        follower.ClearInventory();
        foreach (var itemName in state.FollowerInventory)
            follower.addItem(CreateItemByName(itemName));

        if (state.FollowerIsFollowing) follower.Follow();
        else follower.Stay();

        // 6) protag movement counter
        protag.setProtagStepsCount(state.ProtagonistStepCounter);
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
            _ => throw new InvalidOperationException($"Unknown item name in save: {itemName}")
        };
    }
}
