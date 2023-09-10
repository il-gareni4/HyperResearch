
using HyperResearch.Common;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

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

        public static int ItemSharedValue(int itemId)
        {
            if (ContentSamples.CreativeResearchItemPersistentIdOverride.TryGetValue(itemId, out int value))
                return value;
            else return -1;
        }

        public static int ItemTotalResearchCount(int itemId)
        {
            if (CreativeItemSacrificesCatalog.Instance.TryGetSacrificeCountCapToUnlockInfiniteItems(itemId, out int amountNeeded))
                return amountNeeded;
            return 0;
        }

        public static int ItemResearchedCount(int itemId)
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
            int? remaining = CreativeUI.GetSacrificesRemaining(itemId);
            return remaining.HasValue;
        }

        /// <param name="researchedCraftable">Auto-researched crafting items</param>
        public CreativeUI.ItemSacrificeResult SacrificeItem(Item item, ResearchSource source = ResearchSource.Default, bool researchCraftable = true)
        {
            int itemId = item.type;
            CreativeUI.ItemSacrificeResult result = CreativeUI.SacrificeItem(item, out int _);
            if (result == CreativeUI.ItemSacrificeResult.SacrificedAndDone)
                AfterResearch(itemId, source, researchCraftable);
            return result;
        }

        /// <summary>
        /// Tries to research an item with an ID (<paramref name="itemId"/>).
        /// If the item has already been researched (or it's unresearchable), then the function returns false
        /// </summary>
        /// <param name="researchedCraftable">Auto-researched crafting items</param>
        /// <returns>Whether the item has been researched</returns>
        public bool TryResearchItem(int itemId, ResearchSource source = ResearchSource.Default, bool researchCraftable = true)
        {
            if (!IsResearchable(itemId) || IsResearched(itemId)) return false;

            ResearchItem(itemId, source, researchCraftable);
            return true;
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
            if (!IsResearchable(itemId) || IsResearched(itemId)) return false;

            int remaining = (int)CreativeUI.GetSacrificesRemaining(itemId);
            if (remaining <= itemCount) ResearchItem(itemId, source, researchCraftable);
            else return false;
            return true;
        }

        /// <summary>
        /// Researches the item without preliminary checks
        /// </summary>
        /// <returns>Researched crafting items</returns>
        public void ResearchItem(int itemId, ResearchSource source = ResearchSource.Default, bool researchCraftable = true)
        {
            CreativeUI.ResearchItem(itemId);
            AfterResearch(itemId, source, researchCraftable);
        }

        /// <param name="items">Items to research</param>
        /// <param name="researchedCraftable">Auto-researched crafting items</param>
        /// <returns>List of researched items (not include already researched items)</returns>
        public void ResearchItems(IEnumerable<int> items, ResearchSource source = ResearchSource.Default, bool researchCraftable = true)
        {
            foreach (int itemId in items)
                TryResearchItem(itemId, source, researchCraftable);
            ResearchCraftable();
        }

        public void ResearchCraftable()
        {
            HyperConfig config = ModContent.GetInstance<HyperConfig>();
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

                    bool allConditionsAreMet = config.IgnoreCraftingConditions || recipe.Conditions.All(condition => condition.IsMet());
                    if (!allConditionsAreMet) continue;

                    newItemResearched = newItemResearched || recipe.createItem.material || recipe.createItem.createTile >= TileID.Dirt;
                    ResearchItem(recipe.createItem.type, ResearchSource.Craft, false);
                }
            }
        }

        public bool TryResearchShimmeredItem(int itemId)
        {
            int shimmerItemId = ItemsUtils.GetShimmeredItemId(itemId);
            if (!IsResearched(itemId) || shimmerItemId <= 0) return false;

            return TryResearchItem(shimmerItemId, ResearchSource.Shimmer);
        }

        private void AfterResearch(int itemId, ResearchSource source, bool researchCraftable)
        {
            HyperConfig config = ModContent.GetInstance<HyperConfig>();

            Main.LocalPlayer.GetModPlayer<HyperPlayer>().TryAddToResearchedTiles(itemId);
            if (config.AutoResearchShimmeredItems) TryResearchShimmeredItem(itemId);
            if (researchCraftable && config.AutoResearchCraftable) ResearchCraftable();

            switch (source)
            {
                case ResearchSource.Craft:
                    ResearchedCraftableItems.Add(itemId);
                    break;
                case ResearchSource.Shimmer:
                    ResearchedShimmeredItems.Add(itemId);
                    break;
                default:
                    ResearchedItems.Add(itemId);
                    break;
            }
        }
    }
}
