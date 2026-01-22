//Transcribed Item.java file - Dan Tager

using System;

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
    private string itemName;
    //what does it look like
    private string itemDesc;
    private int itemWeight;
    private int itemID;
    public Item(string itemName, string itemDesc, int itemWeight, int itemID)
    {
        this.itemName = itemName;
        this.itemDesc = itemDesc;
        this.itemWeight = itemWeight;
        this.itemID = itemID;
    }

    //accessors. no mutators defined as items don't need to be modified after creation
    public string getName()
    {
        return itemName;
    }

    public string getDesc()
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