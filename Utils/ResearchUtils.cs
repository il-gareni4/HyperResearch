
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
        public static void ResearchItem(int itemId, bool researchCraftable = true)
        {
            CreativeUI.ResearchItem(itemId);
            Main.LocalPlayer.GetModPlayer<BRPlayer>().TryAddToResearchedTiles(itemId);
            if (researchCraftable && ModContent.GetInstance<BRConfig>().AutoResearchCraftable) ResearchCraftable();
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

        public static void ResearchCraftable()
        {
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

                    bool allConidtionsAreMet = recipe.Conditions.All(condition => condition.IsMet());
                    if (!allConidtionsAreMet) continue;

                    newItemResearched = newItemResearched || recipe.createItem.material;
                    ResearchItem(recipe.createItem.type, false);
                    itemsResearched.Add(recipe.createItem.type);
                }
            }
            if (itemsResearched.Count > 0)
                Main.NewText($"Researched {itemsResearched.Count} craftable items: [i:{string.Join("][i:", itemsResearched)}]");
        }
    }
}
