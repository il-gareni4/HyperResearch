using HyperResearch.Common.Systems;
using HyperResearch.Utils;
using System;
using System.Linq;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace HyperResearch.Common.ModPlayers;

[Flags]
public enum BuffState : byte
{
    Researched = 1,
    Enabled = 2,
}

public static class BuffStateExtension
{
    public static void Toggle(this ref BuffState buffState, BuffState flag) => buffState ^= flag;
    public static void Enable(this ref BuffState buffState, BuffState flag) => buffState |= flag;
    public static void Disable(this ref BuffState buffState, BuffState flag) => buffState &= ~flag;
}

public class BuffPlayer : ModPlayer
{
    public BuffState[] Buffs { get; private set; } = [];

    public override void ProcessTriggers(TriggersSet triggersSet)
    {
        if (!Researcher.IsPlayerInJourneyMode) return;

        if (Main.HoverItem.tooltipContext == ItemSlot.Context.CreativeInfinite
            && BuffUtils.IsBuffPotion(Main.HoverItem)
            && Buffs[Main.HoverItem.buffType].HasFlag(BuffState.Researched)
            && KeybindSystem.EnableDisableBuffBind.JustPressed)
        {
            Buffs[Main.HoverItem.buffType].Toggle(BuffState.Enabled);
        }
        if (KeybindSystem.ForgetAllBind.JustPressed)
            Buffs = new BuffState[BuffLoader.BuffCount];
    }

    public override void OnEnterWorld()
    {
        if (!Researcher.IsPlayerInJourneyMode) return;

        if (Buffs.Length == 0)
            Buffs = new BuffState[BuffLoader.BuffCount];
        for (int itemId = 1; itemId < ItemLoader.ItemCount; itemId++)
        {
            Item item = ContentSamples.ItemsByType[itemId];
            if (Researcher.IsResearched(item.type)) ResearchItem(item, false);
        }
    }

    public override void SaveData(TagCompound tag)
    {
        if (Main.CurrentPlayer.difficulty != 3) return;
        tag["buffsEnabled"] = Buffs.Select(f => f.HasFlag(BuffState.Enabled)).ToArray();
    }

    public override void Unload()
    {
        if (Main.CurrentPlayer.difficulty != 3) return;
        Buffs = [];
    }

    public override void LoadData(TagCompound tag)
    {
        if (Main.CurrentPlayer.difficulty != 3) return;

        Buffs = new BuffState[BuffLoader.BuffCount];
        if (tag.TryGet("buffsEnabled", out bool[] enabled))
        {
            for (int buffId = 0; buffId < Math.Min(enabled.Length, Buffs.Length); buffId++)
                if (enabled[buffId]) Buffs[buffId].Enable(BuffState.Enabled);
        }
    }

    public override void PostUpdateBuffs()
    {
        if (!Researcher.IsPlayerInJourneyMode) return;
        for (int i = 1; i < Buffs.Length; i++)
            if (Buffs[i].HasFlag(BuffState.Researched) && Buffs[i].HasFlag(BuffState.Enabled))
                Player.AddBuff(i, 1);
    }

    public void ResearchItem(Item item, bool enabled = true)
    {
        if (item.buffType != 0)
        {
            Buffs[item.buffType].Enable(BuffState.Researched);
            if (enabled && BuffUtils.IsBuffPotion(item)) Buffs[item.buffType].Enable(BuffState.Enabled);
        }
    }
}
