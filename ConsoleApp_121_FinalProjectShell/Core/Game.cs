using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ConsoleApp_121_FinalProjectShell.Commands;
using ConsoleApp_121_FinalProjectShell.Items;
using ConsoleApp_121_FinalProjectShell.People;


// ADDED FOR TESTING:
// This exposes internal members to the UnitTesting project so we can unit test without making everything public.
[assembly: InternalsVisibleTo("UnitTesting")]

namespace ConsoleApp_121_FinalProjectShell.Core;

/**
 *  This class is the main class of the "World of Zuul" application. 
 *  "World of Zuul" is a very simple, text based adventure game.  Users 
 *  can walk around some scenery. That's all. It should really be extended 
 *  to make it more interesting!
 * 
 *  REFACTOR NOTES (high level):
 *  - Command handling was refactored into a Command Pattern style registry (CommandActionRegistry).
 *    Game no longer contains a giant switch/if chain for commands.
 *  - Item behavior was refactored into polymorphism (Item.Use(Game)) and a Rule system in UseRules.cs.
 *    Game no longer decides most item behaviors with item IDs and conditional logic.
 */
public partial class Game
{
    // Core game dependencies (instances created in the constructor)
    private readonly Parser parser;
    private readonly Player player;
    private readonly Protagonist protag;
    private readonly Follower oldHorseFollower;
    private readonly GameProgress progress;

    // Shared RNG. This is seeded deterministically during tests for predictable behavior.
    internal static Random random = new();

    // Centralized default action used by multiple "use" behaviors.
    // This reduces repeated lambda allocations and keeps the message consistent.
    private static void DefaultNothing() => Console.WriteLine("Nothing to do with that here.");

    // givenItems holds items that are "spawned" or "given" by puzzle logic (ore, sword, etc.)
    // This is used by UseRules (example: hammer spawning ore, hilt forging sword).
    private readonly List<Item> givenItems;

    // ---------------------------
    // Command dispatch
    // ---------------------------

    // REFACTOR: Command Pattern dispatch table.
    // Instead of Game owning many nested command classes, we now look up a handler by CommandWord.
    private readonly Dictionary<CommandWord, ICommandAction> commandActions;

    // ---------------------------
    // Testing helpers
    // ---------------------------

    // Flag used to enable additional state tracking during tests (room/item lists, deterministic random, etc.)
    private readonly bool isTestInstance;

    // Exposed internally for unit testing (because of InternalsVisibleTo).
    internal List<Room> allRooms;
    internal List<Item> allItems;

    //SaveState addition
    internal List<Room> GetAllRooms() { return allRooms; }


    //event handler for player movement
    public event EventHandler<Command>? PlayerMovement;
    /*
     * Create the game and initialise its internal map.
     *
     * ADDITION:
     * Now takes a boolean determining whether to perform extra functions
     * to enable easier testing
     */
    public Game(bool isTestInstance)
    {
        // Stores "spawnable" items used by puzzle logic (ore, sword).
        givenItems = [];

        // REFACTOR: deterministic randomness for repeatable unit tests.
        random = isTestInstance ? new Random(0) : new Random();

        // Instantiate core game objects.
        parser = new Parser();
        player = new Player();
        protag = new Protagonist();
        oldHorseFollower = new Follower(this);
        progress = new GameProgress();

        this.isTestInstance = isTestInstance;

        // REFACTOR: Command actions are now defined in the Commands folder and registered here.
        // This keeps Game from ballooning with command-handler class definitions.
        commandActions = CommandActionRegistry.CreateDefault();

        // Test-only collections so unit tests can verify room/item creation and placement.

        allRooms = [];
        allItems = [];

        CreateRooms();
    }

