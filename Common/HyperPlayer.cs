using HyperResearch.Common.Configs;
using HyperResearch.Common.Systems;
using HyperResearch.Utils;
using HyperResearch.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
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

        public List<int> ResearchedBanners = new();

        /// <summary>Array of items in current shop. Used for <see cref="HyperResearch.ResearchShopBind"/></summary>
        public Item[] CurrentShopItems { get; set; } = Array.Empty<Item>();
        public int ItemsResearchedCount { get; set; }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (!Researcher.IsPlayerInJourneyMode()) return;
#if DEBUG
            if (KeybindSystem.ForgetAllBind.JustPressed)
            {
                Player.creativeTracker.Reset();
                ResearchedTiles.Clear();
                ItemsResearchedCount = 0;
                ResearchedBanners.Clear();
            }
            if (KeybindSystem.ResearchAllBind.JustPressed)
            {
                Researcher researcher = new();
                researcher.ResearchItems(Enumerable.Range(1, ItemLoader.ItemCount - 1), researchCraftable: false);
                TextUtils.MessageResearchedItems(researcher.AllResearchedItems);
            }
#endif
            if (KeybindSystem.SacrificeInventoryBind.JustPressed) SacrificeInventory();
            if (KeybindSystem.ClearResearchedBind.JustPressed) ClearResearched();
            if (KeybindSystem.ResearchCraftableBind.JustPressed) ResearchAndMessageCraftable();
            if (KeybindSystem.MaxStackBind.JustPressed && !Main.HoverItem.IsAir &&
                (Main.HoverItem.tooltipContext == ItemSlot.Context.InventoryItem ||
                Main.HoverItem.tooltipContext == ItemSlot.Context.InventoryCoin ||
                Main.HoverItem.tooltipContext == ItemSlot.Context.InventoryAmmo) &&
                Researcher.IsResearched(Main.HoverItem.type))
            {
                _hoverItem.stack = Main.HoverItem.maxStack;
                SoundEngine.PlaySound(SoundID.Grab);
            }
            if (KeybindSystem.ResearchLootBind.JustPressed && !Main.HoverItem.IsAir &&
                Researcher.IsResearched(Main.HoverItem.type) && ItemsUtils.CanOpenLootItem(Main.HoverItem.type))
            {
                ResearchAndMessageLoot(Main.HoverItem.type);
            }

            if (KeybindSystem.ResearchShopBind.JustPressed && Player.TalkNPC is not null &&
                Main.npcShop > 0 && CurrentShopItems.Length > 0)
            {
                ResearchShop(CurrentShopItems);
            }

            if (KeybindSystem.ShareAllResearched.JustPressed && Main.LocalPlayer.team >= 1 &&
                MainUtils.GetTeamMembers(Main.LocalPlayer.team, Main.myPlayer).Any())
            {
                SyncItemsWithTeam(Enumerable.Range(1, ItemLoader.ItemCount - 1).Where(Researcher.IsResearched), new Dictionary<int, int>());
            }
        }

        public override void OnEnterWorld()
        {
            if (!Researcher.IsPlayerInJourneyMode()) return;

            ItemsResearchedCount = 0;
            Researcher researcher = new();
            for (int itemId = 1; itemId < ItemLoader.ItemCount; itemId++)
            {
                TryAddToResearchedTiles(itemId);

                if (ConfigOptions.ResearchShimmerableItems)
                    researcher.TryResearchShimmeredItem(itemId);

                if (Researcher.IsResearched(itemId))
                {
                    if (Researcher.GetSharedValue(itemId) == -1)
                        ItemsResearchedCount++;
                    if (BannerSystem.ItemToBanner.TryGetValue(itemId, out int bannerId))
                        ResearchedBanners.Add(bannerId);
                }
                else
                {
                    if (ConfigOptions.OnlyOneItemNeeded && Researcher.IsResearchable(itemId) &&
                        Researcher.GetResearchedCount(itemId) >= 1) researcher.ResearchItem(itemId);
                }
            }
            AfterLocalResearch(researcher);
        }

        public override void PostUpdate()
        {
            if (!Researcher.IsPlayerInJourneyMode() || Player != Main.LocalPlayer) return;

            if (HyperConfig.Instance.ResearchInventory) ResearchInventory();
        }

        public override bool HoverSlot(Item[] inventory, int context, int slot)
        {
            if (!Researcher.IsPlayerInJourneyMode()) return base.HoverSlot(inventory, context, slot);
            _hoverItem = inventory[slot];
            return base.HoverSlot(inventory, context, slot);
        }

        public void SyncItemsWithTeam(Researcher researcher) => SyncItemsWithTeam(researcher.AllResearchedItems, researcher.SacrificedItems);

        public void SyncItemsWithTeam(IEnumerable<int> items, IDictionary<int, int> sacrifices)
        {
            if (Main.LocalPlayer.team == 0 || (!ServerConfig.Instance.SyncResearchedItemsInOneTeam && !ServerConfig.Instance.SyncSacrificesInOneTeam)) return;

            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)NetMessageType.ShareItemsWithTeam);

            if (ServerConfig.Instance.SyncResearchedItemsInOneTeam && items.Any())
                packet.Write(items);
            else packet.Write(Array.Empty<int>());

            if (ServerConfig.Instance.SyncSacrificesInOneTeam && sacrifices.Any())
                packet.Write(sacrifices);
            else packet.Write(new Dictionary<int, int>());

            packet.Send(ignoreClient: Main.myPlayer);
        }

        public void SharedItems(int fromPlayer, IEnumerable<int> items, IDictionary<int, int> sacrifices)
        {
            if (HyperConfig.Instance.ShowOtherPlayersResearchedItems)
                TextUtils.MessageOtherPlayerResearchedItems(items, fromPlayer);

            bool anyItemResearched = false;
            Researcher sacrificesResearcher = new();
            sacrificesResearcher.SacrificeItems(sacrifices, researchCraftable: HyperConfig.Instance.AutoResearchCraftableItems);
            if (HyperConfig.Instance.ShowSharedSacrifices && sacrificesResearcher.AnyItemSacrificed())
                TextUtils.MessageSharedSacrifices(sacrificesResearcher.SacrificedItems, fromPlayer);
            if (HyperConfig.Instance.ShowNewlyResearchedItems && sacrificesResearcher.AnyItemResearched())
            {
                TextUtils.MessageResearchedItems(sacrificesResearcher.ResearchedItems);
                anyItemResearched = true;
            }

            if (items.Any())
            {
                Researcher researcher = new();
                researcher.ResearchItems(items, researchCraftable: HyperConfig.Instance.AutoResearchCraftableItems);

                if (HyperConfig.Instance.ShowSharedItems)
                    TextUtils.MessageSharedItems(researcher.ResearchedItems, fromPlayer);
                anyItemResearched = anyItemResearched || researcher.AnyItemResearched();

                sacrificesResearcher.ResearchedCraftableItems.AddRange(researcher.ResearchedCraftableItems);
                sacrificesResearcher.ResearchedShimmeredItems.AddRange(researcher.ResearchedShimmeredItems);
            }

            if (HyperConfig.Instance.ShowResearchedCraftableItems)
                TextUtils.MessageResearchedCraftableItems(sacrificesResearcher.ResearchedCraftableItems);
            if (HyperConfig.Instance.ShowResearchedShimmeredItems)
                TextUtils.MessageResearchedShimmeredItems(sacrificesResearcher.ResearchedShimmeredItems);

            if (anyItemResearched)
            {
                sacrificesResearcher.SacrificedItems.Clear();
                SyncItemsWithTeam(sacrificesResearcher);
                SoundEngine.PlaySound(SoundID.ResearchComplete);
            }
            else if (sacrificesResearcher.AnyItemSacrificed()) SoundEngine.PlaySound(SoundID.MenuTick);
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
            researcher.ResearchItems(items, researchCraftable: HyperConfig.Instance.AutoResearchCraftableItems);
            AfterLocalResearch(researcher);
        }

        /// <summary>
        /// Sacrifices every unresearched item in the inventory  
        /// </summary>
        public void SacrificeInventory()
        {
            List<Item> itemToSacrifice = new();
            for (int slot = 0; slot < Main.InventorySlotsTotal; slot++)
            {
                Item item = Player.inventory[slot];

                if (item.IsAir || item.favorited ||
                    (!HyperConfig.Instance.SacrificeHotbarSlots && slot >= 0 && slot <= 9) ||
                    (!HyperConfig.Instance.SacrificeCoinsSlots && slot >= Main.InventoryCoinSlotsStart &&
                    slot < Main.InventoryCoinSlotsStart + Main.InventoryAmmoSlotsCount) ||
                    (!HyperConfig.Instance.SacrificeAmmoSlots && slot >= Main.InventoryAmmoSlotsStart &&
                    slot < Main.InventoryAmmoSlotsStart + Main.InventoryAmmoSlotsCount))
                {
                    continue;
                }

                itemToSacrifice.Add(item);
            }
            Researcher researcher = new();
            researcher.SacrificeItems(itemToSacrifice, researchCraftable: HyperConfig.Instance.AutoResearchCraftableItems);
            AfterLocalResearch(researcher);
        }

        public void ClearResearched()
        {
            bool anyItemCleaned = false;
            for (int slot = 0; slot < Main.InventorySlotsTotal; slot++)
            {
                Item item = Player.inventory[slot];
                if (item.favorited || item.IsAir || !Researcher.IsResearched(item.type)) continue;
                if (!HyperConfig.Instance.ClearHotbarSlots && slot >= 0 && slot <= 9) continue;
                if (!HyperConfig.Instance.ClearCoinsSlots && slot >= Main.InventoryCoinSlotsStart &&
                    slot < Main.InventoryCoinSlotsStart + Main.InventoryAmmoSlotsCount)
                {
                    continue;
                }

                if (!HyperConfig.Instance.ClearAmmoSlots && slot >= Main.InventoryAmmoSlotsStart &&
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
                if (item.IsAir && !item.favorited) continue;
                if (items.Contains(item.type)) item.TurnToAir();
            }
        }

        /// <summary>
        /// Tries to add to the <see cref="ResearchedTiles"/> the <see cref="Tile.TileType"/> that this item with <paramref name="itemId"/> places
        /// </summary>
        /// <returns>Has any tile been added to the <see cref="ResearchedTiles"/></returns>
        public bool TryAddToResearchedTiles(int itemId)
        {
            if (!ContentSamples.ItemsByType.TryGetValue(itemId, out Item item) ||
                item.createTile < TileID.Dirt || !Researcher.IsResearched(itemId))
            {
                return false;
            }

            foreach (int adj in ItemsUtils.GetAllAdjTiles(item.createTile))
                ResearchedTiles[adj] = true;
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
                if (item == null || item.IsAir || (item.shopSpecialCurrency != -1 && item.shopCustomPrice is null)) continue;
                Dictionary<int, int> currencyValues = ItemsUtils.GetCurrencyItemsAndValues(item.shopSpecialCurrency);
                if (currencyValues is not null && currencyValues.Keys.Any(Researcher.IsResearched))
                    toResearch.Add(item.type);
            }
            Researcher researcher = new();
            researcher.ResearchItems(toResearch, researchCraftable: HyperConfig.Instance.AutoResearchCraftableItems);
            AfterLocalResearch(researcher);
        }

        public void OnClientConfigChanged()
        {
            if (!ConfigOptions.OnlyOneItemNeeded && !ConfigOptions.ResearchShimmerableItems) return;

            Researcher researcher = new();
            for (int itemId = 1; itemId < ItemLoader.ItemCount; itemId++)
            {
                if (ConfigOptions.ResearchShimmerableItems)
                    researcher.TryResearchShimmeredItem(itemId);
                if (ConfigOptions.OnlyOneItemNeeded && Researcher.IsResearchable(itemId) &&
                    Researcher.GetResearchedCount(itemId) >= 1) researcher.ResearchItem(itemId);
            }
            AfterLocalResearch(researcher);
        }

        public void ResearchAndMessageCraftable()
        {
            Researcher researcher = new();
            researcher.ResearchCraftable();
            AfterLocalResearch(researcher);
        }

        public void ResearchAndMessageLoot(int itemId)
        {
            if (!ItemsUtils.IsLootItem(itemId)) return;

            IEnumerable<int> items = ItemsUtils.GetItemLoot(itemId);
            Researcher researcher = new();
            researcher.ResearchItems(items, researchCraftable: HyperConfig.Instance.AutoResearchCraftableItems);
            AfterLocalResearch(researcher);
        }

        public void AfterLocalResearch(Researcher researcher)
        {
            if (researcher.AnyItemResearched())
            {
                TextUtils.MessageResearcherResults(researcher);
                SoundEngine.PlaySound(SoundID.ResearchComplete);

                if (HyperConfig.Instance.AutoTrashAfterResearching)
                    TrashInventoryItems(researcher.AllResearchedItems);
            }
            else if (researcher.AnyItemSacrificed())
            {
                if (HyperConfig.Instance.ShowSacrifices)
                    TextUtils.MessageSacrifices(researcher.SacrificedItems);
                SoundEngine.PlaySound(SoundID.Research);
            }

            if (Main.netMode == NetmodeID.MultiplayerClient && (researcher.AnyItemResearched() || researcher.AnyItemSacrificed()))
                SyncItemsWithTeam(researcher);
        }
    }
}
