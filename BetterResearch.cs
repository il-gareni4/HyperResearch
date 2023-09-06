using Microsoft.Xna.Framework.Input;
using Terraria.ModLoader;

namespace BetterResearch
{
	public class BetterResearch : Mod
	{
        public static ModKeybind ForgetBind { get; set; }
        public static ModKeybind SacrificeInventoryBind { get; set; }
        public static ModKeybind ClearResearchedBind { get; set; }
        public static ModKeybind ResearchCraftableBind { get; set; }
        public static ModKeybind MaxStackBind { get; set; }

        public override void Load()
        {
            ForgetBind = KeybindLoader.RegisterKeybind(this, "Forget All Researches", "P");
            SacrificeInventoryBind = KeybindLoader.RegisterKeybind(this, "Research Inventory Items", Keys.OemComma);
            ClearResearchedBind = KeybindLoader.RegisterKeybind(this, "Clear Researched Items", Keys.OemPeriod);
            ResearchCraftableBind = KeybindLoader.RegisterKeybind(this, "Research Craftable Items", Keys.OemQuestion);
            MaxStackBind = KeybindLoader.RegisterKeybind(this, "Max Stack Researched Item", Keys.OemComma);
        }

        public override void Unload()
        {
            ForgetBind = null;
            SacrificeInventoryBind = null;
            ClearResearchedBind = null;
            ResearchCraftableBind = null;
            MaxStackBind = null;
        }
    }
}