    /**
     * Create all the rooms and link their exits together.
     * In addition, creates and places all items in their respective places.
     */
    internal void CreateRooms()
    {
        Room hub, swamp, battleGr, rocky, lava, graves, castleGate, castleTown, altarGrove;
        Item axe;
        Item ring;
        Item hammer;
        Item ore;
        Item hilt;
        Item sword;

        // Create the rooms (instances).
        hub = new Room("This campsite, used by travelers passing through, right now houses only you. \nA fitting place to rest when the job is done.", 0);
        swamp = new Room("Your boots catch in the stiff and stinking muck of the swamp. \nA large fallen log sits to one side.", 1);
        battleGr = new Room("The site of a once great battle stands silent, unusable weapons dotting the landscape.", 2);
        rocky = new Room("Standing at the remains of a collapsed quarry, you can see veins of ore within the stone.", 3);
        lava = new Room("Lava flows through channels dug into the rock around a vacant smith's shop, \nthe tools of which seem strangely lacking.", 4);
        graves = new Room("You stand in the graveyard where resurrected heroes first wake.", 5);
        castleGate = new Room("The wooden castle gate stands tall, imposing, and completely shut.", 6);
        castleTown = new Room("Standing in the deserted square of the castle's town, \nyou think at one point it must have been bustling with activity.", 7);
        altarGrove = new Room("Sunlight filters through the treetops into the solitary grove. \nA derelict altar stands at its center.", 8);

        // REFACTOR (abstraction + polymorphism):
        // Item is now abstract, so items are created via ItemFactory which returns concrete subclasses
        // (AxeItem, RingItem, etc.) that override Use(Game).
        axe = ItemFactory.Create("axe", "a battered war AXE", 55, 0);
        ring = ItemFactory.Create("ring", "a shining RING with a knight's insignia", 2, 1);
        hammer = ItemFactory.Create("hammer", "a standard issue craft HAMMER with a flat head", 34, 2);
        ore = ItemFactory.Create("ore", "a chunk of unrefined ORE", 46, 3);
        hilt = ItemFactory.Create("hilt", "a HILT of an old sword", 17, 4);
        sword = ItemFactory.Create("sword", "a sharp SWORD with a regal gleam", 22, 5);

        // TESTING ONLY: keep track of created room/item instances so unit tests can assert setup.
        TrackTestArtifacts(
            [hub, swamp, battleGr, rocky, lava, graves, castleGate, castleTown, altarGrove],
            [axe, ring, hammer, ore, hilt, sword]
        );


        // Initialise room exits (graph structure).
        hub.SetExit("north", castleTown);
        hub.SetExit("cave", graves);
        hub.SetExit("east", rocky);

        castleTown.SetExit("south", hub);
        castleTown.SetExit("north", castleGate);
        castleTown.SetExit("east", battleGr);
        castleTown.SetExit("northwest", swamp);

        swamp.SetExit("southeast", castleTown);
        swamp.SetExit("grove", altarGrove);

        battleGr.SetExit("west", castleTown);
        battleGr.SetExit("south", rocky);

        rocky.SetExit("north", battleGr);
        rocky.SetExit("west", hub);
        rocky.SetExit("south", lava);

        lava.SetExit("north", rocky);
        lava.SetExit("slideward", graves);

        graves.SetExit("exit", hub);

        castleGate.SetExit("south", castleTown);

        altarGrove.SetExit("swampward", swamp);

        // Initialize items in rooms (placing item instances into room inventories).
        battleGr.addItem(axe);
        castleGate.addItem(ring);
        graves.addItem(hammer);
        altarGrove.addItem(hilt);

        // givenItems[0] must be ore, givenItems[1] must be sword (used by UseRules logic).
        givenItems.Add(ore);
        givenItems.Add(sword);

        // Start locations.
        player.setCurrentRoom(hub);
        protag.setCurrentRoom(graves);

        oldHorseFollower.setCurrentRoom(castleGate);
        oldHorseFollower.AddIdleText("Your mule brays softly.");
    }

