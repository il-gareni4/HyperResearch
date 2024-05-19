﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;

namespace HyperResearch.UI.Elements;

public class UIAnimatedImageButton(Asset<Texture2D> spriteSheet) : UIElement
{
    private int _subFrame;
    private int CurrentFrame { get; set; }

    public required int HoverFrame { get; init; }
    public required int ReleaseStartFrame { get; init; }
    public required int AnimationFramesCount { get; init; }
    public required float OneFrameCount { get; init; } = 1;
    public LocalizedText? HoverText { get; init; }
    private Asset<Texture2D> SpriteSheet { get; } = spriteSheet;
    public required Func<bool> CanInteract { get; init; }

    public override void Update(GameTime gameTime)
    {
        if (CurrentFrame == 0 || CurrentFrame == HoverFrame)
        {
            if (CurrentFrame == 0 && IsMouseHovering)
                CurrentFrame = HoverFrame;
            else if (CurrentFrame == HoverFrame && !IsMouseHovering)
                CurrentFrame = 0;
            return;
        }

        if (++_subFrame < OneFrameCount)
            return;
        if (CurrentFrame > 0)
            if ((CurrentFrame + 1 == ReleaseStartFrame && !(IsMouseHovering && Main.mouseLeft)) ||
                CurrentFrame + 1 != ReleaseStartFrame)
                CurrentFrame++;

        if (CurrentFrame > AnimationFramesCount) CurrentFrame = 0;
        if (_subFrame >= OneFrameCount) _subFrame = 0;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        if (IsMouseHovering && HoverText != null && CanInteract())
        {
            Main.LocalPlayer.mouseInterface = true;
            Main.instance.MouseText(HoverText.Value);
        }

        CalculatedStyle innerDim = GetInnerDimensions();
        Vector2 pos = new(innerDim.X, innerDim.Y);
        spriteBatch.Draw(SpriteSheet.Value, pos, GetFrameRect(CurrentFrame), Color.White);
    }

    public override void LeftMouseDown(UIMouseEvent evt)
    {
        if (!CanInteract()) return;
        base.LeftMouseDown(evt);
        CurrentFrame = 1;
        SoundEngine.PlaySound(SoundID.MenuTick);
    }

    private Rectangle GetFrameRect(int frame) =>
        new(0, ((int)Height.Pixels + 1) * frame, (int)Width.Pixels, (int)Height.Pixels);
}