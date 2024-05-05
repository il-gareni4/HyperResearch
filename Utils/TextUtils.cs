using HyperResearch.Common.Configs;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace HyperResearch.Utils
{
    /// <summary>Utility class whose functions are related to text formatting and its output</summary>
    public static class TextUtils
    {
        public static void MessageResearcherResults(Researcher researcher)
        {
            if (HyperConfig.Instance.ShowSacrifices && researcher.AnyItemSacrificed())
                MessageSacrifices(researcher.SacrificedItems);
            if (HyperConfig.Instance.ShowNewlyResearchedItems)
                MessageResearchedItems(researcher.ResearchedItems);
            if (HyperConfig.Instance.ShowResearchedDecraftItems)
                MessageDecraftItems(researcher.ResearchedDecraftItems);
            if (HyperConfig.Instance.ShowResearchedShimmeredItems)
                MessageResearchedShimmeredItems(researcher.ResearchedShimmeredItems);
            if (HyperConfig.Instance.ShowResearchedCraftableItems)
                MessageResearchedCraftableItems(researcher.ResearchedCraftableItems);
        }

        /// <summary>Displays information about researched items in the game chat</summary>
        public static void MessageResearchedItems(IEnumerable<int> items)
        {
            if (!items.Any()) return;
            Main.NewText(
                Language.GetTextValue("Mods.HyperResearch.Messages.ResearchedItems", items.Count(), GetItemsString(items)),
                Colors.JourneyMode
            );
        }

        /// <summary>Displays information about researched crating items in the game chat</summary>
        public static void MessageResearchedCraftableItems(IEnumerable<int> items)
        {
            if (!items.Any()) return;
            Main.NewText(
                Language.GetTextValue("Mods.HyperResearch.Messages.ResearchedCraftableItems", items.Count(), GetItemsString(items)),
                Color.Lerp(Colors.JourneyMode, Color.Gold, 0.4f)
            );
        }

        /// <summary>Displays information about researched shimmered items in the game chat</summary>
        public static void MessageResearchedShimmeredItems(IEnumerable<int> items)
        {
            if (!items.Any()) return;
            Main.NewText(
                Language.GetTextValue("Mods.HyperResearch.Messages.ResearchedShimmeredItems", items.Count(), GetItemsString(items)),
                Color.Lerp(Colors.JourneyMode, Colors.RarityPurple, 0.4f)
            );
        }

        public static void MessageDecraftItems(IEnumerable<int> items)
        {
            if (!items.Any()) return;
            Main.NewText(
                Language.GetTextValue("Mods.HyperResearch.Messages.ResearchedDecraftItems", items.Count(), GetItemsString(items)),
                Color.Lerp(Colors.JourneyMode, Color.White, 0.5f)
            );
        }

        public static void MessageSacrifices(IDictionary<int, int> sacrifices)
        {
            if (!sacrifices.Any()) return;
            Main.NewText(
                Language.GetTextValue("Mods.HyperResearch.Messages.Sacrifices", sacrifices.Count, GetSacrificesString(sacrifices)),
                Color.Lerp(Colors.JourneyMode, Colors.CoinPlatinum, 0.4f)
            );
        }

        public static void MessageOtherPlayerResearchedItems(IEnumerable<int> items, int playerId)
        {
            if (!items.Any()) return;
            Main.NewText(
                Language.GetTextValue("Mods.HyperResearch.Messages.OtherPlayerResearchedItems", Main.player[playerId].name, items.Count(), GetItemsString(items)),
                Color.Pink
            );
        }

        public static void MessageSharedItems(IEnumerable<int> items, int playerId)
        {
            if (!items.Any()) return;
            Main.NewText(
                Language.GetTextValue("Mods.HyperResearch.Messages.SharedItems", Main.player[playerId].name, items.Count(), GetItemsString(items)),
                Color.Pink
            );
        }

        public static void MessageSharedSacrifices(IDictionary<int, int> sacrifices, int playerId)
        {
            if (!sacrifices.Any()) return;
            Main.NewText(
                Language.GetTextValue("Mods.HyperResearch.Messages.SharedSacrifices", Main.player[playerId].name, sacrifices.Count, GetSacrificesString(sacrifices)),
                Color.Pink
            );
        }

        private static string GetSacrificesString(IDictionary<int, int> sacrifices)
        {
            string sacrificesStr = "";
            int i = 0;
            foreach ((int itemId, int count) in sacrifices)
            {
                int needed = Researcher.GetTotalNeeded(itemId);
                int researched = Researcher.GetResearchedCount(itemId);
                sacrificesStr += $"[i/s{count}:{itemId}]({researched}/{needed})";
                if (i++ != sacrifices.Count - 1) sacrificesStr += ", ";
            }
            return sacrificesStr;
        }

        private static string GetItemsString(IEnumerable<int> items)
        {
            return $"[i:{string.Join("][i:", items)}]";
        }
    }
}
