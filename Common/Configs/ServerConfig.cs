using System;
using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.Config;

namespace HyperResearch.Common.Configs
{
    public class ServerConfig : ModConfig
    {
        public static event Action Changed;
        public static ServerConfig Instance { get; set; }

        public override ConfigScope Mode => ConfigScope.ServerSide;

        public override void OnLoaded() => Instance = this;
        public override void OnChanged() => Changed?.Invoke();

        public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref NetworkText message)
        {
            if (whoAmI == 0) return true;

            message = NetworkText.FromKey("Mods.HyperResearch.Configs.ServerConfig.Messages.OnlyHost");
            return false;
        }

        [DefaultValue(false)]
        public bool SyncResearchedItemsInOneTeam;

        [DefaultValue(false)]
        public bool SyncSacrificesInOneTeam;


        [Header("HostSettings")]


        [DefaultValue(false)]
        public bool UseServerSettings;


        [Header("$Mods.HyperResearch.Configs.HyperConfig.Headers.AutoResearchSettingsHeader")]

        [LabelKey("$Mods.HyperResearch.Configs.HyperConfig.ResearchDecraftItems.Label")]
        [TooltipKey("$Mods.HyperResearch.Configs.HyperConfig.ResearchDecraftItems.Tooltip")]
        [LabelArgs(ItemID.BottomlessShimmerBucket)]
        [DefaultValue(true)]
        public bool ResearchShimmerableItems;

        [LabelKey("$Mods.HyperResearch.Configs.HyperConfig.ResearchDecraftItems.Label")]
        [TooltipKey("$Mods.HyperResearch.Configs.HyperConfig.ResearchDecraftItems.Tooltip")]
        [LabelArgs(ItemID.WoodenHammer)]
        [DefaultValue(true)]
        public bool ResearchDecraftItems;


        [Header("$Mods.HyperResearch.Configs.HyperConfig.Headers.BuffsSettingsHeader")]

        [LabelKey("$Mods.HyperResearch.Configs.HyperConfig.UseResearchedBannersBuff.Label")]
        [TooltipKey("$Mods.HyperResearch.Configs.HyperConfig.UseResearchedBannersBuff.Tooltip")]
        [LabelArgs(ItemID.ZombieBanner)]
        [DefaultValue(false)]
        public bool UseResearchedBannersBuff;

        [LabelKey("$Mods.HyperResearch.Configs.HyperConfig.UseResearchedPotionsBuff.Label")]
        [TooltipKey("$Mods.HyperResearch.Configs.HyperConfig.UseResearchedPotionsBuff.Tooltip")]
        [LabelArgs(ItemID.WrathPotion)]
        [DefaultValue(false)]
        public bool UseResearchedPotionsBuff;


        [Header("$Mods.HyperResearch.Configs.HyperConfig.Headers.BalanceSettingsHeader")]


        [LabelKey("$Mods.HyperResearch.Configs.HyperConfig.IgnoreCraftingConditions.Label")]
        [TooltipKey("$Mods.HyperResearch.Configs.HyperConfig.IgnoreCraftingConditions.Tooltip")]
        [LabelArgs(ItemID.BottledWater)]
        [DefaultValue(true)]
        public bool IgnoreCraftingConditions;

        [LabelKey("$Mods.HyperResearch.Configs.HyperConfig.BalanceShimmerAutoresearch.Label")]
        [TooltipKey("$Mods.HyperResearch.Configs.HyperConfig.BalanceShimmerAutoresearch.Tooltip")]
        [LabelArgs(ItemID.Shimmerfly)]
        [DefaultValue(true)]
        public bool BalanceShimmerAutoresearch;

        [LabelKey("$Mods.HyperResearch.Configs.HyperConfig.BalancePrefixPicker.Label")]
        [TooltipKey("$Mods.HyperResearch.Configs.HyperConfig.BalancePrefixPicker.Tooltip")]
        [LabelArgs(ItemID.TinkerersWorkshop)]
        [DefaultValue(false)]
        public bool BalancePrefixPicker;

        [LabelKey("$Mods.HyperResearch.Configs.HyperConfig.OnlyOneItemNeeded.Label")]
        [TooltipKey("$Mods.HyperResearch.Configs.HyperConfig.OnlyOneItemNeeded.Tooltip")]
        [LabelArgs(ItemID.AlphabetStatue1)]
        [DefaultValue(false)]
        public bool OnlyOneItemNeeded;

        [LabelKey("$Mods.HyperResearch.Configs.HyperConfig.ItemResearchCountOverride.Label")]
        [TooltipKey("$Mods.HyperResearch.Configs.HyperConfig.ItemResearchCountOverride.Tooltip")]
        public Dictionary<ItemDefinition, uint> ItemResearchCountOverride = [];
    }
}
