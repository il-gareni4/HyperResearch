using System;
using System.Collections.Generic;
using System.Linq;
using HyperResearch.Common.Configs;
using HyperResearch.Common.ModPlayers.Interfaces;
using HyperResearch.Common.Systems;
using HyperResearch.Utils;
using HyperResearch.Utils.Extensions;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace HyperResearch.Common.ModPlayers;

public class HyperPlayer : ModPlayer, IResearchPlayer
{
    /// <summary>Dictionary of researched tiles (contains <c>TileId</c> as Keys)</summary>
    public readonly Dictionary<int, bool> ResearchedTiles = [];

    /// <summary>Same as <see cref="Main.HoverItem" /> but not cloned</summary>
    private Item _hoverItem = new();

    /// <summary>Array of items in current shop</summary>
    /// <seealso cref="KeybindSystem.ResearchShopBind" />
    public Item[] CurrentShopItems { get; set; } = [];

    public int ItemsResearchedCount { get; private set; }

    public bool WasInAether { get; private set; }

    public void OnResearch(Item item)
    {
        TryAddToResearchedTiles(item.type);
        ItemsResearchedCount++;
    }

    public override void ProcessTriggers(TriggersSet triggersSet)
    {
        if (!Researcher.IsPlayerInJourneyMode) return;
#if DEBUG
        if (KeybindSystem.ForgetAllBind!.JustPressed)
        {
            Player.creativeTracker.Reset();
            ResearchedTiles.Clear();
            ItemsResearchedCount = 0;
        }

        if (KeybindSystem.ResearchAllBind!.JustPressed)
        {
            Researcher researcher = new()
            {
                AutoResearchCraftableItems = false,
                AutoResearchShimmerableItems = false,
                AutoResearchDecraftItems = false
            };
            researcher.ResearchItems(Enumerable.Range(1, ItemLoader.ItemCount - 1));
            TextUtils.MessageResearchedItems(researcher.DefaultResearchedItems);
        }
#endif
        if (KeybindSystem.SacrificeInventoryBind!.JustPressed) SacrificeInventory();
        if (KeybindSystem.ClearResearchedBind!.JustPressed) ClearResearched();
        if (KeybindSystem.ResearchCraftableBind!.JustPressed) ResearchAndMessageCraftable();
        if (KeybindSystem.MaxStackBind!.JustPressed
            && !Main.HoverItem.IsAir
            && (Main.HoverItem.tooltipContext == ItemSlot.Context.InventoryItem
                || Main.HoverItem.tooltipContext == ItemSlot.Context.InventoryCoin
                || Main.HoverItem.tooltipContext == ItemSlot.Context.InventoryAmmo)
            && Researcher.IsResearched(Main.HoverItem.type))
        {
            _hoverItem.stack = Main.HoverItem.maxStack;
            SoundEngine.PlaySound(SoundID.Grab);
        }

        if (KeybindSystem.ResearchLootBind!.JustPressed
            && !Main.HoverItem.IsAir
            && Researcher.IsResearched(Main.HoverItem.type)
            && ItemsUtils.CanOpenLootItem(Main.HoverItem.type))
            ResearchAndMessageLoot(Main.HoverItem.type);

        if (KeybindSystem.ResearchShopBind!.JustPressed
            && Player.TalkNPC is not null
            && Main.npcShop > 0
            && CurrentShopItems.Length > 0)
            ResearchShop(CurrentShopItems);

        if (KeybindSystem.ShareAllResearched!.JustPressed
            && Main.LocalPlayer.team >= 1
            && MainUtils.GetTeamMembers(Main.LocalPlayer.team, Main.myPlayer).Any())
        {
            int[] itemsToShare = Enumerable.Range(1, ItemLoader.ItemCount - 1).Where(Researcher.IsResearched).ToArray();
            SyncItemsWithTeam(itemsToShare, new Dictionary<int, int>());
            if (Main.LocalPlayer.team >= 1)
                Main.NewText(Language.GetTextValue("Mods.HyperResearch.Messages.SharedAllItems", itemsToShare.Length));
        }
    }

