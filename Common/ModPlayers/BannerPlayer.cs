using HyperResearch.Common.ModPlayers.Interfaces;
using HyperResearch.Common.Systems;
using HyperResearch.Utils;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace HyperResearch.Common.ModPlayers;

public class BannerPlayer : ModPlayer, IResearchPlayer
{
    public Dictionary<int, bool> ResearchedBanners { get; private set; } = [];

    public override void OnEnterWorld()
    {
        if (!Researcher.IsPlayerInJourneyMode) return;

        for (int itemId = 1; itemId < ItemLoader.ItemCount; itemId++)
            if (Researcher.IsResearched(itemId)) TryAddBanner(itemId, false);
    }

    public override void ProcessTriggers(TriggersSet triggersSet)
    {
        if (!Researcher.IsPlayerInJourneyMode) return;

#if DEBUG
        if (KeybindSystem.ForgetAllBind.JustPressed)
            ResearchedBanners.Clear();
#endif
        if (Main.HoverItem.tooltipContext == ItemSlot.Context.CreativeInfinite
            && BannerSystem.ItemToBanner.TryGetValue(Main.HoverItem.type, out int bannerId)
            && ResearchedBanners.TryGetValue(bannerId, out bool enabled)
            && KeybindSystem.EnableDisableBuffBind.JustPressed)
        {
            ResearchedBanners[bannerId] = !enabled;
        }
    }

    public override void SaveData(TagCompound tag)
    {
        if (Main.CurrentPlayer.difficulty != 3) return;
        tag["bannersEnabled"] = ResearchedBanners.Where(kv => kv.Value).Select(kv => kv.Key).ToArray();
    }

    public override void Unload()
    {
        if (Main.CurrentPlayer.difficulty != 3) return;
        ResearchedBanners.Clear();
    }

    public override void LoadData(TagCompound tag)
    {
        if (Main.CurrentPlayer.difficulty != 3) return;

        if (tag.TryGet("bannersEnabled", out int[] enabledBanners))
        {
            foreach (int bannerId in enabledBanners)
                if (bannerId < Main.SceneMetrics.NPCBannerBuff.Length)
                    ResearchedBanners[bannerId] = true;
        }
    }

    public void OnResearch(Item item) => TryAddBanner(item.type);

    private void TryAddBanner(int itemId, bool enabledIfNotFound = true)
    {
        if (!BannerSystem.ItemToBanner.TryGetValue(itemId, out var bannerId)
            || ResearchedBanners.ContainsKey(bannerId)) return;

        ResearchedBanners[bannerId] = enabledIfNotFound;
    }
}
