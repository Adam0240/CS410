using ConsoleApp_121_FinalProjectShell.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleApp_121_FinalProjectShell.Items;

namespace ConsoleApp_121_FinalProjectShell;

public class Room(string description, int roomId) : IGameInventory
{
    private readonly Dictionary<string, Room> _exits = [];
    private readonly List<Item> _roomItems = [];
    
    /**
 * Define an exit of this room.
 * String is the exit name typed as part of a GO command
 * Room is where that exit leads
 */
    public void SetExit(string direction, Room room) 
    {
        _exits[direction] = room;
    } 
    //accessor for the room's exits
    public Dictionary<string, Room> GetExits()
    {
        return _exits;
    }

 

    //returns ID, used for several checks in Game
    public int GetId()
    {
        return roomId;
    }
    
    /**
    * @return The description of the room.
    * returns unique descriptions depending on the room and what flags are true
    */
    public string GetDescription()
    {
        return description;
    }

    //@returns the list of exits
    //skips the grove if the path has not been cleared
    public string GetExitString()
    {
        var exitString = new System.Text.StringBuilder("Exits:");
        foreach (string exit in _exits.Keys)
        {
            exitString.Append(' ').Append(exit);
        }
        return exitString.ToString();
    }

    //@returns a string compiled from several others
    public string GetLongDesc()
    {
        var builtDescription = new System.Text.StringBuilder(GetDescription());
        builtDescription.Append('\n');

        if (_roomItems.Count > 0)
        {
            builtDescription.Append(itemsText()).Append(". \n");
        }

        builtDescription.Append(GetExitString());
        return builtDescription.ToString();
    }

    //@returns the String from the key to a randomly chosen exit
    public string GetRandomExit()
    {
        var exitsArray = _exits.Keys.ToArray();
        return exitsArray[Game.random.Next(exitsArray.Length)];
    }

    //For testing, returns the number of items in roomItems
    internal int GetItemsCount()
    {
        return _roomItems.Count;
    }
    
    //@returns a string containing the decription of every item in roomItems
    public string itemsText()
    {
        var itemText = new System.Text.StringBuilder("There is ");

        for (int i = 0; i < _roomItems.Count; i++)
        {
            if (i > 0)
            {
                itemText.Append(", \n");
                if (i == _roomItems.Count - 1)
                {
                    itemText.Append("and ");
                }
            }
            itemText.Append(_roomItems[i].GetDesc());
        }
        return itemText.ToString();
    }
    
    public bool HasItem(Item item) => _roomItems.Contains(item);

    public Item? getItemByName(string name)
    {
        return _roomItems.FirstOrDefault(item =>
            item.GetName().Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public void removeItemByName(string name)
    {
        Item? itemToRemove = getItemByName(name);
        if (itemToRemove != null)
        {
            _roomItems.Remove(itemToRemove);
        }
    }

    public bool hasItemByName(string name)
    {
        return _roomItems.Any(item => item.GetName().Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public void addItem(Item item)
    {
        _roomItems.Add(item);
    }

    public bool isValidItem(Item item)
    {
        return true;
    }

    //save state
    internal List<string> GetItemNames()
    {
        return _roomItems.Select(i => i.GetName()).ToList();
    }

    internal void ClearItems()
    {
        _roomItems.Clear();
    }

}