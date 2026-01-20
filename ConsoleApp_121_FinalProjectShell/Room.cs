using System.Collections;

namespace ConsoleApp_121_FinalProjectShell;

/**
 * Class Room - a room in an adventure game.
 *
 * This class is part of the "World of Zuul" application. 
 * "World of Zuul" is a very simple, text based adventure game.  
 *
 * A "Room" represents one location in the scenery of the game.  It is 
 * connected to other rooms via exits.  For each existing exit, the room 
 * stores a reference to the neighboring room.
 * 
 * @author  Michael Kölling and David J. Barnes
 * @version 2016.02.29
 */

public class Room 
{
    private String identifier;
    private Hashtable exits;        // stores exits of this room.

    /**
     * Create a room with a room "identifier". Initially, it has
     * no exits. "identifier" is something like "kitchen" or
     * "court yard".
     * @param identifier The room's name or identification.
     */
    public Room(String identifier) 
    {
        this.identifier = identifier;
        exits = new Hashtable();
    }

    /**
     * Define an exit from this room.
     * @param direction The direction of the exit.
     * @param neighbor  The room to which the exit leads.
     */
    public void setExit(String direction, Room neighbor) 
    {
        exits.Add(direction, neighbor);
    }

    /**
     * @return The identifier assigned to the room
     * (the one that was defined in the constructor).
     */
    public String getIdentifier()
    {
        return identifier + "\n" + getExitString();
    }

    /**
     * Return a string describing the room's exits, for example
     * "Exits: north west".
     * @return Details of the room's exits.
     */
    private String getExitString()
    {
        String returnString = "Exits:";
        foreach(String exit in exits.Keys) {
            returnString += " " + exit;
        }
        return returnString;
    }

    /**
     * Return the room that is reached if we go from this room in direction
     * "direction". If there is no room in that direction, return null.
     * @param direction The exit's direction.
     * @return The room in the given direction.
     */
    public Room getExit(String direction) 
    {
        //return exits.get(direction);
        return (Room)exits[direction];
    }
}