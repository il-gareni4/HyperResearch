using Microsoft.Xna.Framework;
using ReLogic.OS.Windows;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HyperResearch.Utils
{
    /// <summary>Utility class whose functions are related to text formatting and its output</summary>
    public static class TextUtils
    {
        public static void MessageResearcherResults(Researcher researcher) {
            MessageResearchedItems(researcher.ResearchedItems);
            MessageResearchedShimmeredItems(researcher.ResearchedShimmeredItems);
            MessageResearchedCraftableItems(researcher.ResearchedCraftableItems);
        }

        /// <summary>Displays information about researched items in the game chat</summary>
        public static void MessageResearchedItems(IEnumerable<int> items)
        {
            if (!ModContent.GetInstance<HyperConfig>().ShowNewlyResearchedItems || items.Count() == 0) return;
            string researchStr = items.Count() > 1 ? $"{items.Count()} new items" : "new item";
            Main.NewText($"Researched {researchStr}: [i:{string.Join("][i:", items)}]", Colors.JourneyMode);
        }

        /// <summary>Displays information about researched crating items in the game chat</summary>
        public static void MessageResearchedCraftableItems(IEnumerable<int> items)
        {
            if (!ModContent.GetInstance<HyperConfig>().ShowResearchedCraftableItems || items.Count() == 0) return;
            string researchStr = items.Count() > 1 ? $"{items.Count()} craftable items" : "craftable item";
            Main.NewText($"Researched {researchStr}: [i:{string.Join("][i:", items)}]", Color.Lerp(Colors.JourneyMode, Color.Gold, 0.4f));
        }

        /// <summary>Displays information about researched shimmered items in the game chat</summary>
        public static void MessageResearchedShimmeredItems(IEnumerable<int> items) {
            if (!ModContent.GetInstance<HyperConfig>().ShowResearchedShimmeredItems || items.Count() == 0) return;
            string researchStr = items.Count() > 1 ? $"{items.Count()} shimmered items" : "shimmered item";
            Main.NewText($"Researched {researchStr}: [i:{string.Join("][i:", items)}]", Color.Lerp(Colors.JourneyMode, Colors.RarityPurple, 0.4f));
        }
    }
}
