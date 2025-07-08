using System;
using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ID;
using Terraria.ModLoader.Config;

namespace HyperResearch.Common.Configs;

public class CheatsConfig : ModConfig
{
    [LabelArgs(ItemID.AlphabetStatue1)]
    [DefaultValue(false)]
    public bool OnlyOneItemNeeded;
    public Dictionary<ItemDefinition, uint> ItemResearchCountOverride = [];

    public event Action Changed;

    public static CheatsConfig Instance { get; private set; }

    public override ConfigScope Mode => ConfigScope.ClientSide;

    public override void OnLoaded() => Instance = this;

    public override void OnChanged() => Changed?.Invoke();
}
