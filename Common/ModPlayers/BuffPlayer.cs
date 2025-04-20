using System.Collections.Generic;
using System.Linq;
using HyperResearch.Common.Configs;
using HyperResearch.Common.ModPlayers.Interfaces;
using HyperResearch.Common.Systems;
using HyperResearch.Utils;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace HyperResearch.Common.ModPlayers;

public class BuffPlayer : ModPlayer, IResearchPlayer
{
    private readonly List<string> _buffsOfDisabledMods = [];
    public Dictionary<int, bool> Buffs { get; } = [];
    public IEnumerable<int> EnabledBuffs => Buffs.Where(kv => kv.Value).Select(kv => kv.Key);
    public bool? CurrentSetState { get; private set; } = null;

    public void OnResearch(Item item) => ResearchItem(item);

    public override void ProcessTriggers(TriggersSet triggersSet)
    {
        if (!Researcher.IsPlayerInJourneyMode) return;

        if (KeybindSystem.EnableDisableBuffBind!.JustReleased)
            CurrentSetState = null;

        if (ConfigOptions.UseResearchedPotionsBuff &&
            Main.HoverItem.tooltipContext == ItemSlot.Context.CreativeInfinite &&
            Buffs.TryGetValue(Main.HoverItem.buffType, out bool enabled))
        {
            if (KeybindSystem.EnableDisableBuffBind!.JustPressed)
                CurrentSetState = !enabled;

            if (KeybindSystem.EnableDisableBuffBind!.Current && CurrentSetState.HasValue)
                SetBuff(Main.HoverItem, CurrentSetState.Value);
        }
#if DEBUG
        if (KeybindSystem.ForgetAllBind!.JustPressed)
            Buffs.Clear();
#endif
    }

    public override void OnEnterWorld()
    {
        if (!Researcher.IsPlayerInJourneyMode) return;

        foreach (int itemId in Researcher.ReseachedItems)
            ResearchItem(ContentSamples.ItemsByType[itemId], false);
    }

    public override void SaveData(TagCompound tag)
    {
        if (!Researcher.IsPlayerInJourneyMode) return;

        tag["enabledVanillaBuffs"] =
            EnabledBuffs
            .Where(buffId => buffId < BuffID.Count)
            .ToArray();
        tag["enabledModBuffs"] =
            EnabledBuffs
            .Where(buffId => buffId >= BuffID.Count)
            .Select(buffId => BuffLoader.GetBuff(buffId).FullName)
            .Concat(_buffsOfDisabledMods)
            .ToArray();
    }

    public override void Unload()
    {
        if (!Researcher.IsPlayerInJourneyMode) return;

        Buffs.Clear();
        _buffsOfDisabledMods.Clear();
    }

    public override void LoadData(TagCompound tag)
    {
        if (!Researcher.IsPlayerInJourneyMode) return;

        // Migrate from 1.0 to 1.1
        if (tag.TryGet("buffsEnabled", out int[] enabled))
        {
            foreach (int buffId in enabled.Where(buffId => buffId < BuffID.Count))
                Buffs[buffId] = true;
        }

        // Current version
        if (tag.TryGet("enabledVanillaBuffs", out int[] enabledVanillaBuffs))
        {
            foreach (int buffId in enabledVanillaBuffs.Where(buffId => buffId < BuffID.Count && BuffUtils.IsAcceptableBuff(buffId)))
                Buffs[buffId] = true;
        }

        if (tag.TryGet("enabledModBuffs", out string[] enabledModBuffs))
        {
            foreach (string buffFullName in enabledModBuffs)
            {
                if (ModContent.TryFind(buffFullName, out ModBuff modBuff))
                {
                    if (BuffUtils.IsAcceptableBuff(modBuff.Type))
                        Buffs[modBuff.Type] = true;
                }
                else
                {
                    string[] buffFullNameSplit = buffFullName.Split('/');
                    if (buffFullNameSplit.Length == 2 &&
                        !ModLoader.HasMod(buffFullNameSplit[0]) &&
                        _buffsOfDisabledMods.Count < 128) // To prevent save file to be too big
                    {
                        _buffsOfDisabledMods.Add(buffFullName);
                    }
                }
            }
        }
    }

    public override void PostUpdateBuffs()
    {
        if (!Researcher.IsPlayerInJourneyMode || !ConfigOptions.UseResearchedPotionsBuff) return;

        foreach (int buffId in EnabledBuffs)
        {
            if (buffId < 0 || buffId >= BuffLoader.BuffCount)
            {
                Buffs.Remove(buffId);
                continue;
            }
            Player.AddBuff(buffId, 2);
        }
    }

    private void ResearchItem(Item item, bool enable = true)
    {
        if (item.buffType == 0) return;

        Buffs.TryAdd(
            item.buffType,
            BuffUtils.IsABuffPotion(item) && enable && HyperConfig.Instance.PotionBuffEnabledByDefault
        );
    }

    private void SetBuff(Item item, bool enabled)
    {
        if (!Buffs.TryGetValue(item.buffType, out bool wasEnabled) || wasEnabled == enabled) return;

        if (BuffUtils.IsABuffPotion(item)) Buffs[item.buffType] = enabled;
        else if (BuffUtils.IsAFlask(item)) ToggleGroup(item.buffType, BuffID.Sets.IsAFlaskBuff);
        else if (BuffUtils.IsAFood(item)) ToggleGroup(item.buffType, BuffID.Sets.IsWellFed);
    }

    private void ToggleGroup(int buffId, bool[] set)
    {
        if (!Buffs.TryGetValue(Main.HoverItem.buffType, out bool enabled)) return;

        if (!enabled)
        {
            for (var id = 1; id < set.Length; id++)
            {
                if (set[id])
                    Buffs[id] = false;
            }
        }

        Buffs[buffId] = !enabled;
    }
}