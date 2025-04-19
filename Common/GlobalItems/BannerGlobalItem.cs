using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HyperResearch.Common.Configs;
using HyperResearch.Common.ModPlayers;
using HyperResearch.Common.Systems;
using HyperResearch.Utils;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace HyperResearch.Common.GlobalItems;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class BannerGlobalItem : GlobalItem
{
    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (!Researcher.IsPlayerInJourneyMode
            || !HyperConfig.Instance.ShowBannerBuffTooltips
            || item.tooltipContext != ItemSlot.Context.CreativeInfinite
            || !BannerSystem.TryItemToBanner(item.type, out int bannerId)
            || !Main.LocalPlayer.TryGetModPlayer(out BannerPlayer bannerPlayer)
            || !bannerPlayer.ResearchedBanners.TryGetValue(bannerId, out bool enabled)) return;

        string name = enabled ? "DisableBanner" : "EnableBanner";
        string text = enabled
            ? Language.GetTextValue("Mods.HyperResearch.Tooltips.DisableBanner",
                InputUtils.GetKeybindString(KeybindSystem.EnableDisableBuffBind))
            : Language.GetTextValue("Mods.HyperResearch.Tooltips.EnableBanner",
                InputUtils.GetKeybindString(KeybindSystem.EnableDisableBuffBind));
        TooltipLine researched = new(Mod, name, text)
        {
            OverrideColor = enabled ? Colors.RarityGreen : Colors.RarityRed
        };
        tooltips.Add(researched);
    }
}