using HyperResearch.Common.Configs;
using Terraria;
using Terraria.ID;

namespace HyperResearch.Utils
{
    public static class ConfigOptions
    {
        public static bool UseServerSettings { get => Main.netMode == NetmodeID.MultiplayerClient && ServerConfig.Instance.UseServerSettings; }

        public static bool IgnoreCraftingConditions
        {
            get =>
                UseServerSettings ? ServerConfig.Instance.IgnoreCraftingConditions : HyperConfig.Instance.IgnoreCraftingConditions;
        }

        public static bool ResearchShimmerableItems
        {
            get =>
                UseServerSettings ? ServerConfig.Instance.ResearchShimmerableItems : HyperConfig.Instance.ResearchShimmerableItems;
        }

        public static bool ResearchDecraftItems
        {
            get =>
                UseServerSettings ? ServerConfig.Instance.ResearchDecraftItems : HyperConfig.Instance.ResearchDecraftItems;
        }

        public static bool BalanceShimmerAutoresearch
        {
            get =>
                UseServerSettings ? ServerConfig.Instance.BalanceShimmerAutoresearch : HyperConfig.Instance.BalanceShimmerAutoresearch;
        }

        public static bool UseResearchedBannersBuff
        {
            get =>
                UseServerSettings ? ServerConfig.Instance.UseResearchedBannersBuff : HyperConfig.Instance.UseResearchedBannersBuff;
        }

        public static bool OnlyOneItemNeeded
        {
            get =>
                UseServerSettings ? ServerConfig.Instance.OnlyOneItemNeeded : HyperConfig.Instance.OnlyOneItemNeeded;
        }
    }
}
