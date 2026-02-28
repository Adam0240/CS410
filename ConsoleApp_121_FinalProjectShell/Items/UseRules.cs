//UseRules.cs
//Summary: This file implements a Rule Pattern system that controls how items behave when used.

using System;
using System.Collections.Generic;
using ConsoleApp_121_FinalProjectShell.Core;

namespace ConsoleApp_121_FinalProjectShell.Items;

// ============================
// RULE INTERFACE
// ============================

/// INTERFACE (Abstraction):
//Defines a contract for "use rules".
//Any rule can be executed the same way (via Applies + Execute),
//which allows the executor to treat all rules polymorphically.
public interface IUseRule
{
    bool Applies(Game game);        // Determines if this rule is relevant for the current game state

    bool Execute(Game game);        // Runs the rule's behavior if applicable (return value matches the game's "quit" meaning)
}

// ============================
// EXECUTOR
// ============================

//Centralizes the logic of scanning rules, selecting the first applicable rule,
//and executing it. This avoids large if/else chains in Game.
public static class UseRuleExecutor
{
    public static bool ExecuteFirstRuleOrDefault(
        Game game,
        List<IUseRule> rules,
        Action defaultAction)
    {
        foreach (var rule in rules)
        {
            if (rule.Applies(game))
            {
                return rule.Execute(game);
            }
        }

        defaultAction();
        return false;
    }
}

// ============================
// COMMON RULES
// ============================

public class ProtagKillRule : IUseRule
{
    public bool Applies(Game game) =>
        game.getPlayer().getCurrentRoom() == game.getProtag().getCurrentRoom();

    public bool Execute(Game game) =>
        game.protagKill();
}

// ============================
// RULE SETS (DISPATCH HELPERS)
// ============================

public static class UseRuleSets
{
    public static List<IUseRule> AxeRules() => new()
    {
        new AxeSwampLogRule(),
        new AxeCastleGateRule(),
        new ProtagKillRule()
    };

    public static List<IUseRule> HammerRules() => new()
    {
        new HammerQuarrySpawnOreRule(),
        new HammerForgePrepareRule()
    };

    public static List<IUseRule> SwordRules() => new()
    {
        new SwordAltarRule(),
        new ProtagKillRule()
    };

    public static List<IUseRule> RingRules() => new()
    {
        new RingEquipRule()
    };

    public static List<IUseRule> HiltRules() => new()
    {
        new HiltForgeSwordRule()
    };
}

// ============================
// AXE RULES
// ============================

public class AxeSwampLogRule : IUseRule
{
    public bool Applies(Game game) =>
        game.getPlayer().getCurrentRoom().GetId() == 1 &&
        !Room.getClearCons()[0];

    public bool Execute(Game game)
    {
        Room.setClearCon(0, true);
        Console.WriteLine("You chop the large log into several more easily navigable pieces.");
        Console.WriteLine("Beyond where it stood is revealed the entrance to a hidden grove.");
        return false;
    }
}

public class AxeCastleGateRule : IUseRule
{
    public bool Applies(Game game) =>
        game.getPlayer().getCurrentRoom().GetId() == 6 &&
        !Room.getClearCons()[3];

    public bool Execute(Game game)
    {
        Room.setClearCon(3, true);
        Console.WriteLine("Utilizing the hefty weight of the axe, you smash a hole through the wooden gate.");
        return false;
    }
}

// ============================
// HAMMER RULES
// ============================

public class HammerQuarrySpawnOreRule : IUseRule
{
    public bool Applies(Game game) =>
        game.getPlayer().getCurrentRoom().GetId() == 3;

    public bool Execute(Game game)
    {
        if (game.getPlayer().hasItemByName("ore") ||
            game.getPlayer().getCurrentRoom().hasItemByName("ore"))
        {
            Console.WriteLine("Nothing to do with that here.");
            return false;
        }

        game.getPlayer().getCurrentRoom()
            .addItem(game.GetGivenItems()[0]);

        Console.WriteLine("A chunk of ore falls to the ground as you break it free from the surrounding rock.");
        return false;
    }
}

public class HammerForgePrepareRule : IUseRule
{
    public bool Applies(Game game) =>
        game.getPlayer().getCurrentRoom().GetId() == 4;

    public bool Execute(Game game)
    {
        Item? hammerItem = game.getPlayer().getItemByName("hammer");

        if (hammerItem != null)
        {
            game.getPlayer().getCurrentRoom().addItem(hammerItem);
            game.getPlayer().removeItemByName("hammer");
        }
        else if (!game.getPlayer().getCurrentRoom().hasItemByName("hammer"))
        {
            game.getPlayer().getCurrentRoom().addItem(
                ItemFactory.Create(
                    "hammer",
                    "a standard issue craft HAMMER with a flat head",
                    34,
                    2
                )
            );
        }

        Room.setClearCon(1, true);
        Console.WriteLine("You place the hammer with the set of forge tools, completing the set.");
        return false;
    }
}

// ============================
// RING RULES (MOVES ringUse)
// ============================

public class RingEquipRule : IUseRule
{
    // Ring can be used anywhere as long as you have it.
    public bool Applies(Game game) => true;

    public bool Execute(Game game)
    {
        game.getPlayer().setCarryWeight(150);
        game.getPlayer().removeItemByName("ring");
        Console.WriteLine("By equipping the ring, your maximum carryable weight has increased.");
        return false;
    }
}

// ============================
// HILT RULES (MOVES hiltUse)
// ============================

public class HiltForgeSwordRule : IUseRule
{
    public bool Applies(Game game) =>
        game.getPlayer().hasItemByName("ore") &&
        game.getPlayer().hasItemByName("hilt") &&
        game.getPlayer().getCurrentRoom().GetId() == 4 &&
        Room.getClearCons()[1];

    public bool Execute(Game game)
    {
        game.getPlayer().removeItemByName("ore");
        game.getPlayer().removeItemByName("hilt");

        game.getPlayer().getCurrentRoom().addItem(game.GetGivenItems()[1]);
        Console.WriteLine("Forged the hilt into a new sword!");
        return false;
    }
}

// ============================
// SWORD RULES
// ============================

public class SwordAltarRule : IUseRule
{
    public bool Applies(Game game) =>
        game.getPlayer().getCurrentRoom().GetId() == 8;

    public bool Execute(Game game)
    {
        game.getPlayer().removeItemByName("sword");
        Room.setClearCon(2, true);
        Console.WriteLine("You place the sword within the altar, now only to be obtained by a true hero.");
        return false;
    }
}
