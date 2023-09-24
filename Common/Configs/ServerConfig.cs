using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace HyperResearch.Common.Configs
{
    public class ServerConfig : ModConfig
    {
        public static ServerConfig Instance { get; set; }

        public override ConfigScope Mode => ConfigScope.ServerSide;

        public override void OnLoaded() => Instance = this;

        [DefaultValue(false)]
        public bool SyncResearchedItemsInOneTeam;
    }
}
