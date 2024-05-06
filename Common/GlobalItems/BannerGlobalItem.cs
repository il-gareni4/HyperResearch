using HyperResearch.Common.ModPlayers;
using HyperResearch.Common.Systems;
using HyperResearch.Utils;
using System.Collections.Generic;
using Terraria.Localization;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.ID;

namespace HyperResearch.Common.GlobalItems;

public class BannerGlobalItem : GlobalItem
{
    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (!Researcher.IsPlayerInJourneyMode
            || item.tooltipContext != ItemSlot.Context.CreativeInfinite
            || !BannerSystem.ItemToBanner.TryGetValue(item.type, out int bannerId) 
            || !Main.LocalPlayer.TryGetModPlayer(out BannerPlayer bannerPlayer)
            || !bannerPlayer.ResearchedBanners.TryGetValue(bannerId, out bool enabled)) return;

        string name = enabled ? "DisableBanner" : "EnableBanner";
        string text = enabled
            ? Language.GetTextValue("Mods.HyperResearch.Tooltips.DisableBanner", InputUtils.GetKeybindString(KeybindSystem.EnableDisableBuffBind))
            : Language.GetTextValue("Mods.HyperResearch.Tooltips.EnableBanner", InputUtils.GetKeybindString(KeybindSystem.EnableDisableBuffBind));
        TooltipLine researched = new(Mod, name, text)
        {
            OverrideColor = enabled ? Colors.RarityGreen : Colors.RarityRed,
        };
        tooltips.Add(researched);
    }
}
