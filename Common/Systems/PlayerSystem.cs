using System.Diagnostics.CodeAnalysis;
using HyperResearch.Common.Configs;
using HyperResearch.Common.ModPlayers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace HyperResearch.Common.Systems;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class PlayerSystem : ModSystem
{
    private HyperPlayer? HPlayer { get; set; }

    public override void OnWorldLoad()
    {
        if (Main.netMode == NetmodeID.Server) return;
        if (Main.LocalPlayer.TryGetModPlayer(out HyperPlayer modPlayer))
        {
            HPlayer = modPlayer;

            HyperConfig.Changed += HPlayer.OnClientConfigChanged;
            ModContent.GetInstance<UISystem>().InventoryButtons!.ResearchButton.OnLeftMouseDown +=
                OnResearchButtonMouseDown;
            ModContent.GetInstance<UISystem>().InventoryButtons!.ClearButton.OnLeftMouseDown += OnClearButtonMouseDown;
            ModContent.GetInstance<UISystem>().InventoryButtons!.AutoCraftButton.OnLeftMouseDown +=
                OnAutoCraftButtonMouseDown;
            ModContent.GetInstance<UISystem>().ShopButtons!.ResearchShopButton.OnLeftMouseDown += OnShopButtonMouseDown;
        }
    }

    public override void OnWorldUnload()
    {
        if (Main.netMode == NetmodeID.Server || HPlayer == null) return;

        HyperConfig.Changed -= HPlayer.OnClientConfigChanged;
        ModContent.GetInstance<UISystem>().InventoryButtons!.ResearchButton.OnLeftMouseDown -=
            OnResearchButtonMouseDown;
        ModContent.GetInstance<UISystem>().InventoryButtons!.ClearButton.OnLeftMouseDown -= OnClearButtonMouseDown;
        ModContent.GetInstance<UISystem>().InventoryButtons!.AutoCraftButton.OnLeftMouseDown -=
            OnAutoCraftButtonMouseDown;
        ModContent.GetInstance<UISystem>().ShopButtons!.ResearchShopButton.OnLeftMouseDown -= OnShopButtonMouseDown;

        HPlayer = null;
    }

    private void OnResearchButtonMouseDown(UIMouseEvent evt, UIElement el) => HPlayer?.SacrificeInventory();
    private void OnClearButtonMouseDown(UIMouseEvent evt, UIElement el) => HPlayer?.ClearResearched();
    private void OnAutoCraftButtonMouseDown(UIMouseEvent evt, UIElement el) => HPlayer?.ResearchAndMessageCraftable();

    private void OnShopButtonMouseDown(UIMouseEvent evt, UIElement el) =>
        HPlayer?.ResearchShop(HPlayer.CurrentShopItems);
}