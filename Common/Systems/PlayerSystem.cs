using Terraria;
using Terraria.ModLoader;

namespace HyperResearch.Common.Systems
{
    public class PlayerSystem : ModSystem
    {
        public override void OnWorldLoad()
        {
            if (Main.LocalPlayer.TryGetModPlayer(out HyperPlayer modPlayer))
                HyperConfig.Changed += modPlayer.RecheckResearchingItems;
        }

        public override void OnWorldUnload()
        {
            if (Main.LocalPlayer.TryGetModPlayer(out HyperPlayer modPlayer))
                HyperConfig.Changed -= modPlayer.RecheckResearchingItems;
        }
    }
}
