using log4net.Plugin;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;

namespace HyperResearch.UI.Components
{
    public class UIAnimatedImageButton : UIElement
    {
        public int HoverFrame { get; set; }
        public int ReleaseStartFrame { get; set; }
        public int AnimationFramesCount { get; set; }
        public float OneFrameCount { get; set; } = 1;
        public LocalizedText HoverText { get; set; }
        private Asset<Texture2D> SpriteSheet { get; set; }
        private int CurrentFrame { get; set; }
        private int _subFrame = 0;

        public UIAnimatedImageButton(Asset<Texture2D> spriteSheet)
        {
            SpriteSheet = spriteSheet;
        }

        public override void Update(GameTime gameTime)
        {
            if (IsMouseHovering && HoverText is not null && InventoryButtons.ShowUI)
            {
                Main.LocalPlayer.creativeInterface = true;
                Main.LocalPlayer.mouseInterface = true;
                Main.instance.MouseText(HoverText.Value);
            }
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
            {
                if ((CurrentFrame + 1 == ReleaseStartFrame && !(IsMouseHovering && Main.mouseLeft)) ||
                    (CurrentFrame + 1 != ReleaseStartFrame))
                {
                    CurrentFrame++;
                }
            }
            if (CurrentFrame > AnimationFramesCount) CurrentFrame = 0;
            if (_subFrame >= OneFrameCount) _subFrame = 0;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle innerDim = GetInnerDimensions();
            Vector2 pos = new(innerDim.X, innerDim.Y);
            spriteBatch.Draw(SpriteSheet.Value, pos, GetFrameRect(CurrentFrame), Color.White);
        }

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            if (!InventoryButtons.ShowUI) return;
            base.LeftMouseDown(evt);
            CurrentFrame = 1;
            SoundEngine.PlaySound(SoundID.MenuTick);
        }

        private Rectangle GetFrameRect(int frame)
        {
            return new Rectangle(0, ((int)Height.Pixels + 1) * frame, (int)Width.Pixels, (int)Height.Pixels);
        }
    }
}
