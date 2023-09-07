using HyperResearch.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.GameContent.UI;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace HyperResearch.Common
{
    public class HyperPlayer : ModPlayer
    {
        /// <summary>
        /// Same as <see cref="Main.HoverItem"/> but not cloned
        /// </summary>
        private Item _hoverItem = new();
        public readonly Dictionary<int, int> ResearchedTiles = new();

        public Item[] CurrentShopItems { get; set; } = Array.Empty<Item>();

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (Main.GameMode != 3) return;
#if DEBUG
            if (HyperResearch.ForgetBind.JustPressed) Player.creativeTracker.Reset();
#endif
            if (HyperResearch.SacrificeInventoryBind.JustPressed) SacrificeInventory();
            if (HyperResearch.ClearResearchedBind.JustPressed) ClearResearched();
            if (HyperResearch.ResearchCraftableBind.JustPressed) ResearchCraftable();
            if (HyperResearch.MaxStackBind.JustPressed && !Main.HoverItem.IsAir &&
                (Main.HoverItem.tooltipContext == ItemSlot.Context.InventoryItem ||
                Main.HoverItem.tooltipContext == ItemSlot.Context.InventoryCoin ||
                Main.HoverItem.tooltipContext == ItemSlot.Context.InventoryAmmo) &&
                ResearchUtils.IsResearched(Main.HoverItem.type))
            {
                _hoverItem.stack = Main.HoverItem.maxStack;
                SoundEngine.PlaySound(SoundID.Grab);
            }
            if (HyperResearch.ResearchLootBind.JustPressed && !Main.HoverItem.IsAir &&
                ResearchUtils.IsResearched(Main.HoverItem.type)) ResearchLoot(Main.HoverItem.type);
            if (HyperResearch.ResearchShopBind.JustPressed && Player.TalkNPC is not null &&
                Main.npcShop > 0 && CurrentShopItems.Length > 0)
            {
                ResearchShop(CurrentShopItems);
            }
        }

        public override void OnEnterWorld()
        {
            if (Main.GameMode != 3) return;
            foreach (int itemId in CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId.Keys)
            {
                TryAddToResearchedTiles(itemId);
            }
        }

        public override void PostUpdate()
        {
            if (Main.GameMode != 3) return;
            if (ModContent.GetInstance<HyperConfig>().ResearchInventory) ResearchInventory();
        }

        public override bool HoverSlot(Item[] inventory, int context, int slot)
        {
            if (Main.GameMode != 3) return base.HoverSlot(inventory, context, slot);
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
            HyperConfig config = ModContent.GetInstance<HyperConfig>();

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
            HyperConfig config = ModContent.GetInstance<HyperConfig>();

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
        public void ResearchShop(Item[] shop)
        {
            List<int> toResearch = new();
            foreach (Item item in shop)
            {
                if (item == null || item.IsAir) continue;
                if (item.shopSpecialCurrency != -1 && item.shopCustomPrice is not null &&
                    CustomCurrencyManager.TryGetCurrencySystem(item.shopSpecialCurrency, out CustomCurrencySystem system))
                {
                    // Use reflection to get a protected field that stores the item's ID and its local cost
                    FieldInfo info = system.GetType().GetField("_valuePerUnit", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (info is null) continue;

                    Dictionary<int, int> currencyItems = (Dictionary<int, int>)info.GetValue(system);
                    if (currencyItems.Any(itemWorth => ResearchUtils.IsResearched(itemWorth.Key)))
                        toResearch.Add(item.type);
                }
                else
                {
                    bool anyCoinReseached = (new List<int>() { ItemID.CopperCoin, ItemID.SilverCoin, ItemID.GoldCoin, ItemID.PlatinumCoin }).Any(ResearchUtils.IsResearched);
                    if (anyCoinReseached) toResearch.Add(item.type);
                }
            }

            if (TryResearchAndMessage(toResearch)) SoundEngine.PlaySound(SoundID.ResearchComplete);
        }

        private void AddToResearchedTiles(int tileID)
        {
            ResearchedTiles.TryGetValue(tileID, out int count);
            ResearchedTiles[tileID] = count + 1;
        }

        public static void ResearchCraftable()
        {
            List<int> researchedItems = ResearchUtils.ResearchCraftable();
            TextUtils.MessageResearchedCraftable(researchedItems);
            if (researchedItems.Count > 0) SoundEngine.PlaySound(SoundID.ResearchComplete);
        }

        public static void ResearchLoot(int itemId)
        {
            if (!ItemsUtils.IsLootItem(itemId)) return;

            IEnumerable<int> items = ItemsUtils.GetItemLoot(itemId);
            if (TryResearchAndMessage(items)) SoundEngine.PlaySound(SoundID.ResearchComplete);
        }

        private static bool TryResearchAndMessage(IEnumerable<int> items)
        {
            List<int> researched = ResearchUtils.ResearchItems(items, out List<int> craftable);
            TextUtils.MessageResearched(researched);
            TextUtils.MessageResearchedCraftable(craftable);
            return researched.Count > 0;
        }

    }
}
