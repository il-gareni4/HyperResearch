using HyperResearch.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.UI;

namespace HyperResearch.UI.PrefixWindowElements;

public class UIPrefixPanel : UIPanel
{
    private readonly Item _item;

    public UIPrefixPanel(Item prefixItem)
    {
        _item = prefixItem;

        Width.Percent = 1f;
        SetPadding(4f);
        PaddingLeft = 8f;
        var prefixText = new UIText(PrefixString)
        {
            TextColor = ItemsUtils.GetRarityColor(_item),
            VAlign = 0.5f
        };
        Vector2 textSize = FontAssets.MouseText.Value.MeasureString(PrefixString);
        MinWidth.Set(textSize.X, 0f);
        MinHeight.Set(textSize.Y + PaddingTop + PaddingBottom, 0f);

        Append(prefixText);
    }

    private string PrefixString => Lang.prefix[_item.prefix].Value;

    public override int CompareTo(object obj)
    {
        if (obj is UIPrefixPanel prefixPanel)
            return -_item.value.CompareTo(prefixPanel._item.value);
        return base.CompareTo(obj);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        if (IsMouseHovering)
        {
            Main.HoverItem = _item.Clone();
            Main.hoverItemName = Main.HoverItem.Name;

            if (PlayerInput.GetPressedKeys().Contains(Keys.LeftShift))
                Main.cursorOverride = CursorOverrideID.BackInventory;
        }

        base.DrawSelf(spriteBatch);
    }

    public override void LeftMouseDown(UIMouseEvent evt)
    {
        if (!Main.playerInventory || !Main.CreativeMenu.Enabled) return;
        if (PlayerInput.GetPressedKeys().Contains(Keys.LeftShift))
            Main.LocalPlayer.GetItem(Main.LocalPlayer.whoAmI, _item.Clone(), new GetItemSettings());
        else
        {
            Main.mouseItem = _item.Clone();
            SoundEngine.PlaySound(SoundID.Grab);
            base.LeftMouseDown(evt);
        }
    }

    public override void MouseOver(UIMouseEvent evt)
    {
        BackgroundColor = new Color(133, 152, 221) * 0.7f;
        base.MouseOver(evt);
    }

    public override void MouseOut(UIMouseEvent evt)
    {
        BackgroundColor = new Color(63, 82, 151) * 0.7f;
        base.MouseOut(evt);
    }
}