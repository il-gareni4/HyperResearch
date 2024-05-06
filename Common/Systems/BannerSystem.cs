using HyperResearch.Common.ModPlayers;
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
            ItemToBanner = [];
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
            if (!Researcher.IsPlayerInJourneyMode || !ConfigOptions.UseResearchedBannersBuff) return;
            if (Main.LocalPlayer.TryGetModPlayer(out BannerPlayer player) && player.ResearchedBanners.Count > 0)
            {
                bool anyBannerEnabled = false;
                foreach ((int bannerId, bool bannerEnabled) in player.ResearchedBanners)
                    if (bannerEnabled)
                    {
                        Main.SceneMetrics.NPCBannerBuff[bannerId] = true;
                        anyBannerEnabled = true;
                    }
                Main.SceneMetrics.hasBanner = anyBannerEnabled;
            }
        }

        public override void Unload()
        {
            ItemToBanner = null;
        }
    }
}
