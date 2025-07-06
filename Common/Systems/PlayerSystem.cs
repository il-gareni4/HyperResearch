using HyperResearch.Common.Configs;
using HyperResearch.Common.ModPlayers;
using HyperResearch.Utils;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace HyperResearch.Common.Systems;

public class PlayerSystem : ModSystem
{
    private HyperPlayer? HyperPlayer { get; set; }

    public override void OnWorldLoad()
    {
        if (Main.netMode == NetmodeID.Server || !Researcher.IsPlayerInJourneyMode) return;
        if (Main.LocalPlayer.TryGetModPlayer(out HyperPlayer modPlayer))
        {
            HyperPlayer = modPlayer;

            BaseConfig.Changed += HyperPlayer.OnClientConfigChanged;
            UISystem uiSystem = ModContent.GetInstance<UISystem>();

            uiSystem.InventoryButtons!.ResearchButton.OnLeftMouseDown += OnResearchButtonMouseDown;
            uiSystem.InventoryButtons!.ClearButton.OnLeftMouseDown += OnClearButtonMouseDown;
            uiSystem.InventoryButtons!.ResearchCraftableButton.OnLeftMouseDown += OnAutoCraftButtonMouseDown;
            uiSystem.InventoryButtons!.ShimmerButton.OnLeftMouseDown += OnShimmerButtonMouseDown;
            uiSystem.InventoryButtons!.ShimmerDecraftButton.OnLeftMouseDown += OnShimmerDecraftButtonMouseDown;
            uiSystem.InventoryButtons!.ShareButton.OnLeftMouseDown += OnShareButtonMouseDown;

            uiSystem.ShopButtons!.ResearchShopButton.OnLeftMouseDown += OnShopButtonMouseDown;
        }
    }

    public override void OnWorldUnload()
    {
        if (Main.netMode == NetmodeID.Server || HyperPlayer == null || !Researcher.IsPlayerInJourneyMode) return;

        BaseConfig.Changed -= HyperPlayer.OnClientConfigChanged;
        UISystem uiSystem = ModContent.GetInstance<UISystem>();

        uiSystem.InventoryButtons!.ResearchButton.OnLeftMouseDown -= OnResearchButtonMouseDown;
        uiSystem.InventoryButtons!.ClearButton.OnLeftMouseDown -= OnClearButtonMouseDown;
        uiSystem.InventoryButtons!.ResearchCraftableButton.OnLeftMouseDown -= OnAutoCraftButtonMouseDown;
        uiSystem.InventoryButtons!.ShimmerButton.OnLeftMouseDown -= OnShimmerButtonMouseDown;
        uiSystem.InventoryButtons!.ShimmerDecraftButton.OnLeftMouseDown -= OnShimmerDecraftButtonMouseDown;
        uiSystem.InventoryButtons!.ShareButton.OnLeftMouseDown -= OnShareButtonMouseDown;

        uiSystem.ShopButtons!.ResearchShopButton.OnLeftMouseDown -= OnShopButtonMouseDown;

        HyperPlayer = null;
    }

    private void OnResearchButtonMouseDown(UIMouseEvent evt, UIElement el) => 
        HyperPlayer?.SacrificeInventoryAction();
        
    private void OnClearButtonMouseDown(UIMouseEvent evt, UIElement el) => 
        HyperPlayer?.ClearResearched();

    private void OnAutoCraftButtonMouseDown(UIMouseEvent evt, UIElement el) => 
        HyperPlayer?.ResearchCraftableAction();

    private void OnShimmerButtonMouseDown(UIMouseEvent evt, UIElement el) => 
        HyperPlayer?.ResearchShimmerItemsAction();

    private void OnShimmerDecraftButtonMouseDown(UIMouseEvent evt, UIElement el) => 
        HyperPlayer?.ResearchDecraftItemsAction();

    private void OnShareButtonMouseDown(UIMouseEvent evt, UIElement el) => 
        HyperPlayer?.ShareResearchedItemsAction();

    private void OnShopButtonMouseDown(UIMouseEvent evt, UIElement el) =>
        HyperPlayer?.ResearchShop(HyperPlayer.CurrentShopItems);
}