using HyperResearch.UI.PrefixWindowElements;
using HyperResearch.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.UI;

namespace HyperResearch.UI;

public class PrefixWindow : UIState
{
    public UIPanel MainPanel;
    public UIList PrefixesList;
    public UIScrollbar ScrollBar;

    public bool Enabled => Main.playerInventory && Main.CreativeMenu.Enabled && PrefixesList.Count > 0;

    public override void OnInitialize()
    {
        MainPanel = new()
        {
            MinWidth = StyleDimension.FromPixels(200f),
            MinHeight = StyleDimension.FromPixels(150f),
            MaxHeight = StyleDimension.FromPixels(300f),
            Height = StyleDimension.Fill,
            Left = StyleDimension.FromPixels(564f)
        };
        MainPanel.SetPadding(4f);

        ScrollBar = new()
        {
            Top = StyleDimension.FromPixels(4f),
            Left = StyleDimension.FromPixelsAndPercent(-20f, 1f),
            Width = StyleDimension.FromPixels(20f),
            Height = StyleDimension.FromPixelsAndPercent(-8f, 1f)
        };
        MainPanel.Append(ScrollBar);

        PrefixesList = new()
        {
            Width = StyleDimension.FromPixelsAndPercent(-ScrollBar.Width.Pixels - MainPanel.PaddingRight, 1f),
            Height = StyleDimension.Fill,
            ListPadding = 2f
        };
        MainPanel.Append(PrefixesList);
        PrefixesList.SetScrollbar(ScrollBar);

        Append(MainPanel);
    }

    public void SetPrefixes(Item item)
    {
        PrefixesList.Clear();
        item.tooltipContext = ItemSlot.Context.PrefixItem;
        foreach (int prefix in ItemsUtils.GetPossiblePrefixes(item))
        {
            if (!item.CanApplyPrefix(prefix)) continue;
            Item prefixItem = item.Clone();
            prefixItem.Prefix(prefix);

            UIPrefixPanel prefixPanel = new(prefixItem);
            PrefixesList.Add(prefixPanel);
        }
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
        if (MainPanel.IsMouseHovering)
        {
            Main.LocalPlayer.mouseInterface = true;
            PlayerInput.LockVanillaMouseScroll("HyperResearch/PrefixWindow");
        }
    }

    public override void Update(GameTime gameTime)
    {
        if (Enabled) base.Update(gameTime);
    }

    public override void LeftMouseDown(UIMouseEvent evt)
    {
        if (Enabled && (evt.Target == this || evt.Target is UIPrefixPanel || evt.Target.Parent is UIPrefixPanel))
            PrefixesList.Clear();
        base.LeftMouseDown(evt);
    }

    public override void MiddleClick(UIMouseEvent evt)
    {
        if (Main.HoverItem.IsAir
            || Main.HoverItem.tooltipContext != ItemSlot.Context.CreativeInfinite) return;

        MainPanel.MaxHeight.Pixels = MathHelper.Clamp(Main.screenHeight - 352f - 64f, 150f, 300f);
        CalculatedStyle dim = MainPanel.GetDimensions();
        MainPanel.Top.Pixels = MathHelper.Clamp(evt.MousePosition.Y - dim.Height / 2, 352f, Main.screenHeight - dim.Height - 64f);

        SetPrefixes(Main.HoverItem.Clone());
        SoundEngine.PlaySound(SoundID.MenuTick);
        base.MiddleClick(evt);
    }
}
