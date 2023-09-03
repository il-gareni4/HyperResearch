using BetterResearch.Utils;
using log4net.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace BetterResearch.Common
{
    public class BRPlayer : ModPlayer
    {
        public readonly Dictionary<int, int> ResearchedTiles = new();

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (BetterResearch.ForgetBind.JustPressed) ForgetAllResearches();
        }

        public override void OnEnterWorld()
        {
            foreach((int itemId, int _) in CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId)
            {
                TryAddToResearchedTiles(itemId);
            }
        }

        public bool TryAddToResearchedTiles(int itemId)
        {
            if (!ContentSamples.ItemsByType.ContainsKey(itemId)) return false;

            Item item = ContentSamples.ItemsByType[itemId];
            if (item.createTile < TileID.Dirt || !ResearchUtils.IsResearched(itemId)) return false;

            AddToResearchedTiles(item.createTile);
            ModTile t = TileLoader.GetTile(item.createTile);
            if (t != null) 
                foreach (int adj in t.AdjTiles) AddToResearchedTiles(adj);
            return true;
        }

        public void ForgetAllResearches()
        {
            Player.creativeTracker.Reset();
        }

        private void AddToResearchedTiles(int tileID)
        {
            ResearchedTiles.TryGetValue(tileID, out int count);
            ResearchedTiles[tileID] = count + 1;
        }

    }
}
