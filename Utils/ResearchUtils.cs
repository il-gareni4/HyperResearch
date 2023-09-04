
using BetterResearch.Common;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace BetterResearch.Utils
{
    public static class ResearchUtils
    {
        public static bool TryResearchItem(int itemId, int itemCount, out List<int> researchedCraftable) {
            researchedCraftable = new();
            if (!CanBeResearched(itemId) || IsResearched(itemId)) return false;

            int remaining = (int)CreativeUI.GetSacrificesRemaining(itemId);
            if (remaining <= itemCount) researchedCraftable = ResearchItem(itemId);
            else return false;
            return true;
        }

        public static List<int> ResearchItem(int itemId, bool researchCraftable = true)
        {
            CreativeUI.ResearchItem(itemId);
            Main.LocalPlayer.GetModPlayer<BRPlayer>().TryAddToResearchedTiles(itemId);
            if (researchCraftable && ModContent.GetInstance<BRConfig>().AutoResearchCraftable) return ResearchCraftable();
            else return new List<int>();
        }

        public static bool IsResearched(int itemId)
        {
            int? remaining = CreativeUI.GetSacrificesRemaining(itemId);
            return remaining.HasValue && remaining.Value == 0;
        }

        public static bool CanBeResearched(int itemId)
        {
            int? remaining = CreativeUI.GetSacrificesRemaining(itemId);
            return remaining.HasValue;
        }

        public static bool IsTileResearched(int tileId)
        {
            return Main.LocalPlayer.GetModPlayer<BRPlayer>().ResearchedTiles.ContainsKey(tileId);
        }

        public static List<int> ResearchCraftable()
        {
            BRConfig config = ModContent.GetInstance<BRConfig>();
            List<int> itemsResearched = new();
            bool newItemResearched = true;
            while (newItemResearched)
            {
                newItemResearched = false;
                foreach (Recipe recipe in Main.recipe)
                {
                    if (!CanBeResearched(recipe.createItem.type) || IsResearched(recipe.createItem.type)) continue;

                    bool allRecipeGroupsResearched = recipe.acceptedGroups.All((recipeGroupId) =>
                        RecipeGroup.recipeGroups[recipeGroupId].ValidItems.Any(IsResearched)
                    );
                    if (!allRecipeGroupsResearched) continue;

                    bool allItemsResearched = recipe.requiredItem.All(item => IsResearched(item.type));
                    if (!allItemsResearched) continue;

                    bool allTilesResearched = recipe.requiredTile.All(IsTileResearched);
                    if (!allTilesResearched) continue;

                    bool allConidtionsAreMet = config.IgnoreCraftingConditions || recipe.Conditions.All(condition => condition.IsMet());
                    if (!allConidtionsAreMet) continue;

                    newItemResearched = newItemResearched || recipe.createItem.material || recipe.createItem.createTile >= 0;
                    ResearchItem(recipe.createItem.type, false);
                    itemsResearched.Add(recipe.createItem.type);
                }
            }
            return itemsResearched;
        }
    }
}
