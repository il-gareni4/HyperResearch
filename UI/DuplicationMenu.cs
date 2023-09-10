using HyperResearch.Common;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace HyperResearch.UI
{
    class DuplicationMenu : UIState
    {
        private int _lastItemsResearchedCount;
        private int _crativePowerSelected;

        private HyperPlayer _hyperPlayer;
        private LocalizedText _totalResearchedText;
        public UIText UITotalResearchedText;

        public override void OnInitialize()
        {
            _totalResearchedText = Language.GetText("Mods.HyperResearch.UI.DuplicationMenu.TotalResearched");

            UITotalResearchedText = new("")
            {
                Left = StyleDimension.FromPixels(88f),
                Top = StyleDimension.FromPixelsAndPercent(-60f, 1f),
                TextColor = Colors.JourneyMode
            };
            On_UICreativePowersMenu.MainCategoryButtonClick += OnCreativePowerMenuCategoryChanged;
        }

        public override void Update(GameTime gameTime)
        {
            if (Main.GameMode != 3 || !ModContent.GetInstance<HyperConfig>().ShowTotalResearchedItemsCount) return;

            if (Main.CreativeMenu.Enabled && _crativePowerSelected == 1)
            {
                if (Children.Count() == 0) Append(UITotalResearchedText);
            } else RemoveAllChildren();

            if (_hyperPlayer is null)
            {
                if (Main.PlayerLoaded && Main.LocalPlayer.TryGetModPlayer(out HyperPlayer player))
                    _hyperPlayer = player;
                else return;
            }

            if (_lastItemsResearchedCount != _hyperPlayer.ItemsResearchedCount)
            {
                if (_hyperPlayer.ItemsResearchedCount == HyperResearch.ResearchableItemsCount)
                    UITotalResearchedText.TextColor = Colors.CoinGold;
                UITotalResearchedText.SetText(GetTotalResearchedText());
            }
        }

        private string GetTotalResearchedText()
        {

            _lastItemsResearchedCount = _hyperPlayer.ItemsResearchedCount;
            string percentResearched = string.Format("{0:0.00}", (float)_hyperPlayer.ItemsResearchedCount / HyperResearch.ResearchableItemsCount * 100);
            return _totalResearchedText.Format(percentResearched, _hyperPlayer.ItemsResearchedCount, HyperResearch.ResearchableItemsCount);
        }

        private void OnCreativePowerMenuCategoryChanged(On_UICreativePowersMenu.orig_MainCategoryButtonClick orig, UICreativePowersMenu menu, UIMouseEvent evt, UIElement el)
        {
            orig.Invoke(menu, evt, el);
            FieldInfo mainCategoryField = menu.GetType().GetField("_mainCategory", BindingFlags.NonPublic | BindingFlags.Instance);
            if (mainCategoryField is null) return;

            var menuTree = mainCategoryField.GetValue(menu);
            FieldInfo currentOptionField = menuTree.GetType().GetField("CurrentOption");
            if (currentOptionField is null) return;

            UITotalResearchedText.Left = StyleDimension.FromPixels(88f);
            UITotalResearchedText.Top = StyleDimension.FromPixelsAndPercent(-60f, 1f);
            _crativePowerSelected = (int)currentOptionField.GetValue(menuTree);
        }
    }
}