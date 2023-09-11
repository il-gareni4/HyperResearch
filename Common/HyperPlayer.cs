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

        /// <summary>Dictionary of researched tiles (contains <c>TileId</c> as Keys). 
        /// If Value > 0 then tile is researched </summary> 
        /// <seealso cref="Researcher.IsTileResearched(int)"/>
        public readonly Dictionary<int, bool> ResearchedTiles = new();

        /// <summary>Array of items in current shop. Used for <see cref="HyperResearch.ResearchShopBind"/></summary>
        public Item[] CurrentShopItems { get; set; } = Array.Empty<Item>();
        public int ItemsResearchedCount { get; set; }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (!Researcher.IsPlayerInJourneyMode()) return;
#if DEBUG
            if (HyperResearch.ForgetBind.JustPressed)
            {
                Player.creativeTracker.Reset();
                ResearchedTiles.Clear();
                ItemsResearchedCount = 0;
            }

#endif
            if (HyperResearch.SacrificeInventoryBind.JustPressed) SacrificeInventory();
            if (HyperResearch.ClearResearchedBind.JustPressed) ClearResearched();
            if (HyperResearch.ResearchCraftableBind.JustPressed) ResearchAndMessageCraftable();
            if (HyperResearch.MaxStackBind.JustPressed && !Main.HoverItem.IsAir &&
                (Main.HoverItem.tooltipContext == ItemSlot.Context.InventoryItem ||
                Main.HoverItem.tooltipContext == ItemSlot.Context.InventoryCoin ||
                Main.HoverItem.tooltipContext == ItemSlot.Context.InventoryAmmo) &&
                Researcher.IsResearched(Main.HoverItem.type))
            {
                _hoverItem.stack = Main.HoverItem.maxStack;
                SoundEngine.PlaySound(SoundID.Grab);
            }
            if (HyperResearch.ResearchLootBind.JustPressed && !Main.HoverItem.IsAir &&
                Researcher.IsResearched(Main.HoverItem.type))
            {
                ResearchAndMessageLoot(Main.HoverItem.type);
            }

            if (HyperResearch.ResearchShopBind.JustPressed && Player.TalkNPC is not null &&
                Main.npcShop > 0 && CurrentShopItems.Length > 0)
            {
                ResearchShop(CurrentShopItems);
            }
        }

        public override void OnEnterWorld()
        {
            if (!Researcher.IsPlayerInJourneyMode()) return;
            foreach (int itemId in CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId.Keys)
            {
                TryAddToResearchedTiles(itemId);
            }

            ItemsResearchedCount = 0;
            Researcher researcher = new();
            for (int itemId = 0; itemId < ItemLoader.ItemCount; itemId++)
            {
                if (HyperConfig.Instance.AutoResearchShimmeredItems)
                    researcher.TryResearchShimmeredItem(itemId);
                if (Researcher.ItemSharedValue(itemId) == -1 && Researcher.IsResearched(itemId))
                    ItemsResearchedCount++;
                if (HyperConfig.Instance.OnlyOneItemNeeded && Researcher.IsResearchable(itemId) &&
                    !Researcher.IsResearched(itemId) && Researcher.ItemResearchedCount(itemId) >= 1)
                    researcher.ResearchItem(itemId);
            }
            TextUtils.MessageResearcherResults(researcher);
            if (researcher.AnyItemResearched()) SoundEngine.PlaySound(SoundID.ResearchComplete);
        }

        public override void PostUpdate()
        {
            if (!Researcher.IsPlayerInJourneyMode()) return;

            if (HyperConfig.Instance.ResearchInventory) ResearchInventory();
        }

        public override bool HoverSlot(Item[] inventory, int context, int slot)
        {
            if (!Researcher.IsPlayerInJourneyMode()) return base.HoverSlot(inventory, context, slot);
            _hoverItem = inventory[slot];
            return base.HoverSlot(inventory, context, slot);
        }

        /// <summary>
        /// Automatically research items in the inventory if the total amount is equal to or more than required for research
        /// If number of items is not enough then it does nothing
        /// </summary>
        public void ResearchInventory()
        {
            // Counting every item into a single dictionary
            // Used so that items of the same type and different slots are counted together
            Dictionary<int, int> items = new();
            for (int slot = 0; slot < Main.InventorySlotsTotal; slot++)
            {
                Item item = Player.inventory[slot];
                if (item.IsAir) continue;
                items[item.type] = items.GetValueOrDefault(item.type, 0) + item.stack;
            }
            if (items.Count == 0) return;

            Researcher researcher = new();
            foreach ((int itemId, int itemCount) in items)
                researcher.TryResearchItem(itemId, itemCount);

            if (ModContent.GetInstance<HyperConfig>().AutoTrashAfterResearching && researcher.AnyItemResearched())
                TrashInventoryItems(researcher.AllResearchedItems);
            

            TextUtils.MessageResearcherResults(researcher);
            if (researcher.ResearchedItems.Count > 0) SoundEngine.PlaySound(SoundID.ResearchComplete);
        }

        /// <summary>
        /// Sacrifices every unresearched item in the inventory  
        /// </summary>
        public void SacrificeInventory()
        {
            HyperConfig config = ModContent.GetInstance<HyperConfig>();

            Dictionary<int, int> sacrificed = new();
            Researcher researcher = new();
            for (int slot = 0; slot < Main.InventorySlotsTotal; slot++)
            {
                Item item = Player.inventory[slot];
                int itemId = item.type; // If item turned into air than it will lose id

                if (item.favorited || item.IsAir ||
                    Researcher.IsResearched(item.type) || !Researcher.IsResearchable(item.type) ||
                    (!config.SacrificeHotbarSlots && slot >= 0 && slot <= 9) ||
                    (!config.SacrificeCoinsSlots && slot >= Main.InventoryCoinSlotsStart &&
                    slot < Main.InventoryCoinSlotsStart + Main.InventoryAmmoSlotsCount) || 
                    (!config.SacrificeAmmoSlots && slot >= Main.InventoryAmmoSlotsStart &&
                    slot < Main.InventoryAmmoSlotsStart + Main.InventoryAmmoSlotsCount)) {
                        continue;
                    }

                int itemsNeeded = Researcher.ItemTotalResearchCount(itemId) - Researcher.ItemResearchedCount(itemId);
                int researched = researcher.SacrificeItem(item);
                if (researched != 0) {
                    if (itemsNeeded != researched) 
                        sacrificed[itemId] = sacrificed.GetValueOrDefault(itemId, 0) + researched;
                    else
                        sacrificed.Remove(itemId);
                }
            }

            if (ModContent.GetInstance<HyperConfig>().AutoTrashAfterResearching && researcher.AnyItemResearched())
                TrashInventoryItems(researcher.AllResearchedItems);

            TextUtils.MessageSacrifices(sacrificed);
            TextUtils.MessageResearcherResults(researcher);

            if (researcher.AnyItemResearched()) SoundEngine.PlaySound(SoundID.ResearchComplete);
            else if (sacrificed.Count > 0) SoundEngine.PlaySound(SoundID.Research);
        }

        public void ClearResearched()
        {
            HyperConfig config = ModContent.GetInstance<HyperConfig>();

            bool anyItemCleaned = false;
            for (int slot = 0; slot < Main.InventorySlotsTotal; slot++)
            {
                Item item = Player.inventory[slot];
                if (item.favorited || item.IsAir || !Researcher.IsResearched(item.type)) continue;
                if (!config.ClearHotbarSlots && slot >= 0 && slot <= 9) continue;
                if (!config.ClearCoinsSlots && slot >= Main.InventoryCoinSlotsStart &&
                    slot < Main.InventoryCoinSlotsStart + Main.InventoryAmmoSlotsCount)
                {
                    continue;
                }

                if (!config.ClearAmmoSlots && slot >= Main.InventoryAmmoSlotsStart &&
                    slot < Main.InventoryAmmoSlotsStart + Main.InventoryAmmoSlotsCount)
                {
                    continue;
                }

                item.TurnToAir();
                anyItemCleaned = true;
            }
            if (anyItemCleaned) SoundEngine.PlaySound(SoundID.Grab);
        }

        public void TrashInventoryItems(IEnumerable<int> items)
        {
            for (int slot = 0; slot < Main.InventorySlotsTotal; slot++)
            {
                Item item = Player.inventory[slot];
                if (item.IsAir) continue;
                if (items.Contains(item.type)) item.TurnToAir();
            }
        }

        /// <summary>
        /// Tries to add to the <see cref="ResearchedTiles"/> the <see cref="Tile.TileType"/> that this item with <paramref name="itemId"/> places
        /// </summary>
        /// <returns>Has any tile been added to the <see cref="ResearchedTiles"/></returns>
        public bool TryAddToResearchedTiles(int itemId)
        {
            if (!ContentSamples.ItemsByType.ContainsKey(itemId)) return false;

            Item item = ContentSamples.ItemsByType[itemId];
            if (item.createTile < TileID.Dirt || !Researcher.IsResearched(itemId)) return false;

            ResearchedTiles[item.createTile] = true;
            ModTile t = TileLoader.GetTile(item.createTile);
            if (t != null)
                foreach (int adj in t.AdjTiles) ResearchedTiles[adj] = true;

            return true;
        }

        /// <summary>
        /// Researches the <paramref name="shop"/>. 
        /// If the currency for which the item is being sold has not been researched then the item is skipped
        /// </summary>
        /// <param name="shop">Array of shop items</param>
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
                    if (currencyItems.Any(itemWorth => Researcher.IsResearched(itemWorth.Key)))
                        toResearch.Add(item.type);
                }
                else
                {
                    bool anyCoinReseached = new List<int>() { ItemID.CopperCoin, ItemID.SilverCoin, ItemID.GoldCoin, ItemID.PlatinumCoin }.Any(Researcher.IsResearched);
                    if (anyCoinReseached) toResearch.Add(item.type);
                }
            }

            if (TryResearchAndMessage(toResearch)) SoundEngine.PlaySound(SoundID.ResearchComplete);
        }

        public void RecheckResearchingItems()
        {
            if (!HyperConfig.Instance.OnlyOneItemNeeded) return;
            Researcher researcher = new();
            for (int itemId = 0; itemId < ItemLoader.ItemCount; itemId++) {
                if (Researcher.IsResearchable(itemId) && !Researcher.IsResearched(itemId) && Researcher.ItemResearchedCount(itemId) >= 1)
                    researcher.ResearchItem(itemId);
            }
            TextUtils.MessageResearcherResults(researcher);
            if (researcher.ResearchedCraftableItems.Count > 0) SoundEngine.PlaySound(SoundID.ResearchComplete);
        }

        public static void ResearchAndMessageCraftable()
        {
            Researcher researcher = new();
            researcher.ResearchCraftable();
            TextUtils.MessageResearcherResults(researcher);
            if (researcher.ResearchedCraftableItems.Count > 0) SoundEngine.PlaySound(SoundID.ResearchComplete);
        }

        public static void ResearchAndMessageLoot(int itemId)
        {
            if (!ItemsUtils.IsLootItem(itemId)) return;

            IEnumerable<int> items = ItemsUtils.GetItemLoot(itemId);
            if (TryResearchAndMessage(items)) SoundEngine.PlaySound(SoundID.ResearchComplete);
        }


        /// <returns>Has any items been researched</returns>
        private static bool TryResearchAndMessage(IEnumerable<int> items)
        {
            Researcher researcher = new();
            researcher.ResearchItems(items);
            TextUtils.MessageResearcherResults(researcher);
            return researcher.ResearchedItems.Count > 0;
        }
    }
}
