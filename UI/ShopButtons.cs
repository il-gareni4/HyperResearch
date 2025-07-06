using System;
using HyperResearch.Common.Configs;
using HyperResearch.Common.Configs.Enums;
using HyperResearch.Common.Systems;
using HyperResearch.UI.Elements;
using HyperResearch.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.UI;

namespace HyperResearch.UI;

public class ShopButtons : UIState
{
    private const float ShopSlotSize = 39f;

    private const float ShopSlotGap = 3f;

    //public static bool 
    public UIAnimatedImageButton ResearchShopButton = null!;

    public override void OnInitialize()
    {
        HyperConfig.Changed += RebuildButtons;

        ResearchShopButton = new UIAnimatedImageButton(UISystem.ResearchShopButtonTexture!)
        {
            Width = StyleDimension.FromPixels(32),
            Height = StyleDimension.FromPixels(32),
            HoverText = Language.GetText("Mods.HyperResearch.UI.ShopButtons.ResearchButtonHoverText"),
            StopFrame = 3,
            OneFrameCount = 4,
            AnimationFramesCount = 6,
            CanInteract = () => Main.LocalPlayer.TalkNPC is not null && Main.npcShop >= 1
        };
        RebuildButtons();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (Researcher.IsPlayerInJourneyMode && Main.LocalPlayer.TalkNPC is not null && Main.npcShop >= 1)
            base.Draw(spriteBatch);
    }

    public override void Update(GameTime gameTime)
    {
        if (Researcher.IsPlayerInJourneyMode) base.Update(gameTime);
    }

    private void RebuildButtons()
    {
        RemoveAllChildren();
        if (HyperConfig.Instance.ShopResearchMode != ShopResearchMode.Manual)
            return;

        Top = StyleDimension.FromPixels(
            MathF.Floor(
                InventoryButtons.BaseMargin + (InventoryButtons.ItemSlotSize + InventoryButtons.ItemSlotGap) * 5) + 1f +
            (ShopSlotSize + ShopSlotGap) * 4
        );
        Left = StyleDimension.FromPixels(
            MathF.Floor(InventoryButtons.BaseMargin + InventoryButtons.ItemSlotSize * 10 +
                        InventoryButtons.ItemSlotGap * 9) + 1f -
            (ShopSlotSize * 2 + ShopSlotGap)
        );

        ResearchShopButton.MarginTop = MathF.Floor((ShopSlotSize - ResearchShopButton.Width.Pixels) / 2);
        ResearchShopButton.MarginLeft = MathF.Floor((ShopSlotSize - ResearchShopButton.Width.Pixels) / 2);
        Append(ResearchShopButton);
    }
}