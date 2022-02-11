﻿using Hybrasyl.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Hybrasyl;

public class LootRecursionError : Exception
{
    public LootRecursionError()
    {
    }

    public LootRecursionError(string message)
        : base(message)
    {
    }

    public LootRecursionError(string message, Exception inner)
        : base(message, inner)
    {
    }
}

public class Loot
{
    public uint Xp;
    public uint Gold;
    public List<string> Items;

    public Loot(uint xp, uint gold, List<string> items = null)
    {
        Xp = xp;
        Gold = gold;
        if (items == null)
            Items = new List<string>();
        else
            Items = items;
    }

    public static Loot operator +(Loot a) => a;

    public static Loot operator +(Loot a, Xml.Item b)
    {
        var ret = new Loot(a.Xp, a.Gold);
        ret.Items.AddRange(a.Items);
        ret.Items.Add(b.Name);
        return ret;
    }

    public static Loot operator +(Loot a, Loot b) => new Loot(a.Xp + b.Xp, a.Gold + b.Gold, a.Items.Concat(b.Items).ToList());
}

/// <summary>
/// Resolve loot tables, sets, droprates, etc etc into, you know. Loot.
/// </summary>
public static class LootBox
{

    private static readonly Random _global = new Random();
    [ThreadStatic] private static Random _local;

    /// <summary>
    /// Generate a random number in a threadsafe manner.
    /// </summary>
    /// <returns>random double</returns>
    public static double Roll()
    {
        if (_local == null)
        {
            lock (_global)
            {
                if (_local == null)
                {
                    int seed = _global.Next();
                    _local = new Random(seed);
                }
            }
        }

        return _local.NextDouble();
    }

    /// <summary>
    /// Given the specified number of rolls and the chance, calculate how many wins (if any) occurred for  
    /// looting purposes.
    /// </summary>
    /// <param name="numRolls">Number of rolls (chances) to win</param>
    /// <param name="chance">The chance (decimalized percentage) of a given roll winning</param>
    /// <returns>The number of wins (successful rolls)</returns>
    public static int CalculateSuccessfulRolls(int numRolls, double chance)
    {
        var wins = 0;
        for (var x = 0; x <= numRolls; x++)
            if (Roll() <= chance)
                wins++;

        return wins;
    }

    /// <summary>
    /// Generate a random number between two unsigned ints in a threadsafe manner.
    /// </summary>
    /// <param name="a">Lower bound</param>
    /// <param name="b">Upper bound</param>
    /// <returns></returns>
    public static uint RollBetween(uint a, uint b)
    {
        if (_local == null)
        {
            lock (_global)
            {
                if (_local == null)
                {
                    int seed = _global.Next();
                    _local = new Random(seed);
                }
            }
        }

        return (uint)_local.Next((int)a, (int)b + 1);
    }


