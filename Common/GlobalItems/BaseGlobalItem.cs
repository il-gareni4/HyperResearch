using HyperResearch.Utils;
using Terraria;
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
            return !(ResearchUtils.IsResearched(item.type) && config.AutoTrashResearched);
        }

        public override bool CanConsumeAmmo(Item weapon, Item ammo, Player player)
        {
            if (Main.GameMode != 3) return base.CanConsumeAmmo(weapon, ammo, player);
            return ModContent.GetInstance<HyperConfig>().ConsumeResearchedAmmo || !ResearchUtils.IsResearched(ammo.type);
        }

        public override bool? CanConsumeBait(Player player, Item bait)
        {
            if (Main.GameMode != 3) return base.CanConsumeBait(player, bait);
            return ModContent.GetInstance<HyperConfig>().ConsumeResearchedBaits || !ResearchUtils.IsResearched(bait.type);
        }

        public override bool ConsumeItem(Item item, Player player)
        {
            if (Main.GameMode != 3) return base.ConsumeItem(item, player);
            HyperConfig config = ModContent.GetInstance<HyperConfig>();
            if (ResearchUtils.IsResearched(item.type))
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
    }
}
