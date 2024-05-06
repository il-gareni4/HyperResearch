using HyperResearch.Common.Configs;
using HyperResearch.Common.ModPlayers;
using HyperResearch.Common.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;

namespace HyperResearch.Utils;

public enum ResearchSource
{
    Default,
    Craft,
    Shimmer,
    ShimmerDecraft
}

/// <summary>Utility class that contains all the methods related to the research and sacrification of items</summary>
public class Researcher
{
    #region IgnoringCraftConditions
    public static readonly List<Condition> IgnoringCraftConditions =
    [
        // Liqiuds
        Condition.NearWater,
        Condition.NearLava,
        Condition.NearHoney,
        Condition.NearShimmer,
        // Time
        Condition.TimeDay,
        Condition.TimeNight,
        // Biomes
        Condition.InDungeon,
        Condition.InCorrupt,
        Condition.InHallow,
        Condition.InMeteor,
        Condition.InJungle,
        Condition.InSnow,
        Condition.InCrimson,
        Condition.InWaterCandle,
        Condition.InPeaceCandle,
        Condition.InTowerSolar,
        Condition.InTowerVortex,
        Condition.InTowerNebula,
        Condition.InTowerStardust,
        Condition.InDesert,
        Condition.InGlowshroom,
        Condition.InUndergroundDesert,
        Condition.InSkyHeight,
        Condition.InSpace,
        Condition.InOverworldHeight,
        Condition.InDirtLayerHeight,
        Condition.InRockLayerHeight,
        Condition.InUnderworldHeight,
        Condition.InUnderworld,
        Condition.InGemCave,
        Condition.InHive,
        Condition.InMarble,
        Condition.InGranite,
        Condition.InBeach,
        Condition.InAether,
        Condition.InGraveyard,
        Condition.InLihzhardTemple,
        Condition.InBelowSurface,
        Condition.InShoppingZoneForest,
        // Not in biomes
        Condition.NotInEvilBiome,
        Condition.NotInHallow,
        Condition.NotInGraveyard,
        Condition.NotInUnderworld,
        // Events
        Condition.Thunderstorm,
        Condition.HappyWindyDay,
        Condition.LanternNight,
        Condition.BirthdayParty,
        Condition.NightOrEclipse,
        Condition.InRain,
        Condition.InSandstorm,
        Condition.InOldOneArmy,
        Condition.InEvilBiome,
        Condition.Christmas,
        Condition.Halloween,
        Condition.BloodMoon,
        Condition.Eclipse,
        Condition.EclipseOrBloodMoon,
        Condition.BloodMoonOrHardmode,
        // Not events
        Condition.NotBloodMoon,
        Condition.NotEclipse,
        Condition.NotEclipseAndNotBloodMoon,
        // Moon phases
        Condition.MoonPhaseFull,
        Condition.MoonPhaseWaningGibbous,
        Condition.MoonPhaseThirdQuarter,
        Condition.MoonPhaseWaningCrescent,
        Condition.MoonPhaseNew,
        Condition.MoonPhaseWaxingCrescent,
        Condition.MoonPhaseFirstQuarter,
        Condition.MoonPhaseWaxingGibbous,
        Condition.MoonPhasesQuarter0,
        Condition.MoonPhasesQuarter1,
        Condition.MoonPhasesQuarter2,
        Condition.MoonPhasesQuarter3,
        Condition.MoonPhasesHalf0,
        Condition.MoonPhasesHalf1,
        Condition.MoonPhasesEven,
        Condition.MoonPhasesOdd,
        Condition.MoonPhasesNearNew,
        Condition.MoonPhasesEvenQuarters,
        Condition.MoonPhasesOddQuarters,
        Condition.MoonPhases04,
        Condition.MoonPhases15,
        Condition.MoonPhases26,
        Condition.MoonPhases37,
    ];
    #endregion

    public static readonly int ResearchedItemGroups = Enum.GetValues(typeof(ResearchSource)).Length;

    public List<List<int>> ResearchedItems;
    public List<int> DefaultResearchedItems => ResearchedItems[(int)ResearchSource.Default];
    public List<int> CraftResearchedItems => ResearchedItems[(int)ResearchSource.Craft];
    public List<int> ShimmerResearchedItems => ResearchedItems[(int)ResearchSource.Shimmer];
    public List<int> DecraftResearchedItems => ResearchedItems[(int)ResearchSource.ShimmerDecraft];

    private Dictionary<int, int> _sacrificedItems;
    public Dictionary<int, int> SacrificedItems { get => _sacrificedItems ??= []; }
    private Queue<int> _researchedQueue;
    public Queue<int> ResearchedQueue { get => _researchedQueue ??= new(); }

