using System;
using System.Collections.Generic;
using System.Reflection;
using HyperResearch.Common.ModPlayers;
using HyperResearch.Utils;
using Terraria;
using Terraria.ModLoader;

namespace HyperResearch.Common.Systems;

public class BannerSystem : ModSystem
{
    public static Dictionary<int, int> ItemToBanner { get; } = [];

    public override void PostSetupContent()
    {
        const int vanillaBannersCount = 290;
        for (var bannerId = 0; bannerId < vanillaBannersCount; bannerId++)
            ItemToBanner[Item.BannerToItem(bannerId)] = bannerId;

        var bannerToItem = (IDictionary<int, int>?)typeof(NPCLoader)
            .GetField("bannerToItem", BindingFlags.NonPublic | BindingFlags.Static)
            ?.GetValue(null);

        if (bannerToItem is null) return;
        foreach ((int bannerId, int itemId) in bannerToItem)
            ItemToBanner[itemId] = bannerId;
    }

    public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
    {
        if (!Researcher.IsPlayerInJourneyMode || !ConfigOptions.UseResearchedBannersBuff) return;
        if (Main.LocalPlayer.TryGetModPlayer(out BannerPlayer player) && player.ResearchedBanners.Count > 0)
        {
            foreach (int bannerId in player.EnabledBanners)
            {
                if (bannerId <= 0) continue;
                Main.SceneMetrics.NPCBannerBuff[bannerId] = true;
                Main.SceneMetrics.hasBanner = true;
            }
        }
    }

    public override void Unload()
    {
        ItemToBanner.Clear();
    }
}