import java.util.*;
import java.lang.*;
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
 * test
 */
public class Room 
{
    private String description;
    private HashMap<String, Room> exits;
    private ArrayList<Item> roomItems;
    private int roomID;
    private static ArrayList<Item> givenItems;

    //fields used to track game progress
    private static boolean swampCleared;
    private static boolean forgePrepared;
    private static boolean swordPlaced;
    private static boolean gateOpen;
    private static boolean toldProtagGate;
    private static boolean toldProtagSword;

    //every flag begins as false
    static {
        swampCleared = false;
        forgePrepared = false;
        swordPlaced = false;
        gateOpen = false;
        toldProtagGate = false;
        toldProtagSword = false;
    }

    //object constructor
    public Room(String description, int roomID) 
    {
        this.description = description;
        exits = new HashMap<>();
        roomItems = new ArrayList<>();
        this.roomID = roomID;
    }

    /**
     * Define an exit of this room. 
     * String is the exit name typed as part of a GO command
     * Room is where that exit leads
     */
    public void setExit(String direction, Room room) 
    {
        exits.put(direction, room);
    } 
    //accessor for the room's exits
    public HashMap<String, Room> getExits()
    {
        return exits;
    }

    /**
     * mutator and accessor for the game's clear conditions.
     * 
     * accessor returns as a boolean array to reduce number of methods
     * an integer value is used to refer to each flag to stay concise
     */
    public static void setClearCon(int ID, boolean state)
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
            default:
                //something has gone terribly wrong
                break;
        }
    }

    public static boolean[] getClearCons()
    {
        boolean[] tempArray = new boolean[6];

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
        roomItems.add(item);
    }

    public boolean hasItem(Item item)
    {

        if (roomItems.contains(item))
        {
            return true;
        }
        return false;
    }

    public Item getItemByName(String name) 
    {
        for (Item item : roomItems)
        {
            if (item.getName().equalsIgnoreCase(name))
            {
                return item;
            }
        }
        return null;
    }

    public void removeItemByName(String name)
    {
        for (Item item : roomItems)
        {
            if (item.getName().equalsIgnoreCase(name))
            {
                roomItems.remove(item);
                return;
            }
        }
        return;
    }

    public boolean hasItemByName(String name)
    {
        for (Item item : roomItems)
        {
            if (item.getName().equalsIgnoreCase(name))
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
    public String getDescription()
    {
        if (roomID == 4 && forgePrepared) {return "Lava flows through the channels dug into the rock around a vacant smith's shop. \nThe forge and its tools stand complete.";}
        else if (roomID == 6 && gateOpen) {return "The castle gate stands tall and imposing as before. Now however, \na large hole has been hacked through to the other side.";}
        else if (roomID == 1 && swampCleared) {return "Your boots catch in the stiff and stinking muck of the swamp. \nThe large log lies in pieces now, revealing a hidden path.";}
        else if (roomID == 8 && swordPlaced) {return "Sunlight filters through the treetops into the solitary grove. \nA derelict altar stands at its center, now bearing a shining sword.";}
        else {return description;}
    }

    //@returns the list of exits
    //skips the grove if the path has not been cleared
    public String getExitString()
    {
        String returnString = "Exits:";
        Set <String> keys = exits.keySet();
        for(String exit : keys) {
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
    private String getItemsText()
    {
        String tempText = "";
        tempText +=  "There is ";
        for (int i = 0; i < roomItems.size(); i++)
        {
            if (i > 0) {tempText += ", \n";}
            if (i > 0 && i == (roomItems.size() - 1)){ tempText += "and ";}
            tempText += roomItems.get(i).getDesc();
        }
        return tempText;
    }

    //@returns a string compiled from several others
    public String getLongDesc()
    {
        if (roomItems.isEmpty())
        {
            return getDescription() + "\n" + getExitString();    
        } else 
        { return getDescription() + "\n" + getItemsText() + ". \n" + getExitString();}
    }

    //@returns the String from the key to a randomly chosen exit
    public String getRandomExit()
    {
        ArrayList<String> tempExits = new ArrayList<String>(exits.keySet());

    // BUG: This assumes the room always has at least one exit.
    // If exits is empty, tempExits.size() will be 0 and
    // Game.random.nextInt(0) will crash the game.

        //god this sucks so bad
        //if there's a better way to do this let me know
        return tempExits.get(Game.random.nextInt(tempExits.size()));
    }

}
