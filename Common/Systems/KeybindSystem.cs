using Microsoft.Xna.Framework.Input;
using Terraria.ModLoader;

namespace HyperResearch.Common.Systems
{
    public class KeybindSystem : ModSystem
    {
#if DEBUG
        public static ModKeybind ForgetBind { get; set; }
        public static ModKeybind ResearchAllBind { get; set; }
#endif
        public static ModKeybind SacrificeInventoryBind { get; set; }
        public static ModKeybind ClearResearchedBind { get; set; }
        public static ModKeybind ResearchCraftableBind { get; set; }
        public static ModKeybind MaxStackBind { get; set; }
        public static ModKeybind ResearchLootBind { get; set; }
        public static ModKeybind ResearchShopBind { get; set; }

        public override void Load()
        {
#if DEBUG
            ForgetBind = KeybindLoader.RegisterKeybind(Mod, "Forget All Researches", Keys.P);
            ResearchAllBind = KeybindLoader.RegisterKeybind(Mod, "Research All", Keys.O);
#endif
            SacrificeInventoryBind = KeybindLoader.RegisterKeybind(Mod, "Research Inventory Items", Keys.OemComma);
            ClearResearchedBind = KeybindLoader.RegisterKeybind(Mod, "Clear Researched Items", Keys.OemPeriod);
            ResearchCraftableBind = KeybindLoader.RegisterKeybind(Mod, "Research Craftable Items", Keys.OemQuestion);
            MaxStackBind = KeybindLoader.RegisterKeybind(Mod, "Max Stack Researched Item", Keys.OemSemicolon);
            ResearchLootBind = KeybindLoader.RegisterKeybind(Mod, "Research Bag/Crate Contents", Keys.OemQuotes);
            ResearchShopBind = KeybindLoader.RegisterKeybind(Mod, "Research Shop", Keys.OemSemicolon);
        }

        public override void Unload()
        {
#if DEBUG
            ForgetBind = null;
            ResearchAllBind = null;
#endif
            SacrificeInventoryBind = null;
            ClearResearchedBind = null;
            ResearchCraftableBind = null;
            MaxStackBind = null;
            ResearchLootBind = null;
            ResearchShopBind = null;
        }
    }
}
