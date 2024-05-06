using Humanizer;
using HyperResearch.Common.ModPlayers;
using HyperResearch.Common.Systems;
using HyperResearch.Utils;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace HyperResearch.Common.GlobalItems;

public class BuffGlobalItem : GlobalItem
{
    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (!Researcher.IsPlayerInJourneyMode
            || item.tooltipContext != ItemSlot.Context.CreativeInfinite
            || !ConfigOptions.UseResearchedPotionsBuff
            || !BuffUtils.IsBuffPotion(item)
            || !Main.LocalPlayer.TryGetModPlayer(out BuffPlayer buffPlayer)
            || !buffPlayer.Buffs.TryGetValue(item.buffType, out bool enabled)) return;

        string name = enabled ? "DisableBuff" : "EnableBuff";
        string text = enabled
            ? Language.GetTextValue("Mods.HyperResearch.Tooltips.DisableBuff", InputUtils.GetKeybindString(KeybindSystem.EnableDisableBuffBind))
            : Language.GetTextValue("Mods.HyperResearch.Tooltips.EnableBuff", InputUtils.GetKeybindString(KeybindSystem.EnableDisableBuffBind));
        TooltipLine researched = new(Mod, name, text)
        {
            OverrideColor = enabled ? Colors.RarityGreen : Colors.RarityRed,
        };
        tooltips.Add(researched);
    }
}
