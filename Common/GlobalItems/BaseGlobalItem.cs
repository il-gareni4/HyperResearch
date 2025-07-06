using System.Collections.Generic;
using System.Text.RegularExpressions;
using HyperResearch.Common.Configs;
using HyperResearch.Common.ModPlayers.Interfaces;
using HyperResearch.Utils;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace HyperResearch.Common.GlobalItems;

public partial class BaseGlobalItem : GlobalItem
{
    public override bool CanConsumeAmmo(Item weapon, Item ammo, Player player)
    {
        if (!Researcher.IsPlayerInJourneyMode) return base.CanConsumeAmmo(weapon, ammo, player);
        return BaseConfig.Instance.ConsumeResearchedAmmo || !Researcher.IsResearched(ammo.type);
    }

    public override bool? CanConsumeBait(Player player, Item bait)
    {
        if (!Researcher.IsPlayerInJourneyMode) return base.CanConsumeBait(player, bait);
        return BaseConfig.Instance.ConsumeResearchedBaits || !Researcher.IsResearched(bait.type);
    }

    public override bool ConsumeItem(Item item, Player player)
    {
        if (!Researcher.IsPlayerInJourneyMode) return base.ConsumeItem(item, player);
        if (Researcher.IsResearched(item.type))
        {
            if (item.createTile >= TileID.Dirt || item.createWall > WallID.None)
                return BaseConfig.Instance.ConsumeResearchedBlocks;
            if (item.shoot > ProjectileID.None)
                return BaseConfig.Instance.ConsumeResearchedThrowingWeapons;
            if (item.potion || item.healMana > 0 || item.buffType > 0)
                return BaseConfig.Instance.ConsumeResearchedPotions;
            if (ItemsUtils.IsLootItem(item.type))
                return BaseConfig.Instance.ConsumeResearchedLootItems;
            if (!BaseConfig.Instance.ConsumeOtherResearchedItems)
                return false;
        }

        return base.ConsumeItem(item, player);
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (!Researcher.IsPlayerInJourneyMode || !Researcher.IsResearchable(item.type)) return;
        if (Researcher.IsResearched(item.type)
            && VisualConfig.Instance.ShowResearchedTooltip
            && item.tooltipContext != ItemSlot.Context.CreativeInfinite)
        {
            TooltipLine researched =
                new(Mod, "Researched", Language.GetTextValue("Mods.HyperResearch.Tooltips.Researched"))
                {
                    OverrideColor = Colors.FancyUIFatButtonMouseOver
                };
            tooltips.Add(researched);
        }
        else if (!Researcher.IsResearched(item.type))
        {
            int vanillaTooltipIndex = tooltips.FindIndex(tooltip => tooltip.Name == "JourneyResearch");
            if (!VisualConfig.Instance.UseCustomResearchTooltip)
            {
                if (!ConfigOptions.OnlyOneItemNeeded || vanillaTooltipIndex == -1) return;

                tooltips[vanillaTooltipIndex].Text = Regex.Replace(
                    tooltips[vanillaTooltipIndex].Text,
                    @"\d+",
                    "1"
                );
            }

            LocalizedText tooltipText = Language.GetText("Mods.HyperResearch.Tooltips.NeededToResearch");
            TooltipLine hyperResearch = new(Mod, "HyperResearch",
                tooltipText.Format(Researcher.GetRemaining(item.type), Researcher.GetResearchedCount(item.type),
                    Researcher.GetTotalNeeded(item.type))
            )
            {
                OverrideColor = Colors.JourneyMode
            };
            if (vanillaTooltipIndex >= 0) tooltips[vanillaTooltipIndex] = hyperResearch;
            else tooltips.Add(hyperResearch);
        }
    }

    public override void OnResearched(Item item, bool fullyResearched)
    {
        if (!Researcher.IsPlayerInJourneyMode || !fullyResearched || item.IsAir) return;

        foreach (ModPlayer modPlayer in Main.LocalPlayer.ModPlayers)
        {
            if (modPlayer is IResearchPlayer resPlayer)
                resPlayer.OnResearch(item);
        }
    }
}
