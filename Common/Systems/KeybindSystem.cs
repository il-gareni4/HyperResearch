using Microsoft.Xna.Framework.Input;
using Terraria.ModLoader;

namespace HyperResearch.Common.Systems
{
    public class KeybindSystem : ModSystem
    {
#if DEBUG
        public static ModKeybind ForgetAllBind { get; set; }
        public static ModKeybind ResearchAllBind { get; set; }
#endif
        public static ModKeybind SacrificeInventoryBind { get; set; }
        public static ModKeybind ClearResearchedBind { get; set; }
        public static ModKeybind ResearchCraftableBind { get; set; }
        public static ModKeybind MaxStackBind { get; set; }
        public static ModKeybind EnableDisableBuffBind { get; set; }
        public static ModKeybind ResearchLootBind { get; set; }
        public static ModKeybind ResearchShopBind { get; set; }
        public static ModKeybind ShareAllResearched { get; set; }

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
            EnableDisableBuffBind = KeybindLoader.RegisterKeybind(Mod, "EnableDisableBuff", Keys.OemSemicolon);
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
    }
}
