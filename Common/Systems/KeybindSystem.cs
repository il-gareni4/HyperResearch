﻿using Microsoft.Xna.Framework.Input;
using Terraria.ModLoader;

namespace HyperResearch.Common.Systems;

public class KeybindSystem : ModSystem
{
    public static ModKeybind? SacrificeInventoryBind { get; private set; }
    public static ModKeybind? ClearResearchedBind { get; private set; }
    public static ModKeybind? ResearchCraftableBind { get; private set; }
    public static ModKeybind? MaxStackBind { get; private set; }
    public static ModKeybind? EnableDisableBuffBind { get; private set; }
    public static ModKeybind? ResearchLootBind { get; private set; }
    public static ModKeybind? ResearchShopBind { get; private set; }
    public static ModKeybind? ShareAllResearched { get; private set; }

    public override void Load()
    {
#if DEBUG
        ForgetAllBind = KeybindLoader.RegisterKeybind(Mod, "ForgetAll", Keys.P);
        ResearchAllBind = KeybindLoader.RegisterKeybind(Mod, "ResearchAll", Keys.O);
#endif
        SacrificeInventoryBind = KeybindLoader.RegisterKeybind(Mod, "ResearchInventoryItems", Keys.OemComma);
        ClearResearchedBind = KeybindLoader.RegisterKeybind(Mod, "ClearResearchedItems", Keys.OemPeriod);
        ResearchCraftableBind = KeybindLoader.RegisterKeybind(Mod, "ResearchCraftableItems", Keys.OemQuestion);
        MaxStackBind = KeybindLoader.RegisterKeybind(Mod, "MaxStack", Keys.OemTilde);
        EnableDisableBuffBind = KeybindLoader.RegisterKeybind(Mod, "EnableDisableBuff", Keys.OemTilde);
        ResearchLootBind = KeybindLoader.RegisterKeybind(Mod, "ResearchBagContents", Keys.OemQuotes);
        ResearchShopBind = KeybindLoader.RegisterKeybind(Mod, "ResearchShop", Keys.OemSemicolon);
        ShareAllResearched = KeybindLoader.RegisterKeybind(Mod, "ShareAllResearched", Keys.OemPipe);
    }

    public override void Unload()
    {
#if DEBUG
        ForgetAllBind = null;
        ResearchAllBind = null;
#endif
        SacrificeInventoryBind = null;
        ClearResearchedBind = null;
        ResearchCraftableBind = null;
        MaxStackBind = null;
        EnableDisableBuffBind = null;
        ResearchLootBind = null;
        ResearchShopBind = null;
        ShareAllResearched = null;
    }
#if DEBUG
    public static ModKeybind? ForgetAllBind { get; private set; }
    public static ModKeybind? ResearchAllBind { get; private set; }
#endif
}