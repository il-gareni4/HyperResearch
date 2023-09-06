using BetterResearch.Utils;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BetterResearch.Common
{
    public class BRGlobalItem : GlobalItem
    {
        public override bool OnPickup(Item item, Player player)
        {
            BRConfig config = ModContent.GetInstance<BRConfig>();
            return !(ResearchUtils.IsResearched(item.type) && config.AutoTrashResearched);
        }

        public override bool CanConsumeAmmo(Item weapon, Item ammo, Player player)
        {
            return ModContent.GetInstance<BRConfig>().ConsumeResearchedAmmo || !ResearchUtils.IsResearched(ammo.type);
        }

        public override bool? CanConsumeBait(Player player, Item bait)
        {
            return ModContent.GetInstance<BRConfig>().ConsumeResearchedBaits || !ResearchUtils.IsResearched(bait.type);
        }

        public override bool ConsumeItem(Item item, Player player)
        {
            BRConfig config = ModContent.GetInstance<BRConfig>();
            if (ResearchUtils.IsResearched(item.type))
            {
                if (item.createTile >= 0 || item.createWall >= 0) return config.ConsumeResearchedBlocks;
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
