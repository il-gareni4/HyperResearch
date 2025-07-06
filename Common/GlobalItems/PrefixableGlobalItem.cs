using System.Collections.Generic;
using HyperResearch.Common.Configs;
using HyperResearch.UI;
using HyperResearch.Utils;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace HyperResearch.Common.GlobalItems;

public class PrefixableGlobalItem : GlobalItem
{
    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (!ItemsUtils.CanHavePrefixes(item)
            || !VisualConfig.Instance.ShowSelectPrefixTooltip
            || item.tooltipContext != ItemSlot.Context.CreativeInfinite
            || !PrefixWindow.CanBeShown)
            return;

        TooltipLine tooltipLine = new(
            Mod,
            "ChoosePrefix",
            Language.GetText("Mods.HyperResearch.Tooltips.ChoosePrefix").Format("Mouse3")
        )
        {
            OverrideColor = Colors.CoinSilver
        };
        tooltips.Add(tooltipLine);
    }
}