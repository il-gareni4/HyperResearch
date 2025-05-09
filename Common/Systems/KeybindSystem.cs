﻿using Microsoft.Xna.Framework.Input;
using Terraria.ModLoader;

namespace HyperResearch.Common.Systems;

public class KeybindSystem : ModSystem
{
    public static ModKeybind? SacrificeInventoryBind { get; private set; }
    public static ModKeybind? ClearResearchedBind { get; private set; }
    public static ModKeybind? ResearchCraftableBind { get; private set; }
    public static ModKeybind? ResearchShimmerBind { get; private set; }
    public static ModKeybind? ResearchDecraftsBind { get; private set; }
    public static ModKeybind? ShareAllResearched { get; private set; }
    public static ModKeybind? MaxStackBind { get; private set; }
    public static ModKeybind? EnableDisableBuffBind { get; private set; }
    public static ModKeybind? ResearchLootBind { get; private set; }
    public static ModKeybind? ResearchShopBind { get; private set; }

    public override void Load()
    {
#if DEBUG
        ForgetAllBind = KeybindLoader.RegisterKeybind(Mod, "ForgetAll", Keys.P);
        ResearchAllBind = KeybindLoader.RegisterKeybind(Mod, "ResearchAll", Keys.O);
        ForgetAetherBind = KeybindLoader.RegisterKeybind(Mod, "ForgetAether", Keys.NumPad0);
#endif
        SacrificeInventoryBind = KeybindLoader.RegisterKeybind(Mod, "ResearchInventoryItems", Keys.NumPad1);
        ClearResearchedBind = KeybindLoader.RegisterKeybind(Mod, "ClearResearchedItems", Keys.NumPad2);
        ResearchCraftableBind = KeybindLoader.RegisterKeybind(Mod, "ResearchCraftableItems", Keys.NumPad3);
        ResearchShimmerBind = KeybindLoader.RegisterKeybind(Mod, "ResearchShimmerItems", Keys.NumPad4);
        ResearchDecraftsBind = KeybindLoader.RegisterKeybind(Mod, "ResearchDecraftItems", Keys.NumPad5);
        ShareAllResearched = KeybindLoader.RegisterKeybind(Mod, "ShareAllResearched", Keys.PageUp);
        MaxStackBind = KeybindLoader.RegisterKeybind(Mod, "MaxStack", Keys.OemTilde);
        EnableDisableBuffBind = KeybindLoader.RegisterKeybind(Mod, "EnableDisableBuff", "Mouse3");
        ResearchLootBind = KeybindLoader.RegisterKeybind(Mod, "ResearchBagContents", Keys.OemQuotes);
        ResearchShopBind = KeybindLoader.RegisterKeybind(Mod, "ResearchShop", Keys.OemSemicolon);
    }

    public override void Unload()
    {
#if DEBUG
        ForgetAllBind = null;
        ResearchAllBind = null;
        ForgetAetherBind = null;
#endif
        SacrificeInventoryBind = null;
        ClearResearchedBind = null;
        ResearchCraftableBind = null;
        ResearchShimmerBind = null;
        ResearchDecraftsBind = null;
        ShareAllResearched = null;
        MaxStackBind = null;
        EnableDisableBuffBind = null;
        ResearchLootBind = null;
        ResearchShopBind = null;
    }
#if DEBUG
    public static ModKeybind? ForgetAllBind { get; private set; }
    public static ModKeybind? ResearchAllBind { get; private set; }
    public static ModKeybind? ForgetAetherBind { get; private set; }
#endif
}