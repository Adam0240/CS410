//Transcribed Player.java file - Dan Tager

using System;
using System.Collections;
using System.Collections.Generic;
using ConsoleApp_121_FinalProjectShell;

/**
 * This class is part of the "FUGwACL Adventure" application. 
 * "FUGwACL Adventure" is a slightly less simple, text based adventure game.  
 * 
 * Used to keep track of both the player and the protagonist.
 * Has an arraylist to hold items with a weight limit, 
 * and tracks the order of rooms visited for the BACK command.
 *
 * @author  Michael Kölling, David J. Barnes, and Christian Byrne
 * @version 2023-12-03
 */
public class Player
{
    private Room currentRoom;
    private Stack<Room> lastRooms;
    private ArrayList inventory;
    private int carryWeight;
    private int currentWeight;

    //bug: had to change the field name - adam
    private int protagStepsCount;

    //determines if the created player is the "Protagonist", used for some checks
    //was going to be referenced more, but I didn't have time to implement more
    //things for the protag to do
    public readonly bool isProtag;

    
    //basic constructor
    //does not initialize Steps if it's not the protag
    public Player(bool isProtag)
    {
        lastRooms = new Stack<Room>();
        this.isProtag = isProtag;
        inventory = new ArrayList();
        carryWeight = 100;
        currentWeight = 0;
        if (isProtag)
        {
            protagStepsCount = Game.random.Next(6);
        }
    }

    //accessors and mutators for rooms
    public Room getCurrentRoom()
    {
        return currentRoom;
    }

    public void setCurrentRoom(Room room)
    {
        currentRoom = room;
    }

    public Stack<Room> getLastRooms()
    {
        return lastRooms;
    }

    //accessors and mutators for the weights
    public int getCurrentWeight()
    {
        return currentWeight;
    }

    public int getCarryWeight()
    {
        return carryWeight;
    }

    public void setCarryWeight(int number)
    {
        carryWeight = number;
    }

    public void setCurrentWeight(int number)
    {
        currentWeight = number;
    }

    
    //called after adding or removing items from the inventory
    public void updateCarryWeight()
    {
        int tempweight = 0;
        foreach (Item item in inventory)
        {
            tempweight += item.getWeight();
        }
        setCurrentWeight(tempweight);
    }

    //returns description of items held. called by ITEMS command
    public string itemsText()
    {
        string returnString = "";
        returnString += "You are holding";
        if (inventory.Count == 0)
        {
            returnString += " nothing. \n";
        }
        else
        {
            returnString += ":";
            foreach (Item item in inventory)
            {
                returnString +=  "  " + item.getName();
            }
            returnString += "\n";
        }
        returnString += "Current weight: " + getCurrentWeight();

        return returnString;
    }

    
    //methods for interaction with inventory
    public bool hasItemByName(string name)
    {
        foreach (Item item in inventory)
        {
            if (item.getName().Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }

    public Item getItemByName(string name) 
    {
        foreach (Item item in inventory)
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
        foreach (Item item in inventory)
        {
            if (item.getName().Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                inventory.Remove(item);
                updateCarryWeight();
                return;
            }
        }
        return;
    }

    public void addItem(Item item)
    {
        inventory.Add(item);
        updateCarryWeight();
    }
    //returns true if something with a weight of itemWeight can be added to inventory
    public bool weightCheck(int itemWeight)
    {
        return (getCarryWeight() >= (getCurrentWeight() + itemWeight));
    }

    /**
     * methods for handling the movement of the object. 
     * back() only used by the player
     */
    public int back()
    {
        int a = lastRooms.Count;
        if(lastRooms.Count != 0)
        {
            currentRoom = lastRooms.Pop();
        }
        return a;
    }

    public int goRoom(Command command) 
    {
        if(!command.HasSecondWord()) {
            //return zero for no second word
            return 0;
        }

        string direction = command.GetSecondWord();
        // Try to leave current room.
        Room nextRoom = null;
        if (currentRoom.getExits().ContainsKey(direction))
        {
            nextRoom = currentRoom.getExits()[direction];
        }

        if (nextRoom == null) {
            //return -1 if there isn't an exit
            return -1;
        }
        else {
            if (direction.Equals("slide", StringComparison.OrdinalIgnoreCase))
            {
                lastRooms.Push(currentRoom);
                currentRoom = nextRoom;
                //return 2 if we slidin
                return 2;
            }
            lastRooms.Push(currentRoom);
            currentRoom = nextRoom;
            //return 1 if successful
            return 1;
        }
    }

    //moves the protagonist. called after every command.
    public bool protagSteps(Command command)
    {
        if (protagStepsCount >= 8)
        {
            protagStepsCount -= 8;
            goRoom(command);
            return true;
        } else { protagStepsCount += Game.random.Next(4);}

        return false;
    }
}