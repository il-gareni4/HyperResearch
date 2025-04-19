using System;
using Terraria;
using Terraria.ModLoader;

namespace HyperResearch.Common.Systems;

public class HooksSystem : ModSystem
{
    internal static event Action? WorldLoaded;
    internal static event Action? WorldUnloaded;

    public override void OnWorldLoad() => WorldLoaded?.Invoke();

    public override void OnWorldUnload() => WorldUnloaded?.Invoke();
}
