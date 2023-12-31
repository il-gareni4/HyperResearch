﻿using HyperResearch.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace HyperResearch.Common.Systems
{
    class UISystem : ModSystem
    {
        public static Asset<Texture2D> ResearchButtonTexture;
        public static Asset<Texture2D> ClearButtonTexture;
        public static Asset<Texture2D> AutoCraftButtonTexture;
        public static Asset<Texture2D> ResearchShopButtonTexture;

        internal static event Action WorldLoaded;
        internal static event Action WorldUnloaded;

        internal DuplicationMenu DuplicationMenu;
        private UserInterface _duplicationMenu;
        internal InventoryButtons InventoryButtons;
        private UserInterface _inventoryButtons;
        internal ShopButtons ShopButtons;
        private UserInterface _shopButtons;

        public override void Load()
        {
            if (Main.dedServ) return;

            ResearchButtonTexture = Mod.Assets.Request<Texture2D>("Assets/Images/UI/ResearchButton");
            ClearButtonTexture = Mod.Assets.Request<Texture2D>("Assets/Images/UI/ClearButton");
            AutoCraftButtonTexture = Mod.Assets.Request<Texture2D>("Assets/Images/UI/AutoCraftButton");
            ResearchShopButtonTexture = Mod.Assets.Request<Texture2D>("Assets/Images/UI/ResearchShopButton");

            DuplicationMenu = new();
            DuplicationMenu.Activate();
            _duplicationMenu = new UserInterface();
            _duplicationMenu.SetState(DuplicationMenu);

            InventoryButtons = new();
            InventoryButtons.Activate();
            _inventoryButtons = new UserInterface();
            _inventoryButtons.SetState(InventoryButtons);

            ShopButtons = new();
            ShopButtons.Activate();
            _shopButtons = new UserInterface();
            _shopButtons.SetState(ShopButtons);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            _duplicationMenu?.Update(gameTime);
            _inventoryButtons?.Update(gameTime);
            _shopButtons?.Update(gameTime);
        }

        public override void OnWorldLoad() => WorldLoaded?.Invoke();

        public override void OnWorldUnload() => WorldUnloaded?.Invoke();

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "HyperResearch: Duplication Menu Text",
                    () =>
                    {
                        _duplicationMenu.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "HyperResearch: Inventory Buttons",
                    () =>
                    {
                        _inventoryButtons.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "HyperResearch: Shop Buttons",
                    () =>
                    {
                        _shopButtons.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }

        public override void Unload()
        {
            ResearchButtonTexture = null;
            ClearButtonTexture = null;
            AutoCraftButtonTexture = null;
            ResearchShopButtonTexture = null;
        }
    }
}