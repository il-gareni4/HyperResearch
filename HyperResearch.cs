using HyperResearch.Utils;
using Terraria.ModLoader;

namespace HyperResearch
{
    public class HyperResearch : Mod
    {
        public static int ResearchableItemsCount { get; set; }

        public override void PostSetupContent()
        {
            int totalResearchable = 0;
            for (int itemId = 0; itemId < ItemLoader.ItemCount; itemId++)
            {
                if (!Researcher.IsResearchable(itemId)) continue;
                if (Researcher.ItemSharedValue(itemId) != -1) continue;
                totalResearchable++;
            }
            ResearchableItemsCount = totalResearchable;
        }
    }
}