    /// <summary>
    /// Calculate loot for a given spawn.
    /// </summary>
    /// <param name="spawn">The spawn we will use to calculate Loot.</param>
    /// <returns>A Loot struct with XP, gold and items, if any</returns>
    public static Loot CalculateLoot(Xml.Spawn spawn)
    {

        // Loot calculations are not particularly complex but have a lot of components:
        // Spawns can have loot sets, loot tables, or both.
        // We resolve sets first, then tables. We keep a running tab of gold drops.
        // Lastly, we return a Loot struct with our calculations.

        var loot = new Loot(0, 0);
        var tables = new List<Xml.LootTable>();
        if (spawn.Loot is null) 
            // Can't do anything if nothing is defined
            return loot;
        // Assign base XP
        loot.Xp = spawn.Loot.Xp;
        // Sets
        foreach (var set in spawn.Loot?.Set ?? new List<Xml.LootImport>())
        {
            // Is the set present?
            GameLog.SpawnInfo("Processing loot set {Name}", set.Name);
            if (Game.World.WorldData.TryGetValueByIndex(set.Name, out Xml.LootSet lootset))
            {
                // Set is present, does it fire?
                // Chance is implemented as a decimalized percentage, e.g. 0.08 = 8% chance

                // If rolls == 0, all tables fire
                if (set.Rolls == 0)
                {
                    tables.AddRange(lootset.Table);
                    GameLog.SpawnInfo("Processing loot set {Name}: set rolls == 0, looting", set.Name);
                    continue;
                }

                for (var x = 0; x < set.Rolls; x++)
                {
                    if (Roll() <= set.Chance)
                    {
                        GameLog.SpawnInfo("Processing loot set {Name}: set hit, looting", set.Name);

                        // Ok, the set fired. Check the subtables, which can have independent chances.
                        // If no chance is present, we simply award something from each table in the set.
                        // Note that we do table processing last! We just find the tables that fire here.
                        foreach (var setTable in lootset.Table)
                        {
                            // Rolls == 0 (default) means the table is automatically used.
                            if (setTable.Rolls == 0)
                            {
                                tables.Add(setTable);
                                GameLog.SpawnInfo("Processing loot set {Name}: setTable rolls == 0, looting", set.Name);
                                continue;
                            }
                            // Did the subtable hit?
                            for (var y = 0; y < setTable.Rolls; y++)
                            {
                                if (Roll() <= setTable.Chance)
                                {
                                    tables.Add(setTable);
                                    GameLog.SpawnInfo("Processing loot set {Name}: set subtable hit, looting ", set.Name);
                                }
                            }
                        }
                    }
                    else
                        GameLog.SpawnInfo("Processing loot set {Name}: Set subtable missed", set.Name);
                }
            }
            else
                GameLog.Warning("Spawn {name}: Loot set {name} missing", spawn.Base, set.Name);
        }

        // Now, calculate loot for any tables attached to the spawn
        foreach (var table in spawn.Loot?.Table ?? new List<Xml.LootTable>())
        {
            if (table.Rolls == 0)
            {
                tables.Add(table);
                GameLog.SpawnInfo("Processing loot: spawn table for {Name}, rolls == 0, looting", spawn.Base);
                continue;
            }
            for (var z = 0; z <= table.Rolls; z++)
            {
                if (Roll() <= table.Chance)
                {
                    GameLog.SpawnInfo("Processing loot: spawn table for {Name} hit, looting", spawn.Base);
                    tables.Add(table);
                }
                else
                    GameLog.SpawnInfo("Processing loot set {Name}: Spawn subtable missed", spawn.Base);

            }
        }

        // Now that we have all tables that fired, we need to calculate actual loot

        GameLog.SpawnInfo("Loot for {Name}: tables: {Count}", spawn.Base, tables.Count());
        foreach (var table in tables)
            loot += CalculateTable(table);

        GameLog.SpawnInfo("Final loot for {Name}: {Xp} xp, {Gold} gold, items [{items}]", spawn.Base, loot.Xp, loot.Gold, string.Join(",", loot.Items));
        return loot;
    }

    /// <summary>
    /// Calculate drops from a specific loot table.
    /// </summary>
    /// <param name="table">The table to be evaluated.</param>
    /// <returns>Loot structure containing Xp/Gold/List of items to be awarded.</returns>
    public static Loot CalculateTable(Xml.LootTable table)
    {
        var tableLoot = new Loot(0, 0);
        if (table.Gold != null)
        {
            if (table.Gold.Max != 0)
                tableLoot.Gold += RollBetween(table.Gold.Min, table.Gold.Max);
            else
                tableLoot.Gold += table.Gold.Min;
            GameLog.SpawnInfo("Processing loot: added {Gold} gp", tableLoot.Gold);
        }
        if (table.Xp != null)
        {
            if (table.Xp.Max != 0)
                tableLoot.Xp += RollBetween(table.Xp.Min, table.Xp.Max);
            else
                tableLoot.Xp += table.Xp.Min;
            GameLog.SpawnInfo("Processing loot: added {Xp} xp", tableLoot.Xp);
        }
        // Handle items now
        if (table.Items != null)
        {
            foreach (var itemlist in table.Items)
                tableLoot.Items.AddRange(CalculateItems(itemlist));
        }
        else
            GameLog.SpawnWarning("Loot table is null!");
        return tableLoot;
    }

