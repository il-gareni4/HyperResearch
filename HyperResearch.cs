using HyperResearch.Common;
using HyperResearch.Utils;
using HyperResearch.Utils.Extensions;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace HyperResearch
{
    public class HyperResearch : Mod
    {
        public static int ResearchableItemsCount { get; set; }

        public override void PostSetupContent()
        {
            int totalResearchable = 0;
            for (int itemId = 0; itemId < ItemLoader.ItemCount; itemId++)
            {
                if (!Researcher.IsResearchable(itemId)) continue;
                if (Researcher.GetSharedValue(itemId) != -1) continue;
                totalResearchable++;
            }
            ResearchableItemsCount = totalResearchable;
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            NetMessageType messageType = (NetMessageType)reader.ReadByte();
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
                    Main.LocalPlayer.GetModPlayer<HyperPlayer>().SharedItems(reader.ReadByte(), reader.ReadInts32(), reader.ReadDictInt32Int32());
                    break;
                default:
                    Logger.Debug("Invalid message type");
                    break;
            }
        }
    }
}