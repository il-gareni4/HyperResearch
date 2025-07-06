using System;
using System.Collections.Generic;
using System.Linq;
using HyperResearch.Common.Configs;
using HyperResearch.Common.Configs.Enums;
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
    public event Action? OnTeamChanged;

    /// <summary>Dictionary of researched tiles (contains <c>TileId</c> as Keys)</summary>
    public readonly Dictionary<int, bool> ResearchedTiles = [];

    /// <summary>Same as <see cref="Main.HoverItem" /> but not cloned</summary>
    private Item _hoverItem = new();
    private int _previousTeam;
    private bool _wasInAether;

    /// <summary>Array of items in current shop</summary>
    /// <seealso cref="KeybindSystem.ResearchShopBind" />
    public Item[] CurrentShopItems { get; set; } = [];

    public int ItemsResearchedCount { get; private set; }

    public bool WasInAether
    {
        get => _wasInAether; private set
        {
            if (!_wasInAether && value &&
                ConfigOptions.BalanceShimmerAutoresearch &&
                (BaseConfig.Instance.ShimmerResearchMode == ShimmerResearchMode.OnResearch ||
                 BaseConfig.Instance.DecraftsResearchMode == DecraftsResearchMode.OnResearch))
            {
                Researcher researcher = new();
                foreach (int itemId in Researcher.ReseachedItems)
                {
                    if (BaseConfig.Instance.ShimmerResearchMode == ShimmerResearchMode.OnResearch)
                        researcher.TryResearchShimmerItem(itemId);
                    if (BaseConfig.Instance.DecraftsResearchMode == DecraftsResearchMode.OnResearch)
                        researcher.TryResearchDecraftItems(itemId);
                }
                researcher.ProcessResearched(
                    AutoResearchCraftable,
                    BaseConfig.Instance.ShimmerResearchMode == ShimmerResearchMode.OnResearch,
                    BaseConfig.Instance.DecraftsResearchMode == DecraftsResearchMode.OnResearch
                );

                AfterLocalResearch(researcher);
            }
            _wasInAether = value;
        }
    }

    public bool AutoResearchCraftable => BaseConfig.Instance.CraftablesResearchMode == CraftablesResearchMode.OnResearch;
    public bool CanShimmerResearch => !ConfigOptions.BalanceShimmerAutoresearch || WasInAether;
    public bool AutoResearchShimmer => BaseConfig.Instance.ShimmerResearchMode == ShimmerResearchMode.OnResearch && CanShimmerResearch;
    public bool AutoResearchDecraft => BaseConfig.Instance.DecraftsResearchMode == DecraftsResearchMode.OnResearch && CanShimmerResearch;

    public void OnResearch(Item item)
    {
        TryAddToResearchedTiles(item.type);
        ItemsResearchedCount++;
    }

    public override void UpdateAutopause()
    {
        if (!Researcher.IsPlayerInJourneyMode) return;
        if (Main.drawingPlayerChat || Main.editSign || Main.editChest || Main.blockInput)
            return;

#if DEBUG
        if (KeybindSystem.ForgetAllBind!.JustPressed)
            ForgetAllAction();
        if (KeybindSystem.ResearchAllBind!.JustPressed)
            ResearchAllAction();
        if (KeybindSystem.ForgetAetherBind!.JustPressed)
            ForgetAetherAction();
#endif
        if (KeybindSystem.SacrificeInventoryBind!.JustPressed)
            SacrificeInventoryAction();
        if (KeybindSystem.ClearResearchedBind!.JustPressed)
            ClearResearched();
        if (KeybindSystem.ResearchCraftableBind!.JustPressed)
            ResearchCraftableAction();
        if (KeybindSystem.ResearchShimmerBind!.JustPressed)
            ResearchShimmerItemsAction();
        if (KeybindSystem.ResearchDecraftsBind!.JustPressed)
            ResearchDecraftItemsAction();
        if (KeybindSystem.MaxStackBind!.JustPressed)
            MaxStackAction();
        if (KeybindSystem.ResearchLootBind!.JustPressed)
            ResearchLootAction();
    }

    public override void ProcessTriggers(TriggersSet triggersSet)
    {
        if (!Researcher.IsPlayerInJourneyMode) return;
#if DEBUG
        if (KeybindSystem.ForgetAllBind!.JustPressed)
            ForgetAllAction();
        if (KeybindSystem.ResearchAllBind!.JustPressed)
            ResearchAllAction();
        if (KeybindSystem.ForgetAetherBind!.JustPressed)
            ForgetAetherAction();
#endif
        if (KeybindSystem.SacrificeInventoryBind!.JustPressed)
            SacrificeInventoryAction();
        if (KeybindSystem.ClearResearchedBind!.JustPressed)
            ClearResearched();
        if (KeybindSystem.ResearchCraftableBind!.JustPressed)
            ResearchCraftableAction();
        if (KeybindSystem.ResearchShimmerBind!.JustPressed)
            ResearchShimmerItemsAction();
        if (KeybindSystem.ResearchDecraftsBind!.JustPressed)
            ResearchDecraftItemsAction();
        if (KeybindSystem.MaxStackBind!.JustPressed)
            MaxStackAction();
        if (KeybindSystem.ResearchLootBind!.JustPressed)
            ResearchLootAction();
        if (KeybindSystem.ShareAllResearched!.JustPressed)
            ShareResearchedItemsAction();
    }

