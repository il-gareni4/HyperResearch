using HyperResearch.Common.Configs;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.WorldBuilding;

namespace HyperResearch.Utils
{
    /// <summary>Utility class whose functions are related to text formatting and its output</summary>
    public static class TextUtils
    {
        public static void MessageResearcherResults(Researcher researcher, int shared = -1)
        {
            MessageResearchedItems(researcher.ResearchedItems, shared);
            MessageResearchedShimmeredItems(researcher.ResearchedShimmeredItems);
            MessageResearchedCraftableItems(researcher.ResearchedCraftableItems);
        }

        /// <summary>Displays information about researched items in the game chat</summary>
        public static void MessageResearchedItems(IEnumerable<int> items, int shared = -1)
        {
            if (!HyperConfig.Instance.ShowNewlyResearchedItems || !items.Any()) return;
            string itemsStr = $"[i:{string.Join("][i:", items)}]";
            if (shared >= 0)
            {
                Main.NewText(
                    Language.GetTextValue("Mods.HyperResearch.Messages.SharedItems", Main.player[shared].name, items.Count(), itemsStr),
                    Color.Pink
                );
            } else
            {
                Main.NewText(
                   Language.GetTextValue("Mods.HyperResearch.Messages.ResearchedItems", items.Count(), itemsStr),
                   Colors.JourneyMode
               );
            }
        }

        /// <summary>Displays information about researched crating items in the game chat</summary>
        public static void MessageResearchedCraftableItems(IEnumerable<int> items)
        {
            if (!HyperConfig.Instance.ShowResearchedCraftableItems || !items.Any()) return;
            Main.NewText(
                Language.GetTextValue("Mods.HyperResearch.Messages.ResearchedCraftableItems", items.Count(), $"[i:{string.Join("][i:", items)}]"),
                Color.Lerp(Colors.JourneyMode, Color.Gold, 0.4f)
            );
        }

        /// <summary>Displays information about researched shimmered items in the game chat</summary>
        public static void MessageResearchedShimmeredItems(IEnumerable<int> items)
        {
            if (!HyperConfig.Instance.ShowResearchedShimmeredItems || !items.Any()) return;
            Main.NewText(
                Language.GetTextValue("Mods.HyperResearch.Messages.ResearchedShimmeredItems", items.Count(), $"[i:{string.Join("][i:", items)}]"),
                Color.Lerp(Colors.JourneyMode, Colors.RarityPurple, 0.4f)
            );
        }

        public static void MessageSacrifices(IDictionary<int, int> sacrifices)
        {
            if (!HyperConfig.Instance.ShowPartiallyResearchedItems || sacrifices.Count == 0) return;
            string sacrificesStr = "";
            int i = 0;
            foreach ((int itemId, int count) in sacrifices)
            {
                int needed = Researcher.GetTotalNeeded(itemId);
                int researched = Researcher.GetResearchedCount(itemId);
                sacrificesStr += $"[i/s{count}:{itemId}]({researched}/{needed})";
                if (i++ != sacrifices.Count - 1) sacrificesStr += ", ";
            }
            Main.NewText(
                Language.GetTextValue("Mods.HyperResearch.Messages.Sacrifices", sacrifices.Count, sacrificesStr),
                Color.Lerp(Colors.JourneyMode, Colors.CoinPlatinum, 0.4f)
            );
        }

        public static void MessageOtherPlayerResearchedItems(IEnumerable<int> items, int playerId)
        {
            if (!HyperConfig.Instance.ShowOtherPlayersResearchedItems || !items.Any()) return;
            Main.NewText(
                Language.GetTextValue("Mods.HyperResearch.Messages.OtherPlayerResearchedItems", Main.player[playerId].name, items.Count(), $"[i:{ string.Join("][i:", items)}]"),
                Color.Pink
            );
        }
    }
}
