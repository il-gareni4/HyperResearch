using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.Config;

namespace HyperResearch.Common.Configs;

public class ServerConfig : ModConfig
{
    [DefaultValue(false)]
    public bool SyncResearchedItemsInOneTeam;

    [DefaultValue(false)]
    public bool SyncSacrificesInOneTeam;


    [Header("HostSettings")]


    [DefaultValue(false)]
    public bool UseServerSettings;


    [Header("$Mods.HyperResearch.Configs.BaseConfig.Headers.BuffsSettingsHeader")]

    [LabelKey("$Mods.HyperResearch.Configs.BaseConfig.UseResearchedBannersBuff.Label")]
    [TooltipKey("$Mods.HyperResearch.Configs.BaseConfig.UseResearchedBannersBuff.Tooltip")]
    [LabelArgs(ItemID.ZombieBanner)]
    [DefaultValue(true)]
    public bool UseResearchedBannersBuff;

    [LabelKey("$Mods.HyperResearch.Configs.BaseConfig.UseResearchedPotionsBuff.Label")]
    [TooltipKey("$Mods.HyperResearch.Configs.BaseConfig.UseResearchedPotionsBuff.Tooltip")]
    [LabelArgs(ItemID.WrathPotion)]
    [DefaultValue(true)]
    public bool UseResearchedPotionsBuff;


    [Header("$Mods.HyperResearch.Configs.BaseConfig.Headers.BalanceSettingsHeader")]


    [LabelKey("$Mods.HyperResearch.Configs.BaseConfig.IgnoreCraftingConditions.Label")]
    [TooltipKey("$Mods.HyperResearch.Configs.BaseConfig.IgnoreCraftingConditions.Tooltip")]
    [LabelArgs(ItemID.BottledWater)]
    [DefaultValue(true)]
    public bool IgnoreCraftingConditions;

    [LabelKey("$Mods.HyperResearch.Configs.BaseConfig.BalanceShimmerAutoresearch.Label")]
    [TooltipKey("$Mods.HyperResearch.Configs.BaseConfig.BalanceShimmerAutoresearch.Tooltip")]
    [LabelArgs(ItemID.Shimmerfly)]
    [DefaultValue(true)]
    public bool BalanceShimmerAutoresearch;

    [LabelKey("$Mods.HyperResearch.Configs.BaseConfig.BalancePrefixPicker.Label")]
    [TooltipKey("$Mods.HyperResearch.Configs.BaseConfig.BalancePrefixPicker.Tooltip")]
    [LabelArgs(ItemID.TinkerersWorkshop)]
    [DefaultValue(false)]
    public bool BalancePrefixPicker;

    [LabelKey("$Mods.HyperResearch.Configs.BaseConfig.OnlyOneItemNeeded.Label")]
    [TooltipKey("$Mods.HyperResearch.Configs.BaseConfig.OnlyOneItemNeeded.Tooltip")]
    [LabelArgs(ItemID.AlphabetStatue1)]
    [DefaultValue(false)]
    public bool OnlyOneItemNeeded;

    [LabelKey("$Mods.HyperResearch.Configs.BaseConfig.ItemResearchCountOverride.Label")]
    [TooltipKey("$Mods.HyperResearch.Configs.BaseConfig.ItemResearchCountOverride.Tooltip")]
    [ReloadRequired]
    public Dictionary<ItemDefinition, uint> ItemResearchCountOverride = [];

    public static ServerConfig Instance { get; private set; }

    public override ConfigScope Mode => ConfigScope.ServerSide;

    public override void OnLoaded() => Instance = this;

    public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref NetworkText message)
    {
        if (whoAmI == 0) return true;

        message = NetworkText.FromKey("Mods.HyperResearch.Configs.ServerConfig.Messages.OnlyHost");
        return false;
    }
}