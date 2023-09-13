using System;
using System.ComponentModel;
using Terraria.ID;
using Terraria.ModLoader.Config;

namespace HyperResearch
{
    public class HyperConfig : ModConfig
    {
        public static event Action Changed;

        public static HyperConfig Instance;

        public override ConfigScope Mode => ConfigScope.ClientSide;

        public override void OnLoaded() => Instance = this;

        public override void OnChanged() => Changed?.Invoke();

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

        [LabelArgs(ItemID.HandOfCreation)]
        [DefaultValue(true)]
        public bool UseCustomResearchTooltip;

        [LabelArgs(ItemID.BottomlessShimmerBucket)]
        [DefaultValue(true)]
        public bool AutoResearchShimmeredItems;

        [Header("OtherSettingsHeader")]

        [LabelArgs(ItemID.ZombieBanner)]
        [DefaultValue(false)]
        public bool UseResearchedBannersBuff;

        [LabelArgs(ItemID.AlphabetStatue1)]
        [DefaultValue(false)]
        public bool OnlyOneItemNeeded;

        [LabelArgs(ItemID.TrashCan)]
        [DefaultValue(false)]
        public bool AutoTrashAfterResearching;

        [Header("SacrificeSettingsHeader")]

        [LabelArgs(ItemID.LockBox)]
        [DefaultValue(false)]
        public bool SacrificeHotbarSlots;

        [LabelArgs(ItemID.GoldCoin)]
        [DefaultValue(true)]
        public bool SacrificeCoinsSlots;

        [LabelArgs(ItemID.WoodenArrow)]
        [DefaultValue(true)]
        public bool SacrificeAmmoSlots;

        [Header("ClearSettingsHeader")]

        [LabelArgs(ItemID.LockBox)]
        [DefaultValue(false)]
        public bool ClearHotbarSlots;

        [LabelArgs(ItemID.GoldCoin)]
        [DefaultValue(false)]
        public bool ClearCoinsSlots;

        [LabelArgs(ItemID.WoodenArrow)]
        [DefaultValue(true)]
        public bool ClearAmmoSlots;

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

        [Header("MessagesSettingsHeader")]

        [DefaultValue(true)]
        public bool ShowNewlyResearchedItems;

        [DefaultValue(true)]
        public bool ShowResearchedCraftableItems;

        [DefaultValue(true)]
        public bool ShowResearchedShimmeredItems;

        [DefaultValue(false)]
        public bool ShowPartiallyResearchedItems;

        [Header("UISettingsHeader")]

        [DefaultValue(true)]
        public bool ShowTotalResearchedItemsCount;
    }
}