    public override void OnEnterWorld()
    {
        if (!Researcher.IsPlayerInJourneyMode) return;

        ItemsResearchedCount = 0;
        Researcher researcher = new();
        for (var itemId = 1; itemId < ItemLoader.ItemCount; itemId++)
        {
            TryAddToResearchedTiles(itemId);

            if (Researcher.IsResearched(itemId))
            {
                if (Researcher.GetSharedValue(itemId) == -1)
                    ItemsResearchedCount++;

                if (!ConfigOptions.BalanceShimmerAutoresearch || WasInAether)
                {
                    researcher.TryResearchShimmeredItem(itemId);
                    researcher.ResearchDecraftItems(itemId);
                }
            }
            else if (ConfigOptions.OnlyOneItemNeeded
                     && Researcher.IsResearchable(itemId)
                     && Researcher.GetResearchedCount(itemId) >= 1)
                researcher.ResearchItem(itemId);
        }

        AfterLocalResearch(researcher);
    }

    public override void PostUpdate()
    {
        if (!Researcher.IsPlayerInJourneyMode || Player != Main.LocalPlayer) return;

        if (HyperConfig.Instance.ResearchInventory) ResearchInventory();

        if (!WasInAether && Main.LocalPlayer.ZoneShimmer)
        {
            WasInAether = true;
            if (!ConfigOptions.BalanceShimmerAutoresearch) return;

            Researcher researcher = new();
            for (var itemId = 1; itemId < ItemLoader.ItemCount; itemId++)
            {
                researcher.TryResearchShimmeredItem(itemId);
                researcher.ResearchDecraftItems(itemId);
            }

            AfterLocalResearch(researcher);
        }
    }

    public override void SaveData(TagCompound tag)
    {
        if (Main.CurrentPlayer.difficulty != 3) return;
        tag["WasInAether"] = WasInAether;
    }

    public override void LoadData(TagCompound tag)
    {
        if (Main.CurrentPlayer.difficulty != 3) return;
        if (tag.TryGet("WasInAether", out bool wasInAether))
            WasInAether = wasInAether;
    }

    public override bool HoverSlot(Item[] inventory, int context, int slot)
    {
        if (!Researcher.IsPlayerInJourneyMode) return base.HoverSlot(inventory, context, slot);
        _hoverItem = inventory[slot];
        return base.HoverSlot(inventory, context, slot);
    }

    private void SyncItemsWithTeam(Researcher researcher)
    {
        SyncItemsWithTeam(researcher.AllNonSharedItems.ToArray(), researcher.DefaultSacrifices?.ToDictionary() ?? []);
    }

    private void SyncItemsWithTeam(int[] items, Dictionary<int, int> sacrifices)
    {
        if (Main.netMode != NetmodeID.MultiplayerClient
            || Main.LocalPlayer.team == 0
            || (items.Length == 0 && sacrifices.Count == 0)
            || (!ServerConfig.Instance.SyncResearchedItemsInOneTeam
                && !ServerConfig.Instance.SyncSacrificesInOneTeam)) return;
        if ((ServerConfig.Instance.SyncResearchedItemsInOneTeam
             && !ServerConfig.Instance.SyncSacrificesInOneTeam
             && items.Length == 0)
            || (!ServerConfig.Instance.SyncResearchedItemsInOneTeam
                && ServerConfig.Instance.SyncSacrificesInOneTeam
                && sacrifices.Count == 0))
            return;

        ModPacket packet = Mod.GetPacket();
        packet.Write((byte)NetMessageType.ShareItemsWithTeam);

        if (ServerConfig.Instance.SyncResearchedItemsInOneTeam && items.Length != 0)
            packet.Write(items);
        else packet.Write(Array.Empty<int>());

        if (ServerConfig.Instance.SyncSacrificesInOneTeam && sacrifices.Count != 0)
            packet.Write(sacrifices);
        else packet.Write(new Dictionary<int, int>());

        packet.Send(ignoreClient: Main.myPlayer);
    }

