namespace ConsoleApp_121_FinalProjectShell.Items;

/**
 * 
 */
public interface IGameInventory
{
    String itemsText();
    bool hasItemByName(string name);
    
    Item getItemByName(string name);
    
    void removeItemByName(string name);
    
    void addItem(Item item);
    
    bool isValidItem(Item item);
}