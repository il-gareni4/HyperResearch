using Terraria;
using Terraria.ModLoader;

namespace HyperResearch.Common
{
    public class GlobalTownNPC : GlobalNPC
    {
        public override void ModifyActiveShop(NPC npc, string shopName, Item[] items)
        {
            if (Main.GameMode != 3) return;

            HyperPlayer modPlayer = Main.LocalPlayer.GetModPlayer<HyperPlayer>();
            modPlayer.CurrentShopItems = items;
            if (ModContent.GetInstance<HyperConfig>().AutoResearchShop) modPlayer.ResearchShop(items);
            base.ModifyActiveShop(npc, shopName, items);
        }
    }
}
