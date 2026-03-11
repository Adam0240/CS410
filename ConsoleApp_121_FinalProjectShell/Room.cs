//Transcribed Rooms.java.java file - Dan Tager

using ConsoleApp_121_FinalProjectShell.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleApp_121_FinalProjectShell.Items;

/**
* This class is part of the "FUGwACL Adventure" application.
* "FUGwACL Adventure" is a slightly less simple, text based adventure game.
*
* Room - a single room, static fields are flags that track completion of certain events.
*
* Exits are stored in a String + Room HashMap.
*
* @author  Michael Kölling, David J. Barnes, and Christian Byrne
* @version 2023-12-03
*/
public class Room(string description, int roomId) : IGameInventory
{
    private readonly Dictionary<string, Room> _exits = new();
    private readonly List<Item?> _roomItems = [];



    //object constructor

    /**
 * Define an exit of this room.
 * String is the exit name typed as part of a GO command
 * Room is where that exit leads
 */
    public void setExit(string direction, Room room) 
    {
        _exits[direction] = room;
    } 
    //accessor for the room's exits
    public Dictionary<string, Room> getExits()
    {
        return _exits;
    }

 

    //returns ID, used for several checks in Game
    public int GetId()
    {
        return roomId;
    }

    //technical debt:
    //unique room descriptions for rooms with alternate descriptions are all stored here
    //rooms are dependent on an instance in a class, and adding more rooms with clear conditions
    //requires modifying this method
    //potential solution: refactor Room to allow rooms to store their own flag for an alternative description.
    //Factory pattern could also be implemented to add each room with a clear condition to a static list in Game
    //to allow for easier checking of all clear conditions. 
    /**
 * @return The description of the room.
 * returns unique descriptions depending on the room and what flags are true
 */
    public string getDescription()
    {
        return description;
    }

    //@returns the list of exits
    //skips the grove if the path has not been cleared
    public string getExitString()
    {
        var exitString = new System.Text.StringBuilder("Exits:");
        foreach (string exit in _exits.Keys)
        {
            exitString.Append(" ").Append(exit);
        }
        return exitString.ToString();
    }

    //@returns a string compiled from several others
    public string getLongDesc()
    {
        var builtDescription = new System.Text.StringBuilder(getDescription());
        builtDescription.Append("\n");

        if (_roomItems.Count > 0)
        {
            builtDescription.Append(itemsText()).Append(". \n");
        }

        builtDescription.Append(getExitString());
        return builtDescription.ToString();
    }

    //@returns the String from the key to a randomly chosen exit
    public string getRandomExit()
    {
        var exitsArray = _exits.Keys.ToArray();
        return exitsArray[Game.random.Next(exitsArray.Length)];
    }

    //For testing, returns the number of items in roomItems
    internal int getItemsCount()
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
    
    public bool hasItem(Item? item) => _roomItems.Contains(item);

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
    
    public void addItem(Item? item)
    {
        _roomItems.Add(item);
    }

    public bool isValidItem(Item? item)
    {
        return true;
    }

}