//Transcribed Player.java file - Dan Tager

using System.Collections;
using System.Transactions;
using ConsoleApp_121_FinalProjectShell.Commands;
using ConsoleApp_121_FinalProjectShell.Core;
using ConsoleApp_121_FinalProjectShell.Items;
namespace ConsoleApp_121_FinalProjectShell.People;


/// <summary>
/// Manages traits and actions unique to the player character, such as item inventory.
/// Holds items in an arraylist and manages it using a weight limit.
/// Would have much more functionality in an actually complex game. 
///</summary>
public class Player : Character, IGameInventory
{
    //inventory-based parameters
    //may be tied into an interface in the future
    private readonly ArrayList _inventory;
    private int _carryWeight;
    private int _currentWeight;

    
    /// <summary>
    /// Creates a new instance of Player and initializes default values.
    /// Barring testing there should only be one of these in a given game.
    /// </summary>
    public Player()
    {
        _inventory = new ArrayList();
        _carryWeight = 100;
        _currentWeight = 0;
    }
    
    /// <summary>
    /// Overloaded constructor for also initializing a starting room.
    /// </summary>
    /// <param name="startroom">The room to place the instance of Player in.</param>
    public Player(Room startroom) : base(startroom)
    {
        _inventory = new ArrayList();
        _carryWeight = 100;
        _currentWeight = 0;
    }

    //Accessors and Mutators 
    public int getCurrentWeight() { return _currentWeight; }
    
    public int getCarryWeight() { return _carryWeight; }
    
    public void setCarryWeight(int number) { _carryWeight = number; }

    /// <summary>
    /// Called when adding or removing items from the inventory, recalculates current total inventory weight
    /// Used for consistency purposes to ensure nothing else ever has to update it.
    /// </summary>
    private void updateCarryWeight()
    {
        int tempweight = 0;
        foreach (Item item in _inventory)
        
            tempweight += item.GetWeight();
        
        _currentWeight = tempweight;
    }
    
    /// <summary>
    /// Helper method that keeps inventory searching contained to a single method to prevent duplication.
    /// </summary>
    /// <param name="name">The name of the item to find.</param>
    /// <returns>The instance of Item with a matching name, or null if there is none.</returns>
    private Item? findItem(string name)
    {
        foreach (Item? item in _inventory)
        {
            if (item.GetName().Equals(name, StringComparison.OrdinalIgnoreCase))
                return item;
        }
        return null;
    }

    //returns description of items held. called by ITEMS command
    /// <summary>
    /// Returns the list of descriptions of every item held in the inventory, exclusively called when using the ITEMS
    /// command in-game.
    /// </summary>
    /// <returns>A string description of the Player object's inventory.</returns>
    public string itemsText()
    {
        var iText = new System.Text.StringBuilder();
        iText.Append("You are holding");

        if (_inventory.Count > 0)
        {
            iText.Append(':');
            foreach (Item item in _inventory)
                iText.Append("  " + item.GetName());
            iText.AppendLine();
        }
        else
        {
            iText.AppendLine(" nothing.");
            
        }

        iText.Append("Current weight: " + _currentWeight);
        return iText.ToString();
    }

    /// <summary>
    /// Uses findItem() to check if an Item is in the player's inventory. 
    /// </summary>
    /// <param name="name">The name of the item to look for.</param>
    /// <returns>True if the named item is in the inventory, false otherwise. </returns>
    public bool hasItemByName(string name)
    {
        return findItem(name) != null;
    }

    /// <summary>
    /// Uses findItem() to fetch a named Item from the player's inventory. Note that this does not  remove the Item
    /// from the inventory.
    /// </summary>
    /// <param name="name">The name of the item to retrieve.</param>
    /// <returns>The object instance of the item needed, or null if it doesn't exist.</returns>
    public Item? getItemByName(string name)
    {
        return findItem(name);
    }

    /// <summary>
    /// Uses findItem() to identify a named Item in the player's inventory and removes it.
    /// Does not return the removed item.
    /// </summary>
    /// <param name="name">The name of the item to remove.</param>
    public void removeItemByName(string name)
    {
        Item? item = findItem(name);
        if (item != null)
        {
            _inventory.Remove(item);
            updateCarryWeight();
        }
    }

    /// <summary>
    /// Takes in an Item and adds it to the player's inventory. Weight checking is done externally (to allow for it to
    /// potentially be bypassed for some reason)>
    /// </summary>
    /// <param name="item">The Item to add to the player's inventory.</param>
    public void addItem(Item? item)
    {
        _inventory.Add(item);
        updateCarryWeight();
    }
    
    //Helper method that tells whether something can be added to the inventory without overcapping the weight limit.
    public bool isValidItem(Item item)
    {
        return getCarryWeight() >= getCurrentWeight();
    }
    
}