    /**
     *  Main play routine. Loops until end of play.
     */
    public void Play()
    {
        PrintWelcome();

        bool finished = false;
        while (!finished)
        {
            Command command = parser.GetCommand();
            finished = ProcessCommand(command);
        }

        Console.WriteLine("Play again, if you'd like.");
    }

    /**
     * Print out the opening message for the player.
     */
    private void PrintWelcome()
    {
        Console.WriteLine();
        Console.WriteLine("Welcome to Messed Up NPC with a Creepy Laugh!");
        Console.WriteLine("Help the dumb protagonist have the slimmest chance to survive.");
        Console.WriteLine("Type 'help' if you need help.");
        Console.WriteLine();
        Room? room = player.GetCurrentRoom();
if (room != null)
{
    PrintLocationInfo(room);
}
    }

    /**
     * Given a command, process (that is: execute) the command.
     * @return true If the command ends the game, false otherwise.
     */
    internal bool ProcessCommand(Command command)
    {
        // Extract the enum that represents the parsed command word.
        CommandWord commandWord = command.GetCommandWord();

        // REFACTOR: command dispatch through a registry (Command Pattern).
        // If no handler is found, fallback to the Unknown handler.
        if (!commandActions.TryGetValue(commandWord, out ICommandAction? action) || action is null)
        {
            // This assumes UnknownCommandAction exists in the Commands layer (not nested in Game anymore).
            action = new UnknownCommandAction();
            commandWord = CommandWord.UNKNOWN;
        }

        // Execute the selected command handler.
        bool wantToQuit = action.Execute(this, command);

        // Sprint 4 fix: Protagonist should not move on UNKNOWN commands.
        if (commandWord != CommandWord.UNKNOWN)
        {
            ProtagMove();
        }

        return wantToQuit;
    }

    // ---------------------------
    // Command helper methods
    // ---------------------------
    // REFACTOR: These are internal so the command handlers in the Commands folder can call them
    // without making them public API.

    internal void PrintHelp()
    {
        Console.WriteLine("Assist the protagonist in progressing through the beginning areas. \nThey will need to be able to obtain a weapon and have a way into the castle.");
        Console.WriteLine();
        Console.WriteLine("Your command words are:");
        parser.ShowCommands();
    }

    internal void PrintLocationInfo(Room currentRoom)
    {
        string roomText = RoomTextService.GetLongDescription(currentRoom, progress);

        if (oldHorseFollower.getCurrentRoom() == currentRoom)
        {
            roomText += "\n" + oldHorseFollower.GetRandomIdleText();
        }

        if (protag.getCurrentRoom() == currentRoom)
        {
            roomText += "\nThe protagonist is here, bumbling about the area.";
        }

        Console.WriteLine(roomText);
    }

    internal static bool Quit(Command command)
    {
        if (command.HasSecondWord())
        {
            Console.WriteLine("Quit what?");
            return false;
        }

        return true;
    }

    // ---------------------------
    // Inventory methods
    // ---------------------------

    internal void Take(Command command)
    {
        if (!command.HasSecondWord())
        {
            Console.WriteLine("Take what?");
            return;
        }

        string itemName = command.GetSecondWord()!;
        Item? tempItem = player.GetCurrentRoom()!.getItemByName(itemName);

        if (tempItem != null)
        {
            if (player.isValidItem(tempItem))
            {
                player.addItem(tempItem);
                player.GetCurrentRoom()!.removeItemByName(itemName);
                Console.WriteLine("Picked up the " + tempItem.GetName() + "!");

                // Legacy puzzle flag behavior kept as-is (forge tool set completeness).
                if (tempItem.GetName() == "hammer" && player.GetCurrentRoom()!.GetId() == 4)
                {
                    progress.ForgePrepared = false;
                    Console.WriteLine("The forge's tool set is once again incomplete.");
                }
            }
            else
            {
                Console.WriteLine("That's too heavy to carry right now.");
            }
        }
        else
        {
            Console.WriteLine("There isn't anything like that around.");
        }
    }

