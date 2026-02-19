//Transcribed Player.java file - Dan Tager

using System.Collections;
using System.Transactions;
using ConsoleApp_121_FinalProjectShell.Commands;
using ConsoleApp_121_FinalProjectShell.Core;

namespace ConsoleApp_121_FinalProjectShell.People;

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
public class Player : Character
{
    
    private ArrayList inventory;
    private int carryWeight;
    private int currentWeight;

    //bug: had to change the field name - adam
    

    
    //basic constructor
    //does not initialize Steps if it's not the protag
    public Player() : base()
    {
        inventory = new ArrayList();
        carryWeight = 100;
        currentWeight = 0;
        
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

    
    //called after adding or removing items from the inventory
    public void updateCarryWeight()
    {
        int tempweight = 0;
        foreach (Item item in inventory)
        
            tempweight += item.getWeight();
        
        currentWeight = tempweight;
    }

    //returns description of items held. called by ITEMS command
    public string itemsText()
    {
        var iText = new System.Text.StringBuilder();
        iText.Append("You are holding");

        if (inventory.Count > 0)
        {
            iText.Append(":");
            foreach (Item item in inventory)
                iText.Append("  " + item.getName());
            iText.AppendLine();
        }
        else
        {
            iText.AppendLine(" nothing.");
            
        }

        iText.Append("Current weight: " + currentWeight);
        return iText.ToString();
    }

    //Helper method
    private Item findItem(string name)
    {
        foreach (Item item in inventory)
        {
            if (item.getName().Equals(name, StringComparison.OrdinalIgnoreCase))
                return item;
        }
        return null;
    }

    //methods for interaction with inventory
    public bool hasItemByName(string name)
    {
        return findItem(name) != null;
    }

    public Item getItemByName(string name)
    {
        return findItem(name);
    }


    public void removeItemByName(string name)
    {
        Item item = findItem(name);
        if (item != null)
        {
            inventory.Remove(item);
            updateCarryWeight();
        }
    }

    public void addItem(Item item)
    {
        inventory.Add(item);
        updateCarryWeight();
    }
    //returns true if something with a weight of itemWeight can be added to inventory
    public bool weightCheck(int itemWeight)
    {
        return getCarryWeight() >= getCurrentWeight() + itemWeight;
    }
    
}