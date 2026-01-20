import java.lang.*;
/**
 * This class is part of the "FUGwACL Adventure" application. 
 * "FUGwACL Adventure" is a slightly less simple, text based adventure game.
 * 
 * Holds information about Items. Does nothing on its own.
 *
 * @author  Michael Kölling, David J. Barnes, and Christian Byrne
 * @version 2023-12-03
 */
public class Item
{
    //what is it called
    private String itemName;
    //what does it look like
    private String itemDesc;
    private int itemWeight;
    private int itemID;
    public Item(String itemName, String itemDesc, int itemWeight, int itemID)
    {
        this.itemName = itemName;
        this.itemDesc = itemDesc;
        this.itemWeight = itemWeight;
        this.itemID = itemID;
    }

    //accessors. no mutators defined as items don't need to be modified after creation
    public String getName()
    {
        return itemName;
    }

    public String getDesc()
    {
        return itemDesc;
    }

    public int getWeight()
    {
        return itemWeight;
    }

    public int getID()
    {
        return itemID;
    }
}