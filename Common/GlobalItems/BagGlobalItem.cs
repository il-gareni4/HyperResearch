using System.Collections.Generic;
using BetterResearch.Utils;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BetterResearch.Common.GlobalItems
{
    class BagGlobalItem : GlobalItem
    {
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (!ResearchUtils.IsResearched(item.type) ||
                !ItemLoader.CanRightClick(item) ||
                Main.ItemDropsDB.GetRulesForItemID(item.type).Count == 0) return;

            string keybindStr = InputUtils.GetKeybindString(BetterResearch.ResearchLootBind);
            TooltipLine tooltipLine = new(Mod, "ResearchLoot", Language.GetText("Mods.BetterResearch.Tooltips.ResearchLoot").Format(keybindStr)) 
            {
                OverrideColor = Terraria.ID.Colors.JourneyMode
            };
            tooltips.Add(tooltipLine);
        }
    }
}