using HyperResearch.Utils;
using System.Collections.Generic;
using Terraria;
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
                if (Researcher.GetSharedValue(itemId) != -1) continue;
                totalResearchable++;
            }
            ResearchableItemsCount = totalResearchable;
        }
    }
}