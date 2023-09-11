using HyperResearch.Utils;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HyperResearch.Common.GlobalItems
{
    public class LootGlobalItem : GlobalItem
    {
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (!Researcher.IsPlayerInJourneyMode() ||
                !Researcher.IsResearched(item.type) ||
                !ItemLoader.CanRightClick(item) ||
                !ItemsUtils.IsLootItem(item.type)) return;

            if (ItemsUtils.GetItemLoot(item.type).All(Researcher.IsResearched)) return;

            string keybindStr = InputUtils.GetKeybindString(HyperResearch.ResearchLootBind);
            TooltipLine tooltipLine = new(Mod, "ResearchLoot", Language.GetText("Mods.HyperResearch.Tooltips.ResearchLoot").Format(keybindStr))
            {
                OverrideColor = Terraria.ID.Colors.JourneyMode
            };
            tooltips.Add(tooltipLine);
        }
    }
}