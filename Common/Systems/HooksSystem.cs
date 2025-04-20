using System;
using HyperResearch.Common.ModPlayers;
using HyperResearch.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace HyperResearch.Common.Systems;

public class HooksSystem : ModSystem
{
    internal static event Action? WorldLoaded;
    internal static event Action? WorldUnloaded;

    public override void Load()
    {
        IL_ItemSlot.Draw_SpriteBatch_ItemArray_int_int_Vector2_Color += EditItemSlotDraw;
    }

    public override void Unload()
    {
        IL_ItemSlot.Draw_SpriteBatch_ItemArray_int_int_Vector2_Color -= EditItemSlotDraw;
    }

    public override void OnWorldLoad() => WorldLoaded?.Invoke();

    public override void OnWorldUnload() => WorldUnloaded?.Invoke();

    private void EditItemSlotDraw(ILContext il)
    {
        try
        {
            ILCursor c = new(il)
            {
                Index = 528
            };
            c.RemoveRange(15);
            c.EmitLdarg0(); // spriteBatch
            c.EmitLdloc(7); // texture
            c.EmitLdarg(4); // position
            c.EmitLdloc(8); // color2
            c.EmitLdarg2(); // context
            c.EmitLdloc1(); // item
            c.EmitDelegate((SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Color color, int context, Item item) =>
            {
                if (context == ItemSlot.Context.CreativeInfinite)
                {
                    if (ConfigOptions.UseResearchedBannersBuff &&
                        BannerSystem.TryItemToBanner(item.type, out int bannerId) &&
                        Main.LocalPlayer.GetModPlayer<BannerPlayer>().ResearchedBanners.TryGetValue(bannerId, out bool bannerEnabled))
                    {
                        color = bannerEnabled ? new Color(53, 111, 85) : new Color(107, 57, 81);
                    }
                    else if (ConfigOptions.UseResearchedPotionsBuff &&
                    BuffUtils.IsAcceptableBuffItem(item) &&
                    Main.LocalPlayer.GetModPlayer<BuffPlayer>().Buffs.TryGetValue(item.buffType, out bool buffEnabled))
                    {
                        color = buffEnabled ? new Color(53, 111, 85) : new Color(107, 57, 81);
                    }
                }
                spriteBatch.Draw(texture, position, null, color, 0f, default, Main.inventoryScale, SpriteEffects.None, 0f);
            });
        }
        catch
        {
            MonoModHooks.DumpIL(ModContent.GetInstance<HyperResearch>(), il);
        }
    }
}
