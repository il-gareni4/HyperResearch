using BetterResearch.Utils;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;

namespace BetterResearch.Common
{
    public class BRGlobalItem : GlobalItem
    {
        public override bool OnPickup(Item item, Terraria.Player player)
        {
            BRConfig config = ModContent.GetInstance<BRConfig>();

            if (ResearchUtils.IsResearched(item.type))
            {
                if (config.AutoTrashResearched) return false;
                else if (!config.ResearchPickup) return true;
            }
            else if (!ResearchUtils.CanBeResearched(item.type)) return true;

            int? remaining = CreativeUI.GetSacrificesRemaining(item.type);
            int count = player.CountItem(item.type, remaining.Value - item.stack);
            if (count + item.stack >= remaining)
            {
                ResearchUtils.ResearchItem(item.type);
                Main.NewText($"{Language.GetText("Mods.BetterResearch.Messages.Researched")}: [i:{item.type}]");
                SoundEngine.PlaySound(SoundID.ResearchComplete);
            }
            return true;
        }

        //public override void OnResearched(Item item, bool fullyResearched)
        //{
        //    if (!fullyResearched) return;
        //    ResearchUtils.ResearchItem(item);
        //}
    }
}
