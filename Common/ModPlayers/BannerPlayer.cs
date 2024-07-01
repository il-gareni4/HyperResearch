using System.Collections.Generic;
using System.Linq;
using HyperResearch.Common.Configs;
using HyperResearch.Common.ModPlayers.Interfaces;
using HyperResearch.Common.Systems;
using HyperResearch.Utils;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace HyperResearch.Common.ModPlayers;

public class BannerPlayer : ModPlayer, IResearchPlayer
{
    public SortedDictionary<int, bool> ResearchedBanners { get; } = [];

    public IEnumerable<int> EnabledBanners
    {
        get
        {
            foreach ((int bannerId, bool bannerEnabled) in ResearchedBanners)
            {
                if (bannerEnabled)
                    yield return bannerId;
            }
        }
    }

    public void OnResearch(Item item) => TryAddBanner(item.type);

    public override void OnEnterWorld()
    {
        if (!Researcher.IsPlayerInJourneyMode) return;

        for (var itemId = 1; itemId < ItemLoader.ItemCount; itemId++)
            if (Researcher.IsResearched(itemId))
                TryAddBanner(itemId, false);
    }

    public override void ProcessTriggers(TriggersSet triggersSet)
    {
        if (!Researcher.IsPlayerInJourneyMode) return;

#if DEBUG
        if (KeybindSystem.ForgetAllBind!.JustPressed)
            ResearchedBanners.Clear();
#endif
        if (Main.HoverItem.tooltipContext == ItemSlot.Context.CreativeInfinite
            && BannerSystem.ItemToBanner.TryGetValue(Main.HoverItem.type, out int bannerId)
            && ResearchedBanners.TryGetValue(bannerId, out bool enabled)
            && KeybindSystem.EnableDisableBuffBind!.JustPressed)
            ResearchedBanners[bannerId] = !enabled;
    }

    public override void SaveData(TagCompound tag)
    {
        if (!Researcher.IsPlayerInJourneyMode) return;
        tag["bannersEnabled"] = ResearchedBanners.Where(kv => kv.Value)
            .Select(kv => kv.Key)
            .ToArray();
    }

    public override void Unload()
    {
        if (!Researcher.IsPlayerInJourneyMode) return;
        ResearchedBanners.Clear();
    }

    public override void LoadData(TagCompound tag)
    {
        if (!Researcher.IsPlayerInJourneyMode) return;

        if (tag.TryGet("bannersEnabled", out int[] enabledBanners))
            foreach (int bannerId in enabledBanners)
                if (bannerId < Main.SceneMetrics.NPCBannerBuff.Length)
                    ResearchedBanners[bannerId] = true;
    }

    private void TryAddBanner(int itemId, bool enabled = true)
    {
        if (!BannerSystem.ItemToBanner.TryGetValue(itemId, out int bannerId)) return;
        ResearchedBanners.TryAdd(bannerId, enabled && HyperConfig.Instance.BannerBuffEnabledByDefault);
    }
}