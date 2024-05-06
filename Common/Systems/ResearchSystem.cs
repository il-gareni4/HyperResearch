using HyperResearch.Common.Configs;
using HyperResearch.Utils;
using System.Collections.Generic;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace HyperResearch.Common.Systems;

public class ResearchSystem : ModSystem
{
    private Dictionary<int, int> CountOverride { get; set; } = [];
    public static int ResearchableItemsCount { get; private set; } = 0;

    public override void PostSetupContent()
    {
        OverrideDefaultResearchCount();
        CalculateResearchable();
    }

    public override void Load()
    {
        HyperConfig.Changed += OnClientConfigChanged;
        ServerConfig.Changed += OnServerConfigChanged;
    }

    public override void Unload()
    {
        HyperConfig.Changed -= OnClientConfigChanged;
        ServerConfig.Changed -= OnServerConfigChanged;
    }

    private void OverrideDefaultResearchCount()
    {
        CountOverride.Clear();
        CreativeItemSacrificesCatalog.Instance.Initialize();
        foreach ((ItemDefinition def, uint count) in ConfigOptions.ItemResearchCountOverride)
        {
            if (def.Type == 0 || def.IsUnloaded) continue;

            if (count > 0)
                CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[def.Type] = (int)count;
            else if (CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId.TryGetValue(def.Type, out int _))
                CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId.Remove(def.Type);
            CountOverride[def.Type] = (int)count;
        }
    }


    private void CalculateResearchable()
    {
        int totalResearchable = 0;
        for (int itemId = 1; itemId < ItemLoader.ItemCount; itemId++)
        {
            if (Researcher.GetSharedValue(itemId) != -1) continue;
            if (Researcher.IsResearchable(itemId)
                || (!CountOverride.TryGetValue(itemId, out int count) && count > 0))
            {
                totalResearchable++;
            }
        }
        ResearchableItemsCount = totalResearchable;
    }

    private void OnClientConfigChanged()
    {
        if (ConfigOptions.UseServerSettings) return;
        OverrideDefaultResearchCount();
        CalculateResearchable();
    }

    private void OnServerConfigChanged()
    {
        if (!ConfigOptions.UseServerSettings) return;
        OverrideDefaultResearchCount();
        CalculateResearchable();
    }
}