    public void SharedItems(int fromPlayer, int[] items, Dictionary<int, int> sacrifices)
    {
        TextUtils.MessageOtherPlayerResearchedItems(items.ToList(), fromPlayer);

        Researcher researcher = new();
        researcher.SacrificeItems(sacrifices, SacrificeSource.Shared);
        researcher.ResearchItems(items, ResearchSource.Shared);

        AfterLocalResearch(researcher, playerShared: fromPlayer);

        researcher.SacrificedItems[(int)SacrificeSource.Shared] = null;
        SyncItemsWithTeam(researcher);
    }

    /// <summary>
    ///     Automatically research items in the inventory if the total amount is equal to or more than required for research
    ///     If number of items is not enough then it does nothing
    /// </summary>
    private void ResearchInventory()
    {
        // Counting every item into a single dictionary
        // Used so that items of the same type and different slots are counted together
        Dictionary<int, int> items = [];
        for (var slot = 0; slot < Main.InventorySlotsTotal; slot++)
        {
            Item? item = Player.inventory[slot];
            if (item.IsAir) continue;
            items[item.type] = items.GetValueOrDefault(item.type, 0) + item.stack;
        }

        if (items.Count == 0) return;
        Researcher researcher = new();
        researcher.TryResearchItems(items);
        AfterLocalResearch(researcher);
    }

    /// <summary>
    ///     Sacrifices every unresearched item in the inventory
    /// </summary>
    public void SacrificeInventory()
    {
        List<Item> itemToSacrifice = [];
        for (var slot = 0; slot < Main.InventorySlotsTotal; slot++)
        {
            Item? item = Player.inventory[slot];

            if (item.IsAir || item.favorited
                           || (!HyperConfig.Instance.SacrificeHotbarSlots && IsHotbarSlot(slot))
                           || (!HyperConfig.Instance.SacrificeCoinsSlots && IsCoinSlot(slot))
                           || (!HyperConfig.Instance.SacrificeAmmoSlots && IsAmmoSlot(slot)))
                continue;

            itemToSacrifice.Add(item);
        }

        Researcher researcher = new();
        researcher.SacrificeItems(itemToSacrifice);
        AfterLocalResearch(researcher);
    }

    public void ClearResearched()
    {
        var anyItemCleaned = false;
        for (var slot = 0; slot < Main.InventorySlotsTotal; slot++)
        {
            Item? item = Player.inventory[slot];
            if (item.favorited || item.IsAir || !Researcher.IsResearched(item.type)
                || (!HyperConfig.Instance.ClearHotbarSlots && IsHotbarSlot(slot))
                || (!HyperConfig.Instance.ClearCoinsSlots && IsCoinSlot(slot))
                || (!HyperConfig.Instance.ClearAmmoSlots && IsAmmoSlot(slot)))
                continue;

            item.TurnToAir();
            anyItemCleaned = true;
        }

        if (anyItemCleaned) SoundEngine.PlaySound(SoundID.Grab);
    }

    private void TrashInventoryItems(int[] items)
    {
        if (!HyperConfig.Instance.AutoTrashAfterResearching) return;

        for (var slot = 0; slot < Main.InventorySlotsTotal; slot++)
        {
            Item? item = Player.inventory[slot];
            if (item.IsAir && !item.favorited) continue;
            if (items.Contains(item.type)) item.TurnToAir();
        }
    }

