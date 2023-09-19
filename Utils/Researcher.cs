
using HyperResearch.Common;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;

namespace HyperResearch.Utils
{
    public enum ResearchSource
    {
        Default,
        Craft,
        Shimmer
    }

    /// <summary>Utility class that contains all the methods related to the research and sacrification of items</summary>
    public class Researcher
    {
        public static readonly List<Condition> IgnoringCraftConditions = new()
        {
            Condition.NearWater, Condition.NearLava, Condition.NearHoney, Condition.NearShimmer,
            Condition.TimeDay, Condition.TimeNight, Condition.InDungeon, Condition.InCorrupt,
            Condition.InHallow, Condition.InMeteor, Condition.InJungle, Condition.InSnow,
            Condition.InCrimson, Condition.InWaterCandle, Condition.InPeaceCandle, Condition.InTowerSolar,
            Condition.InTowerVortex, Condition.InTowerNebula, Condition.InTowerStardust, Condition.InDesert,
            Condition.InGlowshroom, Condition.InUndergroundDesert, Condition.InSkyHeight, Condition.InSpace,
            Condition.InOverworldHeight, Condition.InDirtLayerHeight, Condition.InRockLayerHeight,
            Condition.InUnderworldHeight, Condition.InUnderworld, Condition.InBeach, Condition.InRain,
            Condition.InSandstorm, Condition.InOldOneArmy, Condition.InGranite, Condition.InMarble,
            Condition.InHive, Condition.InGemCave, Condition.InLihzhardTemple, Condition.InGraveyard,
            Condition.InAether, Condition.InShoppingZoneForest, Condition.InBelowSurface, Condition.InEvilBiome,
            Condition.NotInEvilBiome, Condition.NotInHallow, Condition.NotInGraveyard, Condition.NotInUnderworld,
            Condition.Christmas, Condition.Halloween, Condition.BloodMoon, Condition.NotBloodMoon, Condition.Eclipse,
            Condition.NotEclipse, Condition.EclipseOrBloodMoon, Condition.NotEclipseAndNotBloodMoon,
            Condition.Thunderstorm, Condition.BirthdayParty, Condition.LanternNight, Condition.HappyWindyDay,
            Condition.BloodMoonOrHardmode, Condition.NightOrEclipse, Condition.MoonPhaseFull,
            Condition.MoonPhaseWaningGibbous, Condition.MoonPhaseThirdQuarter, Condition.MoonPhaseWaningCrescent,
            Condition.MoonPhaseNew, Condition.MoonPhaseWaxingCrescent, Condition.MoonPhaseFirstQuarter,
            Condition.MoonPhaseWaxingGibbous, Condition.MoonPhasesQuarter0, Condition.MoonPhasesQuarter1,
            Condition.MoonPhasesQuarter2, Condition.MoonPhasesQuarter3, Condition.MoonPhasesHalf0,
            Condition.MoonPhasesHalf1, Condition.MoonPhasesEven, Condition.MoonPhasesOdd, Condition.MoonPhasesNearNew,
            Condition.MoonPhasesEvenQuarters, Condition.MoonPhasesOddQuarters, Condition.MoonPhases04,
            Condition.MoonPhases15, Condition.MoonPhases26, Condition.MoonPhases37,
        };
        public List<int> ResearchedItems;
        public List<int> ResearchedCraftableItems;
        public List<int> ResearchedShimmeredItems;

        private Dictionary<int, int> _sacrificedItems;
        public Dictionary<int, int> SacrificedItems { get => _sacrificedItems ??= new(); }

        public Researcher()
        {
            ResearchedItems = new();
            ResearchedCraftableItems = new();
            ResearchedShimmeredItems = new();
        }

        public List<int> AllResearchedItems
        {
            get
            {
                return ResearchedItems.Concat(ResearchedCraftableItems).Concat(ResearchedShimmeredItems).ToList();
            }
        }

        public static bool IsPlayerInJourneyMode()
        {
            return Main.LocalPlayer.difficulty == 3;
        }

        public static int GetSharedValue(int itemId)
        {
            if (ContentSamples.CreativeResearchItemPersistentIdOverride.TryGetValue(itemId, out int value))
                return value;
            else return -1;
        }

        public static int GetTotalNeeded(int itemId)
        {
            if (CreativeItemSacrificesCatalog.Instance.TryGetSacrificeCountCapToUnlockInfiniteItems(itemId, out int amountNeeded))
                return HyperConfig.Instance.OnlyOneItemNeeded ? 1 : amountNeeded;
            return 0;
        }

        public static int GetResearchedCount(int itemId)
        {
            return Main.LocalPlayerCreativeTracker.ItemSacrifices.GetSacrificeCount(itemId);
        }

        public static int GetRemaining(int itemId)
        {
            return GetTotalNeeded(itemId) - GetResearchedCount(itemId);
        }

        public static bool IsResearched(int itemId)
        {
            if (CreativeItemSacrificesCatalog.Instance.TryGetSacrificeCountCapToUnlockInfiniteItems(itemId, out int amountNeeded))
            {
                int researched = Main.LocalPlayerCreativeTracker.ItemSacrifices.GetSacrificeCount(itemId);
                return amountNeeded - researched == 0;
            }
            else return false;
        }

        public static bool IsResearchable(int itemId)
        {
            if (ContentSamples.ItemsByType.TryGetValue(itemId, out Item item) && (item.IsAir || item.type != itemId))
                return false;
            return CreativeItemSacrificesCatalog.Instance.TryGetSacrificeCountCapToUnlockInfiniteItems(itemId, out _);
        }

