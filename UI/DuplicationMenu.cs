using HyperResearch.Common.Configs;
using HyperResearch.Common.ModPlayers;
using HyperResearch.Common.Systems;
using HyperResearch.Utils;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;

namespace HyperResearch.UI
{
    class DuplicationMenu : UIState
    {
        private int _lastItemsResearchedCount;
        public int CreativePowerSelected = 0;

        private HyperPlayer _hyperPlayer;
        private LocalizedText _totalResearchedText;
        public UIText UITotalResearchedText;

        public override void OnInitialize()
        {
            UISystem.WorldLoaded += OnWorldLoad;
            UISystem.WorldUnloaded += OnWorldUnload;
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
            if (!Researcher.IsPlayerInJourneyMode) return;
            if (!HyperConfig.Instance.ShowTotalResearchedItemsCount)
            {
                if (Children.Any()) RemoveAllChildren();
                return;
            }

            if (Main.CreativeMenu.Enabled && CreativePowerSelected == 1)
            {
                if (!Children.Any()) Append(UITotalResearchedText);
            }
            else RemoveAllChildren();

            if (_hyperPlayer is null) return;

            if (_lastItemsResearchedCount != _hyperPlayer.ItemsResearchedCount)
            {
                if (_hyperPlayer.ItemsResearchedCount == HyperResearch.ResearchableItemsCount)
                    UITotalResearchedText.TextColor = Colors.CoinGold;
                else
                    UITotalResearchedText.TextColor = Colors.JourneyMode;
                _lastItemsResearchedCount = _hyperPlayer.ItemsResearchedCount;
                UITotalResearchedText.SetText(GetTotalResearchedText());
            }
        }

        public void OnWorldLoad()
        {
            CreativePowerSelected = 0;
            _hyperPlayer = Main.LocalPlayer.GetModPlayer<HyperPlayer>();
        }

        public void OnWorldUnload()
        {
            if (Children.Any()) RemoveAllChildren();
        }

        private string GetTotalResearchedText()
        {
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
            CreativePowerSelected = (int)currentOptionField.GetValue(menuTree);
        }
    }
}