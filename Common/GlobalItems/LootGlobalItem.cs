using System.Collections.Generic;
using System.Linq;
using HyperResearch.Common.Configs;
using HyperResearch.Common.Systems;
using HyperResearch.Utils;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HyperResearch.Common.GlobalItems;

public class LootGlobalItem : GlobalItem
{
    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (!Researcher.IsPlayerInJourneyMode ||
            !VisualConfig.Instance.ShowResearchBagTooltip ||
            !Researcher.IsResearched(item.type) ||
            !ItemLoader.CanRightClick(item) ||
            !ItemsUtils.IsLootItem(item.type) ||
            !ItemsUtils.CanOpenLootItem(item.type)) return;

        if (ItemsUtils.GetItemLoot(item.type).All(Researcher.IsResearched)) return;

        string keybindStr = InputUtils.GetKeybindString(KeybindSystem.ResearchLootBind);
        TooltipLine tooltipLine = new(
            Mod,
            "ResearchLoot",
            Language.GetText("Mods.HyperResearch.Tooltips.ResearchLoot").Format(keybindStr)
        )
        {
            OverrideColor = Colors.JourneyMode
        };
        tooltips.Add(tooltipLine);
    }
}