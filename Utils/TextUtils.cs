using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BetterResearch.Utils
{
    public static class TextUtils
    {
        public static void MessageResearched(List<int> items)
        {
            if (!ModContent.GetInstance<BRConfig>().ShowNewlyResearchedItems || items.Count == 0) return;
            string researchStr = items.Count > 1 ? $"{items.Count} new items" : "new item";
            Main.NewText($"Researched {researchStr}: [i:{string.Join("][i:", items)}]", Terraria.ID.Colors.JourneyMode);
        }

        public static void MessageResearchedCraftable(List<int> items)
        {
            if (!ModContent.GetInstance<BRConfig>().ShowResearchedCraftableItems || items.Count == 0) return;
            string researchStr = items.Count > 1 ? $"{items.Count} craftable items" : "craftable item";
            Main.NewText($"Researched {researchStr}: [i:{string.Join("][i:", items)}]", Color.Lerp(Terraria.ID.Colors.JourneyMode, Color.White, 0.25f));
        }
    }
}
