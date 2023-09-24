using HyperResearch.Common;
using HyperResearch.Common.Configs;
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
                    for (int i = 0; i < Main.maxNetPlayers; i++)
                    {
                        if (Main.player[i].active && i != whoAmI && Main.player[i].team == Main.player[whoAmI].team)
                            packet.Send(i, whoAmI);
                    }
                    break;
                case NetMessageType.SharedItems:
                    byte fromPlayer = reader.ReadByte();
                    int[] items = reader.ReadInts32();
                    Researcher researcher = new();
                    researcher.ResearchItems(items, researchCraftable: HyperConfig.Instance.AutoResearchCraftable);
                    Main.LocalPlayer.GetModPlayer<HyperPlayer>().AfterResearch(researcher, fromPlayer);
                    break;
                default:
                    Logger.Debug("Invalid message type");
                    break;
            }
        }
    }
}