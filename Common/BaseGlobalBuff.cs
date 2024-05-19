using System.Diagnostics.CodeAnalysis;
using HyperResearch.Common.ModPlayers;
using HyperResearch.Utils;
using Terraria;
using Terraria.ModLoader;

namespace HyperResearch.Common;

[SuppressMessage("ReSharper", "UnusedType.Global")]
internal class BaseGlobalBuff : GlobalBuff
{
    public override bool RightClick(int type, int buffIndex)
    {
        if (!Researcher.IsPlayerInJourneyMode
            || !Main.LocalPlayer.TryGetModPlayer(out BuffPlayer buffPlayer)
            || !buffPlayer.Buffs.TryGetValue(type, out bool _)) return true;

        buffPlayer.Buffs[type] = false;

        return true;
    }
}