using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;

namespace HyperResearch.Utils
{
    /// <summary>A utility class whose functions are directly related to items in the game</summary>
    /// <seealso cref="Item"/>
    public static class ItemsUtils
    {
        /// <summary>
        /// More readable variant of <code>Main.ItemDropsDB.GetRulesForItemID(itemId).Count == 0</code>
        /// </summary>
        public static bool IsLootItem(int itemId)
        {
            List<IItemDropRule> rules = Main.ItemDropsDB.GetRulesForItemID(itemId);
            return rules.Count > 0;
        }

        public static IEnumerable<int> GetItemLoot(int itemId)
        {
            List<IItemDropRule> itemDropRules = Main.ItemDropsDB.GetRulesForItemID(itemId);
            List<DropRateInfo> dropRateInfos = new();
            DropRateInfoChainFeed dropRateInfo = new(1f);
            foreach (IItemDropRule item in itemDropRules) item.ReportDroprates(dropRateInfos, dropRateInfo);

            DropAttemptInfo attemptInfo = default;
            attemptInfo.player = Main.LocalPlayer;
            attemptInfo.item = itemId;
            attemptInfo.IsExpertMode = Main.expertMode;
            attemptInfo.IsMasterMode = Main.masterMode;
            attemptInfo.rng = Main.rand;

            return dropRateInfos.Where(info => info.conditions?.All(c => c.CanDrop(attemptInfo)) ?? true)
                                .Select(info =>  info.itemId);
        }

        public static int GetShimmeredItemId(int itemId)
        {
            if (ItemID.Sets.ShimmerCountsAsItem[itemId] != -1)
                itemId = ItemID.Sets.ShimmerCountsAsItem[itemId];
            return ItemID.Sets.ShimmerTransformToItem[itemId];
        }
    }
}