    public bool AutoResearchCraftableItems { get; init; } = HyperConfig.Instance.AutoResearchCraftableItems;
    public bool AutoResearchShimmerableItems { get; init; } = ConfigOptions.ResearchShimmerableItems;
    public bool AutoResearchDecraftItems { get; init; } = ConfigOptions.ResearchDecraftItems;
    public bool BalanceShimmerAutoresearch { get; init; } = ConfigOptions.BalanceShimmerAutoresearch;

    public Researcher()
    {
        ResearchedItems = new(ResearchedItemGroups);
        for (int i = 0; i < ResearchedItemGroups; i++)
            ResearchedItems.Add([]);
    }

    public int[] AllResearchedItems => ResearchedItems.SelectMany(list => list).Distinct().ToArray();

    public static bool IsPlayerInJourneyMode => Main.LocalPlayer.difficulty == 3;

    public static int GetSharedValue(int itemId)
    {
        if (ContentSamples.CreativeResearchItemPersistentIdOverride.TryGetValue(itemId, out int value))
            return value;
        else return -1;
    }

    public static int GetTotalNeeded(int itemId)
    {
        if (CreativeItemSacrificesCatalog.Instance.TryGetSacrificeCountCapToUnlockInfiniteItems(itemId, out int amountNeeded))
            return ConfigOptions.OnlyOneItemNeeded ? 1 : amountNeeded;
        return 0;
    }

    public static int GetResearchedCount(int itemId)
    {
        return Main.LocalPlayerCreativeTracker.ItemSacrifices.GetSacrificeCount(itemId);
    }

    public static int GetRemaining(int itemId) => GetTotalNeeded(itemId) - GetResearchedCount(itemId);

    public static bool IsResearched(int itemId)
    {
        if (CreativeItemSacrificesCatalog.Instance.TryGetSacrificeCountCapToUnlockInfiniteItems(itemId, out int amountNeeded))
        {
            int researched = Main.LocalPlayerCreativeTracker.ItemSacrifices.GetSacrificeCount(itemId);
            return amountNeeded <= researched;
        }
        else return false;
    }

    public static bool IsResearchable(int itemId)
    {
        if (ContentSamples.ItemsByType.TryGetValue(itemId, out Item item) && (item.IsAir || item.type != itemId))
            return false;
        return CreativeItemSacrificesCatalog.Instance.TryGetSacrificeCountCapToUnlockInfiniteItems(itemId, out _);
    }

    public bool AnyItemResearched => ResearchedItems.Any(list => list.Count > 0);

    public bool AnyItemSacrificed =>
        _sacrificedItems is not null
        && SacrificedItems.Count > 0;

    public void SacrificeItems(IDictionary<int, int> itemCount, ResearchSource source = default)
    {
        SacrificeItems(itemCount.Select(pair => new Item(pair.Key, pair.Value)), source);
    }

    public void SacrificeItems(IEnumerable<Item> items, ResearchSource source = default)
    {
        foreach (Item item in items)
            SacrificeItem(item, source);
    }

    public CreativeUI.ItemSacrificeResult SacrificeItem(Item item, ResearchSource source = default)
    {
        if (item.stack == 0) return 0;

        int itemId = item.type; // Save the ID becase sacrfice can turn item into air
        CreativeUI.ItemSacrificeResult result = CreativeUI.ItemSacrificeResult.CannotSacrifice;
        bool researched;
        if (ConfigOptions.OnlyOneItemNeeded)
        {
            researched = ResearchItem(itemId, source);
            if (researched)
            {
                result = CreativeUI.ItemSacrificeResult.SacrificedAndDone;
                if (item.stack == 1) item.TurnToAir();
            }
        }
        else
        {
            result = CreativeUI.SacrificeItem(item, out int amountSacrificed);
            researched = result == CreativeUI.ItemSacrificeResult.SacrificedAndDone;
            if (researched)
                AfterResearch(itemId, source);
            else if (result == CreativeUI.ItemSacrificeResult.SacrificedButNotDone)
                SacrificedItems[itemId] = SacrificedItems.GetValueOrDefault(itemId, 0) + amountSacrificed;
        }
        if (researched) SacrificedItems.Remove(itemId);
        return result;
    }

    public void ResearchItems(IEnumerable<int> items, ResearchSource source = default)
    {
        foreach (int itemId in items)
            ResearchItem(itemId, source);
    }

    public void ResearchItems(IDictionary<int, int> items, ResearchSource source = default)
    {
        foreach ((int itemId, int itemCount) in items)
            if (IsResearchable(itemId) && itemCount >= GetRemaining(itemId))
                ResearchItem(itemId, source);
    }

    public bool TryResearchItem(int itemId, int itemCount, ResearchSource source = default)
    {
        if (ConfigOptions.OnlyOneItemNeeded && itemCount >= 1)
            return ResearchItem(itemId, source);

        if (itemCount >= GetRemaining(itemId)) return ResearchItem(itemId, source);
        else return false;
    }

