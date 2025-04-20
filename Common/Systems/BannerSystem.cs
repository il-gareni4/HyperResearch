using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HyperResearch.Common.ModPlayers;
using HyperResearch.Utils;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HyperResearch.Common.Systems;

public class BannerSystem : ModSystem
{
    public const int vanillaBannersCount = 290;
    public static Dictionary<int, int> VanillaItemToBanner { get; } = [];

    public override void PostSetupContent()
    {

        for (var bannerId = 0; bannerId < vanillaBannersCount; bannerId++)
            VanillaItemToBanner[Item.BannerToItem(bannerId)] = bannerId;
    }

    public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
    {
        if (!Researcher.IsPlayerInJourneyMode ||
         !ConfigOptions.UseResearchedBannersBuff ||
         !Main.LocalPlayer.TryGetModPlayer(out BannerPlayer player) ||
         player.ResearchedBanners.Count == 0)
        {
            return;
        }

        foreach (int bannerId in player.EnabledBanners)
        {
            if (!Main.SceneMetrics.NPCBannerBuff.IndexInRange(bannerId))
            {
                player.ResearchedBanners.Remove(bannerId);
                continue;
            }

            Main.SceneMetrics.NPCBannerBuff[bannerId] = true;
            Main.SceneMetrics.hasBanner = true;
        }
    }

    public static int ItemToBanner(int itemId)
    {
        if (itemId < ItemID.Count)
        {
            if (VanillaItemToBanner.TryGetValue(itemId, out int bannerId))
                return bannerId;
            else
                return -1;
        }
        return NPCLoader.BannerItemToNPC(itemId);
    }

    public static bool TryItemToBanner(int itemId, out int bannerId)
    {
        bannerId = ItemToBanner(itemId);
        return bannerId >= 0;
    }
}