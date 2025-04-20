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
    private readonly List<string> _bannersOfDisabledMods = [];
    public Dictionary<int, bool> ResearchedBanners { get; } = [];

    public IEnumerable<int> EnabledBanners =>
        ResearchedBanners.Where(kv => kv.Value).Select(kv => kv.Key);

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
        if (KeybindSystem.EnableDisableBuffBind!.JustPressed &&
            Main.HoverItem.tooltipContext == ItemSlot.Context.CreativeInfinite &&
            BannerSystem.TryItemToBanner(Main.HoverItem.type, out int bannerId) &&
            ResearchedBanners.TryGetValue(bannerId, out bool enabled))
        {
            ResearchedBanners[bannerId] = !enabled;
        }
    }

    public override void SaveData(TagCompound tag)
    {
        if (!Researcher.IsPlayerInJourneyMode) return;
        tag["enabledVanillaBanners"] =
            ResearchedBanners
            .Where(kv => kv.Key < BannerSystem.vanillaBannersCount && kv.Value)
            .Select(kv => kv.Key)
            .ToArray();
        tag["enabledModBanners"] =
            ResearchedBanners
            .Where(kv => kv.Key >= BannerSystem.vanillaBannersCount && kv.Value)
            .Select(kv => NPCLoader.GetNPC(kv.Key).FullName)
            .Concat(_bannersOfDisabledMods)
            .ToArray();
    }

    public override void Unload()
    {
        if (!Researcher.IsPlayerInJourneyMode) return;
        
        ResearchedBanners.Clear();
        _bannersOfDisabledMods.Clear();
    }

    public override void LoadData(TagCompound tag)
    {
        if (!Researcher.IsPlayerInJourneyMode) return;

        // Migrate from 1.0 to 1.1
        if (tag.TryGet("bannersEnabled", out int[] enabledBanners))
            foreach (int bannerId in enabledBanners)
                if (bannerId < BannerSystem.vanillaBannersCount)
                    ResearchedBanners[bannerId] = true;

        // Current version
        if (tag.TryGet("enabledVanillaBanners", out int[] enabledVanillaBanners))
            foreach (int bannerId in enabledVanillaBanners.Where(bannerId => bannerId < BannerSystem.vanillaBannersCount))
                ResearchedBanners[bannerId] = true;

        if (tag.TryGet("enabledModBanners", out string[] enabledModBanners))
            foreach (string npcFullName in enabledModBanners)
            {
                if (ModContent.TryFind(npcFullName, out ModNPC modNPC))
                    ResearchedBanners[modNPC.Banner] = true;
                else
                {
                    string[] npcFullNameSplit = npcFullName.Split('/');
                    if (npcFullNameSplit.Length == 2 &&
                        !ModLoader.HasMod(npcFullNameSplit[0]) &&
                        _bannersOfDisabledMods.Count < 128) // To prevent save file to be too big
                    {
                        _bannersOfDisabledMods.Add(npcFullName);
                    }
                }
            }
    }

    private void TryAddBanner(int itemId, bool enabled = true)
    {
        if (!BannerSystem.TryItemToBanner(itemId, out int bannerId)) return;
        ResearchedBanners.TryAdd(bannerId, enabled && HyperConfig.Instance.BannerBuffEnabledByDefault);
    }
}