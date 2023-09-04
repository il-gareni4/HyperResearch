using BetterResearch.Utils;
using Terraria;
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
    }
}
