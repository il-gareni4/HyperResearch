using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace BetterResearch
{
	public class BetterResearch : Mod
	{
        public static ModKeybind ForgetBind { get; set; }

        public override void Load()
        {
            ForgetBind = KeybindLoader.RegisterKeybind(this, "Forget All Researches", "P");    
        }

        public override void Unload()
        {
            ForgetBind = null;
        }
    }
}