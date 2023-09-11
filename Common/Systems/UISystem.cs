using System.Collections.Generic;
using HyperResearch.UI;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace HyperResearch.Common.Systems
{
    class UISystem : ModSystem
    {
        internal DuplicationMenu DuplicationMenu;
        private UserInterface _duplicationMenu;

        public override void Load()
        {
            if (Main.dedServ) return;

            DuplicationMenu = new();
            DuplicationMenu.Activate();
            _duplicationMenu = new UserInterface();
            _duplicationMenu.SetState(DuplicationMenu);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            _duplicationMenu?.Update(gameTime);
        }

        public override void OnWorldUnload()
        {
            DuplicationMenu.CreativePowerSelected = 0;
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "HyperResearch: Duplication Menu Text",
                    () => {
                        _duplicationMenu.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
    }
}