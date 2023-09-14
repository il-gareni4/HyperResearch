using HyperResearch.Common.Systems;
using HyperResearch.UI.Components;
using HyperResearch.Utils;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.UI;

namespace HyperResearch.UI
{
    public class ShopButtons : UIState
    {
        public const float ShopSlotSize = 39f;
        public const float ShopSlotGap = 3f;
        //public static bool 
        public UIAnimatedImageButton ResearchShopButton;
        public override void OnInitialize()
        {
            HyperConfig.Changed += RebuildButtons;

            ResearchShopButton = new(UISystem.ResearchShopButtonTexture)
            {
                Width = StyleDimension.FromPixels(32),
                Height = StyleDimension.FromPixels(32),
                HoverText = Language.GetText("Mods.HyperResearch.UI.ShopButtons.ResearchButtonHoverText"),
                ReleaseStartFrame = 4,
                OneFrameCount = 4,
                AnimationFramesCount = 5,
                HoverFrame = 6,
                CanInteract = () => Main.npcShop >= 1
            };
            RebuildButtons();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Researcher.IsPlayerInJourneyMode() && Main.npcShop >= 1)
                base.Draw(spriteBatch);
        }

        public void RebuildButtons()
        {
            if (HyperConfig.Instance is null) return;

            Top = StyleDimension.FromPixels(
                MathF.Floor(InventoryButtons.BaseMargin + (InventoryButtons.ItemSlotSize + InventoryButtons.ItemSlotGap) * 5) + 1f +
                (ShopSlotSize + ShopSlotGap) * 4
            );
            Left = StyleDimension.FromPixels(
                MathF.Floor(InventoryButtons.BaseMargin + InventoryButtons.ItemSlotSize * 10 + InventoryButtons.ItemSlotGap * 9) + 1f -
                (ShopSlotSize * 2 + ShopSlotGap)
            );

            RemoveAllChildren();
            if (HyperConfig.Instance.ShowResearchShopButton)
            {
                ResearchShopButton.MarginTop = MathF.Floor((ShopSlotSize - ResearchShopButton.Width.Pixels) / 2);
                ResearchShopButton.MarginLeft = MathF.Floor((ShopSlotSize - ResearchShopButton.Width.Pixels) / 2);
                Append(ResearchShopButton);
            }
        }
    }
}
