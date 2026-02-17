//Item.cs
//Abstract class with shared data. Uses inheritance + overridden Use() methods (plus a factory)
//so each item controls its own behavior instead of Game deciding behavior with conditional logic.

/**
 * This class is part of the "FUGwACL Adventure" application. 
 * "FUGwACL Adventure" is a slightly less simple, text based adventure game.
 * 
 * Holds information about Items.
 *
 * REFACTOR:
 * - Item is now ABSTRACT, because "Item" is a concept, but each usable item needs custom behavior.
 * - Behavior is provided by derived classes (AxeItem, RingItem, etc.) via overriding Use().
 * - This removes the need for Game to switch on IDs to decide behavior.
 */

using ConsoleApp_121_FinalProjectShell.Core;
using System;

public interface IUsable
{
    //Returns true only if using the item should end the game.

    bool Use(Game game);
}


public abstract class Item : IUsable
{
    // what is it called
    private string itemName;
    // what does it look like
    private string itemDesc;
    private int itemWeight;
    private int itemID;

    /// PROTECTED constructor:
    /// Can only be called by derived classes (enforces inheritance usage).
    /// Prevents direct instantiation of abstract Item.
    /// </summary>
    protected Item(string itemName, string itemDesc, int itemWeight, int itemID)
    {
        this.itemName = itemName;
        this.itemDesc = itemDesc;
        this.itemWeight = itemWeight;
        this.itemID = itemID;
    }

    //Accessors (encapsulation — fields remain private). 
    public string getName() => itemName;
    public string getDesc() => itemDesc;
    public int getWeight() => itemWeight;

    //ABSTRACT METHOD (Polymorphism):
    //Each derived class MUST implement its own behavior when used.
    //This replaces old switch/if logic in Game.
    public int getID() => itemID;
    public abstract bool Use(Game game);
}

/* -------------------------------------------------------------------------
 * CONCRETE ITEM CLASSES (Inheritance)
 * Each class inherits from Item and overrides Use().
 * ------------------------------------------------------------------------- */

// AxeItem derives from Item.
//PUBLIC: This class must be public so it can be instantiated by the ItemFactory
//and accessed across project namespaces.
//SEALED: This class is sealed to prevent further inheritance.
//There is no reason for other classes to extend AxeItem, and sealing it
//protects its behavior from being modified through subclassing.
//This enforces design intent and keeps item behavior predictable.
public sealed class AxeItem : Item
{
    public AxeItem(string itemName, string itemDesc, int itemWeight, int itemID)
        : base(itemName, itemDesc, itemWeight, itemID) { }


    //POLYMORPHISM:
    //Game calls Use() on Item reference.
    //Runtime dispatch chooses AxeItem.Use().
    public override bool Use(Game game)
    {
        // Calls into Game logic (Game must expose axeUse as internal for this to compile)
        return game.axeUse();
    }
}

public sealed class RingItem : Item
{
    public RingItem(string itemName, string itemDesc, int itemWeight, int itemID)
        : base(itemName, itemDesc, itemWeight, itemID) { }

    public override bool Use(Game game)
    {
        // Game must expose ringUse as internal for this to compile
        game.ringUse();
        return false;
    }
}

public sealed class HammerItem : Item
{
    public HammerItem(string itemName, string itemDesc, int itemWeight, int itemID)
        : base(itemName, itemDesc, itemWeight, itemID) { }

    public override bool Use(Game game)
    {
        game.hammerUse();
        return false;
    }
}

public sealed class OreItem : Item
{
    public OreItem(string itemName, string itemDesc, int itemWeight, int itemID)
        : base(itemName, itemDesc, itemWeight, itemID) { }

    public override bool Use(Game game)
    {
        // Matches the existing “ore does nothing” behavior
        Console.WriteLine("There's nothing to do with this on its own.");
        return false;
    }
}

public sealed class HiltItem : Item
{
    public HiltItem(string itemName, string itemDesc, int itemWeight, int itemID)
        : base(itemName, itemDesc, itemWeight, itemID) { }

    public override bool Use(Game game)
    {
        game.hiltUse();
        return false;
    }
}

public sealed class SwordItem : Item
{
    public SwordItem(string itemName, string itemDesc, int itemWeight, int itemID)
        : base(itemName, itemDesc, itemWeight, itemID) { }

    public override bool Use(Game game)
    {
        return game.swordUse();
    }
}

/* -------------------------------------------------------------------------
 * FACTORY (Optional but highly useful)
 * Lets you keep your existing createRooms() lines mostly the same:
 *    Item axe = ItemFactory.Create(... id ...);
 * instead of new AxeItem(...) etc everywhere.
 * ------------------------------------------------------------------------- */
public static class ItemFactory
{
    public static Item Create(string name, string desc, int weight, int id)
    {
        return id switch
        {
            0 => new AxeItem(name, desc, weight, id),
            1 => new RingItem(name, desc, weight, id),
            2 => new HammerItem(name, desc, weight, id),
            3 => new OreItem(name, desc, weight, id),
            4 => new HiltItem(name, desc, weight, id),
            5 => new SwordItem(name, desc, weight, id),
            _ => new OreItem(name, desc, weight, id) //Safe fallback prevents null return
        };
    }
}
