import java.util.*;
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
    private ArrayList<Item> inventory;
    private int carryWeight;
    private int currentWeight;
    private int protagSteps;

    //determines if the created player is the "Protagonist", used for some checks
    //was going to be referenced more, but I didn't have time to implement more
    //things for the protag to do
    public final boolean isProtag;

    
    //basic constructor
    //does not initialize Steps if it's not the protag
    public Player(boolean isProtag)
    {
        lastRooms = new Stack<Room>();
        this.isProtag = isProtag;
        inventory = new ArrayList<Item>();
        carryWeight = 100;
        currentWeight = 0;
        if (isProtag)
        {
            protagSteps = Game.random.nextInt(6);
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
        for (Item item : inventory)
        {
            tempweight += item.getWeight();
        }
        setCurrentWeight(tempweight);
    }

    //returns description of items held. called by ITEMS command
    public String itemsText()
    {
        String returnString = "";
        returnString += "You are holding";
        if (inventory.isEmpty())
        {
            returnString += " nothing. \n";
        }
        else
        {
            returnString += ":";
            for (Item item : inventory)
            {
                returnString +=  "  " + item.getName();
            }
            returnString += "\n";
        }
        returnString += "Current weight: " + getCurrentWeight();

        return returnString;
    }

    
    //methods for interaction with inventory
    public boolean hasItemByName(String name)
    {
        for (Item item : inventory)
        {
            if (item.getName().equalsIgnoreCase(name))
            {
                return true;
            }
        }
        return false;
    }

    public Item getItemByName(String name) 
    {
        for (Item item : inventory)
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
        for (Item item : inventory)
        {
            if (item.getName().equalsIgnoreCase(name))
            {
                inventory.remove(item);
                updateCarryWeight();
                return;
            }
        }
        return;
    }

    public void addItem(Item item)
    {
        inventory.add(item);
        updateCarryWeight();
    }
    //returns true if something with a weight of itemWeight can be added to inventory
    public boolean weightCheck(int itemWeight)
    {
        return (getCarryWeight() >= (getCurrentWeight() + itemWeight));
    }

    /**
     * methods for handling the movement of the object. 
     * back() only used by the player
     */
    public int back()
    {
        int a = lastRooms.size();
        if(!lastRooms.empty())
        {
            currentRoom = lastRooms.pop();
        }
        return a;
    }

    public int goRoom(Command command) 
    {
        if(!command.hasSecondWord()) {
            //return zero for no second word
            return 0;
        }

        String direction = command.getSecondWord();
        // Try to leave current room.
        Room nextRoom = null;
        if (currentRoom.getExits().containsKey(direction))
        {
            nextRoom = currentRoom.getExits().get(direction);
        }

        if (nextRoom == null) {
            //return -1 if there isn't an exit
            return -1;
        }
        else {
            if (direction.equalsIgnoreCase("slide"))
            {
                lastRooms.push(currentRoom);
                currentRoom = nextRoom;
                //return 2 if we slidin
                return 2;
            }
            lastRooms.push(currentRoom);
            currentRoom = nextRoom;
            //return 1 if successful
            return 1;
        }
    }

    //moves the protagonist. called after every command.
    public boolean protagSteps(Command command)
    {
        if (protagSteps >= 8)
        {
            protagSteps -= 8;
            goRoom(command);
            return true;
        } else {protagSteps += Game.random.nextInt(4);}

        return false;
    }
}