using BetterResearch.Utils;
using FullSerializer;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using tModPorter;

namespace BetterResearch.Common
{
    public class BRPlayer : ModPlayer
    {
        /// <summary>
        /// Same as <see cref="Main.HoverItem"/> but not cloned
        /// </summary>
        private Item _hoverItem = new();
        public readonly Dictionary<int, int> ResearchedTiles = new();

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (BetterResearch.ForgetBind.JustPressed) Player.creativeTracker.Reset();
            if (BetterResearch.SacrificeInventoryBind.JustPressed) SacrificeInventory();
            if (BetterResearch.ClearResearchedBind.JustPressed) ClearResearched();
            if (BetterResearch.ResearchCraftableBind.JustPressed) ResearchCraftable();
            if (BetterResearch.MaxStackBind.JustPressed && !Main.HoverItem.IsAir &&
                (Main.HoverItem.tooltipContext == ItemSlot.Context.InventoryItem ||
                Main.HoverItem.tooltipContext == ItemSlot.Context.InventoryCoin ||
                Main.HoverItem.tooltipContext == ItemSlot.Context.InventoryAmmo) &&
                ResearchUtils.IsResearched(Main.HoverItem.type))
            {
                _hoverItem.stack = Main.HoverItem.maxStack;
                SoundEngine.PlaySound(SoundID.Grab);
            }
            if (BetterResearch.ResearchLootBind.JustPressed && !Main.HoverItem.IsAir &&
                ResearchUtils.IsResearched(Main.HoverItem.type)) ResearchLoot(Main.HoverItem.type);

        }

        public override void OnEnterWorld()
        {
            foreach (int itemId in CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId.Keys)
            {
                TryAddToResearchedTiles(itemId);
            }
        }

        public override void PostUpdate()
        {
            if (ModContent.GetInstance<BRConfig>().ResearchInventory) ResearchInventory();
        }

        public override bool HoverSlot(Item[] inventory, int context, int slot)
        {
            _hoverItem = inventory[slot];
            return base.HoverSlot(inventory, context, slot);
        }

        public void ResearchInventory()
        {
            Dictionary<int, int> items = new();
            for (int slot = 0; slot < Main.InventorySlotsTotal; slot++)
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

            TextUtils.MessageResearched(researchedItems);
            TextUtils.MessageResearchedCraftable(researchedCraftableItems);
            if (researchedItems.Count > 0) SoundEngine.PlaySound(SoundID.ResearchComplete);
        }

        public void SacrificeInventory()
        {
            BRConfig config = ModContent.GetInstance<BRConfig>();

            bool anyItemSacrificed = false;
            List<int> researchedItems = new();
            List<int> researchedCraftableItems = new();
            for (int slot = 0; slot < Main.InventorySlotsTotal; slot++)
            {
                Item item = Player.inventory[slot];
                int itemId = item.type;
                if (item.favorited || item.IsAir || ResearchUtils.IsResearched(item.type)) continue;

                anyItemSacrificed = true;
                if (ResearchUtils.SacrificeItem(item, out List<int> craftable) == CreativeUI.ItemSacrificeResult.SacrificedAndDone)
                {
                    researchedItems.Add(itemId);
                    researchedCraftableItems.AddRange(craftable);
                }
            }

            TextUtils.MessageResearched(researchedItems);
            TextUtils.MessageResearchedCraftable(researchedCraftableItems);

            if (researchedItems.Count > 0 || researchedCraftableItems.Count > 0) SoundEngine.PlaySound(SoundID.ResearchComplete);
            else if (anyItemSacrificed) SoundEngine.PlaySound(SoundID.Research);
        }

        public void ClearResearched()
        {
            BRConfig config = ModContent.GetInstance<BRConfig>();

            bool anyItemCleaned = false;
            for (int slot = 0; slot < Main.InventorySlotsTotal; slot++)
            {
                Item item = Player.inventory[slot];
                if (item.favorited || item.IsAir || !ResearchUtils.IsResearched(item.type)) continue;
                if (!config.ClearCoins && slot >= Main.InventoryCoinSlotsStart &&
                    slot < Main.InventoryCoinSlotsStart + Main.InventoryAmmoSlotsCount) continue;
                if (!config.ClearAmmo && slot >= Main.InventoryAmmoSlotsStart &&
                    slot < Main.InventoryAmmoSlotsStart + Main.InventoryAmmoSlotsCount) continue;
                item.TurnToAir();
                anyItemCleaned = true;
            }
            if (anyItemCleaned) SoundEngine.PlaySound(SoundID.Grab);
        }

        public void ResearchCraftable()
        {
            List<int> researchedItems = ResearchUtils.ResearchCraftable();
            TextUtils.MessageResearchedCraftable(researchedItems);
            if (researchedItems.Count > 0) SoundEngine.PlaySound(SoundID.ResearchComplete);
        }

        public void ResearchLoot(int itemId) {
            if (!ItemsUtils.IsLootItem(itemId)) return;
            IEnumerable<int> items = ItemsUtils.GetItemLoot(itemId);
            List<int> researched = ResearchUtils.ResearchItems(items, out List<int> craftable);
            TextUtils.MessageResearched(researched);
            TextUtils.MessageResearchedCraftable(craftable);
            if (researched.Count > 0) SoundEngine.PlaySound(SoundID.ResearchComplete);
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
