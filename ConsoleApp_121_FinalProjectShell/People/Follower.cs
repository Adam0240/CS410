// Follower.cs
// Companion character that can follow/stay and maintain its own inventory.
// Designed to match Player + Character coding style in this project.

using System.Collections;
using ConsoleApp_121_FinalProjectShell.Commands;
using ConsoleApp_121_FinalProjectShell.Core;
using ConsoleApp_121_FinalProjectShell.Items;

namespace ConsoleApp_121_FinalProjectShell.People;

/// <summary>
/// A companion NPC that can follow the player or stay in a room.
/// The follower has its own inventory and can trade items with the player.
/// </summary>
public class Follower : Character, IGameInventory
{
    // Inventory and weight tracking (mirrors Player pattern).
    private readonly ArrayList _inventory;
    private int _carryWeight;
    private int _currentWeight;

    private readonly List<string> _idleText = [];

    // Follow-state control.
    // true  = follower auto-moves with player
    // false = follower waits in place
    private bool _isFollowing;

    // Optional flavor/name for dialogue/output.
    private readonly string _name;
    
    
    // Default constructor.
    // Starts with no room, in following mode, and a modest carry limit.
    public Follower(Game? game = null, string name = "companion", int carryWeight = 80)
    {
        _inventory = [];
        _carryWeight = carryWeight;
        _currentWeight = 0;
        _isFollowing = false;
        _name = name;
        if (game != null)
        {
            game.PlayerMovement += PlayerMoved;
        }
    }

    // Overloaded constructor with a starting room.
    public Follower(Room startRoom, Game? game = null, string name = "Old Mule", int carryWeight = 80) : base(startRoom)
    {
        _inventory = [];
        _carryWeight = carryWeight;
        _currentWeight = 0;
        _isFollowing = false;
        _name = name;

        if (game != null)
        {
            game.PlayerMovement += PlayerMoved;
        }

    }

    // ---------------------------
    // Follow / stay behavior
    // ---------------------------

    public bool IsFollowing() { return _isFollowing; }

    public void Follow()
    {
        _isFollowing = true;
    }

    public void Stay()
    {
        _isFollowing = false;
    }

    public string GetName() { return _name; }
    
    void PlayerMoved(object? sender, Command command)
    {
        if (_isFollowing)
        {
            goRoom(command);
        }
    }


    // ---------------------------
    // Inventory helpers
    // ---------------------------

    internal int GetCurrentWeight() { return _currentWeight; }
    internal int GetCarryWeight() { return _carryWeight; }
    internal void SetCarryWeight(int number) { _carryWeight = number; }

    private void UpdateCarryWeight()
    {
        int tempWeight = 0;
        foreach (Item item in _inventory)
        {
            tempWeight += item.GetWeight();
        }
        _currentWeight = tempWeight;
    }

    private Item? FindItem(string name)
    {
        foreach (Item item in _inventory)
        {
            if (item.GetName().Equals(name, StringComparison.OrdinalIgnoreCase))
                return item;
        }
        return null;
    }

    // ---------------------------
    // IGameInventory implementation
    // ---------------------------

    public string itemsText()
    {
        var iText = new System.Text.StringBuilder();
        iText.Append($"{_name} is holding");

        if (_inventory.Count > 0)
        {
            iText.Append(':');
            foreach (Item item in _inventory)
                iText.Append("  " + item.GetName());
            iText.AppendLine();
        }
        else
        {
            iText.AppendLine(" nothing.");
        }

        iText.Append("Current weight: " + _currentWeight + "/" + _carryWeight);
        return iText.ToString();
    }

    public bool hasItemByName(string name)
    {
        return FindItem(name) != null;
    }

    public Item? getItemByName(string name)
    {
        return FindItem(name);
    }

    public void removeItemByName(string name)
    {
        Item? item = FindItem(name);
        if (item != null)
        {
            _inventory.Remove(item);
            UpdateCarryWeight();
        }
    }

    public void addItem(Item item)
    {
        _inventory.Add(item);
        UpdateCarryWeight();
    }

    public bool isValidItem(Item item)
    {
        if (item == null) return false;
        return GetCarryWeight() >= GetCurrentWeight() + item.GetWeight();
    }

    // ---------------------------
    // Trading helpers
    // ---------------------------

    // Moves an item from the player inventory to follower inventory.
    // Caller (Game) should check that both are in the same room first.
    // Returns true on success.
    public bool ReceiveFromPlayer(Player player, string itemName)
    {
        Item? item = player.getItemByName(itemName);
        if (item == null) return false;
        if (!isValidItem(item)) return false;

        player.removeItemByName(itemName);
        addItem(item);
        return true;
    }

    // Moves an item from follower inventory to player inventory.
    // Caller (Game) should check room proximity first.
    // Returns true on success.
    public bool GiveToPlayer(Player player, string itemName)
    {
        Item? item = getItemByName(itemName);
        if (item == null) return false;
        if (!player.isValidItem(item)) return false;

        removeItemByName(itemName);
        player.addItem(item);
        return true;
    }



    public void AddIdleText(string text)
    {
        _idleText.Add(text);
    }
    public List<string> GetAllIdleText()
    {
        return _idleText;
    }

    public string GetRandomIdleText()
    {
        if (_idleText.Count == 0)
        {
            return $"{_name} waits quietly.";
        }

        return _idleText[Game.random.Next(_idleText.Count)];
    }

    //save state addition
    internal List<string> GetInventoryItemNames()
    {
        var names = new List<string>();
        foreach (Item item in _inventory)
        {
            names.Add(item.GetName());
        }
        return names;
    } 

    internal void ClearInventory()
    {
        _inventory.Clear();
        UpdateCarryWeight();
    }
}

