using HyperResearch.Utils;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace HyperResearch.Common.Systems
{
    public class RecipesSystem : ModSystem
    {
        public static readonly Dictionary<int, List<int>> ItemRecipesOccurrences = [];
        public static readonly Dictionary<int, List<int>> TileRecipesOccurrences = [];

        public override void PostAddRecipes()
        {
            void AddToItemOccurrences(int itemId, int recipeId)
            {
                if (ItemRecipesOccurrences.TryGetValue(itemId, out List<int> recipeIds))
                    recipeIds.Add(recipeId);
                else ItemRecipesOccurrences[itemId] = [recipeId];
            }

            foreach (Recipe recipe in Main.recipe)
            {
                if (!Researcher.IsResearchable(recipe.createItem.type)) return;

                Dictionary<int, IEnumerable<int>> iconicAndOthers = [];
                foreach (int recipeGroupId in recipe.acceptedGroups)
                {
                    RecipeGroup recipeGroup = RecipeGroup.recipeGroups[recipeGroupId];
                    iconicAndOthers[recipeGroup.IconicItemId] = recipeGroup.ValidItems;
                }

                foreach (Item item in recipe.requiredItem)
                {
                    if (iconicAndOthers.TryGetValue(item.type, out IEnumerable<int> validItems))
                    {
                        foreach (int validItemId in validItems)
                            AddToItemOccurrences(validItemId, recipe.RecipeIndex);
                    }
                    else AddToItemOccurrences(item.type, recipe.RecipeIndex);
                }

                foreach (int tileId in recipe.requiredTile)
                {
                    if (TileRecipesOccurrences.TryGetValue(tileId, out List<int> recipeIds))
                        recipeIds.Add(recipe.RecipeIndex);
                    else TileRecipesOccurrences[tileId] = [recipe.RecipeIndex];
                }
            }
        }
    }
}
