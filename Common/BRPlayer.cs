using BetterResearch.Utils;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
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
            if (BetterResearch.ForgetBind.JustPressed) Player.creativeTracker.Reset();
        }

        public override void OnEnterWorld()
        {
            foreach (int itemId in CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId.Keys)
            {
                TryAddToResearchedTiles(itemId);
            }
        }

        public override void PostUpdate() => ResearchInventory();

        public void ResearchInventory()
        {
            Dictionary<int, int> items = new();
            for (int slot = 0; slot < Player.inventory.Length; slot++)
            {
                Item item = Player.inventory[slot];
                if (item.IsAir) continue;
                items[item.type] = items.GetValueOrDefault(item.type, 0) + item.stack;
            }

            List<int> researchedItems = new();
            List<int> researchedCraftableItems = new();
            foreach ((int itemId, int itemCount) in items)
            {
                if (ResearchUtils.TryResearchItem(itemId, itemCount, out List<int> researchedCraftable))
                {
                    researchedItems.Add(itemId);
                    researchedCraftableItems.AddRange(researchedCraftable);
                }
            }

            if (researchedItems.Count > 0)
            {
                string researchStr = researchedItems.Count > 1 ? $"{researchedItems.Count} new items" : "new item";
                Main.NewText($"Researched {researchStr}: [i:{string.Join("][i:", researchedItems)}]");
                SoundEngine.PlaySound(SoundID.ResearchComplete);
            }
            if (researchedCraftableItems.Count > 0) {
                string researchStr = researchedCraftableItems.Count > 1 ? $"{researchedCraftableItems.Count} craftable items" : "craftable item";
                Main.NewText($"Researched {researchStr}: [i:{string.Join("][i:", researchedCraftableItems)}]");
            }
        }

        public bool TryAddToResearchedTiles(int itemId)
        {
            if (!ContentSamples.ItemsByType.ContainsKey(itemId)) return false;

            Item item = ContentSamples.ItemsByType[itemId];
            if (item.createTile < TileID.Dirt || !ResearchUtils.IsResearched(itemId)) return false;

            ModTile t = TileLoader.GetTile(item.createTile);
            if (t != null)
                foreach (int adj in t.AdjTiles) AddToResearchedTiles(adj);
            else
                AddToResearchedTiles(item.createTile);

            return true;
        }

        private void AddToResearchedTiles(int tileID)
        {
            ResearchedTiles.TryGetValue(tileID, out int count);
            ResearchedTiles[tileID] = count + 1;
        }

    }
}
