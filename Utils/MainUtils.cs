using System.Collections.Generic;
using Terraria;

namespace HyperResearch.Utils
{
    public static class MainUtils
    {
        public static IEnumerable<int> GetTeamMembers(int teamId, int ignorePlayer = -1)
        {
            for (int i = 0; i < Main.maxNetPlayers; i++)
            {
                if (Main.player[i].active && i != ignorePlayer && Main.player[i].team == teamId)
                    yield return i;
            }
        }
    }
}
