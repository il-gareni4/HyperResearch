
using HyperResearch.Common;
using System;
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
        public List<int> ResearchedItems;
        public List<int> ResearchedCraftableItems;
        public List<int> ResearchedShimmeredItems;

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

        public int SacrificeItem(Item item, ResearchSource source = ResearchSource.Default, bool researchCraftable = true)
        {
            int itemId = item.type;
            int amountSacrificed;
            if (HyperConfig.Instance.OnlyOneItemNeeded)
            {
                if (!IsResearchable(itemId) || IsResearched(itemId)) return 0;
                if (--item.stack <= 0) item.TurnToAir();
                amountSacrificed = Convert.ToInt32(ResearchItem(itemId, source, researchCraftable));
            }
            else
            {
                CreativeUI.ItemSacrificeResult result = CreativeUI.SacrificeItem(item, out amountSacrificed);
                if (result == CreativeUI.ItemSacrificeResult.SacrificedAndDone)
                    AfterResearch(itemId, source, researchCraftable);
            }
            return amountSacrificed;
        }

        /// <summary>
        /// Tries to research an item with an ID (<paramref name="itemId"/>) and a count (<paramref name="itemCount"/>). 
        /// If the number of items is not enough to research, then it does nothing, otherwise it researches the item 
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="itemCount"></param>
        /// <param name="researchedCraftable">Auto-researched crafting items</param>
        /// <returns>Whether an item has been researched</returns>
        public bool TryResearchItem(int itemId, int itemCount, ResearchSource source = ResearchSource.Default, bool researchCraftable = true)
        {
            if (HyperConfig.Instance.OnlyOneItemNeeded && itemCount >= 1) ResearchItem(itemId, source, researchCraftable);
            if (!IsResearchable(itemId) || IsResearched(itemId)) return false;

            int remaining = GetTotalNeeded(itemId) - GetResearchedCount(itemId);
            if (itemCount >= remaining) return ResearchItem(itemId, source, researchCraftable);
            else return false;
        }

        public void ResearchItems(IEnumerable<int> items, ResearchSource source = ResearchSource.Default, bool researchCraftable = true)
        {
            foreach (int itemId in items)
                ResearchItem(itemId, source, false);
            if (researchCraftable) ResearchCraftable();
        }

        public bool ResearchItem(int itemId, ResearchSource source = ResearchSource.Default, bool researchCraftable = true)
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

                    bool allRecipeGroupsResearched = recipe.acceptedGroups.All((recipeGroupId) =>
                        RecipeGroup.recipeGroups[recipeGroupId].ValidItems.Any(IsResearched)
                    );
                    if (!allRecipeGroupsResearched) continue;

                    bool allItemsResearched = recipe.requiredItem.All(item => IsResearched(item.type));
                    if (!allItemsResearched) continue;

                    bool allTilesResearched = recipe.requiredTile.All(
                        tileId => player.ResearchedTiles.GetValueOrDefault(tileId, false) || Main.LocalPlayer.adjTile[tileId]);
                    if (!allTilesResearched) continue;

                    bool allConditionsAreMet = HyperConfig.Instance.IgnoreCraftingConditions || recipe.Conditions.All(condition => condition.IsMet());
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
