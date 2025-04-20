using Terraria;
using Terraria.ID;

namespace HyperResearch.Utils;

public static class BuffUtils
{
    public static bool IsABuffPotion(Item item) =>
        item.buffType > 0
        && ItemsUtils.IsInItemGroup(item, ContentSamples.CreativeHelper.ItemGroup.BuffPotion);

    public static bool IsABuffFromPotion(int buffType) =>
        !BuffID.Sets.IsWellFed[buffType] &&
        !BuffID.Sets.IsAFlaskBuff[buffType] &&
        BuffID.Sets.BasicMountData[buffType] == null &&
        !Main.vanityPet[buffType] &&
        !Main.lightPet[buffType] &&
        !Main.debuff[buffType] &&
        !Main.pvpBuff[buffType];

    public static bool IsAcceptableBuff(int buffType) =>
        BuffID.Sets.BasicMountData[buffType] == null &&
        !Main.vanityPet[buffType] &&
        !Main.lightPet[buffType] &&
        !Main.debuff[buffType] &&
        !Main.pvpBuff[buffType];

    public static bool IsAFlask(Item item) =>
        item.buffType > 0
        && BuffID.Sets.IsAFlaskBuff[item.buffType];

    public static bool IsAFood(Item item) =>
        item.buffType > 0
        && BuffID.Sets.IsWellFed[item.buffType];
}
