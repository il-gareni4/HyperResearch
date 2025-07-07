using System.Collections.Generic;
using System.Text;
using HyperResearch.Common.Configs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace HyperResearch.Utils;

/// <summary>Utility class whose functions are related to text formatting and its output</summary>
public static class TextUtils
{
    public static void MessageResearcherResults(Researcher researcher, int playerShared = -1)
    {
        if (researcher is { AnyItemResearched: false, AnyItemSacrificed: false }) return;

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
    public static void MessageResearchedItems(List<int> items)
    {
        if (!VisualConfig.Instance.ShowNewlyResearchedItems || items == null || items.Count == 0) return;
        Main.NewText(
            Language.GetTextValue(
                "Mods.HyperResearch.Messages.ResearchedItems",
                items.Count,
                GetItemsString(items)
            ),
            Colors.JourneyMode
        );
    }

    /// <summary>Displays information about researched crating items in the game chat</summary>
    private static void MessageResearchedCraftableItems(List<int> items)
    {
        if (!VisualConfig.Instance.ShowResearchedCraftableItems || items == null || items.Count == 0) return;
        Main.NewText(
            Language.GetTextValue(
                "Mods.HyperResearch.Messages.CraftResearchedItems",
                items.Count,
                GetItemsString(items)
            ),
            Color.Lerp(Colors.JourneyMode, Color.Gold, 0.4f)
        );
    }

    /// <summary>Displays information about researched shimmered items in the game chat</summary>
    private static void MessageResearchedShimmeredItems(List<int> items)
    {
        if (!VisualConfig.Instance.ShowResearchedShimmeredItems || items == null || items.Count == 0) return;
        Main.NewText(
            Language.GetTextValue(
                "Mods.HyperResearch.Messages.ShimmerResearchedItems",
                items.Count,
                GetItemsString(items)
            ),
            Color.Lerp(Colors.JourneyMode, Colors.RarityPurple, 0.4f)
        );
    }

    private static void MessageDecraftItems(List<int> items)
    {
        if (!VisualConfig.Instance.ShowResearchedDecraftItems || items == null || items.Count == 0) return;
        Main.NewText(
            Language.GetTextValue(
                "Mods.HyperResearch.Messages.DecraftResearchedItems",
                items.Count,
                GetItemsString(items)
            ),
            Color.Lerp(Colors.JourneyMode, Color.White, 0.5f)
        );
    }

    private static void MessageSacrifices(Dictionary<int, int> sacrifices)
    {
        if (!VisualConfig.Instance.ShowSacrifices || sacrifices == null || sacrifices.Count == 0) return;
        Main.NewText(
            Language.GetTextValue(
                "Mods.HyperResearch.Messages.Sacrifices",
                sacrifices.Count,
                GetSacrificesString(sacrifices)
            ),
            Color.Lerp(Colors.JourneyMode, Colors.CoinPlatinum, 0.4f)
        );
    }

    public static void MessageOtherPlayerResearchedItems(List<int> items, int playerId)
    {
        if (!VisualConfig.Instance.ShowOtherPlayersResearchedItems || items == null || items.Count == 0) return;
        Main.NewText(
            Language.GetTextValue(
                "Mods.HyperResearch.Messages.OtherPlayerResearchedItems",
                Main.player[playerId].name,
                items.Count,
                GetItemsString(items)
            ),
            Color.Pink
        );
    }

    private static void MessageSharedItems(List<int> items, int playerId)
    {
        if (!VisualConfig.Instance.ShowSharedItems || items == null || items.Count == 0) return;
        Main.NewText(
            Language.GetTextValue(
                "Mods.HyperResearch.Messages.SharedItems",
                Main.player[playerId].name,
                items.Count,
                GetItemsString(items)
            ),
            Color.Pink
        );
    }

    private static void MessageSharedSacrifices(Dictionary<int, int> sacrifices, int playerId)
    {
        if (!VisualConfig.Instance.ShowSharedSacrifices || sacrifices == null || sacrifices.Count == 0) return;
        Main.NewText(
            Language.GetTextValue(
                "Mods.HyperResearch.Messages.SharedSacrifices",
                Main.player[playerId].name,
                sacrifices.Count,
                GetSacrificesString(sacrifices)
            ),
            Color.Pink
        );
    }

    private static string GetSacrificesString(Dictionary<int, int> sacrifices)
    {
        StringBuilder sb = new();
        var i = 0;
        foreach ((int itemId, int count) in sacrifices)
        {
            int needed = Researcher.GetTotalNeeded(itemId);
            int researched = Researcher.GetResearchedCount(itemId);
            sb.Append($"[i/s{count}:{itemId}]({researched}/{needed})");
            if (i++ != sacrifices.Count - 1) sb.Append(", ");
        }

        return sb.ToString();
    }

    private static string GetItemsString(List<int> items) => $"[i:{string.Join("][i:", items)}]";
}