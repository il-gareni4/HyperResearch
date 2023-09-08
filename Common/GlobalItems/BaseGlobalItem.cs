using HyperResearch.Utils;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace HyperResearch.Common
{
    public class BaseGlobalItem : GlobalItem
    {
        public override bool OnPickup(Item item, Player player)
        {
            if (Main.GameMode != 3) return base.OnPickup(item, player);
            HyperConfig config = ModContent.GetInstance<HyperConfig>();
            return !(Researcher.IsResearched(item.type) && config.AutoTrashResearched);
        }

        public override bool CanConsumeAmmo(Item weapon, Item ammo, Player player)
        {
            if (Main.GameMode != 3) return base.CanConsumeAmmo(weapon, ammo, player);
            return ModContent.GetInstance<HyperConfig>().ConsumeResearchedAmmo || !Researcher.IsResearched(ammo.type);
        }

        public override bool? CanConsumeBait(Player player, Item bait)
        {
            if (Main.GameMode != 3) return base.CanConsumeBait(player, bait);
            return ModContent.GetInstance<HyperConfig>().ConsumeResearchedBaits || !Researcher.IsResearched(bait.type);
        }

        public override bool ConsumeItem(Item item, Player player)
        {
            if (Main.GameMode != 3) return base.ConsumeItem(item, player);
            HyperConfig config = ModContent.GetInstance<HyperConfig>();
            if (Researcher.IsResearched(item.type))
            {
                if (item.createTile >= TileID.Dirt || item.createWall > WallID.None) return config.ConsumeResearchedBlocks;
                else if (item.shoot > ProjectileID.None) return config.ConsumeResearchedThrowingWeapons;
                else if (item.potion || item.healMana > 0 || item.buffType > 0) return config.ConsumeResearchedPotions;
                else if (ItemsUtils.IsLootItem(item.type)) return config.ConsumeResearchedLootItems;
                else if (!config.ConsumeOtherResearchedItems)
                    return false;
            }
            return base.ConsumeItem(item, player);
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (Main.GameMode != 3 || !ModContent.GetInstance<HyperConfig>().UseCustomResearchTooltip) return;
            if (Researcher.IsResearched(item.type) || !Researcher.IsResearchable(item.type)) return;

            int remaining = (int)CreativeUI.GetSacrificesRemaining(item.type);
            TooltipLine hyperResearch = new(Mod, "HyperResearch", $"Research {remaining} more to unlock duplication")
            {
                OverrideColor = Colors.JourneyMode
            };
            int vanillaTooltipIndex = tooltips.FindIndex(tooltip => tooltip.Name == "JourneyResearch");
            if (vanillaTooltipIndex >= 0) tooltips[vanillaTooltipIndex] = hyperResearch;
            else tooltips.Add(hyperResearch);
        }
    }
}
