using HyperResearch.Common.Configs;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace HyperResearch.Utils
{
    /// <summary>Utility class whose functions are related to text formatting and its output</summary>
    public static class TextUtils
    {
        public static void MessageResearcherResults(Researcher researcher, int playerShared = -1)
        {
            if (playerShared >= 0)
            {
                MessageSharedSacrifices(researcher.SharedSacrifices, playerShared);
                MessageSharedItems(researcher.SharedItems, playerShared);
            }
            MessageSacrifices(researcher.DefaultSacrifices);
            MessageResearchedItems(researcher.DefaultResearchedItems);
            MessageDecraftItems(researcher.DecraftResearchedItems);
            MessageResearchedShimmeredItems(researcher.ShimmerResearchedItems);
            MessageResearchedCraftableItems(researcher.CraftResearchedItems);
        }

        /// <summary>Displays information about researched items in the game chat</summary>
        public static void MessageResearchedItems(IEnumerable<int> items)
        {
            if (!HyperConfig.Instance.ShowNewlyResearchedItems || items == null || !items.Any()) return;
            Main.NewText(
                Language.GetTextValue("Mods.HyperResearch.Messages.ResearchedItems", items.Count(), GetItemsString(items)),
                Colors.JourneyMode
            );
        }

        /// <summary>Displays information about researched crating items in the game chat</summary>
        public static void MessageResearchedCraftableItems(IEnumerable<int> items)
        {
            if (!HyperConfig.Instance.ShowResearchedCraftableItems || items == null || !items.Any()) return;
            Main.NewText(
                Language.GetTextValue("Mods.HyperResearch.Messages.CraftResearchedItems", items.Count(), GetItemsString(items)),
                Color.Lerp(Colors.JourneyMode, Color.Gold, 0.4f)
            );
        }

        /// <summary>Displays information about researched shimmered items in the game chat</summary>
        public static void MessageResearchedShimmeredItems(IEnumerable<int> items)
        {
            if (!HyperConfig.Instance.ShowResearchedShimmeredItems || items == null || !items.Any()) return;
            Main.NewText(
                Language.GetTextValue("Mods.HyperResearch.Messages.ShimmerResearchedItems", items.Count(), GetItemsString(items)),
                Color.Lerp(Colors.JourneyMode, Colors.RarityPurple, 0.4f)
            );
        }

        public static void MessageDecraftItems(IEnumerable<int> items)
        {
            if (!HyperConfig.Instance.ShowResearchedDecraftItems || items == null || !items.Any()) return;
            Main.NewText(
                Language.GetTextValue("Mods.HyperResearch.Messages.DecraftResearchedItems", items.Count(), GetItemsString(items)),
                Color.Lerp(Colors.JourneyMode, Color.White, 0.5f)
            );
        }

        public static void MessageSacrifices(IDictionary<int, int> sacrifices)
        {
            if (!HyperConfig.Instance.ShowSacrifices || sacrifices == null || !sacrifices.Any()) return;
            Main.NewText(
                Language.GetTextValue("Mods.HyperResearch.Messages.Sacrifices", sacrifices.Count, GetSacrificesString(sacrifices)),
                Color.Lerp(Colors.JourneyMode, Colors.CoinPlatinum, 0.4f)
            );
        }

        public static void MessageOtherPlayerResearchedItems(IEnumerable<int> items, int playerId)
        {
            if (!HyperConfig.Instance.ShowOtherPlayersResearchedItems || items == null || !items.Any()) return;
            Main.NewText(
                Language.GetTextValue("Mods.HyperResearch.Messages.OtherPlayerResearchedItems", Main.player[playerId].name, items.Count(), GetItemsString(items)),
                Color.Pink
            );
        }

        public static void MessageSharedItems(IEnumerable<int> items, int playerId)
        {
            if (!HyperConfig.Instance.ShowSharedItems || items == null || !items.Any()) return;
            Main.NewText(
                Language.GetTextValue("Mods.HyperResearch.Messages.SharedItems", Main.player[playerId].name, items.Count(), GetItemsString(items)),
                Color.Pink
            );
        }

        public static void MessageSharedSacrifices(IDictionary<int, int> sacrifices, int playerId)
        {
            if (!HyperConfig.Instance.ShowSharedSacrifices || sacrifices == null || !sacrifices.Any()) return;
            Main.NewText(
                Language.GetTextValue("Mods.HyperResearch.Messages.SharedSacrifices", Main.player[playerId].name, sacrifices.Count, GetSacrificesString(sacrifices)),
                Color.Pink
            );
        }

        private static string GetSacrificesString(IDictionary<int, int> sacrifices)
        {
            StringBuilder sb = new();
            int i = 0;
            foreach ((int itemId, int count) in sacrifices)
            {
                int needed = Researcher.GetTotalNeeded(itemId);
                int researched = Researcher.GetResearchedCount(itemId);
                sb.Append($"[i/s{count}:{itemId}]({researched}/{needed})");
                if (i++ != sacrifices.Count - 1) sb.Append(", ");
            }
            return sb.ToString();
        }

        private static string GetItemsString(IEnumerable<int> items)
        {
            return $"[i:{string.Join("][i:", items)}]";
        }
    }
}
