using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace HyperResearch.Common.GlobalItems;

public class PrefixableGlobalItem : GlobalItem
{
    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (!item.CanHavePrefixes() || item.tooltipContext != ItemSlot.Context.CreativeInfinite)
            return;

        TooltipLine tooltipLine = new(Mod, "ChoosePrefix", Language.GetText("Mods.HyperResearch.Tooltips.ChoosePrefix").Format("Mouse3"))
        {
            OverrideColor = Terraria.ID.Colors.CoinSilver
        };
        tooltips.Add(tooltipLine);
    }
}