        public void SacrificeItems(IEnumerable<Item> items, ResearchSource source = default, bool researchCraftable = true)
        {
            bool anyItemResearched = false;
            foreach (Item item in items)
            {
                CreativeUI.ItemSacrificeResult result = SacrificeItem(item, source, false);
                anyItemResearched = anyItemResearched || result == CreativeUI.ItemSacrificeResult.SacrificedAndDone;
            }
            if (researchCraftable && anyItemResearched) ResearchCraftable();
        }

        public CreativeUI.ItemSacrificeResult SacrificeItem(Item item, ResearchSource source = default, bool researchCraftable = true)
        {
            if (item.stack == 0) return 0;

            int itemId = item.type;
            CreativeUI.ItemSacrificeResult result = CreativeUI.ItemSacrificeResult.CannotSacrifice;
            bool researched;
            if (HyperConfig.Instance.OnlyOneItemNeeded)
            {
                researched = ResearchItem(itemId, source, researchCraftable);
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
                    AfterResearch(itemId, source, researchCraftable);
                else if (result == CreativeUI.ItemSacrificeResult.SacrificedButNotDone)
                    SacrificedItems[itemId] = SacrificedItems.GetValueOrDefault(itemId, 0) + amountSacrificed;
            }
            if (researched) SacrificedItems.Remove(itemId);
            return result;
        }


        public void ResearchItems(IEnumerable<int> items, ResearchSource source = default, bool researchCraftable = true)
        {
            bool anyItemResearched = false;
            foreach (int itemId in items)
            {
                if (ResearchItem(itemId, source, false))
                    anyItemResearched = true;
            }
            if (researchCraftable && anyItemResearched) ResearchCraftable();
        }

        public void ResearchItems(IDictionary<int, int> items, ResearchSource source = default, bool researchCraftable = true)
        {
            bool anyItemResearched = false;
            foreach ((int itemId, int itemCount) in items)
            {
                if (!IsResearchable(itemId) || itemCount < GetRemaining(itemId)) continue;
                if (ResearchItem(itemId, source, false))
                    anyItemResearched = true;
            }
            if (researchCraftable && anyItemResearched) ResearchCraftable();
        }

        public bool ResearchItem(int itemId, int itemCount, ResearchSource source = default, bool researchCraftable = true)
        {
            if (HyperConfig.Instance.OnlyOneItemNeeded && itemCount >= 1)
                return ResearchItem(itemId, source, researchCraftable);

            if (itemCount >= GetRemaining(itemId)) return ResearchItem(itemId, source, researchCraftable);
            else return false;
        }

        public bool ResearchItem(int itemId, ResearchSource source = default, bool researchCraftable = true)
        {
            CreativeUI.ItemSacrificeResult result = CreativeUI.ResearchItem(itemId);
            if (result == CreativeUI.ItemSacrificeResult.SacrificedAndDone) AfterResearch(itemId, source, researchCraftable);
            return result == CreativeUI.ItemSacrificeResult.SacrificedAndDone;
        }

        public void ResearchCraftable()
        {
            HyperPlayer player = Main.LocalPlayer.GetModPlayer<HyperPlayer>();

            bool newItemResearched = true;
            while (newItemResearched)
            {
                newItemResearched = false;
                foreach (Recipe recipe in Main.recipe)
                {
                    if (!IsResearchable(recipe.createItem.type) || IsResearched(recipe.createItem.type)) continue;

                    Dictionary<int, IEnumerable<int>> iconicAndOthers = new();
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
                    if (!allItemsResearched) continue;

                    bool allTilesResearched = recipe.requiredTile.All(
                        tileId => player.ResearchedTiles.GetValueOrDefault(tileId, false) || Main.LocalPlayer.adjTile[tileId]);
                    if (!allTilesResearched) continue;

                    bool allConditionsAreMet = recipe.Conditions.All(condition =>
                        IgnoringCraftConditions.Contains(condition) && HyperConfig.Instance.IgnoreCraftingConditions ? true : condition.IsMet()
                    );
                    if (!allConditionsAreMet) continue;

                    newItemResearched = newItemResearched || recipe.createItem.material || recipe.createItem.createTile >= TileID.Dirt;
                    ResearchItem(recipe.createItem.type, ResearchSource.Craft, false);
                }
            }
        }

        public bool TryResearchShimmeredItem(int itemId, bool researchCraftable = true)
        {
            int shimmerItemId = ItemsUtils.GetShimmeredItemId(itemId);
            if (!IsResearched(itemId) || shimmerItemId <= 0) return false;

            return ResearchItem(shimmerItemId, ResearchSource.Shimmer, researchCraftable);
        }

        private void AfterResearch(int itemId, ResearchSource source, bool researchCraftable)
        {
            if (HyperConfig.Instance.AutoResearchShimmeredItems) TryResearchShimmeredItem(itemId);
            if (researchCraftable && HyperConfig.Instance.AutoResearchCraftable) ResearchCraftable();

            GetItemsListBySource(source).Add(itemId);
        }

        public bool AnyItemResearched()
        {
            return ResearchedItems.Count > 0 || ResearchedCraftableItems.Count > 0 || ResearchedShimmeredItems.Count > 0;
        }

        private List<int> GetItemsListBySource(ResearchSource source)
        {
            return source switch
            {
                ResearchSource.Craft => ResearchedCraftableItems,
                ResearchSource.Shimmer => ResearchedShimmeredItems,
                _ => ResearchedItems
            };
        }
    }
}
