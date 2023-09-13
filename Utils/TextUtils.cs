using Microsoft.Xna.Framework;
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
        public static void MessageResearcherResults(Researcher researcher)
        {
            MessageResearchedItems(researcher.ResearchedItems);
            MessageResearchedShimmeredItems(researcher.ResearchedShimmeredItems);
            MessageResearchedCraftableItems(researcher.ResearchedCraftableItems);
        }

        /// <summary>Displays information about researched items in the game chat</summary>
        public static void MessageResearchedItems(IEnumerable<int> items)
        {
            if (!HyperConfig.Instance.ShowNewlyResearchedItems || items.Count() == 0) return;
            string researchStr = items.Count() > 1 ? $"{items.Count()} new items" : "new item";
            Main.NewText($"Researched {researchStr}: [i:{string.Join("][i:", items)}]", Colors.JourneyMode);
        }

        /// <summary>Displays information about researched crating items in the game chat</summary>
        public static void MessageResearchedCraftableItems(IEnumerable<int> items)
        {
            if (!HyperConfig.Instance.ShowResearchedCraftableItems || items.Count() == 0) return;
            string researchStr = items.Count() > 1 ? $"{items.Count()} craftable items" : "craftable item";
            Main.NewText($"Researched {researchStr}: [i:{string.Join("][i:", items)}]", Color.Lerp(Colors.JourneyMode, Color.Gold, 0.4f));
        }

        /// <summary>Displays information about researched shimmered items in the game chat</summary>
        public static void MessageResearchedShimmeredItems(IEnumerable<int> items)
        {
            if (!HyperConfig.Instance.ShowResearchedShimmeredItems || items.Count() == 0) return;
            string researchStr = items.Count() > 1 ? $"{items.Count()} shimmered items" : "shimmered item";
            Main.NewText($"Researched {researchStr}: [i:{string.Join("][i:", items)}]", Color.Lerp(Colors.JourneyMode, Colors.RarityPurple, 0.4f));
        }

        public static void MessageSacrifices(Dictionary<int, int> sacrifices) {
            if (!HyperConfig.Instance.ShowPartiallyResearchedItems || sacrifices.Count == 0) return;
            string researchStr = sacrifices.Count() > 1 ? $"{sacrifices.Count()} items" : "item";
            string sacrificesStr = "";
            int i = 0;
            foreach ((int itemId, int count) in sacrifices)  {
                int needed = Researcher.ItemTotalResearchCount(itemId);
                int researched = Researcher.ItemResearchedCount(itemId);
                sacrificesStr += $"[i/s{count}:{itemId}]({researched}/{needed})";
                if (i++ != sacrifices.Count - 1) sacrificesStr += ", ";
            }
            Main.NewText($"Partially researched {researchStr}: {sacrificesStr}", Color.Lerp(Colors.JourneyMode, Colors.CoinPlatinum, 0.4f));
        }
    }
}
