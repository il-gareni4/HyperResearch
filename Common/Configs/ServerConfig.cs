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

        [LabelArgs(ItemID.BottledWater)]
        [DefaultValue(true)]
        public bool IgnoreCraftingConditions;

        [LabelArgs(ItemID.BottomlessShimmerBucket)]
        [DefaultValue(true)]
        public bool ResearchShimmerableItems;

        [LabelArgs(ItemID.WoodenHammer)]
        [DefaultValue(true)]
        public bool ResearchDecraftItems;

        [LabelArgs(ItemID.Shimmerfly)]
        [DefaultValue(true)]
        public bool BalanceShimmerAutoresearch;

        [LabelArgs(ItemID.ZombieBanner)]
        [DefaultValue(false)]
        public bool UseResearchedBannersBuff;

        [LabelArgs(ItemID.WrathPotion)]
        [DefaultValue(false)]
        public bool UseResearchedPotionsBuff;

        [LabelArgs(ItemID.AlphabetStatue1)]
        [DefaultValue(false)]
        public bool OnlyOneItemNeeded;

        [LabelArgs(ItemID.DD2EnergyCrystal)]
        public Dictionary<ItemDefinition, uint> ItemResearchCountOverride = [];
    }
}
