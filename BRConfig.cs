using System.ComponentModel;
using Terraria.ID;
using Terraria.ModLoader.Config;

namespace BetterResearch
{
    public class BRConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [LabelArgs(ItemID.YoyoBag)]
        [DefaultValue(true)]
        public bool ResearchInventory;

        [LabelArgs(ItemID.TrashCan)]
        [DefaultValue(false)]
        public bool AutoTrashResearched;

        [LabelArgs(ItemID.WorkBench)]
        [DefaultValue(true)]
        public bool AutoResearchCraftable;

        [LabelArgs(ItemID.BottledWater)]
        [DefaultValue(true)]
        public bool IgnoreCraftingConditions;
    }
}
