using System.ComponentModel;
using Terraria.ID;
using Terraria.ModLoader.Config;

namespace BetterResearch
{
    public class BRConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [LabelArgs(ItemID.HandOfCreation)]
        [DefaultValue(true)]
        public bool ResearchPickup;

        [LabelArgs(ItemID.TrashCan)]
        [DefaultValue(false)]
        public bool AutoTrashResearched;

        [LabelArgs(ItemID.WorkBench)]
        [DefaultValue(true)]
        public bool AutoResearchCraftable;


    }
}
