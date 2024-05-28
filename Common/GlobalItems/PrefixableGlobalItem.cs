using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HyperResearch.Common.Configs;
using HyperResearch.UI;
using HyperResearch.Utils;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace HyperResearch.Common.GlobalItems;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class PrefixableGlobalItem : GlobalItem
{
    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (!ItemsUtils.CanHavePrefixes(item)
            || !HyperConfig.Instance.ShowSelectPrefixTooltip
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