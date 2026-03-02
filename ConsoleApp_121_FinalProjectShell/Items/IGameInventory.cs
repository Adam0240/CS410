namespace ConsoleApp_121_FinalProjectShell.Items;

/**
 * 
 */
public interface IGameInventory
{
    /// <summary>
    /// Returns the list of descriptions of every item held in the inventory. 
    /// </summary>
    /// <returns>A string description of the object's inventory.</returns>
    String itemsText();
    
    /// <summary>
    /// Uses findItem() to check if an Item is in the inventory. 
    /// </summary>
    /// <param name="name">The name of the item to look for.</param>
    /// <returns>True if the named item is in the inventory, false otherwise. </returns>
    bool hasItemByName(string name);
    
    /// <summary>
    /// Fetches a named Item from the inventory. Note that this does not  remove the Item from the inventory. 
    /// </summary>
    /// <param name="name">The name of the item to retrieve.</param>
    /// <returns>The object instance of the item needed, or null if it doesn't exist.</returns>
    Item getItemByName(string name);
    
    /// <summary>
    /// Identifies a named Item in the inventory and removes it.
    /// Does not return the removed item.
    /// </summary>
    /// <param name="name">The name of the item to remove.</param>
    void removeItemByName(string name);
    
    /// <summary>
    /// Takes in an Item and adds it to the inventory. IsValidItem should be called before this to ensure whether
    /// that the item can be added to the inventory or not. 
    /// </summary>
    /// <param name="item">The Item to add to the inventory.</param>
    void addItem(Item item);
    
    /// <summary>
    /// Reads in an item and tests to see if it can be added to the inventory. Inventory control functionality
    /// is unique to a given class' implementation of IGameInventory. Player uses a weight-based system, for instance.
    /// </summary>
    /// <param name="item">Item to check for validity.</param>
    /// <returns>True if the item can be added to that implementation of IGameInventory, false otherwise.</returns>
    bool isValidItem(Item item);
}