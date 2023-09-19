using HyperResearch.Utils;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace HyperResearch
{
    public class HyperResearch : Mod
    {
        public static readonly Dictionary<int, List<int>> ItemRecipesOccurrences = new();
        public static readonly Dictionary<int, List<int>> TileRecipesOccurrences = new();
        public static int ResearchableItemsCount { get; set; }

        public override void PostSetupContent()
        {
            int totalResearchable = 0;
            for (int itemId = 0; itemId < ItemLoader.ItemCount; itemId++)
            {
                if (!Researcher.IsResearchable(itemId)) continue;
                if (Researcher.GetSharedValue(itemId) != -1) continue;
                totalResearchable++;
            }
            ResearchableItemsCount = totalResearchable;

            void AddToItemOccurrences(int itemId, int recipeId)
            {
                if (ItemRecipesOccurrences.TryGetValue(itemId, out List<int> recipeIds))
                    recipeIds.Add(recipeId);
                else ItemRecipesOccurrences[itemId] = new List<int> { recipeId };
            }

            foreach (Recipe recipe in Main.recipe)
            {
                if (!Researcher.IsResearchable(recipe.createItem.type)) return;

                Dictionary<int, IEnumerable<int>> iconicAndOthers = new();
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
                    else TileRecipesOccurrences[tileId] = new List<int> { recipe.RecipeIndex };
                }
            }
        }
    }
}