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
    private UIPanel _mainPanel;
    private UIList _prefixesList;
    private UIScrollbar _scrollBar;

    public static bool CanBeShown =>
        (!ConfigOptions.BalancePrefixPicker || Condition.DownedGoblinArmy.IsMet())
        && Main.playerInventory
        && Main.CreativeMenu.Enabled;

    public bool Enabled => CanBeShown && _prefixesList.Count > 0;

    public override void OnInitialize()
    {
        _mainPanel = new UIPanel
        {
            MinWidth = StyleDimension.FromPixels(200f),
            MinHeight = StyleDimension.FromPixels(150f),
            MaxHeight = StyleDimension.FromPixels(300f),
            Height = StyleDimension.Fill,
            Left = StyleDimension.FromPixels(564f)
        };
        _mainPanel.SetPadding(4f);

        _scrollBar = new UIScrollbar
        {
            Top = StyleDimension.FromPixels(4f),
            Left = StyleDimension.FromPixelsAndPercent(-20f, 1f),
            Width = StyleDimension.FromPixels(20f),
            Height = StyleDimension.FromPixelsAndPercent(-8f, 1f)
        };
        _mainPanel.Append(_scrollBar);

        _prefixesList = new UIList
        {
            Width = StyleDimension.FromPixelsAndPercent(-_scrollBar.Width.Pixels - _mainPanel.PaddingRight, 1f),
            Height = StyleDimension.Fill,
            ListPadding = 2f
        };
        _mainPanel.Append(_prefixesList);
        _prefixesList.SetScrollbar(_scrollBar);

        Append(_mainPanel);
    }

    public void TrySetPrefixes(Item item)
    {
        if (!CanBeShown || !ItemsUtils.CanHavePrefixes(Main.HoverItem) ||
            Main.HoverItem.tooltipContext != ItemSlot.Context.CreativeInfinite) return;

        _mainPanel.MaxHeight.Pixels = MathHelper.Clamp(Main.screenHeight - 352f - 64f, 150f, 300f);
        CalculatedStyle dim = _mainPanel.GetDimensions();
        _mainPanel.Top.Pixels =
            MathHelper.Clamp(Main.mouseY - dim.Height / 2, 352f, Main.screenHeight - dim.Height - 64f);

        SetPrefixes(Main.HoverItem.Clone());
        SoundEngine.PlaySound(SoundID.MenuTick);
    }

    private void SetPrefixes(Item item)
    {
        _prefixesList.Clear();
        item.tooltipContext = ItemSlot.Context.PrefixItem;
        foreach (int prefix in ItemsUtils.GetPossiblePrefixes(item))
        {
            Item prefixItem = item.Clone();
            prefixItem.Prefix(prefix);

            UIPrefixPanel prefixPanel = new(prefixItem);
            _prefixesList.Add(prefixPanel);
        }
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
        if (_mainPanel.IsMouseHovering)
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
            _prefixesList.Clear();
        base.LeftMouseDown(evt);
    }
}