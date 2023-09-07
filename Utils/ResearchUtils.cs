
using HyperResearch.Common;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace HyperResearch.Utils
{
    /// <summary>Utility class that contains all the methods related to the research and sacrification of items</summary>
    public static class ResearchUtils
    {

        /// <param name="researchedCraftable">Auto-researched crafting items</param>
        public static CreativeUI.ItemSacrificeResult SacrificeItem(Item item, out List<int> researchedCraftable)
        {
            researchedCraftable = new();
            CreativeUI.ItemSacrificeResult result = CreativeUI.SacrificeItem(item, out int _);
            if (
                ModContent.GetInstance<HyperConfig>().AutoResearchCraftable &&
                result == CreativeUI.ItemSacrificeResult.SacrificedAndDone
            ) researchedCraftable = ResearchCraftable();
            return result;
        }

        /// <summary>
        /// Tries to research an item with an ID (<paramref name="itemId"/>).
        /// If the item has already been researched (or it's unresearchable), then the function returns false
        /// </summary>
        /// <param name="researchedCraftable">Auto-researched crafting items</param>
        /// <returns>Whether the item has been researched</returns>
        public static bool TryResearchItem(int itemId, out List<int> researchedCraftable)
        {
            researchedCraftable = new();
            if (!IsResearchable(itemId) || IsResearched(itemId)) return false;
            else researchedCraftable = ResearchItem(itemId);
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
        public static bool TryResearchItem(int itemId, int itemCount, out List<int> researchedCraftable)
        {
            researchedCraftable = new();
            if (!IsResearchable(itemId) || IsResearched(itemId)) return false;

            int remaining = (int)CreativeUI.GetSacrificesRemaining(itemId);
            if (remaining <= itemCount) researchedCraftable = ResearchItem(itemId);
            else return false;
            return true;
        }

        /// <summary>
        /// Researches the item without preliminary checks
        /// </summary>
        /// <returns>Researched crafting items</returns>
        public static List<int> ResearchItem(int itemId, bool researchCraftable = true)
        {
            CreativeUI.ResearchItem(itemId);
            Main.LocalPlayer.GetModPlayer<HyperPlayer>().TryAddToResearchedTiles(itemId);
            if (researchCraftable && ModContent.GetInstance<HyperConfig>().AutoResearchCraftable) return ResearchCraftable();
            else return new List<int>();
        }

        /// <param name="items">Items to research</param>
        /// <param name="researchedCraftable">Auto-researched crafting items</param>
        /// <returns>List of researched items (not include already researched items)</returns>
        public static List<int> ResearchItems(IEnumerable<int> items, out List<int> researchedCraftable)
        {
            List<int> researched = new();
            researchedCraftable = new();
            foreach (int itemId in items)
            {
                if (TryResearchItem(itemId, out List<int> craftable))
                {
                    researched.Add(itemId);
                    researchedCraftable.AddRange(craftable);
                }
            }
            return researched;
        }

        public static bool IsResearched(int itemId)
        {
            int? remaining = CreativeUI.GetSacrificesRemaining(itemId);
            return remaining.HasValue && remaining.Value == 0;
        }

        public static bool IsResearchable(int itemId)
        {
            int? remaining = CreativeUI.GetSacrificesRemaining(itemId);
            return remaining.HasValue;
        }

        public static bool IsTileResearched(int tileId)
        {
            return Main.LocalPlayer.GetModPlayer<HyperPlayer>().ResearchedTiles.ContainsKey(tileId);
        }

        public static List<int> ResearchCraftable()
        {
            HyperConfig config = ModContent.GetInstance<HyperConfig>();
            List<int> itemsResearched = new();
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

                    bool allTilesResearched = recipe.requiredTile.All(tileId => {
                        return IsTileResearched(tileId) || Main.LocalPlayer.IsTileTypeInInteractionRange(tileId, TileReachCheckSettings.Simple);
                    });
                    if (!allTilesResearched) continue;

                    bool allConidtionsAreMet = config.IgnoreCraftingConditions || recipe.Conditions.All(condition => condition.IsMet());
                    if (!allConidtionsAreMet) continue;

                    newItemResearched = newItemResearched || recipe.createItem.material || recipe.createItem.createTile >= TileID.Dirt;
                    ResearchItem(recipe.createItem.type, false);
                    itemsResearched.Add(recipe.createItem.type);
                }
            }
            return itemsResearched;
        }
    }
}
