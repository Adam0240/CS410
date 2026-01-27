//Transcribed Rooms.java.java file - Dan Tager

using ConsoleApp_121_FinalProjectShell;
using System;
using System.Collections.Generic;
using System.Linq;

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
public class Room 
{
    private string description;
    private Dictionary<string, Room> exits;
    private List<Item> roomItems;
    private int roomID;
    private static List<Item> givenItems;

    //fields used to track game progress
    private static bool swampCleared;
    private static bool forgePrepared;
    private static bool swordPlaced;
    private static bool gateOpen;
    private static bool toldProtagGate;
    private static bool toldProtagSword;

    //every flag begins as false
    static Room()
    {
        swampCleared = false;
        forgePrepared = false;
        swordPlaced = false;
        gateOpen = false;
        toldProtagGate = false;
        toldProtagSword = false;
    }

    //object constructor
    public Room(string description, int roomID) 
    {
        this.description = description;
        exits = new Dictionary<string, Room>();
        roomItems = new List<Item>();
        this.roomID = roomID;
    }

    /**
     * Define an exit of this room. 
     * String is the exit name typed as part of a GO command
     * Room is where that exit leads
     */
    public void setExit(string direction, Room room) 
    {
        exits[direction] = room;
    } 
    //accessor for the room's exits
    public Dictionary<string, Room> getExits()
    {
        return exits;
    }

    /**
     * mutator and accessor for the game's clear conditions.
     * 
     * accessor returns as a boolean array to reduce number of methods
     * an integer value is used to refer to each flag to stay concise
     */
    public static void setClearCon(int ID, bool state)
    {
        switch (ID)
        {
            case 0:
                swampCleared = state;
                break;
            case 1:
                forgePrepared = state;
                break;
            case 2:
                swordPlaced = state;
                break;
            case 3:
                gateOpen = state;
                break;
            case 4:
                toldProtagGate = state;
                break;
            case 5:
                toldProtagSword = state;
                break;
            default:
                //something has gone terribly wrong
                break;
        }
    }

    public static bool[] getClearCons()
    {
        bool[] tempArray = new bool[6];

        tempArray[0] = swampCleared;
        tempArray[1] = forgePrepared;
        tempArray[2] = swordPlaced;
        tempArray[3] = gateOpen;
        tempArray[4] = toldProtagGate;
        tempArray[5] = toldProtagSword;

        return tempArray;
    }

    //returns ID, used for several checks in Game
    public int getID()
    {
        return roomID;
    }

    //accessors, mutators, and the like for items in the room
    public void addItem(Item item)
    {
        roomItems.Add(item);
    }

    public bool hasItem(Item item)
    {

        if (roomItems.Contains(item))
        {
            return true;
        }
        return false;
    }

    public Item getItemByName(string name) 
    {
        foreach (Item item in roomItems)
        {
            if (item.getName().Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                return item;
            }
        }
        return null;
    }

    public void removeItemByName(string name)
    {
        foreach (Item item in roomItems)
        {
            if (item.getName().Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                roomItems.Remove(item);
                return;
            }
        }
        return;
    }

    public bool hasItemByName(string name)
    {
        foreach (Item item in roomItems)
        {
            if (item.getName().Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }

    /**
     * @return The description of the room.
     * returns unique descriptions depending on the room and what flags are true
     */
    public string getDescription()
    {
        if (roomID == 4 && forgePrepared) {return "Lava flows through the channels dug into the rock around a vacant smith's shop. \nThe forge and its tools stand complete.";}
        else if (roomID == 6 && gateOpen) {return "The castle gate stands tall and imposing as before. Now however, \na large hole has been hacked through to the other side.";}
        else if (roomID == 1 && swampCleared) {return "Your boots catch in the stiff and stinking muck of the swamp. \nThe large log lies in pieces now, revealing a hidden path.";}
        else if (roomID == 8 && swordPlaced) {return "Sunlight filters through the treetops into the solitary grove. \nA derelict altar stands at its center, now bearing a shining sword.";}
        else {return description;}
    }

    //@returns the list of exits
    //skips the grove if the path has not been cleared
    public string getExitString()
    {
        string returnString = "Exits:";
        var keys = exits.Keys;
        foreach(string exit in keys) {
            if (exit == "grove" && swampCleared == false)
            {
                continue;
            } else
            {
                returnString += " " + exit;
            }
        }
        return returnString;

    }

    //@returns a string containing the decription of every item in roomItems
    private string getItemsText()
    {
        string tempText = "";
        tempText +=  "There is ";
        for (int i = 0; i < roomItems.Count; i++)
        {
            if (i > 0) {tempText += ", \n";}
            if (i > 0 && i == (roomItems.Count - 1)){ tempText += "and ";}
            tempText += roomItems[i].getDesc();
        }
        return tempText;
    }

    //@returns a string compiled from several others
    public string getLongDesc()
    {
        if (roomItems.Count == 0)
        {
            return getDescription() + "\n" + getExitString();    
        } else 
        { return getDescription() + "\n" + getItemsText() + ". \n" + getExitString();}
    }

    //@returns the String from the key to a randomly chosen exit
    public string getRandomExit()
    {
        List<string> tempExits = new List<string>(exits.Keys);
        //god this sucks so bad
        //if there's a better way to do this let me know
        return tempExits[Game.random.Next(tempExits.Count)];
    }

}