using System;
using System.Collections.Generic;
using System.Linq;
using HyperResearch.Common.Configs;
using HyperResearch.Common.ModPlayers;
using HyperResearch.Common.Systems;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;

namespace HyperResearch.Utils;

public enum ResearchSource
{
    Default,
    Craft,
    Shimmer,
    ShimmerDecraft,
    Shared
}

public enum SacrificeSource
{
    Default,
    Shared
}

/// <summary>Utility class that contains all the methods related to the research and sacrification of items</summary>
public class Researcher
{
    #region IgnoringCraftConditions

    private static readonly List<Condition> IgnoringCraftConditions =
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
        Condition.MoonPhases37
    ];

    #endregion

    private static readonly int ResearchedItemGroups = Enum.GetValues(typeof(ResearchSource)).Length;
    private static readonly int SacrificeGroups = Enum.GetValues(typeof(SacrificeSource)).Length;

    private Queue<int>? _researchedQueue;

    private Queue<int> ResearchedQueue => _researchedQueue ??= new Queue<int>();

    public static bool IsPlayerInJourneyMode => Main.LocalPlayer.difficulty == 3;


    private List<int>?[] ResearchedItems { get; } = new List<int>?[ResearchedItemGroups];
    public List<int>? DefaultResearchedItems => ResearchedItems[(int)ResearchSource.Default];
    public List<int>? CraftResearchedItems => ResearchedItems[(int)ResearchSource.Craft];
    public List<int>? ShimmerResearchedItems => ResearchedItems[(int)ResearchSource.Shimmer];
    public List<int>? DecraftResearchedItems => ResearchedItems[(int)ResearchSource.ShimmerDecraft];
    public List<int>? SharedItems => ResearchedItems[(int)ResearchSource.Shared];

    public Dictionary<int, int>?[] SacrificedItems { get; } = new Dictionary<int, int>?[SacrificeGroups];
    public Dictionary<int, int>? DefaultSacrifices => SacrificedItems[(int)SacrificeSource.Default];
    public Dictionary<int, int>? SharedSacrifices => SacrificedItems[(int)SacrificeSource.Shared];

    public bool AutoResearchCraftableItems { get; init; } = HyperConfig.Instance.AutoResearchCraftableItems;
    public bool AutoResearchShimmerableItems { get; init; } = ConfigOptions.ResearchShimmerableItems;
    public bool AutoResearchDecraftItems { get; init; } = ConfigOptions.ResearchDecraftItems;
    private bool BalanceShimmerAutoresearch { get; } = ConfigOptions.BalanceShimmerAutoresearch;

    public IEnumerable<int> AllResearchedItems =>
        ResearchedItems.Where(list => list is { Count: > 0 })
            .SelectMany(list => list!)
            .Distinct();

    public IEnumerable<int> AllNonSharedItems =>
        ResearchedItems.Take((int)ResearchSource.Shared)
            .Where(list => list is { Count: > 0 })
            .SelectMany(list => list!)
            .Distinct();

    public bool AnyItemResearched => ResearchedItems.Any(list => list is { Count: > 0 });
    public bool AnyItemSacrificed => SacrificedItems.Any(list => list is { Count: > 0 });

    public void SacrificeItems(IDictionary<int, int> itemCount, SacrificeSource sacrificeSource = default)
    {
        SacrificeItems(itemCount.Select(pair => new Item(pair.Key, pair.Value)), sacrificeSource);
    }

    public void SacrificeItems(IEnumerable<Item> items, SacrificeSource sacrificeSource = default)
    {
        foreach (Item item in items)
            SacrificeItem(item, sacrificeSource);
    }

    public void SacrificeItem(Item item, SacrificeSource source = default)
    {
        if (item.stack == 0) return;

        int itemId = item.type; // Save the ID because sacrifice can turn item into air
        ResearchSource researchSource = source == SacrificeSource.Shared ? ResearchSource.Shared : default;

        if (ConfigOptions.OnlyOneItemNeeded && ResearchItem(itemId, researchSource))
        {
            if (item.stack == 1) item.TurnToAir();
        }
        else
        {
            CreativeUI.ItemSacrificeResult result = CreativeUI.SacrificeItem(item, out int amountSacrificed);
            if (result == CreativeUI.ItemSacrificeResult.SacrificedAndDone)
                AfterResearch(itemId, researchSource);
            AddOrRemoveSacrifice(itemId, amountSacrificed, source);
        }
    }

    public void ResearchItems(IEnumerable<int>? items, ResearchSource source = default)
    {
        if (items == null) return;
        foreach (int itemId in items)
            ResearchItem(itemId, source);
    }

    public void TryResearchItems(IDictionary<int, int> items, ResearchSource source = default)
    {
        foreach ((int itemId, int itemCount) in items)
            TryResearchItem(itemId, itemCount, source);
    }

    private void TryResearchItem(int itemId, int itemCount, ResearchSource source = default)
    {
        if (IsResearchable(itemId) && itemCount >= GetRemaining(itemId))
            ResearchItem(itemId, source);
    }

    /// <returns><c>false</c> if you have already been researched or item is unresearchable</returns>
    public bool ResearchItem(int itemId, ResearchSource source = default, bool researchQueue = true)
    {
        CreativeUI.ItemSacrificeResult result = CreativeUI.ResearchItem(itemId);
        if (result == CreativeUI.ItemSacrificeResult.SacrificedAndDone) AfterResearch(itemId, source, researchQueue);
        return result == CreativeUI.ItemSacrificeResult.SacrificedAndDone;
    }

    private void ResearchQueue()
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
        }
    }

    public void TryResearchShimmeredItem(int itemId)
    {
        if (!AutoResearchShimmerableItems || !IsResearched(itemId)) return;

        int shimmerItemId = ItemsUtils.GetShimmeredItemId(itemId);
        if (shimmerItemId > 0)
            ResearchItem(shimmerItemId, ResearchSource.Shimmer);
    }

    public void ResearchDecraftItems(int itemId)
    {
        if (!AutoResearchDecraftItems || !IsResearched(itemId)) return;

        List<int> decraftItems = ItemsUtils.GetDecraftItems(itemId);
        if (decraftItems.Count == 0) return;
        foreach (int decraftItemId in decraftItems) ResearchItem(decraftItemId, ResearchSource.ShimmerDecraft);
    }

    private void AfterResearch(int itemId, ResearchSource source = default, bool researchQueue = true)
    {
        if ((BalanceShimmerAutoresearch && Main.LocalPlayer.GetModPlayer<HyperPlayer>().WasInAether) ||
            !BalanceShimmerAutoresearch)
        {
            TryResearchShimmeredItem(itemId);
            ResearchDecraftItems(itemId);
        }

        if (AutoResearchCraftableItems)
        {
            ResearchedQueue.Enqueue(itemId);
            if (researchQueue) ResearchQueue();
        }

        AddToResearched(itemId, source);
    }

    private void ResearchItemOccurrences(int itemId) =>
        ResearchItemOccurrences(ContentSamples.ItemsByType[itemId]);

    private void ResearchItemOccurrences(Item item)
    {
        ResearchItemRecipesOccurrences(item);
        ResearchTileRecipesOccurrences(item);
    }

    private void ResearchItemRecipesOccurrences(Item item)
    {
        if (!RecipesSystem.ItemRecipesOccurrences.TryGetValue(item.type, out List<int>? itemRecipeIds))
            return;

        foreach (Recipe recipe in itemRecipeIds
                     .Select(recipeId => Main.recipe[recipeId])
                     .Where(IsRecipeResearchable))
            ResearchItem(recipe.createItem.type, ResearchSource.Craft, false);
    }

    private void ResearchTileRecipesOccurrences(Item item)
    {
        if (item.createTile < TileID.Dirt)
            return;

        foreach (int adjTile in ItemsUtils.GetAllAdjTiles(item.createTile))
        {
            if (!RecipesSystem.TileRecipesOccurrences.TryGetValue(adjTile, out List<int>? tileRecipeIds))
                continue;

            foreach (Recipe recipe in tileRecipeIds
                         .Select(recipeId => Main.recipe[recipeId])
                         .Where(IsRecipeResearchable))
                ResearchItem(recipe.createItem.type, ResearchSource.Craft, false);
        }
    }

    private static bool IsRecipeResearchable(Recipe recipe)
    {
        if (!IsResearchable(recipe.createItem.type) || IsResearched(recipe.createItem.type)) return false;

        Dictionary<int, IEnumerable<int>?> iconicAndOthers = [];
        foreach (RecipeGroup recipeGroup in recipe.acceptedGroups
                     .Select(recipeGroupId => RecipeGroup.recipeGroups[recipeGroupId]))
            iconicAndOthers[recipeGroup.IconicItemId] = recipeGroup.ValidItems;

        bool allItemsResearched = recipe.requiredItem.All(item =>
        {
            if (iconicAndOthers.TryGetValue(item.type, out IEnumerable<int>? validItems) && validItems != null)
                return validItems.Any(IsResearched);
            return IsResearched(item.type);
        });
        if (!allItemsResearched) return false;

        var hyperPlayer = Main.LocalPlayer.GetModPlayer<HyperPlayer>();
        bool allTilesResearched = recipe.requiredTile.All(
            tileId => hyperPlayer.ResearchedTiles.GetValueOrDefault(tileId, false) || Main.LocalPlayer.adjTile[tileId]);
        if (!allTilesResearched) return false;

        bool allConditionsAreMet = recipe.Conditions.All(condition =>
            (ConfigOptions.IgnoreCraftingConditions && IgnoringCraftConditions.Contains(condition)) || condition.IsMet()
        );
        return allConditionsAreMet;
    }

    private void AddToResearched(int itemId, ResearchSource source)
    {
        if (ResearchedItems[(int)source] == null)
            ResearchedItems[(int)source] = [itemId];
        else
            ResearchedItems[(int)source]!.Add(itemId);
    }


    private void AddOrRemoveSacrifice(int itemId, int amount, SacrificeSource source = default)
    {
        if (IsResearched(itemId))
        {
            SacrificedItems[(int)source]?.Remove(itemId);
            return;
        }

        if (SacrificedItems[(int)source] == null)
            SacrificedItems[(int)source] = new Dictionary<int, int> { [itemId] = amount };
        else
            SacrificedItems[(int)source]![itemId] = SacrificedItems[(int)source]!.GetValueOrDefault(itemId, 0) + amount;
    }

    #region StaticMethods

    public static int GetSharedValue(int itemId) =>
        ContentSamples.CreativeResearchItemPersistentIdOverride.GetValueOrDefault(itemId, -1);

    public static int GetTotalNeeded(int itemId)
    {
        if (CreativeItemSacrificesCatalog.Instance.TryGetSacrificeCountCapToUnlockInfiniteItems(itemId,
                out int amountNeeded))
            return ConfigOptions.OnlyOneItemNeeded ? 1 : amountNeeded;
        return 0;
    }

    public static int GetResearchedCount(int itemId) =>
        Main.LocalPlayerCreativeTracker.ItemSacrifices.GetSacrificeCount(itemId);

    public static int GetRemaining(int itemId) => GetTotalNeeded(itemId) - GetResearchedCount(itemId);

    public static bool IsResearched(int itemId)
    {
        if (CreativeItemSacrificesCatalog.Instance.TryGetSacrificeCountCapToUnlockInfiniteItems(itemId,
                out int amountNeeded))
        {
            int researched = Main.LocalPlayerCreativeTracker.ItemSacrifices.GetSacrificeCount(itemId);
            return amountNeeded <= researched;
        }

        return false;
    }

    private static bool IsValidResearchItem(int itemId) =>
        ContentSamples.ItemsByType.TryGetValue(itemId, out Item? item)
        && item is { IsAir: false }
        && item.type == itemId;

    public static bool IsResearchable(int itemId) =>
        IsValidResearchItem(itemId)
        && CreativeItemSacrificesCatalog.Instance.TryGetSacrificeCountCapToUnlockInfiniteItems(itemId, out _);

    #endregion
}