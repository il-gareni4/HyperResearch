using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace HyperResearch.Utils
{
    /// <summary>Utility class whose functions are related to text formatting and its output</summary>
    public static class TextUtils
    {
        /// <summary>Displays information about researched items in the game chat</summary>
        public static void MessageResearched(IEnumerable<int> items)
        {
            if (!ModContent.GetInstance<HyperConfig>().ShowNewlyResearchedItems || items.Count() == 0) return;
            string researchStr = items.Count() > 1 ? $"{items.Count()} new items" : "new item";
            Main.NewText($"Researched {researchStr}: [i:{string.Join("][i:", items)}]", Terraria.ID.Colors.JourneyMode);
        }

        /// <summary>Displays information about researched crating items in the game chat</summary>
        public static void MessageResearchedCraftable(IEnumerable<int> items)
        {
            if (!ModContent.GetInstance<HyperConfig>().ShowResearchedCraftableItems || items.Count() == 0) return;
            string researchStr = items.Count() > 1 ? $"{items.Count()} craftable items" : "craftable item";
            Main.NewText($"Researched {researchStr}: [i:{string.Join("][i:", items)}]", Color.Lerp(Terraria.ID.Colors.JourneyMode, Color.White, 0.25f));
        }
    }
}
