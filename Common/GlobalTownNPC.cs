using Terraria;
using Terraria.ModLoader;

namespace BetterResearch.Common
{
    public class GlobalTownNPC : GlobalNPC
    {
        public override void ModifyActiveShop(NPC npc, string shopName, Item[] items)
        {
            Main.LocalPlayer.GetModPlayer<BRPlayer>().CurrentShopItems = items;
            base.ModifyActiveShop(npc, shopName, items);
        }
    }
}
