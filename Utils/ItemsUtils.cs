using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.ItemDropRules;

namespace BetterResearch.Utils
{
    public static class ItemsUtils
    {
        /// <summary>
        /// More readable variant of <code>Main.ItemDropsDB.GetRulesForItemID(itemId).Count == 0</code>
        /// </summary>
        public static bool IsLootItem(int itemId)
        {
            List<IItemDropRule> rules = Main.ItemDropsDB.GetRulesForItemID(itemId);
            return rules.Count >= 0;
        }

        public static IEnumerable<int> GetItemLoot(int itemId)
        {
            List<IItemDropRule> itemDropRules = Main.ItemDropsDB.GetRulesForItemID(itemId);
            List<DropRateInfo> dropRateInfos = new();
            DropRateInfoChainFeed dropRateInfo = new(1f);
            foreach (IItemDropRule item in itemDropRules) item.ReportDroprates(dropRateInfos, dropRateInfo);
            return dropRateInfos.Select(info => info.itemId);
        }
    }
}