    private bool IsFollowerPresent()
    {
        return oldHorseFollower.getCurrentRoom() == player.GetCurrentRoom();
    }

    internal void Drop(Command command)
    {
        if (!command.HasSecondWord())
        {
            Console.WriteLine("Drop what?");
            return;
        }

        string itemName = command.GetSecondWord()!;
        Item? tempItem = player.getItemByName(itemName);

        if (tempItem != null)
        {
            player.GetCurrentRoom()!.addItem(tempItem);
            player.removeItemByName(itemName);
            Console.WriteLine("Dropped the " + tempItem.GetName() + "!");
        }
        else
        {
            Console.WriteLine("You don't have anything like that.");
        }
    }

    internal void Follow(Command _)
    {
        if (!IsFollowerPresent())
        {
            Console.WriteLine("Your trusty mule isn't around right now.");
            return;
        }

        if (oldHorseFollower.IsFollowing())
        {
            Console.WriteLine("Your mule is already following you!");
            return;
        }
        Console.WriteLine("You untether your mule.");
        oldHorseFollower.Follow();
    }

    internal void Stay(Command _)
    {
        if (!IsFollowerPresent())
        {
            Console.WriteLine("Your trusty mule isn't around right now.");
            return;
        }
        if (!oldHorseFollower.IsFollowing())
        {
            Console.WriteLine("Your mule has already been tethered here!");
            return;
        }
        Console.WriteLine("You tie your mule's reins to a nearby post or fixture.");
        oldHorseFollower.Stay();
    }

    internal void Trade(Command _)
    {
        if (!IsFollowerPresent())
        {
            Console.WriteLine("Your trusty mule isn't around right now.");
            return;
        }

        Console.WriteLine("What do you want to trade?");
        bool finished = false;
        while (!finished)
        {
            DisplayTradeInfo();
            string word = Parser.GetSingleCommand();
            finished = TryTrade(word);
        }
    }

    private bool TryTrade(string word)
    {
        if (word.Equals("quit"))
        {
            Console.WriteLine("Stopping trading.");
            return true;
        }

        if (oldHorseFollower.ReceiveFromPlayer(player, word))
        {
            Console.WriteLine("Gave the " + word + " to your mule.");
            return false;
        }

        if (oldHorseFollower.GiveToPlayer(player, word))
        {
            Console.WriteLine("Took the " + word + " from your mule.");
        }

        Console.WriteLine("Invalid trade!");
        return false;
    }

    private void DisplayTradeInfo()
    {
        ItemsPrint();
        Console.WriteLine("[Type QUIT to stop trading]");
    }


    internal void ItemsPrint()
    {
        Console.WriteLine(player.itemsText());
        Console.WriteLine("-------------------------------------------------");
        Console.WriteLine(oldHorseFollower.itemsText());
    }

    // ---------------------------
    // Movement methods
    // ---------------------------

    internal void GoTo(Command command)
    {
        switch (player.goRoom(command))
        {
            case 0:
                Console.WriteLine("Go where?");
                break;
            case -1:
                Console.WriteLine("There is no path!");
                break;
            case 1:
                OnPlayerMove(this, command);
                break;
            case 2:
                Console.WriteLine("Woohoo!");
                OnPlayerMove(this, command);
                break;
            default:
                Console.WriteLine("Something has gone terribly wrong.");
                break;
        }
    }

    internal void BackTo()
    {
        if (player.back() == 0)
        {
            Console.WriteLine("You haven't gone anywhere!");
        }
    }

    // ---------------------------
    // Item behavior entry point
    // ---------------------------

