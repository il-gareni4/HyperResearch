using HyperResearch.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace HyperResearch.Common.Systems
{
    public class BannerSystem : ModSystem
    {
        public static Dictionary<int, int> ItemToBanner { get; set; }

        public override void Load()
        {
            ItemToBanner = new();
        }

        public override void PostSetupContent()
        {
            const int vanillaBannersCount = 290;
            for (int bannerId = 0; bannerId < vanillaBannersCount; bannerId++)
            {
                int itemId = Item.BannerToItem(bannerId);
                ItemToBanner[itemId] = bannerId;
            }

            IDictionary<int, int> bannerToItem = (IDictionary<int, int>)typeof(NPCLoader)
                                                                        .GetField("bannerToItem", BindingFlags.NonPublic | BindingFlags.Static)
                                                                        ?.GetValue(null);
            if (bannerToItem is null) return;
            foreach ((int bannerId, int itemId) in bannerToItem)
                ItemToBanner[itemId] = bannerId;
        }

        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
        {
            if (!Researcher.IsPlayerInJourneyMode() || !ConfigOptions.UseResearchedBannersBuff) return;
            if (Main.LocalPlayer.TryGetModPlayer(out HyperPlayer player) && player.ResearchedBanners.Count > 0)
            {
                foreach (int bannerId in player.ResearchedBanners)
                    Main.SceneMetrics.NPCBannerBuff[bannerId] = true;
                Main.SceneMetrics.hasBanner = true;
            }
        }

        public override void Unload()
        {
            ItemToBanner = null;
        }
    }
}
