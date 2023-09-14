using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace HyperResearch.Common.Systems
{
    public class PlayerSystem : ModSystem
    {
        public HyperPlayer HPlayer { get; set; }
        public override void OnWorldLoad()
        {
            if (Main.LocalPlayer.TryGetModPlayer(out HyperPlayer modPlayer))
                HPlayer = modPlayer;

            HyperConfig.Changed += HPlayer.RecheckResearchingItems;
            ModContent.GetInstance<UISystem>().InventoryButtons.ResearchButton.OnLeftMouseDown += OnResearchButtonMouseDown;
            ModContent.GetInstance<UISystem>().InventoryButtons.ClearButton.OnLeftMouseDown += OnClearButtonMouseDown;
            ModContent.GetInstance<UISystem>().InventoryButtons.AutoCraftButton.OnLeftMouseDown += OnAutoCraftButtonMouseDown;
        }

        public override void OnWorldUnload()
        {
            HyperConfig.Changed -= HPlayer.RecheckResearchingItems;
            HPlayer = null;
        }

        private void OnResearchButtonMouseDown(UIMouseEvent evt, UIElement el) => HPlayer?.SacrificeInventory();
        private void OnClearButtonMouseDown(UIMouseEvent evt, UIElement el) => HPlayer?.ClearResearched();
        private void OnAutoCraftButtonMouseDown(UIMouseEvent evt, UIElement el) => HyperPlayer.ResearchAndMessageCraftable();
    }
}
