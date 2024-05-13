using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

[SuppressMessage("ReSharper", "UnusedType.Global")]
public partial class BaseGlobalItem : GlobalItem
{
    public override bool OnPickup(Item item, Player player)
    {
        if (!Researcher.IsPlayerInJourneyMode) return base.OnPickup(item, player);
        return !(Researcher.IsResearched(item.type) && HyperConfig.Instance.AutoTrashResearched);
    }

    public override bool CanConsumeAmmo(Item weapon, Item ammo, Player player)
    {
        if (!Researcher.IsPlayerInJourneyMode) return base.CanConsumeAmmo(weapon, ammo, player);
        return HyperConfig.Instance.ConsumeResearchedAmmo || !Researcher.IsResearched(ammo.type);
    }

    public override bool? CanConsumeBait(Player player, Item bait)
    {
        if (!Researcher.IsPlayerInJourneyMode) return base.CanConsumeBait(player, bait);
        return HyperConfig.Instance.ConsumeResearchedBaits || !Researcher.IsResearched(bait.type);
    }

    public override bool ConsumeItem(Item item, Player player)
    {
        if (!Researcher.IsPlayerInJourneyMode) return base.ConsumeItem(item, player);
        if (Researcher.IsResearched(item.type))
        {
            if (item.createTile >= TileID.Dirt || item.createWall > WallID.None)
                return HyperConfig.Instance.ConsumeResearchedBlocks;
            if (item.shoot > ProjectileID.None)
                return HyperConfig.Instance.ConsumeResearchedThrowingWeapons;
            if (item.potion || item.healMana > 0 || item.buffType > 0)
                return HyperConfig.Instance.ConsumeResearchedPotions;
            if (ItemsUtils.IsLootItem(item.type))
                return HyperConfig.Instance.ConsumeResearchedLootItems;
            if (!HyperConfig.Instance.ConsumeOtherResearchedItems)
                return false;
        }

        return base.ConsumeItem(item, player);
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (!Researcher.IsPlayerInJourneyMode || !Researcher.IsResearchable(item.type)) return;
        if (Researcher.IsResearched(item.type) && HyperConfig.Instance.ShowResearchedTooltip &&
            item.tooltipContext != ItemSlot.Context.CreativeInfinite)
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
            if (!HyperConfig.Instance.UseCustomResearchTooltip)
            {
                if (ConfigOptions.OnlyOneItemNeeded && vanillaTooltipIndex >= 0)
                    tooltips[vanillaTooltipIndex].Text = DigitsRegex().Replace(tooltips[vanillaTooltipIndex].Text, "1");
                return;
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

        ItemLoader.OnPickup(item, Main.LocalPlayer); // For ItemChecklist and BossChecklist
        foreach (ModPlayer modPlayer in Main.LocalPlayer.ModPlayers)
        {
            if (modPlayer is IResearchPlayer resPlayer)
                resPlayer.OnResearch(item);
        }
    }

    [GeneratedRegex(@"\d+")]
    private static partial Regex DigitsRegex();
}