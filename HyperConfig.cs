using System.ComponentModel;
using Terraria.ID;
using Terraria.ModLoader.Config;

namespace HyperResearch
{
    public class HyperConfig : ModConfig
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

        [LabelArgs(ItemID.PlatinumCoin)]
        [TooltipArgs(ItemID.DefenderMedal)]
        [DefaultValue(true)]
        public bool AutoResearchShop;

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

        [Header("ConsumptionSettingsHeader")]

        [LabelArgs(ItemID.EndlessMusketPouch)]
        [DefaultValue(false)]
        public bool ConsumeResearchedAmmo;

        [LabelArgs(ItemID.GoldWorm)]
        [DefaultValue(false)]
        public bool ConsumeResearchedBaits;

        [LabelArgs(ItemID.StoneSlab)]
        [DefaultValue(false)]
        public bool ConsumeResearchedBlocks;

        [LabelArgs(ItemID.Shuriken)]
        [DefaultValue(false)]
        public bool ConsumeResearchedThrowingWeapons;

        [LabelArgs(ItemID.RegenerationPotion)]
        [TooltipArgs(ItemID.IronskinPotion, ItemID.HealingPotion, ItemID.ManaPotion, ItemID.RecallPotion)]
        [DefaultValue(false)]
        public bool ConsumeResearchedPotions;

        [LabelArgs(ItemID.EyeOfCthulhuBossBag)]
        [DefaultValue(false)]
        public bool ConsumeResearchedLootItems;

        [LabelArgs(ItemID.SuspiciousLookingEye)]
        [DefaultValue(true)]
        public bool ConsumeOtherResearchedItems;
    }
}
