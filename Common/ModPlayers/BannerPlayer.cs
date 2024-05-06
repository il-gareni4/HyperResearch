using HyperResearch.Common.ModPlayers.Interfaces;
using HyperResearch.Common.Systems;
using HyperResearch.Utils;
using System.Collections.Generic;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace HyperResearch.Common.ModPlayers;

public class BannerPlayer : ModPlayer, IResearchPlayer
{
    public List<int> ResearchedBanners = [];

    public override void OnEnterWorld()
    {
        for (int itemId = 1; itemId < ItemLoader.ItemCount; itemId++)
            if (Researcher.IsResearched(itemId)) TryAddBanner(itemId);
    }

    public override void ProcessTriggers(TriggersSet triggersSet)
    {
        if (KeybindSystem.ForgetAllBind.JustPressed)
            ResearchedBanners.Clear();
    }

    public void OnResearch(Item item) => TryAddBanner(item.type);

    private void TryAddBanner(int itemId)
    {
        if (BannerSystem.ItemToBanner.TryGetValue(itemId, out var bannerId))
            ResearchedBanners.Add(bannerId);
    }
}
