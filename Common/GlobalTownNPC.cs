using HyperResearch.Common.Configs;
using HyperResearch.Common.ModPlayers;
using HyperResearch.Utils;
using Terraria;
using Terraria.ModLoader;

namespace HyperResearch.Common
{
    public class GlobalTownNPC : GlobalNPC
    {
        public override void ModifyActiveShop(NPC npc, string shopName, Item[] items)
        {
            if (!Researcher.IsPlayerInJourneyMode) return;

            if (Main.LocalPlayer.TryGetModPlayer(out HyperPlayer modPlayer))
            {
                modPlayer.CurrentShopItems = items;
                if (HyperConfig.Instance.AutoResearchShop) modPlayer.ResearchShop(items);
            }
        }
    }
}
