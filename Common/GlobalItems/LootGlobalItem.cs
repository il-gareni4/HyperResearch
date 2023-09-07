using HyperResearch.Utils;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HyperResearch.Common.GlobalItems
{
    class LootGlobalItem : GlobalItem
    {
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (Main.GameMode != 3) return;
            if (!ResearchUtils.IsResearched(item.type) ||
                !ItemLoader.CanRightClick(item) ||
                !ItemsUtils.IsLootItem(item.type)) return;

            string keybindStr = InputUtils.GetKeybindString(HyperResearch.ResearchLootBind);
            TooltipLine tooltipLine = new(Mod, "ResearchLoot", Language.GetText("Mods.HyperResearch.Tooltips.ResearchLoot").Format(keybindStr))
            {
                OverrideColor = Terraria.ID.Colors.JourneyMode
            };
            tooltips.Add(tooltipLine);
        }
    }
}