#if DEBUG
    public void ForgetAllAction()
    {
        Player.creativeTracker.Reset();
        ResearchedTiles.Clear();
        ItemsResearchedCount = 0;
    }

    public void ResearchAllAction()
    {
        Researcher researcher = new();
        researcher.ResearchItems(Enumerable.Range(1, ItemLoader.ItemCount - 1));
        TextUtils.MessageResearchedItems(researcher.DefaultResearchedItems);
    }

    public void ForgetAetherAction()
    {
        _wasInAether = false;
    }
#endif

    public void SacrificeInventoryAction()
    {
        if (BaseConfig.Instance.ResearchMode == ResearchMode.None ||
            BaseConfig.Instance.ResearchMode == ResearchMode.AutoSacrificeAlways)
            return;

        SacrificeInventory();
    }

    public void ResearchCraftableAction()
    {
        if (BaseConfig.Instance.CraftablesResearchMode == CraftablesResearchMode.None)
            return;

        ResearchCraftable();
    }

    public void ResearchShimmerItemsAction()
    {
        if (!ConfigOptions.BalanceShimmerAutoresearch || WasInAether)
            ResearchShimmerItems();
    }

    public void ResearchDecraftItemsAction()
    {
        if (!ConfigOptions.BalanceShimmerAutoresearch || WasInAether)
            ResearchDecraftItems();
    }

    public void MaxStackAction()
    {
        if (Main.HoverItem.IsAir || !Researcher.IsResearched(Main.HoverItem.type)) return;

        if (Main.HoverItem.tooltipContext != ItemSlot.Context.InventoryItem &&
            Main.HoverItem.tooltipContext != ItemSlot.Context.InventoryCoin &&
            Main.HoverItem.tooltipContext != ItemSlot.Context.InventoryAmmo)
        {
            return;
        }

        _hoverItem.stack = Main.HoverItem.maxStack;
        SoundEngine.PlaySound(SoundID.Grab);
    }

    public void ResearchLootAction()
    {
        if (Main.HoverItem.IsAir ||
            !Researcher.IsResearched(Main.HoverItem.type) ||
            !ItemsUtils.CanOpenLootItem(Main.HoverItem.type))
        {
            return;
        }

        ResearchAndMessageLoot(Main.HoverItem.type);
    }

    public void ResearchShopAction()
    {
        if (Player.TalkNPC is null ||
            Main.npcShop <= 0 ||
            CurrentShopItems.Length == 0)
        {
            return;
        }

        ResearchShop(CurrentShopItems);
    }

    public void ShareResearchedItemsAction()
    {
        if (Main.LocalPlayer.team < 1 ||
            !MainUtils.GetTeamMembers(Main.LocalPlayer.team, Main.myPlayer).Any())
        {
            return;
        }

        int[] itemsToShare = [.. Researcher.ReseachedItems];
        SyncItemsWithTeam(itemsToShare, []);
        if (Main.LocalPlayer.team >= 1)
            Main.NewText(Language.GetTextValue("Mods.HyperResearch.Messages.SharedAllItems", itemsToShare.Length));
    }

    public override void OnEnterWorld()
    {
        if (!Researcher.IsPlayerInJourneyMode) return;

        ItemsResearchedCount = 0;
        Researcher researcher = new();
        if (AutoResearchCraftable)
            researcher.ResearchCraftable();
        for (var itemId = 1; itemId < ItemLoader.ItemCount; itemId++)
        {
            if (Researcher.IsResearched(itemId))
            {
                TryAddToResearchedTiles(itemId);
                if (AutoResearchShimmer)
                    researcher.TryResearchShimmerItem(itemId);
                if (AutoResearchDecraft)
                    researcher.TryResearchDecraftItems(itemId);

                if (Researcher.GetSharedValue(itemId) == -1)
                    ItemsResearchedCount++;
            }
            else if (ConfigOptions.OnlyOneItemNeeded &&
                Researcher.IsResearchable(itemId) &&
                Researcher.GetResearchedCount(itemId) >= 1)
            {
                researcher.ResearchItem(itemId);
            }
        }
        researcher.ProcessResearched(this);

        AfterLocalResearch(researcher);
    }

    public override void PreUpdate()
    {
        if (!Researcher.IsPlayerInJourneyMode || Player != Main.LocalPlayer) return;

        if (_previousTeam != Player.team)
            OnTeamChanged?.Invoke();
    }

    public override void PostUpdate()
    {
        if (!Researcher.IsPlayerInJourneyMode || Player != Main.LocalPlayer) return;

        if (BaseConfig.Instance.ResearchMode == ResearchMode.FavouriteSacrifice)
            SacrificeInventory(true);
        if (BaseConfig.Instance.ResearchMode == ResearchMode.Favourite)
            ResearchInventory(true);
        if (BaseConfig.Instance.ResearchMode == ResearchMode.AutoResearch)
            ResearchInventory();
        if (BaseConfig.Instance.ResearchMode == ResearchMode.AutoSacrificeAlways)
            SacrificeInventory(silent: true);

        if (!WasInAether && Main.LocalPlayer.ZoneShimmer)
                WasInAether = true;

        _previousTeam = Player.team;
    }

    public override bool OnPickup(Item item)
    {
        if (!Researcher.IsPlayerInJourneyMode)
            return true;
        if (Researcher.IsResearched(item.type))
            return !BaseConfig.Instance.AutoTrashResearched;
        if (BaseConfig.Instance.ResearchMode != ResearchMode.AutoSacrificeOnPickup)
            return true;

        Researcher researcher = new();
        researcher.SacrificeItem(item);
        researcher.ProcessResearched(this);
        AfterLocalResearch(researcher, false);

        return true;
    }

    public override void SaveData(TagCompound tag)
    {
        if (!Researcher.IsPlayerInJourneyMode) return;
        tag["WasInAether"] = WasInAether;
    }

    public override void LoadData(TagCompound tag)
    {
        if (!Researcher.IsPlayerInJourneyMode) return;
        if (tag.TryGet("WasInAether", out bool wasInAether))
            _wasInAether = wasInAether;
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
        researcher.ProcessResearched(this);

        AfterLocalResearch(researcher, playerShared: fromPlayer);
    }

    private void ResearchInventory(bool favouriteOnly = false)
    {
        Dictionary<int, int> items = [];
        for (var slot = 0; slot < Main.InventorySlotsTotal; slot++)
        {
            Item? item = Player.inventory[slot];
            if (item.IsAir) continue;
            if (favouriteOnly && !item.favorited) continue;
            items[item.type] = items.GetValueOrDefault(item.type, 0) + item.stack;
        }

        if (items.Count == 0) return;

        Researcher researcher = new();
        researcher.ResearchItemsWithCount(items);
        researcher.ProcessResearched(this);
        AfterLocalResearch(researcher);
    }

    /// <summary>
    ///     Sacrifices every unresearched item in the inventory
    /// </summary>
    public void SacrificeInventory(bool favouriteOnly = false, bool silent = false)
    {
        List<Item> itemToSacrifice = [];
        for (var slot = 0; slot < Main.InventorySlotsTotal; slot++)
        {
            Item? item = Player.inventory[slot];
            if (item.IsAir)
                continue;
            if (favouriteOnly) 
                if (!item.favorited)
                    continue;
            else if (item.favorited ||
                    (!BaseConfig.Instance.SacrificeHotbarSlots && IsHotbarSlot(slot)) ||
                    (!BaseConfig.Instance.SacrificeCoinsSlots && IsCoinSlot(slot)) ||
                    (!BaseConfig.Instance.SacrificeAmmoSlots && IsAmmoSlot(slot)))
                continue;

            itemToSacrifice.Add(item);
        }

        if (itemToSacrifice.Count == 0) return;

        Researcher researcher = new();
        researcher.SacrificeItems(itemToSacrifice);
        researcher.ProcessResearched(this);
        AfterLocalResearch(researcher, !silent);
    }

    public void ClearResearched()
    {
        var anyItemCleaned = false;
        for (var slot = 0; slot < Main.InventorySlotsTotal; slot++)
        {
            Item? item = Player.inventory[slot];
            if (item.favorited || item.IsAir || !Researcher.IsResearched(item.type) ||
                (!BaseConfig.Instance.ClearHotbarSlots && IsHotbarSlot(slot)) ||
                (!BaseConfig.Instance.ClearCoinsSlots && IsCoinSlot(slot)) ||
                (!BaseConfig.Instance.ClearAmmoSlots && IsAmmoSlot(slot)))
                continue;

            item.TurnToAir();
            anyItemCleaned = true;
        }

        if (anyItemCleaned) SoundEngine.PlaySound(SoundID.Grab);
    }

    private void TrashInventoryItems(int[] items)
    {
        for (var slot = 0; slot < Main.InventorySlotsTotal; slot++)
        {
            Item? item = Player.inventory[slot];
            if (item.IsAir || item.favorited || !Researcher.IsResearched(item.type) ||
                (!BaseConfig.Instance.ClearHotbarSlots && IsHotbarSlot(slot)) ||
                (!BaseConfig.Instance.ClearCoinsSlots && IsCoinSlot(slot)) ||
                (!BaseConfig.Instance.ClearAmmoSlots && IsAmmoSlot(slot)))
                continue;
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
        List<int> toResearch = [..
            from item in shop
            where item is { IsAir: false }
                  && (item.shopSpecialCurrency == -1 || item.shopCustomPrice is not null)
            let currencyValues = ItemsUtils.GetCurrencyItemsAndValues(item.shopSpecialCurrency)
            where currencyValues is not null && currencyValues.Keys.Any(Researcher.IsResearched)
            select item.type
        ];

        Researcher researcher = new();
        researcher.ResearchItems(toResearch);
        researcher.ProcessResearched(this);
        AfterLocalResearch(researcher);
    }

    public void OnClientConfigChanged()
    {
        if (!ConfigOptions.OnlyOneItemNeeded &&
            BaseConfig.Instance.ShimmerResearchMode != ShimmerResearchMode.OnResearch &&
            BaseConfig.Instance.DecraftsResearchMode != DecraftsResearchMode.OnResearch)
            return;

        Researcher researcher = new();
        if (AutoResearchCraftable)
            researcher.ResearchCraftable();
        for (var itemId = 1; itemId < ItemLoader.ItemCount; itemId++)
        {
            if (Researcher.IsResearched(itemId))
            {
                if (AutoResearchShimmer)
                    researcher.TryResearchShimmerItem(itemId);
                if (AutoResearchDecraft)
                    researcher.TryResearchDecraftItems(itemId);
            }
            else if (ConfigOptions.OnlyOneItemNeeded &&
                Researcher.IsResearchable(itemId) &&
                Researcher.GetResearchedCount(itemId) >= 1)
            {
                researcher.ResearchItem(itemId);
            }
        }
        researcher.ProcessResearched(this);

        AfterLocalResearch(researcher);
    }

    public void ResearchCraftable()
    {
        Researcher researcher = new();
        researcher.ResearchCraftable();
        researcher.ProcessResearched(true, AutoResearchShimmer, AutoResearchDecraft);
        AfterLocalResearch(researcher);
    }

    public void ResearchShimmerItems()
    {
        Researcher researcher = new();
        foreach (int itemId in Researcher.ReseachedItems)
            researcher.TryResearchShimmerItem(itemId);
        researcher.ProcessResearched(AutoResearchCraftable, true, AutoResearchDecraft);
        AfterLocalResearch(researcher);
    }

    public void ResearchDecraftItems()
    {
        Researcher researcher = new();
        foreach (int itemId in Researcher.ReseachedItems)
            researcher.TryResearchDecraftItems(itemId);
        researcher.ProcessResearched(AutoResearchCraftable, AutoResearchShimmer, true);
        AfterLocalResearch(researcher);
    }

    private void ResearchAndMessageLoot(int itemId)
    {
        if (!ItemsUtils.IsLootItem(itemId)) return;

        IEnumerable<int> items = ItemsUtils.GetItemLoot(itemId);
        Researcher researcher = new();
        researcher.ResearchItems(items);
        researcher.ProcessResearched(this);
        AfterLocalResearch(researcher);
    }

    internal void AfterLocalResearch(Researcher researcher, bool playSacrificeSounds = true, int playerShared = -1)
    {
        if (researcher is { AnyItemResearched: false, AnyItemSacrificed: false }) return;

        TextUtils.MessageResearcherResults(researcher, playerShared);
        if (researcher.AnyItemResearched)
        {
            SoundEngine.PlaySound(SoundID.ResearchComplete);
            if (BaseConfig.Instance.AutoTrashAfterResearching)
                TrashInventoryItems([.. researcher.AllResearchedItems]);
        }
        else if (researcher.DefaultSacrifices is { Count: > 0 } && playSacrificeSounds)
            SoundEngine.PlaySound(SoundID.Research);
        else if (researcher.SharedSacrifices is { Count: > 0 })
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            researcher.SacrificedItems[(int)SacrificeSource.Shared] = null;
        }

        SyncItemsWithTeam(researcher);
    }

    private static bool IsHotbarSlot(int slot) => slot is >= 0 and <= 9;

    private static bool IsCoinSlot(int slot) => slot is >= Main.InventoryCoinSlotsStart
        and < Main.InventoryCoinSlotsStart + Main.InventoryAmmoSlotsCount;

    private static bool IsAmmoSlot(int slot) => slot is >= Main.InventoryAmmoSlotsStart
        and < Main.InventoryAmmoSlotsStart + Main.InventoryAmmoSlotsCount;
}