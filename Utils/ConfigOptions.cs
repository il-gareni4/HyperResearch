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
        : BaseConfig.Instance.IgnoreCraftingConditions;

    public static bool BalanceShimmerAutoresearch => UseServerSettings
        ? ServerConfig.Instance.BalanceShimmerAutoresearch
        : BaseConfig.Instance.BalanceShimmerAutoresearch;

    public static bool BalancePrefixPicker => UseServerSettings
        ? ServerConfig.Instance.BalancePrefixPicker
        : BaseConfig.Instance.BalancePrefixPicker;

    public static bool UseResearchedBannersBuff => UseServerSettings
        ? ServerConfig.Instance.UseResearchedBannersBuff
        : BaseConfig.Instance.UseResearchedBannersBuff;

    public static bool UseResearchedPotionsBuff => UseServerSettings
        ? ServerConfig.Instance.UseResearchedPotionsBuff
        : BaseConfig.Instance.UseResearchedPotionsBuff;

    public static bool OnlyOneItemNeeded => UseServerSettings
        ? ServerConfig.Instance.OnlyOneItemNeeded
        : CheatsConfig.Instance.OnlyOneItemNeeded;

    public static Dictionary<ItemDefinition, uint> ItemResearchCountOverride => UseServerSettings
        ? ServerConfig.Instance.ItemResearchCountOverride
        : CheatsConfig.Instance.ItemResearchCountOverride;
}