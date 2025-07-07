using System;
using System.ComponentModel;
using HyperResearch.Common.Configs.Enums;
using Terraria.ID;
using Terraria.ModLoader.Config;

namespace HyperResearch.Common.Configs;

public class BaseConfig : ModConfig
{
    [Header("AutoResearchSettingsHeader")]

    [LabelArgs(ItemID.YoyoBag)]
    [DefaultValue(ResearchMode.AutoResearch)]
    public ResearchMode ResearchMode;

    [LabelArgs(ItemID.WorkBench)]
    [DefaultValue(CraftablesResearchMode.OnResearch)]
    public CraftablesResearchMode CraftablesResearchMode;

    [LabelArgs(ItemID.BottomlessShimmerBucket)]
    [DefaultValue(ShimmerResearchMode.OnResearch)]
    public ShimmerResearchMode ShimmerResearchMode;

    [LabelArgs(ItemID.IronOre)]
    [DefaultValue(DecraftsResearchMode.OnResearch)]
    public DecraftsResearchMode DecraftsResearchMode;

    [LabelArgs(ItemID.PlatinumCoin)]
    [TooltipArgs(ItemID.DefenderMedal)]
    [DefaultValue(ShopResearchMode.OnShopOpen)]
    public ShopResearchMode ShopResearchMode;


    [Header("AutoTrashSettingsHeader")]

    [LabelArgs(ItemID.TrashCan)]
    [DefaultValue(false)]
    public bool AutoTrashResearched;

    [LabelArgs(ItemID.TrashCan)]
    [DefaultValue(false)]
    public bool AutoTrashAfterResearching;


    [Header("BuffsSettingsHeader")]

    [LabelArgs(ItemID.ZombieBanner)]
    [DefaultValue(true)]
    public bool UseResearchedBannersBuff;

    [DefaultValue(true)]
    public bool BannerBuffEnabledByDefault;

    [LabelArgs(ItemID.WrathPotion)]
    [DefaultValue(true)]
    public bool UseResearchedPotionsBuff;

    [DefaultValue(true)]
    public bool PotionBuffEnabledByDefault;


    [Header("BalanceSettingsHeader")]

    [LabelArgs(ItemID.BottledWater)]
    [DefaultValue(true)]
    public bool IgnoreCraftingConditions;

    [LabelArgs(ItemID.BottomlessShimmerBucket)]
    [DefaultValue(true)]
    public bool BalanceShimmerAutoresearch;

    [LabelArgs(ItemID.TinkerersWorkshop)]
    [DefaultValue(false)]
    public bool BalancePrefixPicker;


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


    public static BaseConfig Instance { get; private set; }

    public override ConfigScope Mode => ConfigScope.ClientSide;
    public static event Action Changed;

    public override void OnLoaded() => Instance = this;

    public override void OnChanged() => Changed?.Invoke();
}