    /// <returns><c>false</c> if have already been researched or item is unresearchable</returns>
    public bool ResearchItem(int itemId, ResearchSource source = default, bool researchQueue = true)
    {
        CreativeUI.ItemSacrificeResult result = CreativeUI.ResearchItem(itemId);
        if (result == CreativeUI.ItemSacrificeResult.SacrificedAndDone) AfterResearch(itemId, source, researchQueue);
        return result == CreativeUI.ItemSacrificeResult.SacrificedAndDone;
    }

    public void ResearchQueue()
    {
        while (ResearchedQueue.TryDequeue(out int itemId))
            ResearchItemOccurrences(itemId);
    }

    public void ResearchCraftable()
    {
        foreach (Recipe recipe in Main.recipe)
        {
            if (IsRecipeResearchable(recipe))
                ResearchItem(recipe.createItem.type, ResearchSource.Craft);
        };
    }

    public bool TryResearchShimmeredItem(int itemId)
    {
        int shimmerItemId = ItemsUtils.GetShimmeredItemId(itemId);
        if (shimmerItemId <= 0 || !IsResearched(itemId)) return false;

        return ResearchItem(shimmerItemId, ResearchSource.Shimmer);
    }

    public void ResearchDecraftItems(int itemId)
    {
        List<int> decraftItems = ItemsUtils.GetDecraftItems(itemId);
        if (decraftItems is null) return;
        foreach (int decraftItemId in decraftItems) ResearchItem(decraftItemId, ResearchSource.ShimmerDecraft);
    }

    private void AfterResearch(int itemId, ResearchSource source, bool researchQueue = true)
    {
        if (BalanceShimmerAutoresearch && Main.LocalPlayer.GetModPlayer<HyperPlayer>().WasInAether || !BalanceShimmerAutoresearch)
        {
            if (AutoResearchShimmerableItems) TryResearchShimmeredItem(itemId);
            if (AutoResearchDecraftItems) ResearchDecraftItems(itemId);
        }
        if (AutoResearchCraftableItems)
        {
            ResearchedQueue.Enqueue(itemId);
            if (researchQueue) ResearchQueue();
        }

        ResearchedItems[(int)source].Add(itemId);
    }

    private void ResearchItemOccurrences(int itemId) =>
        ResearchItemOccurrences(ContentSamples.ItemsByType[itemId]);

    private void ResearchItemOccurrences(Item item)
    {
        if (RecipesSystem.ItemRecipesOccurrences.TryGetValue(item.type, out List<int> itemRecipeIds))
        {
            foreach (int recipeId in itemRecipeIds)
            {
                Recipe recipe = Main.recipe[recipeId];
                if (IsRecipeResearchable(recipe))
                    ResearchItem(recipe.createItem.type, ResearchSource.Craft, false);
            }
        }
        if (item.createTile >= TileID.Dirt)
        {
            foreach (int adjTile in ItemsUtils.GetAllAdjTiles(item.createTile))
            {
                if (RecipesSystem.TileRecipesOccurrences.TryGetValue(adjTile, out List<int> tileRecipeIds))
                {
                    foreach (int recipeId in tileRecipeIds)
                    {
                        Recipe recipe = Main.recipe[recipeId];
                        if (IsRecipeResearchable(recipe))
                            ResearchItem(recipe.createItem.type, ResearchSource.Craft, false);
                    }
                }
            }
        }
    }

    private static bool IsRecipeResearchable(Recipe recipe)
    {
        if (!IsResearchable(recipe.createItem.type) || IsResearched(recipe.createItem.type)) return false;

        Dictionary<int, IEnumerable<int>> iconicAndOthers = [];
        foreach (int recipeGroupId in recipe.acceptedGroups)
        {
            RecipeGroup recipeGroup = RecipeGroup.recipeGroups[recipeGroupId];
            iconicAndOthers[recipeGroup.IconicItemId] = recipeGroup.ValidItems;
        }

        bool allItemsResearched = recipe.requiredItem.All(item =>
        {
            if (iconicAndOthers.TryGetValue(item.type, out IEnumerable<int> validItems))
                return validItems.Any(IsResearched);
            else return IsResearched(item.type);
        });
        if (!allItemsResearched) return false;

        HyperPlayer hyperPlayer = Main.LocalPlayer.GetModPlayer<HyperPlayer>();
        bool allTilesResearched = recipe.requiredTile.All(
            tileId => hyperPlayer.ResearchedTiles.GetValueOrDefault(tileId, false) || Main.LocalPlayer.adjTile[tileId]);
        if (!allTilesResearched) return false;

        bool allConditionsAreMet = recipe.Conditions.All(condition =>
            ConfigOptions.IgnoreCraftingConditions && IgnoringCraftConditions.Contains(condition) || condition.IsMet()
        );
        if (!allConditionsAreMet) return false;
        return true;
    }
}