    internal bool Use(Command command)
    {
        if (!command.HasSecondWord())
        {
            Console.WriteLine("Use what?");
            return false;
        }

        string item = command.GetSecondWord()!;

        // REFACTOR (polymorphism):
        // Instead of Game checking item IDs and branching, the item decides its own behavior
        // via Item.Use(Game) implemented by each concrete subclass (AxeItem, RingItem, etc.).
        Item? heldItem = player.getItemByName(item);
        if (heldItem != null)
        {
            return heldItem.Use(this);
        }

        Console.WriteLine("You don't have an item like that.");
        return false;
    }

    // These are the actual behaviors items call into (AxeItem.Use -> game.axeUse(), etc.)
    // REFACTOR (abstraction + rule pattern):
    // Item-specific logic is now expressed as lists of IUseRule and executed by UseRuleExecutor.

    internal bool AxeUse() =>
        UseRuleExecutor.ExecuteFirstRuleOrDefault(this, UseRuleSets.AxeRules(), DefaultNothing);

    internal void RingUse() =>
        UseRuleExecutor.ExecuteFirstRuleOrDefault(this, UseRuleSets.RingRules(), DefaultNothing);

    internal void HammerUse() =>
        UseRuleExecutor.ExecuteFirstRuleOrDefault(this, UseRuleSets.HammerRules(), DefaultNothing);

    internal void HiltUse()
    {
        // Hilt is still void, but its behavior is now driven by the HiltRules set (HiltForgeSwordRule).
        UseRuleExecutor.ExecuteFirstRuleOrDefault(
            this,
            UseRuleSets.HiltRules(),
            () => Console.WriteLine("Can't do anything with that right now.")
        );
    }

    internal bool SwordUse() =>
        UseRuleExecutor.ExecuteFirstRuleOrDefault(this, UseRuleSets.SwordRules(), DefaultNothing);

    internal void Talk()
    {
        if (player.GetCurrentRoom() == protag.getCurrentRoom())
        {
            if (progress.SwordPlaced && !progress.ToldProtagSword)
            {
                progress.ToldProtagSword = true;
                Console.WriteLine("You inform the protagonist of the location of a weapon.");
            }
            else if (progress.GateOpen && !progress.ToldProtagGate)
            {
                progress.ToldProtagGate = true;
                Console.WriteLine("You inform the protagonist of a way forward.");
            }
            else
            {
                Console.WriteLine("Nothing to say to the protagonist right now.");
            }
        }
        else
        {
            Console.WriteLine("There's no-one to talk to!");
        }
    }

    internal bool Sleep()
    {
        bool quitSleep = false;

        if (player.GetCurrentRoom()!.GetId() == 0)
        {
            if (progress.ToldProtagGate && progress.ToldProtagSword)
            {
                Console.WriteLine("You lay your head down to sleep, your (likely fruitless) endeavors complete.");
                quitSleep = true;
            }
            else
            {
                Console.WriteLine("You've not finished all that you need to!");
            }
        }
        else
        {
            Console.WriteLine("This is a terrible place to sleep.");
        }

        return quitSleep;
    }

    internal static bool ProtagKill()
    {
        Console.WriteLine("In a single mighty blow, you strike down the oblivious protagonist.");
        Console.WriteLine("With this character's death the thread of prophecy... et cetera.");
        return true;
    }

    private void ProtagMove()
    {
        // Protagonist movement is automated by generating a "GO" command with a random exit.
        Command command = new(CommandWord.GO, protag.getCurrentRoom()!.GetRandomExit());
        protag.protagSteps(command);
    }

    protected virtual void OnPlayerMove(object sender, Command command)
    {
        PlayerMovement?.Invoke(sender, command);
    }

    // Internal getters used by rules and item behaviors (kept internal for tests + cross-namespace access).
    internal Player GetPlayer() { return player; }
    internal Protagonist GetProtag() { return protag; }
    internal Follower GetFollower() { return oldHorseFollower; }

    // Exposes the spawnable items list to UseRules (ore/sword).
    internal List<Item> GetGivenItems() { return givenItems; }
    internal GameProgress GetProgress() { return progress; }
}
