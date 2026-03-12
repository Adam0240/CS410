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

    //weight manipulation methods for ease of use and readability
    internal int getCurrentWeight() { return _currentWeight; }
    internal int getCarryWeight() { return _carryWeight; }
    internal void setCarryWeight(int number) { _carryWeight = number; }

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
        foreach (Item item in _inventory)
        {
            if (item.GetName().Equals(name, StringComparison.OrdinalIgnoreCase))
                return item;
        }
        return null;
    }

    
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

        iText.Append("Current weight: " + _currentWeight + "/" + _carryWeight);
        return iText.ToString();
    }

    
    public bool hasItemByName(string name)
    {
        return findItem(name) != null;
    }
    
    public Item? getItemByName(string name)
    {
        return findItem(name);
    }
    
    public void removeItemByName(string name)
    {
        Item? item = findItem(name);
        if (item != null)
        {
            _inventory.Remove(item);
            updateCarryWeight();
        }
    }
    
    public void addItem(Item item)
    {
        _inventory.Add(item);
        updateCarryWeight();
    }
    
    
    public bool isValidItem(Item item)
    {
        return getCarryWeight() >= getCurrentWeight() + item.GetWeight();
    }

    internal Room GetCurrentRoom()
    {
        return getCurrentRoom();
    }
}