    /// <summary>
    ///     Tries to add to the <see cref="ResearchedTiles" /> the <see cref="Tile.TileType" /> that this item with
    ///     <paramref name="itemId" /> places
    /// </summary>
    /// <returns>Has any tile been added to the <see cref="ResearchedTiles" /></returns>
    private void TryAddToResearchedTiles(int itemId)
    {
        if (!ContentSamples.ItemsByType.TryGetValue(itemId, out Item? item)
            || item.createTile < TileID.Dirt
            || !Researcher.IsResearched(itemId))
            return;

        foreach (int adj in ItemsUtils.GetAllAdjTiles(item.createTile))
            ResearchedTiles[adj] = true;
    }

    /// <summary>
    ///     Researches the <paramref name="shop" />.
    ///     If the currency for which the item is being sold has not been researched then the item is skipped
    /// </summary>
    /// <param name="shop">Array of shop items</param>
    public void ResearchShop(Item[] shop)
    {
        List<int> toResearch = [];
        toResearch.AddRange(
            from item in shop
            where item is { IsAir: false }
                  && (item.shopSpecialCurrency == -1 || item.shopCustomPrice is not null)
            let currencyValues = ItemsUtils.GetCurrencyItemsAndValues(item.shopSpecialCurrency)
            where currencyValues is not null && currencyValues.Keys.Any(Researcher.IsResearched)
            select item.type
        );

        Researcher researcher = new();
        researcher.ResearchItems(toResearch);
        AfterLocalResearch(researcher);
    }

    public void OnClientConfigChanged()
    {
        if (!ConfigOptions.OnlyOneItemNeeded && !ConfigOptions.ResearchShimmerableItems) return;

        Researcher researcher = new();
        for (var itemId = 1; itemId < ItemLoader.ItemCount; itemId++)
        {
            if (Researcher.IsResearched(itemId))
            {
                if (!ConfigOptions.BalanceShimmerAutoresearch || WasInAether)
                {
                    researcher.TryResearchShimmeredItem(itemId);
                    researcher.ResearchDecraftItems(itemId);
                }
            }
            else if (ConfigOptions.OnlyOneItemNeeded && Researcher.IsResearchable(itemId) &&
                     Researcher.GetResearchedCount(itemId) >= 1)
                researcher.ResearchItem(itemId);
        }

        AfterLocalResearch(researcher);
    }

    public void ResearchAndMessageCraftable()
    {
        Researcher researcher = new();
        researcher.ResearchCraftable();
        AfterLocalResearch(researcher);
    }

    private void ResearchAndMessageLoot(int itemId)
    {
        if (!ItemsUtils.IsLootItem(itemId)) return;

        IEnumerable<int> items = ItemsUtils.GetItemLoot(itemId);
        Researcher researcher = new();
        researcher.ResearchItems(items);
        AfterLocalResearch(researcher);
    }

    internal void AfterLocalResearch(Researcher researcher, bool playSacrificeSounds = true, int playerShared = -1)
    {
        TextUtils.MessageResearcherResults(researcher, playerShared);
        if (researcher.AnyItemResearched)
        {
            SoundEngine.PlaySound(SoundID.ResearchComplete);
            TrashInventoryItems(researcher.AllResearchedItems.ToArray());
        }
        else if (researcher.DefaultSacrifices is { Count: > 0 } && playSacrificeSounds)
            SoundEngine.PlaySound(SoundID.Research);
        else if (researcher.SharedSacrifices is { Count: > 0 }) SoundEngine.PlaySound(SoundID.MenuTick);

        SyncItemsWithTeam(researcher);
    }

    private static bool IsHotbarSlot(int slot) => slot is >= 0 and <= 9;

    private static bool IsCoinSlot(int slot) => slot is >= Main.InventoryCoinSlotsStart
        and < Main.InventoryCoinSlotsStart + Main.InventoryAmmoSlotsCount;

    private static bool IsAmmoSlot(int slot) => slot is >= Main.InventoryAmmoSlotsStart
        and < Main.InventoryAmmoSlotsStart + Main.InventoryAmmoSlotsCount;
}