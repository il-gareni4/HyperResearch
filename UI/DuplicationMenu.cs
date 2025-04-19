using System.Linq;
using System.Reflection;
using HyperResearch.Common.Configs;
using HyperResearch.Common.ModPlayers;
using HyperResearch.Common.Systems;
using HyperResearch.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;

namespace HyperResearch.UI;

internal class DuplicationMenu : UIState
{
    private int _creativePowerSelected;
    private HyperPlayer? _hyperPlayer;
    private int _lastItemsResearchedCount;
    private int _lastResearchableItemsCount;
    private LocalizedText _totalResearchedText = null!;
    private UIText _uiTotalResearchedText = null!;

    public override void OnInitialize()
    {
        HooksSystem.WorldLoaded += OnWorldLoad;
        HooksSystem.WorldUnloaded += OnWorldUnload;
        _totalResearchedText = Language.GetText("Mods.HyperResearch.UI.DuplicationMenu.TotalResearched");

        _uiTotalResearchedText = new UIText("")
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

        if (Main.CreativeMenu.Enabled && _creativePowerSelected == 1)
        {
            if (!Children.Any()) Append(_uiTotalResearchedText);
        }
        else
        {
            RemoveAllChildren();
        }

        if (_hyperPlayer == null) return;

        if (_lastItemsResearchedCount != _hyperPlayer.ItemsResearchedCount
            || _lastResearchableItemsCount != ResearchSystem.ResearchableItemsCount)
        {
            _uiTotalResearchedText.TextColor =
                _hyperPlayer.ItemsResearchedCount >= ResearchSystem.ResearchableItemsCount
                    ? Colors.CoinGold
                    : Colors.JourneyMode;
            _lastItemsResearchedCount = _hyperPlayer.ItemsResearchedCount;
            _lastResearchableItemsCount = ResearchSystem.ResearchableItemsCount;
            _uiTotalResearchedText.SetText(GetTotalResearchedText());
        }
    }

    private void OnWorldLoad()
    {
        _creativePowerSelected = 0;
        _hyperPlayer = Main.LocalPlayer.GetModPlayer<HyperPlayer>();
    }

    private void OnWorldUnload()
    {
        if (Children.Any()) RemoveAllChildren();
    }

    private string GetTotalResearchedText()
    {
        if (_hyperPlayer == null) return "";

        var percentResearched =
            $"{(float)_hyperPlayer.ItemsResearchedCount / ResearchSystem.ResearchableItemsCount * 100:0.00}";
        return _totalResearchedText.Format(percentResearched, _hyperPlayer.ItemsResearchedCount,
            ResearchSystem.ResearchableItemsCount);
    }

    private void OnCreativePowerMenuCategoryChanged(On_UICreativePowersMenu.orig_MainCategoryButtonClick orig,
        UICreativePowersMenu menu, UIMouseEvent evt, UIElement el)
    {
        orig.Invoke(menu, evt, el);
        FieldInfo? mainCategoryField =
            menu.GetType().GetField("_mainCategory", BindingFlags.NonPublic | BindingFlags.Instance);
        if (mainCategoryField is null) return;

        object? menuTree = mainCategoryField.GetValue(menu);
        FieldInfo? currentOptionField = menuTree?.GetType().GetField("CurrentOption");
        if (currentOptionField is null) return;

        _uiTotalResearchedText.Left = StyleDimension.FromPixels(88f);
        _uiTotalResearchedText.Top = StyleDimension.FromPixelsAndPercent(-60f, 1f);
        _creativePowerSelected = (int)(currentOptionField.GetValue(menuTree) ?? 0);
    }
}