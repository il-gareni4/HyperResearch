using HyperResearch.Utils;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HyperResearch
{
    public class HyperResearch : Mod
    {
        public static int ResearchableItemsCount { get; set; }

#if DEBUG
        public static ModKeybind ForgetBind { get; set; }
#endif
        public static ModKeybind SacrificeInventoryBind { get; set; }
        public static ModKeybind ClearResearchedBind { get; set; }
        public static ModKeybind ResearchCraftableBind { get; set; }
        public static ModKeybind MaxStackBind { get; set; }
        public static ModKeybind ResearchLootBind { get; set; }
        public static ModKeybind ResearchShopBind { get; set; }

        public override void Load()
        {
#if DEBUG
            ForgetBind = KeybindLoader.RegisterKeybind(this, "Forget All Researches", Keys.P);
#endif
            SacrificeInventoryBind = KeybindLoader.RegisterKeybind(this, "Research Inventory Items", Keys.OemComma);
            ClearResearchedBind = KeybindLoader.RegisterKeybind(this, "Clear Researched Items", Keys.OemPeriod);
            ResearchCraftableBind = KeybindLoader.RegisterKeybind(this, "Research Craftable Items", Keys.OemQuestion);
            MaxStackBind = KeybindLoader.RegisterKeybind(this, "Max Stack Researched Item", Keys.OemSemicolon);
            ResearchLootBind = KeybindLoader.RegisterKeybind(this, "Research Bag/Crate Contents", Keys.OemQuotes);
            ResearchShopBind = KeybindLoader.RegisterKeybind(this, "Research Shop", Keys.OemSemicolon);
        }

        public override void PostSetupContent()
        {
            int totalResearchable = 0;
            for (int itemId = 0; itemId < ItemLoader.ItemCount; itemId++)
            {
                if (!Researcher.IsResearchable(itemId)) continue;
                if (Researcher.ItemSharedValue(itemId) != -1) continue;
                totalResearchable++;
            }
            ResearchableItemsCount = totalResearchable;
        }

        public override void Unload()
        {
#if DEBUG
            ForgetBind = null;
#endif
            SacrificeInventoryBind = null;
            ClearResearchedBind = null;
            ResearchCraftableBind = null;
            MaxStackBind = null;
            ResearchLootBind = null;
            ResearchShopBind = null;
        }
    }
}