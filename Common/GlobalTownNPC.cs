using HyperResearch.Common.Configs;
using HyperResearch.Common.Configs.Enums;
using HyperResearch.Common.ModPlayers;
using HyperResearch.Utils;
using Terraria;
using Terraria.ModLoader;

namespace HyperResearch.Common;

public class GlobalTownNPC : GlobalNPC
{
    public override void ModifyActiveShop(NPC npc, string shopName, Item[] items)
    {
        if (!Researcher.IsPlayerInJourneyMode ||
            !Main.LocalPlayer.TryGetModPlayer(out HyperPlayer hyperPlayer))
            return;

        hyperPlayer.CurrentShopItems = items;
        if (BaseConfig.Instance.ShopResearchMode == ShopResearchMode.OnShopOpen)
            hyperPlayer.ResearchShop(items);
    }
}