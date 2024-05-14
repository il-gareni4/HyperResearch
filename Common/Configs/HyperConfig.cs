using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Terraria.ID;
using Terraria.ModLoader.Config;

namespace HyperResearch.Common.Configs;

[SuppressMessage("ReSharper", "UnassignedField.Global")]
[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
public class HyperConfig : ModConfig
{
    [Header("AutoResearchSettingsHeader")]

    [LabelArgs(ItemID.YoyoBag)]
    [DefaultValue(true)]
    public bool ResearchInventory;

    [LabelArgs(ItemID.Toolbelt)]
    [DefaultValue(false)]
    public bool AutoSacrifice;

    [LabelArgs(ItemID.WorkBench)]
    [DefaultValue(true)]
    public bool AutoResearchCraftableItems;

    [LabelArgs(ItemID.BottomlessShimmerBucket)]
    [DefaultValue(true)]
    public bool ResearchShimmerableItems;

    [LabelArgs(ItemID.IronOre)]
    [DefaultValue(true)]
    public bool ResearchDecraftItems;

    [LabelArgs(ItemID.PlatinumCoin)]
    [TooltipArgs(ItemID.DefenderMedal)]
    [DefaultValue(true)]
    public bool AutoResearchShop;


    [Header("BuffsSettingsHeader")]

    [LabelArgs(ItemID.ZombieBanner)]
    [DefaultValue(true)]
    public bool UseResearchedBannersBuff;

    [LabelArgs(ItemID.WrathPotion)]
    [DefaultValue(true)]
    public bool UseResearchedPotionsBuff;


    [Header("AutoTrashSettingsHeader")]

    [LabelArgs(ItemID.TrashCan)]
    [DefaultValue(false)]
    public bool AutoTrashResearched;

    [LabelArgs(ItemID.TrashCan)]
    [DefaultValue(false)]
    public bool AutoTrashAfterResearching;


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

    [LabelArgs(ItemID.AlphabetStatue1)]
    [DefaultValue(false)]
    public bool OnlyOneItemNeeded;

    public Dictionary<ItemDefinition, uint> ItemResearchCountOverride = [];


    [Header("TooltipsSettingsHeader")]

    [LabelArgs(ItemID.HandOfCreation)]
    [DefaultValue(true)]
    public bool UseCustomResearchTooltip;

    [LabelArgs(ItemID.Sign)]
    [DefaultValue(false)]
    public bool ShowResearchedTooltip;


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

    [DefaultValue(true)]
    public bool ShowResearchedDecraftItems;

    [DefaultValue(false)]
    public bool ShowSacrifices;


    [Header("MultiplayerMessagesSettingsHeader")]

    [DefaultValue(true)]
    public bool ShowSharedItems;

    [DefaultValue(true)]
    public bool ShowSharedSacrifices;

    [DefaultValue(false)]
    public bool ShowOtherPlayersResearchedItems;


    [Header("UISettingsHeader")]

    [DefaultValue(true)]
    public bool ShowResearchInventoryButton;

    [DefaultValue(true)]
    public bool ShowClearInventoryButton;

    [DefaultValue(true)]
    public bool ShowAutoCraftButton;

    [Range(2, 9)]
    [Slider()]
    [DefaultValue(2)]
    public int InventoryButtonsSlotOffset;

    [DefaultValue(true)]
    public bool ShowResearchShopButton;

    [DefaultValue(true)]
    public bool ShowTotalResearchedItemsCount;
        

    public static HyperConfig Instance { get; private set; } = null!;

    public override ConfigScope Mode => ConfigScope.ClientSide;
    public static event Action? Changed;

    public override void OnLoaded() => Instance = this;

    public override void OnChanged() => Changed?.Invoke();
}