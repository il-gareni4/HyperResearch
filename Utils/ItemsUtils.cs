using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.ModLoader;

namespace HyperResearch.Utils;

/// <summary>A utility class whose functions are directly related to items in the game</summary>
/// <seealso cref="Item" />
public static class ItemsUtils
{
    private static readonly Dictionary<int, int> CoinsCurrency = new()
    {
        { ItemID.CopperCoin, 1 },
        { ItemID.SilverCoin, 100 },
        { ItemID.GoldCoin, 10_000 },
        { ItemID.PlatinumCoin, 1_000_000 }
    };

    /// <summary>
    ///     More readable variant of <code>Main.ItemDropsDB.GetRulesForItemID(itemId).Count == 0</code>
    /// </summary>
    public static bool IsLootItem(int itemId)
    {
        List<IItemDropRule> rules = Main.ItemDropsDB.GetRulesForItemID(itemId);
        return rules.Count > 0;
    }

    public static bool CanOpenLootItem(int itemId)
    {
        return itemId switch
        {
            ItemID.LockBox => Researcher.IsResearched(ItemID.GoldenKey),
            ItemID.ObsidianLockbox => Researcher.IsResearched(ItemID.ShadowKey) ||
                                      Main.LocalPlayer.HasItemInInventoryOrOpenVoidBag(ItemID.ShadowKey),
            _ => true
        };
    }

    public static IEnumerable<int> GetItemLoot(int itemId)
    {
        List<IItemDropRule> itemDropRules = Main.ItemDropsDB.GetRulesForItemID(itemId);
        List<DropRateInfo> dropRateInfos = [];
        DropRateInfoChainFeed dropRateInfo = new(1f);
        foreach (IItemDropRule item in itemDropRules) item.ReportDroprates(dropRateInfos, dropRateInfo);

        DropAttemptInfo attemptInfo = default;
        attemptInfo.player = Main.LocalPlayer;
        attemptInfo.item = itemId;
        attemptInfo.IsExpertMode = Main.expertMode;
        attemptInfo.IsMasterMode = Main.masterMode;
        attemptInfo.rng = Main.rand;

        return dropRateInfos.Where(info => info.conditions?.All(c => c.CanDrop(attemptInfo)) ?? true)
            .Select(info => info.itemId);
    }

    public static int GetShimmerItemId(int itemId)
    {
        if (ItemID.Sets.ShimmerCountsAsItem.IndexInRange(itemId) && ItemID.Sets.ShimmerCountsAsItem[itemId] > 0)
            itemId = ItemID.Sets.ShimmerCountsAsItem[itemId];
        if (ItemID.Sets.ShimmerTransformToItem.IndexInRange(itemId) && ItemID.Sets.ShimmerTransformToItem[itemId] > 0)
            return ItemID.Sets.ShimmerTransformToItem[itemId];
        else
            return -1;
    }

    private static Recipe GetDecraftRecipe(int itemId)
    {
        if (ContentSamples.ItemsByType[itemId].createTile == TileID.MusicBoxes)
            return null;

        if (ItemID.Sets.ShimmerCountsAsItem.IndexInRange(itemId) && ItemID.Sets.ShimmerCountsAsItem[itemId] > 0)
            itemId = ItemID.Sets.ShimmerCountsAsItem[itemId];

        if (!ItemID.Sets.CraftingRecipeIndices.IndexInRange(itemId) || ShimmerTransforms.IsItemTransformLocked(itemId)) return null;
        int recipeIndex = ShimmerTransforms.GetDecraftingRecipeIndex(itemId);

        return recipeIndex < 0 ? null : Main.recipe[recipeIndex];
    }

    public static List<int> GetDecraftItemIds(int itemId) => GetDecraftItemIds(GetDecraftRecipe(itemId));

    private static List<int> GetDecraftItemIds(Recipe recipe)
    {
        if (recipe is null) return [];
        return [.. (recipe.customShimmerResults ?? recipe.requiredItem).Select(r => r.type)];
    }

    public static Dictionary<int, int> GetCurrencyItemsAndValues(int currencyId)
    {
        if (currencyId == -1)
            return CoinsCurrency;

        if (CustomCurrencyManager.TryGetCurrencySystem(currencyId, out CustomCurrencySystem system))
        {
            return (Dictionary<int, int>)system.GetType()
                .GetField("_valuePerUnit", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.GetValue(system);
        }

        return null;
    }

    public static List<int> GetAllAdjTiles(int tileId) => [tileId, .. GetAdjTiles(tileId)];

    private static List<int> GetAdjTiles(int tileId)
    {
        List<int> adjTiles = [];
        switch (tileId)
        {
            case 77:
            case 302:
                adjTiles.Add(17);
                break;
            case 133:
                adjTiles.Add(17);
                adjTiles.Add(77);
                break;
            case 134:
                adjTiles.Add(16);
                break;
            case 354:
            case 469:
            case 487:
                adjTiles.Add(14);
                break;
            case 355:
                adjTiles.Add(13);
                adjTiles.Add(14);
                break;
        }

        ModTile t = TileLoader.GetTile(tileId);
        if (t is not null) adjTiles.AddRange(t.AdjTiles);
        return adjTiles;
    }

    public static bool IsInItemGroup(Item item, ContentSamples.CreativeHelper.ItemGroup group) =>
        ContentSamples.CreativeHelper.GetItemGroup(item, out int _) == group;

    private static IEnumerable<int> GetAllPrefixes(Item item)
    {
        List<PrefixCategory> categories = item.GetPrefixCategories();
        if (categories == null || categories.Count == 0) return [];

        IEnumerable<int> vanillaPrefixes =
            categories
            .SelectMany(Item.GetVanillaPrefixes)
            .Distinct();

        IEnumerable<ModPrefix> modPrefixes =
            categories
            .SelectMany(PrefixLoader.GetPrefixesInCategory)
            .Distinct();

        return vanillaPrefixes.Concat(modPrefixes.Select(p => p.Type));
    }

    public static IEnumerable<int> GetPossiblePrefixes(Item item) =>
        GetAllPrefixes(item).Where(item.CanApplyPrefix);

    public static Color GetRarityColor(Item item)
    {
        return item.rare switch
        {
            ItemRarityID.Quest => Colors.RarityAmber,
            ItemRarityID.Gray => Colors.RarityTrash,
            ItemRarityID.White => Colors.RarityNormal,
            ItemRarityID.Blue => Colors.RarityBlue,
            ItemRarityID.Green => Colors.RarityGreen,
            ItemRarityID.Orange => Colors.RarityOrange,
            ItemRarityID.LightRed => Colors.RarityRed,
            ItemRarityID.Pink => Colors.RarityPink,
            ItemRarityID.LightPurple => Colors.RarityPurple,
            ItemRarityID.Lime => Colors.RarityLime,
            ItemRarityID.Yellow => Colors.RarityYellow,
            ItemRarityID.Cyan => Colors.RarityCyan,
            ItemRarityID.Red => new Color(255, 40, 100),
            ItemRarityID.Purple => new Color(180, 40, 255),
            _ => RarityLoader.GetRarity(item.rare)?.RarityColor ?? Colors.RarityNormal
        };
    }

    public static bool CanHavePrefixes(Item item) =>
        item.CanHavePrefixes() && (item.ModItem?.CanReforge() ?? true) && GetPossiblePrefixes(item).Any();
}