    /// <summary>
    /// Given a list of items in a loot table, return the items awarded.
    /// </summary>
    /// <param name="list">LootTableItemList containing items</param>
    /// <returns>List of items</returns>
    public static List<string> CalculateItems(Xml.LootTableItemList list)
    {
        // Ordinarily, return one item from the list.
        var rolls = CalculateSuccessfulRolls(list.Rolls, list.Chance);
        var loot = new List<Xml.LootItem>();
        var itemList = new List<ItemObject>();

        // First, process any "always" items, which always drop when the container fires
        foreach (var item in list.Item.Where(i => i.Always))
        {
            GameLog.SpawnInfo("Processing loot: added always item {item}", item.Value);
            loot.Add(item);
        }
        var totalRolls = 0;
        // Process the rest of the rolls now
        do
        {
            // Get a random item from the list
            var item = list.Item.Where(i => !i.Always).PickRandom();
            // As soon as we get an item from our table, we've "rolled"; we'll add another roll below if needed
            rolls--;

            // Check uniqueness. If something has already dropped, don't drop it again, and reroll
            if (item.Unique && loot.Contains(item))
            {
                rolls++;
                GameLog.SpawnInfo("Processing loot: added duplicate unique item {item}. Rerolling", item.Value);
                continue;
            }

            // Check max quantity. If it is exceeded, reroll
            if (item.Max > 0 && loot.Where(i => i.Value == item.Value).Count() >= item.Max)
            {
                rolls++;
                GameLog.SpawnInfo("Processing loot: added over max quantity for {item}. Rerolling", item.Value);
                continue;
            }

            // If quantity and uniqueness are good, add the item
            loot.Add(item);
            GameLog.SpawnInfo("Processing loot: added {item}", item.Value);
            totalRolls++;
            // As a check against something incredibly stupid in XML, we only allow a maximum of
            // 100 rolls
            if (totalRolls > 100)
            {
                GameLog.SpawnInfo("Processing loot: maximum number of rolls exceeded..?");
                throw new LootRecursionError("Maximum number of rolls (100) exceeded!");
            }
        }
        while (rolls > 0);

        // Now we have the canonical droplist, which needs resolving into Items

        foreach (var lootitem in loot)
        {
            // Does the base item exist?
            var xmlItemList = Game.World.WorldData.FindItem(lootitem.Value);
            // Don't handle the edge case of multiple genders .... yet
            if (xmlItemList.Count != 0)
            {
                var xmlItem = xmlItemList.First();
                // Handle variants.
                // If multiple variants are specified, we pick one at random
                if (lootitem.Variants.Count() > 0)
                {
                    var lootedVariant = lootitem.Variants.PickRandom();
                    if (xmlItem.Variants.TryGetValue(lootedVariant, out List<Xml.Item> variantItems))
                        itemList.Add(Game.World.CreateItem(variantItems.PickRandom().Id));
                    else
                        GameLog.SpawnError("Spawn loot calculation: variant group {name} not found", lootedVariant);
                }
                else
                    itemList.Add(Game.World.CreateItem(xmlItem.Id));
            }
            else
                GameLog.SpawnError("Spawn loot calculation: item {name} not found!", lootitem.Value);

        }
        // We store loot as strings inside mobs to avoid having tens or hundreds of thousands of ItemObjects or
        // Items lying around - they're made into real objects at the time of mob death
        if (itemList.Count > 0)
            return itemList.Where(x => x != null).Select(y => y.Name).ToList();
        return new List<String>();
    }
}