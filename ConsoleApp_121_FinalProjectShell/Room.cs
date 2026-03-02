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
public class Room(string description, int roomId)
{
    private readonly Dictionary<string, Room> _exits = new();
    private readonly List<Item?> _roomItems = [];

    //fields used to track game progress
    private static bool _swampCleared;
    private static bool _forgePrepared;
    private static bool _swordPlaced;
    private static bool _gateOpen;
    private static bool _toldProtagGate;
    private static bool _toldProtagSword;

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

    /**
 * mutator and accessor for the game's clear conditions.
 *
 * accessor returns as a boolean array to reduce number of methods
 * an integer value is used to refer to each flag to stay concise
 */
    public static void setClearCon(int id, bool state)
    {
        switch (id)
        {
            case 0:
                _swampCleared = state;
                break;
            case 1:
                _forgePrepared = state;
                break;
            case 2:
                _swordPlaced = state;
                break;
            case 3:
                _gateOpen = state;
                break;
            case 4:
                _toldProtagGate = state;
                break;
            case 5:
                _toldProtagSword = state;
                break;
        }
    }

    public static bool[] getClearCons() =>
    [
        _swampCleared,
        _forgePrepared,
        _swordPlaced,
        _gateOpen,
        _toldProtagGate,
        _toldProtagSword
    ];

    //returns ID, used for several checks in Game
    public int GetId()
    {
        return roomId;
    }

    //accessors, mutators, and the like for items in the room
    public void addItem(Item? item)
    {
        _roomItems.Add(item);
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


    //technical debt:
    //unique room descriptions for rooms with alternate descriptions are all stored here
    //rooms are dependent on an instance in a class, and adding more rooms with clear conditions
    //requires modifying this method
    //potential solution: refactor Room to allow rooms to store their own Clear Condition flag or
    //alternative description. Factory pattern could also be implemented to add each room with a clear condition
    //to a static list in Room to allow for easier checking of all clearcons. 
    /**
 * @return The description of the room.
 * returns unique descriptions depending on the room and what flags are true
 */
    public string getDescription()
    {
        if (roomId == 4 && _forgePrepared)
        {
            return "Lava flows through the channels dug into the rock around a vacant smith's shop. \nThe forge and its tools stand complete.";
        }

        if (roomId == 6 && _gateOpen)
        {
            return "The castle gate stands tall and imposing as before. Now however, \na large hole has been hacked through to the other side.";
        }

        if (roomId == 1 && _swampCleared)
        {
            return "Your boots catch in the stiff and stinking muck of the swamp. \nThe large log lies in pieces now, revealing a hidden path.";
        }

        if (roomId == 8 && _swordPlaced)
        {
            return "Sunlight filters through the treetops into the solitary grove. \nA derelict altar stands at its center, now bearing a shining sword.";
        }
        
        return description;
    }

    //@returns the list of exits
    //skips the grove if the path has not been cleared
    public string getExitString()
    {
        var exitString = new System.Text.StringBuilder("Exits:");
        foreach (string exit in _exits.Keys)
        {
            if (exit != "grove" || _swampCleared)
            {
                exitString.Append(" ").Append(exit);
            }
        }
        return exitString.ToString();
    }

    //@returns a string containing the decription of every item in roomItems
    private string getItemsText()
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

    //@returns a string compiled from several others
    public string getLongDesc()
    {
        var builtDescription = new System.Text.StringBuilder(getDescription());
        builtDescription.Append("\n");

        if (_roomItems.Count > 0)
        {
            builtDescription.Append(getItemsText()).Append(". \n");
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
    public int getItemsCount()
    {
        return _roomItems.Count;
    }

}