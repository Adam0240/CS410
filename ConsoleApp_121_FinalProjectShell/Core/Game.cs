using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ConsoleApp_121_FinalProjectShell.Commands;
using ConsoleApp_121_FinalProjectShell.Core.Persistence;
using ConsoleApp_121_FinalProjectShell.Items;
using ConsoleApp_121_FinalProjectShell.People;
using System.Threading;


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
    //Save State Edit 14
    private readonly IGameSaveRepository? saveRepository;

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
    //Save State Edit 15
    public Game(bool isTestInstance, IGameSaveRepository? saveRepository = null)
    {
        // Stores "spawnable" items used by puzzle logic (ore, sword).
        givenItems = [];

        // REFACTOR: deterministic randomness for repeatable unit tests.
        random = isTestInstance ? new Random(0) : new Random();

        // Instantiate core game objects.
        parser = new Parser();
        // Multiplayer Change 1:
        // The original player is now explicitly registered as Player 1 for shared-world multiplayer state.
        player = new Player(1, "Player 1");
        protag = new Protagonist();
        oldHorseFollower = new Follower(this);
        progress = new GameProgress();
        //Save State Edit 16
        this.saveRepository = saveRepository;
        this.saveRepository?.Initialize();

        this.isTestInstance = isTestInstance;

        // REFACTOR: Command actions are now defined in the Commands folder and registered here.
        // This keeps Game from ballooning with command-handler class definitions.
        commandActions = CommandActionRegistry.CreateDefault();

        // Test-only collections so unit tests can verify room/item creation and placement.

        allRooms = [];
        allItems = [];
        // Multiplayer Change 2:
        // Initialize the multiplayer player registry alongside the existing single-player setup.
        InitializePlayerRegistry();

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

        allRooms = [hub, swamp, battleGr, rocky, lava, graves, castleGate, castleTown, altarGrove];


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

    private void TypeCentered(string text, int delay = 25)
    {
        int windowWidth = Console.WindowWidth;
        int padding = (windowWidth - text.Length) / 2;

        if (padding < 0) padding = 0;

        Console.Write(new string(' ', padding));

        foreach (char c in text)
        {
            Console.Write(c);
            Thread.Sleep(delay);
        }
        Console.WriteLine();
    }

    // Multiplayer Change 20:
    // The splash screen is now callable from Program so it can display before the mode-selection menu.
    public void ShowSplashScreen()
    {
        int lastWidth = Console.WindowWidth;
        int lastHeight = Console.WindowHeight;

        // First draw: animated
        DrawSplashScreen(animated: true);

        while (true)
        {
            // If the user resized the console, redraw everything instantly
            if (Console.WindowWidth != lastWidth || Console.WindowHeight != lastHeight)
            {
                lastWidth = Console.WindowWidth;
                lastHeight = Console.WindowHeight;

                DrawSplashScreen(animated: false);
            }

            // Exit splash when a key is pressed
            if (Console.KeyAvailable)
            {
                Console.ReadKey(true);
                break;
            }

            Thread.Sleep(50);
        }

        Console.Clear();
    }

    private void DrawSplashScreen(bool animated)
    {
        Console.Clear();

        // If the window is too small, show a simple message instead of broken art
        if (Console.WindowWidth < 60 || Console.WindowHeight < 25)
        {
            WriteCentered("Window too small for splash screen");
            WriteCentered("Please resize the console larger");
            WriteCentered("Press any key to begin...");
            return;
        }

        Console.ForegroundColor = ConsoleColor.Cyan;

        ShowCastleArt();

        if (animated)
        {
            TypeCentered("=====================================", 5);
            Thread.Sleep(500);
            TypeCentered("ACT: A Clueless Traveler", 50);
            Thread.Sleep(500);
            TypeCentered("=====================================", 5);
            Thread.Sleep(500);
        }
        else
        {
            WriteCentered("=====================================");
            WriteCentered("ACT: A Clueless Traveler");
            WriteCentered("=====================================");
        }

        Console.ResetColor();

        Console.WriteLine();

        if (animated)
        {
            TypeCentered("A cursed land. A clueless hero.", 40);
            Thread.Sleep(500);
            TypeCentered("Your choices decide his fate.", 40);
            Thread.Sleep(500);
            Console.WriteLine();
            TypeCentered("Press any key to begin...", 25);
        }
        else
        {
            WriteCentered("A cursed land. A clueless hero.");
            WriteCentered("Your choices decide his fate.");
            Console.WriteLine();
            WriteCentered("Press any key to begin...");
        }
    }

    private void WriteCentered(string text)
    {
        int windowWidth = Console.WindowWidth;
        int textLength = text.Length;

        int padding = (windowWidth - textLength) / 2;

        if (padding < 0) padding = 0;

        Console.WriteLine(new string(' ', padding) + text);
    }

    private void ShowCastleArt()
    {
        string[] art =
        {
        " |>>>",
        "|",
        "_  _|_  _",
        "|;|_|;|_|;|",
        "\\\\.    .  /",
        "\\\\:  .  /",
        "||:   |",
        "||:.  |",
        "||:  .|",
        "||:   |",
        "||: , |",
        "||:   |",
        "||: . |",
        "__||_   |__",
        "(_____) (____)"
    };

        foreach (var line in art)
        {
            WriteCentered(line);
        }
    }


    /**
     * Print out the opening message for the player.
     */
    private void PrintWelcome()
    {
        Console.WriteLine();
        Console.WriteLine("Welcome to A Clueless Traveler!");
        Console.WriteLine("Help the dumb protagonist have the slimmest chance to survive.");
        Console.WriteLine("Type 'help' if you need help.");
        Console.WriteLine();
        // Multiplayer Change 3:
        // Resolve the welcome room through the local multiplayer player accessor instead of assuming one player.
        Room? room = GetPlayer(localPlayerId).GetCurrentRoom();
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
        //Save State Edit 17
        if (commandWord != CommandWord.UNKNOWN &&
            commandWord != CommandWord.SAVE &&
            commandWord != CommandWord.LOAD &&
            //Save State Edit 31
            commandWord != CommandWord.DELETE)
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

    internal void PrintHelp(Command command)
    {
        if (!command.HasSecondWord())
        {
            Console.WriteLine("Assist the protagonist in surviving the beginning areas.");
            Console.WriteLine("Main goals:");
            Console.WriteLine("- Secure a weapon for the protagonist (sword path).");
            Console.WriteLine("- Open a path through the castle gate.");
            Console.WriteLine("Tip: use 'talk' near the protagonist for dynamic step-by-step objective hints.");
            Console.WriteLine();
            Console.WriteLine("Your command words are:");
            parser.ShowCommands();
            return;
        }

        CommandWords commandWords = new CommandWords();
        CommandWord helpWord = commandWords.GetCommandWord(command.GetSecondWord()!);

        switch (helpWord)
        {
            case CommandWord.GO:
                Console.WriteLine("The 'go' command moves you between rooms." +
                                  "\nSynonyms: walk, move" +
                                  "\nUsage: 'go north'");
                break;
            case CommandWord.HELP:
                Console.WriteLine("You're already using it, silly!");
                break;
            case CommandWord.QUIT:
                Console.WriteLine("The 'quit' command exits the game. Progress is not auto-saved.");
                break;
            case CommandWord.BACK:
                Console.WriteLine("The 'back' command returns you to the most recently visited room.");
                break;
            case CommandWord.LOOK:
                Console.WriteLine("The 'look' command displays the current room description.");
                break;
            case CommandWord.TAKE:
                Console.WriteLine("The 'take' command picks up an item in the room." +
                                  "\nUsage: 'take hammer'");
                break;
            case CommandWord.DROP:
                Console.WriteLine("The 'drop' command leaves an inventory item in the room." +
                                  "\nUsage: 'drop hammer'");
                break;
            case CommandWord.ITEMS:
                Console.WriteLine("The 'items' command lists your inventory and carry weight.");
                break;
            case CommandWord.USE:
                Console.WriteLine("The 'use' command uses an item from your inventory." +
                                  "\nUsage: 'use axe'");
                break;
            case CommandWord.TALK:
                Console.WriteLine("The 'talk' command speaks to the protagonist (must be in the same room)." +
                                  "\nIt gives dynamic objective guidance and acknowledges completed tasks.");
                break;
            case CommandWord.SLEEP:
                Console.WriteLine("The 'sleep' command ends the run only when both objectives have been delivered to the protagonist." +
                                  "\nYou must be at camp to sleep.");
                break;
            case CommandWord.FOLLOW:
                Console.WriteLine("The 'follow' command unties your mule so it follows you.");
                break;
            case CommandWord.STAY:
                Console.WriteLine("The 'stay' command tethers your mule to keep it in place.");
                break;
            case CommandWord.TRADE:
                Console.WriteLine("The 'trade' command exchanges items with your mule.");
                break;
            case CommandWord.SAVE:
                Console.WriteLine("The 'save' command writes current progress to disk.");
                break;
            case CommandWord.LOAD:
                Console.WriteLine("The 'load' command restores progress from disk.");
                break;
            case CommandWord.DELETE:
                Console.WriteLine("The 'delete' command erases existing save data.");
                break;
            default:
                Console.WriteLine("Command not recognized.");
                break;
        }
    }

    internal void PrintLocationInfo(Room currentRoom)
    {
        // Multiplayer Change 4:
        // Room text now flows through the multiplayer-aware formatter so it can mention nearby players.
        Console.WriteLine(GetLocationInfoText(activePlayerId, currentRoom));
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
        // Multiplayer Change 5:
        // Item pickup now applies to whichever player issued the command on the host.
        Player activePlayer = GetPlayer();
        Item? tempItem = activePlayer.GetCurrentRoom()!.getItemByName(itemName);

        if (tempItem != null)
        {
            if (activePlayer.isValidItem(tempItem))
            {
                activePlayer.addItem(tempItem);
                activePlayer.GetCurrentRoom()!.removeItemByName(itemName);
                Console.WriteLine("Picked up the " + tempItem.GetName() + "!");

                // Legacy puzzle flag behavior kept as-is (forge tool set completeness).
                if (tempItem.GetName() == "hammer" && activePlayer.GetCurrentRoom()!.GetId() == 4)
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
        // Multiplayer Change 6:
        // Follower proximity checks now use the currently active player context.
        return oldHorseFollower.getCurrentRoom() == GetPlayer().GetCurrentRoom();
    }

    internal void Drop(Command command)
    {
        if (!command.HasSecondWord())
        {
            Console.WriteLine("Drop what?");
            return;
        }

        string itemName = command.GetSecondWord()!;
        // Multiplayer Change 7:
        // Item drops are resolved against the active multiplayer player instead of always Player 1.
        Player activePlayer = GetPlayer();
        Item? tempItem = activePlayer.getItemByName(itemName);

        if (tempItem != null)
        {
            activePlayer.GetCurrentRoom()!.addItem(tempItem);
            activePlayer.removeItemByName(itemName);
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

        // Multiplayer Change 8:
        // Trade inventory transfers now run against whichever player is executing the command.
        Player activePlayer = GetPlayer();

        if (oldHorseFollower.ReceiveFromPlayer(activePlayer, word))
        {
            Console.WriteLine("Gave the " + word + " to your mule.");
            return false;
        }

        if (oldHorseFollower.GiveToPlayer(activePlayer, word))
        {
            Console.WriteLine("Took the " + word + " from your mule.");
            return false; 
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
        // Multiplayer Change 9:
        // Inventory output now reports the active player's inventory in multiplayer sessions.
        Console.WriteLine(GetPlayer().itemsText());
        Console.WriteLine("-------------------------------------------------");
        Console.WriteLine(oldHorseFollower.itemsText());
    }

    // ---------------------------
    // Movement methods
    // ---------------------------

    internal void GoTo(Command command)
    {
        // Multiplayer Change 10:
        // Movement is applied to the active player so both players can move independently on the host.
        switch (GetPlayer().goRoom(command))
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
        // Multiplayer Change 11:
        // Backtracking now uses the active player's own room history.
        if (GetPlayer().back() == 0)
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
        // Multiplayer Change 12:
        // Item use now resolves against the active player's inventory during authoritative command execution.
        Item? heldItem = GetPlayer().getItemByName(item);
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
        if (GetPlayer().GetCurrentRoom() != protag.getCurrentRoom())
        {
            Console.WriteLine("There's no-one to talk to!");
            return;
        }

        bool gateDone = progress.GateOpen;
        bool swordDone = progress.SwordPlaced;
        bool toldGate = progress.ToldProtagGate;
        bool toldSword = progress.ToldProtagSword;

        // Acknowledge newly completed objective(s) once.
        if (swordDone && !toldSword)
        {
            progress.ToldProtagSword = true;
            Console.WriteLine("Thank you for preparing a weapon for me.");
        }

        if (gateDone && !toldGate)
        {
            progress.ToldProtagGate = true;
            Console.WriteLine("Thank you for opening a path through the gate.");
        }

        // If both are done and both acknowledged, finish cleanly.
        if (progress.ToldProtagGate && progress.ToldProtagSword)
        {
            Console.WriteLine("I have what I need now. Thank you.");
            return;
        }

        // Build dynamic objective + sub-step guidance.
        List<string> reminders = BuildProtagonistReminders();

        if (reminders.Count == 0)
        {
            Console.WriteLine("I think we're ready. Let's move on soon.");
            return;
        }

        Console.WriteLine("I still need your help with the following:");
        foreach (string reminder in reminders)
        {
            Console.WriteLine($"- {reminder}");
        }
    }

    private List<string> BuildProtagonistReminders()
    {
        var reminders = new List<string>();
        Player activePlayer = GetPlayer();

        bool needsSwordPath = !progress.SwordPlaced;
        bool needsGatePath = !progress.GateOpen;
        bool hasAxe = activePlayer.hasItemByName("axe");

        // Axe is only required if one of the axe-driven world-state steps is still incomplete.
        bool stillNeedsAxeActions = !progress.SwampCleared || !progress.GateOpen;

        if (stillNeedsAxeActions && !hasAxe)
        {
            reminders.Add("Find the axe at the old battlefield, then take it with you.");
        }

        if (needsSwordPath)
        {
            reminders.Add(BuildSwordObjectiveHint());
        }

        if (needsGatePath)
        {
            reminders.Add(BuildGateObjectiveHint());
        }

        return reminders;
    }

    private string BuildGateObjectiveHint()
    {
        Player activePlayer = GetPlayer();

        if (!activePlayer.hasItemByName("axe"))
        {
            return "You need the axe before you can break through the gate.";
        }

        return "Use the axe at the castle gate to break a path through.";
    }


    private string BuildSwordObjectiveHint()
    {
        Player activePlayer = GetPlayer();

        bool hasAxe = activePlayer.hasItemByName("axe");
        bool hasHammer = activePlayer.hasItemByName("hammer");
        bool hasOre = activePlayer.hasItemByName("ore");
        bool hasHilt = activePlayer.hasItemByName("hilt");
        bool hasSword = activePlayer.hasItemByName("sword");

        // If already forged, next step is always altar placement.
        if (hasSword)
        {
            return "Use the sword at the grove altar.";
        }

        // Axe is only relevant for sword path before swamp is cleared.
        if (!progress.SwampCleared)
        {
            if (!hasAxe)
            {
                return "You need the axe before you can clear the swamp path to the grove.";
            }

            return "Use the axe on the swamp log to open the grove path.";
        }

        if (!hasHilt)
        {
            return "Find the sword hilt in the hidden grove.";
        }

        if (!hasHammer)
        {
            return "Retrieve the hammer from the graveyard.";
        }

        if (!hasOre)
        {
            return "Use the hammer at the quarry to obtain ore.";
        }

        if (!progress.ForgePrepared)
        {
            return "Bring the hammer to the forge and use it to prepare the tools.";
        }

        return "At the forge, use the hilt while carrying ore to forge a sword.";
    }

    internal bool Sleep()
    {
        bool quitSleep = false;

        // Multiplayer Change 14:
        // Sleep/end-condition checks now evaluate the active player's location.
        if (GetPlayer().GetCurrentRoom()!.GetId() == 0)
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

    //Save State Edit 18
    internal void SaveGame(Command command)
    {
        // Multiplayer Change 15:
        // Saving is disabled while multiplayer is active to avoid host/client state divergence.
        if (IsSaveLoadBlocked())
        {
            return;
        }

        if (command.HasSecondWord())
        {
            Console.WriteLine("Save doesn't take any extra words.");
            return;
        }

        if (saveRepository is null)
        {
            Console.WriteLine("Saving is unavailable right now.");
            return;
        }

        //Save State Edit 23
        try
        {
            GameSaveState state = GameStateMapper.Capture(this);
            string saveJson = GameSaveSerializer.ToJson(state);
            saveRepository.SaveJson(saveJson);
            Console.WriteLine("Game saved.");
        }
        catch (Exception)
        {
            Console.WriteLine("Saving failed.");
        }
    }

    //Save State Edit 19
    internal void LoadGame(Command command)
    {
        // Multiplayer Change 16:
        // Loading is disabled while multiplayer is active to avoid host/client state divergence.
        if (IsSaveLoadBlocked())
        {
            return;
        }

        if (command.HasSecondWord())
        {
            Console.WriteLine("Load doesn't take any extra words.");
            return;
        }

        if (saveRepository is null)
        {
            Console.WriteLine("Loading is unavailable right now.");
            return;
        }

        //Save State Edit 24
        try
        {
            string? saveJson = saveRepository.LoadJson();
            if (string.IsNullOrWhiteSpace(saveJson))
            {
                Console.WriteLine("No save data found.");
                return;
            }

            GameSaveState? state = GameSaveSerializer.FromJson(saveJson);
            if (state is null)
            {
                Console.WriteLine("Save data is invalid.");
                return;
            }

            GameStateMapper.Apply(this, state);
            Console.WriteLine("Game loaded.");
            // Multiplayer Change 17:
            // After load, refresh the local player's room text through the multiplayer-aware accessor.
            PrintLocationInfo(GetPlayer(localPlayerId).GetCurrentRoom());
        }
        catch (Exception)
        {
            Console.WriteLine("Loading failed.");
        }
    }

    //Save State Edit 32
    internal void DeleteSave(Command command)
    {
        // Multiplayer Change 18:
        // Save deletion is also disabled in multiplayer to keep persistence behavior consistent.
        if (IsSaveLoadBlocked())
        {
            return;
        }

        if (command.HasSecondWord())
        {
            Console.WriteLine("Delete doesn't take any extra words.");
            return;
        }

        if (saveRepository is null)
        {
            Console.WriteLine("Deleting saves is unavailable right now.");
            return;
        }

        //Save State Edit 33
        try
        {
            bool deleted = saveRepository.DeleteSave();
            Console.WriteLine(deleted ? "Save deleted." : "No save data found.");
        }
        catch (Exception)
        {
            Console.WriteLine("Deleting save failed.");
        }
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
    // Multiplayer Change 19:
    // Added player-aware getters so shared-world commands can target Player 1 or Player 2 explicitly.
    internal Player GetPlayer() { return GetPlayer(activePlayerId); }
    internal Player GetPlayer(int playerId)
    {
        if (players.TryGetValue(playerId, out Player? existingPlayer))
        {
            return existingPlayer;
        }

        if (playerId == 1)
        {
            return player;
        }

        throw new KeyNotFoundException($"Player {playerId} is not registered.");
    }
    internal Protagonist GetProtag() { return protag; }
    internal Follower GetFollower() { return oldHorseFollower; }

    // Exposes the spawnable items list to UseRules (ore/sword).
    internal List<Item> GetGivenItems() { return givenItems; }
    internal GameProgress GetProgress() { return progress; }
}
