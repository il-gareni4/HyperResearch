using Terraria;
using Terraria.ModLoader;

namespace BetterResearch.Common
{
    public class GlobalTownNPC : GlobalNPC
    {
        public override void ModifyActiveShop(NPC npc, string shopName, Item[] items)
        {
            BRPlayer modPlayer = Main.LocalPlayer.GetModPlayer<BRPlayer>();
            modPlayer.CurrentShopItems = items;
            if (ModContent.GetInstance<BRConfig>().AutoResearchShop) modPlayer.ResearchShop(items);
            base.ModifyActiveShop(npc, shopName, items);
        }
    }
}
