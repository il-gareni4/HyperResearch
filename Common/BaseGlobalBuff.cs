using HyperResearch.Common.ModPlayers;
using HyperResearch.Utils;
using Terraria;
using Terraria.ModLoader;

namespace HyperResearch.Common;

internal class BaseGlobalBuff : GlobalBuff
{
    public override bool RightClick(int type, int buffIndex)
    {
        if (!Researcher.IsPlayerInJourneyMode()) return true;
        if (!Main.LocalPlayer.TryGetModPlayer(out BuffPlayer buffPlayer)) return true;
        if (!buffPlayer.Buffs[type].HasFlag(BuffState.Researched)) return true;

        buffPlayer.Buffs[type].Disable(BuffState.Enabled);

        return true;
    }
}
