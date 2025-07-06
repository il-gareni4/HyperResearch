using System;
using System.Linq;
using HyperResearch.Common.Configs;
using HyperResearch.Common.Configs.Enums;
using HyperResearch.Common.ModPlayers;
using HyperResearch.Common.Systems;
using HyperResearch.UI.Elements;
using HyperResearch.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
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
    public UIAnimatedImageButton ResearchCraftableButton { get; private set; } = null!;
    public UIAnimatedImageButton ShimmerButton { get; private set; } = null!;
    public UIAnimatedImageButton ShimmerDecraftButton { get; private set; } = null!;
    public UIAnimatedImageButton ShareButton { get; private set; } = null!;
    private static bool ShowUI() => !Main.CreativeMenu.Blocked && Main.playerInventory;

    public override void OnInitialize()
    {
        HooksSystem.WorldLoaded += RebuildButtons;
        HooksSystem.WorldLoaded += SetupEvents;
        HooksSystem.WorldUnloaded += RemoveEvents;

        ResearchButton = new UIAnimatedImageButton(UISystem.ResearchButtonTexture!)
        {
            Width = StyleDimension.FromPixels(32),
            Height = StyleDimension.FromPixels(32),
            HoverText = Language.GetText("Mods.HyperResearch.UI.InventoryButtons.ResearchButtonHoverText"),
            StopFrame = 3,
            OneFrameCount = 4,
            AnimationFramesCount = 6,
            CanInteract = ShowUI
        };
        ClearButton = new UIAnimatedImageButton(UISystem.ClearButtonTexture!)
        {
            Width = StyleDimension.FromPixels(32),
            Height = StyleDimension.FromPixels(32),
            HoverText = Language.GetText("Mods.HyperResearch.UI.InventoryButtons.ClearButtonHoverText"),
            StopFrame = 4,
            OneFrameCount = 4,
            AnimationFramesCount = 8,
            CanInteract = ShowUI
        };
        ResearchCraftableButton = new UIAnimatedImageButton(UISystem.AutoCraftButtonTexture!)
        {
            Width = StyleDimension.FromPixels(32),
            Height = StyleDimension.FromPixels(32),
            HoverText = Language.GetText("Mods.HyperResearch.UI.InventoryButtons.AutoCraftHoverText"),
            StopFrame = 5,
            OneFrameCount = 4,
            AnimationFramesCount = 8,
            CanInteract = ShowUI
        };
        ShimmerButton = new UIAnimatedImageButton(UISystem.ShimmerButtonTexture!)
        {
            Width = StyleDimension.FromPixels(32),
            Height = StyleDimension.FromPixels(32),
            HoverText = Language.GetText("Mods.HyperResearch.UI.InventoryButtons.ShimmerButtonHoverText"),
            StopFrame = 5,
            OneFrameCount = 5,
            AnimationFramesCount = 12,
            CanInteract = ShowUI
        };
        ShimmerDecraftButton = new UIAnimatedImageButton(UISystem.ShimmerDecraftButtonTexture!)
        {
            Width = StyleDimension.FromPixels(32),
            Height = StyleDimension.FromPixels(32),
            HoverText = Language.GetText("Mods.HyperResearch.UI.InventoryButtons.ShimmerDecraftButtonHoverText"),
            StopFrame = 9,
            OneFrameCount = 5,
            AnimationFramesCount = 13,
            CanInteract = ShowUI
        };
        ShareButton = new UIAnimatedImageButton(UISystem.RedShareButtonTexture!)
        {
            Width = StyleDimension.FromPixels(32),
            Height = StyleDimension.FromPixels(32),
            HoverText = Language.GetText("Mods.HyperResearch.UI.InventoryButtons.ShareButtonHoverText"),
            StopFrame = 4,
            OneFrameCount = 4,
            AnimationFramesCount = 8,
            CanInteract = ShowUI
        };
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (Researcher.IsPlayerInJourneyMode && ShowUI())
            base.Draw(spriteBatch);
    }

    public override void Update(GameTime gameTime)
    {
        if (!Researcher.IsPlayerInJourneyMode) return;

        ShimmerButton.Disabled = ShimmerCondition();
        ShimmerDecraftButton.Disabled = ShimmerCondition();

        ShareButton.Disabled = TeamCondition();

        if (ShimmerButton.Disabled) 
            ShimmerButton.HoverText = Language.GetText("Mods.HyperResearch.UI.InventoryButtons.ShimmerDisabledHoverText");
        else 
            ShimmerButton.HoverText = Language.GetText("Mods.HyperResearch.UI.InventoryButtons.ShimmerButtonHoverText");

        if (ShimmerDecraftButton.Disabled)
            ShimmerDecraftButton.HoverText = Language.GetText("Mods.HyperResearch.UI.InventoryButtons.DecraftDisabledHoverText");
        else
            ShimmerDecraftButton.HoverText = Language.GetText("Mods.HyperResearch.UI.InventoryButtons.ShimmerDecraftButtonHoverText");

        if (ShareButton.Disabled) {
            if (Main.LocalPlayer.team == 0)
                ShareButton.HoverText = Language.GetText("Mods.HyperResearch.UI.InventoryButtons.ShareDisabledNoTeamHoverText");
            else
                ShareButton.HoverText = Language.GetText("Mods.HyperResearch.UI.InventoryButtons.ShareDisabledNoMembersHoverText");
        }
        else
            ShareButton.HoverText = Language.GetText("Mods.HyperResearch.UI.InventoryButtons.ShareButtonHoverText");

        base.Update(gameTime);
    }

    private void RebuildButtons()
    {
        Left = StyleDimension.FromPixels(MathF.Floor(BaseMargin +
                                                     (ItemSlotSize + ItemSlotGap) *
                                                     (HyperConfig.Instance.InventoryButtonsSlotOffset - 1)));
        Top = StyleDimension.FromPixels(MathF.Floor(BaseMargin + (ItemSlotSize + ItemSlotGap) * 5));

        var left = 0f;
        RemoveAllChildren();
        if (HyperConfig.Instance.ShowResearchInventoryButton &&
            HyperConfig.Instance.ResearchMode != ResearchMode.None &&
            HyperConfig.Instance.ResearchMode != ResearchMode.AutoSacrificeAlways)
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

        if (HyperConfig.Instance.ShowAutoCraftButton &&
            HyperConfig.Instance.CraftablesResearchMode != CraftablesResearchMode.None)
        {
            ResearchCraftableButton.MarginTop = BaseTopMargin;
            ResearchCraftableButton.MarginLeft = MathF.Floor(left) + (ItemSlotSize - ResearchCraftableButton.Width.Pixels) / 2;
            Append(ResearchCraftableButton);
            left += ItemSlotSize + ItemSlotGap;
        }

        if (HyperConfig.Instance.ShowShimmerButton)
        {
            ShimmerButton.MarginTop = BaseTopMargin;
            ShimmerButton.MarginLeft = MathF.Floor(left) + (ItemSlotSize - ShimmerButton.Width.Pixels) / 2;
            Append(ShimmerButton);
            left += ItemSlotSize + ItemSlotGap;
        }

        if (HyperConfig.Instance.ShowShimmerDecraftButton)
        {
            ShimmerDecraftButton.MarginTop = BaseTopMargin;
            ShimmerDecraftButton.MarginLeft = MathF.Floor(left) + (ItemSlotSize - ShimmerDecraftButton.Width.Pixels) / 2;
            Append(ShimmerDecraftButton);
            left += ItemSlotSize + ItemSlotGap;
        }

        if (HyperConfig.Instance.ShowTeamShareButton && Main.netMode == NetmodeID.MultiplayerClient)
        {
            ShareButton.MarginTop = BaseTopMargin;
            ShareButton.MarginLeft = MathF.Floor(left) + (ItemSlotSize - ShareButton.Width.Pixels) / 2;
            Append(ShareButton);
        }
    }

    private Asset<Texture2D> GetShareButtonTexture(int team)
    {
        return team switch
        {
            2 => UISystem.GreenShareButtonTexture!,
            3 => UISystem.BlueShareButtonTexture!,
            4 => UISystem.YellowShareButtonTexture!,
            5 => UISystem.PinkShareButtonTexture!,
            _ => UISystem.RedShareButtonTexture!,
        };
    }

    private void SetupEvents() {
        HyperConfig.Changed += RebuildButtons;
        Main.LocalPlayer.GetModPlayer<HyperPlayer>().OnTeamChanged += RecolorShareButton;
    }

    private void RemoveEvents() {
        HyperConfig.Changed -= RebuildButtons;
        Main.LocalPlayer.GetModPlayer<HyperPlayer>().OnTeamChanged -= RecolorShareButton;
    }

    private void RecolorShareButton() {
        ShareButton.SpriteSheet = GetShareButtonTexture(Main.LocalPlayer.team);
    }

    private static bool ShimmerCondition() =>
        ConfigOptions.BalanceShimmerAutoresearch &&
        !Main.LocalPlayer.GetModPlayer<HyperPlayer>().WasInAether;

    private static bool TeamCondition() =>
        Main.LocalPlayer.team == 0 ||
        !MainUtils.GetTeamMembers(Main.LocalPlayer.team, Main.myPlayer).Any();
}