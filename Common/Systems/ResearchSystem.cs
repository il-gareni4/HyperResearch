using System.Collections.Generic;
using System.Linq;
using HyperResearch.Common.Configs;
using HyperResearch.Utils;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace HyperResearch.Common.Systems;

public class ResearchSystem : ModSystem
{
    public static int ResearchableItemsCount { get; private set; }

    public override void OnModLoad()
    {
        CheatsConfig.Instance.Changed += ResetOverridesAndRecalculate;
        ServerConfig.Instance.Changed += ResetOverridesAndRecalculate;
        ResetOverridesAndRecalculate();
    }

    public override void OnModUnload()
    {
        CheatsConfig.Instance.Changed -= ResetOverridesAndRecalculate;
        ServerConfig.Instance.Changed -= ResetOverridesAndRecalculate;
    }

    private void ResetOverridesAndRecalculate()
    {
        CreativeItemSacrificesCatalog.Instance.Initialize();
        OverrideDefaultResearchCount();
        CalculateResearchable();
    }

    private void OverrideDefaultResearchCount()
    {
        if (ConfigOptions.OnlyOneItemNeeded)
        {
            foreach (int itemId in CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId.Keys)
                if (CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[itemId] > 1)
                    CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[itemId] = 1;
        }

        foreach ((ItemDefinition def, uint count) in ConfigOptions.ItemResearchCountOverride)
        {
            if (def.Type == 0 || def.IsUnloaded) continue;

            if (count > 0)
                CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[def.Type] = (int)count;
            else
                CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId.Remove(def.Type, out int _);
        }
    }

    private void CalculateResearchable()
    {
        ResearchableItemsCount = CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId
            .Count(pair =>
            {
                if (Researcher.GetSharedValue(pair.Key) >= 0) return false;
                return Researcher.IsValidResearchItem(pair.Key) && pair.Value >= 1;
            });
    }
}