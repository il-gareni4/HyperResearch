using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using HyperResearch.Common;
using HyperResearch.Common.ModPlayers;
using HyperResearch.Utils;
using HyperResearch.Utils.Extensions;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HyperResearch;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class HyperResearch : Mod
{
    public override void HandlePacket(BinaryReader reader, int whoAmI)
    {
        var messageType = (NetMessageType)reader.ReadByte();
        switch (messageType)
        {
            case NetMessageType.ShareItemsWithTeam:
                ModPacket packet = GetPacket();
                packet.Write((byte)NetMessageType.SharedItems);
                packet.Write((byte)whoAmI);
                packet.Write(reader.ReadInts32());
                packet.Write(reader.ReadDictInt32Int32());
                foreach (int playerId in MainUtils.GetTeamMembers(Main.player[whoAmI].team, whoAmI))
                    packet.Send(playerId, whoAmI);
                break;
            case NetMessageType.SharedItems:
                Main.LocalPlayer.GetModPlayer<HyperPlayer>().SharedItems(
                    reader.ReadByte(),
                    reader.ReadInts32(),
                    reader.ReadDictInt32Int32()
                );
                break;
            default:
                Logger.Debug("Invalid message type");
                break;
        }
    }

    public override uint ExtraPlayerBuffSlots
    {
        get
        {
            uint extraBuffs = 0;
            for (int i = 0; i < BuffLoader.BuffCount; i++)
            {
                ModBuff? modBuff = BuffLoader.GetBuff(i);
                if (modBuff != null &&
                    modBuff.Mod.Side != ModSide.Both &&
                    modBuff.Mod.Side != ModSide.Server)
                {
                    continue;
                }
                if (!BuffUtils.IsABuffFromPotion(i))
                    continue;
                extraBuffs++;
            }
            // 2 is for flask and food, 44 is default max buff count
            return Math.Min(extraBuffs + 2 - 44, 0);
        }
    }
}