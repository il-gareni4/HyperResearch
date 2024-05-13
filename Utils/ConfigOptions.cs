using System.Collections.Generic;
using HyperResearch.Common.Configs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.Config;

namespace HyperResearch.Utils;

public static class ConfigOptions
{
    private static bool UseServerSettings =>
        Main.netMode == NetmodeID.MultiplayerClient && ServerConfig.Instance.UseServerSettings;

    public static bool IgnoreCraftingConditions => UseServerSettings
        ? ServerConfig.Instance.IgnoreCraftingConditions
        : HyperConfig.Instance.IgnoreCraftingConditions;

    public static bool ResearchShimmerableItems => UseServerSettings
        ? ServerConfig.Instance.ResearchShimmerableItems
        : HyperConfig.Instance.ResearchShimmerableItems;

    public static bool ResearchDecraftItems => UseServerSettings
        ? ServerConfig.Instance.ResearchDecraftItems
        : HyperConfig.Instance.ResearchDecraftItems;

    public static bool BalanceShimmerAutoresearch => UseServerSettings
        ? ServerConfig.Instance.BalanceShimmerAutoresearch
        : HyperConfig.Instance.BalanceShimmerAutoresearch;

    public static bool BalancePrefixPicker => UseServerSettings
        ? ServerConfig.Instance.BalancePrefixPicker
        : HyperConfig.Instance.BalancePrefixPicker;

    public static bool UseResearchedBannersBuff => UseServerSettings
        ? ServerConfig.Instance.UseResearchedBannersBuff
        : HyperConfig.Instance.UseResearchedBannersBuff;

    public static bool UseResearchedPotionsBuff => UseServerSettings
        ? ServerConfig.Instance.UseResearchedPotionsBuff
        : HyperConfig.Instance.UseResearchedPotionsBuff;

    public static bool OnlyOneItemNeeded => UseServerSettings
        ? ServerConfig.Instance.OnlyOneItemNeeded
        : HyperConfig.Instance.OnlyOneItemNeeded;

    public static Dictionary<ItemDefinition, uint> ItemResearchCountOverride => UseServerSettings
        ? ServerConfig.Instance.ItemResearchCountOverride
        : HyperConfig.Instance.ItemResearchCountOverride;
}