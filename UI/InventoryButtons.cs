using System;
using HyperResearch.Common.Configs;
using HyperResearch.Common.Systems;
using HyperResearch.UI.Elements;
using HyperResearch.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.UI;

namespace HyperResearch.UI;

public class InventoryButtons : UIState
{
    public const float BaseMargin = 20f;
    private const float BaseTopMargin = 10f;
    public const float ItemSlotSize = 44f;
    public const float ItemSlotGap = 3.5f;
    public UIAnimatedImageButton ResearchButton { get; private set; } = null!;
    public UIAnimatedImageButton ClearButton { get; private set; } = null!;
    public UIAnimatedImageButton AutoCraftButton { get; private set; } = null!;
    private static bool ShowUI() => !Main.CreativeMenu.Blocked && Main.playerInventory;

    public override void OnInitialize()
    {
        HyperConfig.Changed += RebuildButtons;

        ResearchButton = new UIAnimatedImageButton(UISystem.ResearchButtonTexture!)
        {
            Width = StyleDimension.FromPixels(32),
            Height = StyleDimension.FromPixels(32),
            HoverText = Language.GetText("Mods.HyperResearch.UI.InventoryButtons.ResearchButtonHoverText"),
            ReleaseStartFrame = 4,
            OneFrameCount = 4,
            AnimationFramesCount = 5,
            HoverFrame = 6,
            CanInteract = ShowUI
        };
        ClearButton = new UIAnimatedImageButton(UISystem.ClearButtonTexture!)
        {
            Width = StyleDimension.FromPixels(32),
            Height = StyleDimension.FromPixels(32),
            HoverText = Language.GetText("Mods.HyperResearch.UI.InventoryButtons.ClearButtonHoverText"),
            ReleaseStartFrame = 5,
            OneFrameCount = 3,
            AnimationFramesCount = 8,
            HoverFrame = 8,
            CanInteract = ShowUI
        };
        AutoCraftButton = new UIAnimatedImageButton(UISystem.AutoCraftButtonTexture!)
        {
            Width = StyleDimension.FromPixels(32),
            Height = StyleDimension.FromPixels(32),
            HoverText = Language.GetText("Mods.HyperResearch.UI.InventoryButtons.AutoCraftHoverText"),
            ReleaseStartFrame = 6,
            OneFrameCount = 3,
            AnimationFramesCount = 8,
            HoverFrame = 8,
            CanInteract = ShowUI
        };
        RebuildButtons();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (Researcher.IsPlayerInJourneyMode && ShowUI())
            base.Draw(spriteBatch);
    }

    public override void Update(GameTime gameTime)
    {
        if (Researcher.IsPlayerInJourneyMode) base.Update(gameTime);
    }

    private void RebuildButtons()
    {
        Left = StyleDimension.FromPixels(MathF.Floor(BaseMargin +
                                                     (ItemSlotSize + ItemSlotGap) *
                                                     (HyperConfig.Instance.InventoryButtonsSlotOffset - 1)));
        Top = StyleDimension.FromPixels(MathF.Floor(BaseMargin + (ItemSlotSize + ItemSlotGap) * 5));

        var left = 0f;
        RemoveAllChildren();
        if (HyperConfig.Instance.ShowResearchInventoryButton)
        {
            ResearchButton.MarginTop = BaseTopMargin;
            ResearchButton.MarginLeft = (ItemSlotSize - ResearchButton.Width.Pixels) / 2;
            Append(ResearchButton);
            left += ItemSlotSize + ItemSlotGap;
        }

        if (HyperConfig.Instance.ShowClearInventoryButton)
        {
            ClearButton.MarginTop = BaseTopMargin;
            ClearButton.MarginLeft = MathF.Floor(left) + (ItemSlotSize - ClearButton.Width.Pixels) / 2;
            Append(ClearButton);
            left += ItemSlotSize + ItemSlotGap;
        }

        if (HyperConfig.Instance.ShowAutoCraftButton)
        {
            AutoCraftButton.MarginTop = BaseTopMargin;
            AutoCraftButton.MarginLeft = left + (ItemSlotSize - AutoCraftButton.Width.Pixels) / 2;
            Append(AutoCraftButton);
        }
    }
}