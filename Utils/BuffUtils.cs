using Terraria;
using Terraria.ID;

namespace HyperResearch.Utils;

public static class BuffUtils
{
    public static bool IsBuffPotion(Item item) =>
        item.buffType > 0
        && ItemsUtils.IsInItemGroup(item, ContentSamples.CreativeHelper.ItemGroup.BuffPotion);
}
