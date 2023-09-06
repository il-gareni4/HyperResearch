using System.ComponentModel;
using BetterResearch.Utils;
using Terraria.ID;
using Terraria.ModLoader.Config;

namespace BetterResearch
{
    public class BRConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [LabelArgs(ItemID.YoyoBag)]
        [DefaultValue(true)]
        public bool ResearchInventory;

        [LabelArgs(ItemID.TrashCan)]
        [DefaultValue(false)]
        public bool AutoTrashResearched;

        [LabelArgs(ItemID.WorkBench)]
        [DefaultValue(true)]
        public bool AutoResearchCraftable;

        [LabelArgs(ItemID.BottledWater)]
        [DefaultValue(true)]
        public bool IgnoreCraftingConditions;

        [Header("ClearSettingsHeader")]

        [LabelArgs(ItemID.GoldCoin)]
        [DefaultValue(false)]
        public bool ClearCoins;

        [LabelArgs(ItemID.WoodenArrow)]
        [DefaultValue(true)]
        public bool ClearAmmo;

        [Header("MessagesSettingsHeader")]

        [DefaultValue(true)]
        public bool ShowNewlyResearchedItems;

        [DefaultValue(true)]
        public bool ShowResearchedCraftableItems;
    }
}
