using HyperResearch.Common.Systems;
using HyperResearch.Utils;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace HyperResearch.Common.GlobalItems
{
    public class BaseGlobalItem : GlobalItem
    {
        public override bool OnPickup(Item item, Player player)
        {
            if (!Researcher.IsPlayerInJourneyMode()) return base.OnPickup(item, player);
            return !(Researcher.IsResearched(item.type) && HyperConfig.Instance.AutoTrashResearched);
        }

        public override bool CanConsumeAmmo(Item weapon, Item ammo, Player player)
        {
            if (!Researcher.IsPlayerInJourneyMode()) return base.CanConsumeAmmo(weapon, ammo, player);
            return HyperConfig.Instance.ConsumeResearchedAmmo || !Researcher.IsResearched(ammo.type);
        }

        public override bool? CanConsumeBait(Player player, Item bait)
        {
            if (!Researcher.IsPlayerInJourneyMode()) return base.CanConsumeBait(player, bait);
            return HyperConfig.Instance.ConsumeResearchedBaits || !Researcher.IsResearched(bait.type);
        }

        public override bool ConsumeItem(Item item, Player player)
        {
            if (!Researcher.IsPlayerInJourneyMode()) return base.ConsumeItem(item, player);
            if (Researcher.IsResearched(item.type))
            {
                if (item.createTile >= TileID.Dirt || item.createWall > WallID.None)
                    return HyperConfig.Instance.ConsumeResearchedBlocks;
                else if (item.shoot > ProjectileID.None)
                    return HyperConfig.Instance.ConsumeResearchedThrowingWeapons;
                else if (item.potion || item.healMana > 0 || item.buffType > 0)
                    return HyperConfig.Instance.ConsumeResearchedPotions;
                else if (ItemsUtils.IsLootItem(item.type))
                    return HyperConfig.Instance.ConsumeResearchedLootItems;
                else if (!HyperConfig.Instance.ConsumeOtherResearchedItems)
                    return false;
            }
            return base.ConsumeItem(item, player);
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (!Researcher.IsPlayerInJourneyMode() || !Researcher.IsResearchable(item.type)) return;
            if (Researcher.IsResearched(item.type) && HyperConfig.Instance.ShowResearchedTooltip && item.tooltipContext != ItemSlot.Context.CreativeInfinite)
            {
                TooltipLine researched = new(Mod, "Researched", "Researched")
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
                    if (HyperConfig.Instance.OnlyOneItemNeeded && vanillaTooltipIndex >= 0)
                        tooltips[vanillaTooltipIndex].Text = Regex.Replace(tooltips[vanillaTooltipIndex].Text, @"\d+", "1");
                    return;
                }

                LocalizedText tooltipText = Language.GetText("Mods.HyperResearch.Tooltips.NeededToResearch");
                TooltipLine hyperResearch = new(Mod, "HyperResearch",
                    tooltipText.Format(Researcher.GetRemaining(item.type), Researcher.GetResearchedCount(item.type), Researcher.GetTotalNeeded(item.type)))
                {
                    OverrideColor = Colors.JourneyMode
                };
                if (vanillaTooltipIndex >= 0) tooltips[vanillaTooltipIndex] = hyperResearch;
                else tooltips.Add(hyperResearch);
            }
        }

        public override void OnResearched(Item item, bool fullyResearched)
        {
            if (!Researcher.IsPlayerInJourneyMode() || !fullyResearched) return;

            ItemLoader.OnPickup(item, Main.LocalPlayer); // For ItemChecklist and BossChecklist
            if (Main.LocalPlayer.TryGetModPlayer(out HyperPlayer modPlayer))
            {
                modPlayer.TryAddToResearchedTiles(item.type);
                modPlayer.ItemsResearchedCount++;
                if (BannerSystem.ItemToBanner.TryGetValue(item.type, out var bannerId))
                    modPlayer.ResearchedBanners.Add(bannerId);
            }
        }
    }
}
