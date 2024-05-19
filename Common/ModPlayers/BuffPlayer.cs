using System.Collections.Generic;
using System.Linq;
using HyperResearch.Common.Configs;
using HyperResearch.Common.ModPlayers.Interfaces;
using HyperResearch.Common.Systems;
using HyperResearch.Utils;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace HyperResearch.Common.ModPlayers;

public class BuffPlayer : ModPlayer, IResearchPlayer
{
    public DictionaryAnalysisData<int, bool> Buffs { get; } = [];

    public void OnResearch(Item item) => ResearchItem(item);

    public override void ProcessTriggers(TriggersSet triggersSet)
    {
        if (!Researcher.IsPlayerInJourneyMode) return;

        if (Main.HoverItem.tooltipContext == ItemSlot.Context.CreativeInfinite
            && KeybindSystem.EnableDisableBuffBind!.JustPressed)
            ToggleBuffItem(Main.HoverItem);
#if DEBUG
        if (KeybindSystem.ForgetAllBind!.JustPressed)
            Buffs.Clear();
#endif
    }

    public override void OnEnterWorld()
    {
        if (!Researcher.IsPlayerInJourneyMode) return;

        for (var itemId = 1; itemId < ItemLoader.ItemCount; itemId++)
        {
            Item item = ContentSamples.ItemsByType[itemId];
            if (Researcher.IsResearched(item.type)) ResearchItem(item, false);
        }
    }

    public override void SaveData(TagCompound tag)
    {
        if (Main.CurrentPlayer.difficulty != 3) return;
        tag["buffsEnabled"] = Buffs.Where(kv => kv.Value).Select(kv => kv.Key).ToArray();
    }

    public override void Unload()
    {
        if (Main.CurrentPlayer.difficulty != 3) return;
        Buffs.Clear();
    }

    public override void LoadData(TagCompound tag)
    {
        if (Main.CurrentPlayer.difficulty != 3) return;

        if (tag.TryGet("buffsEnabled", out int[] enabled))
        {
            foreach (int buffId in enabled)
            {
                if (buffId < BuffLoader.BuffCount)
                    Buffs[buffId] = true;
            }
        }
    }

    public override void PostUpdateBuffs()
    {
        if (!Researcher.IsPlayerInJourneyMode || !ConfigOptions.UseResearchedPotionsBuff) return;

        foreach ((int buffId, bool enabled) in Buffs)
        {
            if (enabled)
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

    private void ToggleBuffItem(Item item)
    {
        if (!Buffs.TryGetValue(Main.HoverItem.buffType, out bool enabled)) return;

        if (BuffUtils.IsABuffPotion(item)) Buffs[Main.HoverItem.buffType] = !enabled;
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