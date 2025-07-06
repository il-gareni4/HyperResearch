using System;
using System.ComponentModel;
using Terraria.ID;
using Terraria.ModLoader.Config;

namespace HyperResearch.Common.Configs;

public class VisualConfig : ModConfig
{
    [Header("TooltipsSettingsHeader")]

    [LabelArgs(ItemID.HandOfCreation)]
    [DefaultValue(true)]
    public bool UseCustomResearchTooltip;

    [LabelArgs(ItemID.Sign)]
    [DefaultValue(false)]
    public bool ShowResearchedTooltip;

    [DefaultValue(true)]
    public bool ShowResearchBagTooltip;

    [DefaultValue(true)]
    public bool ShowBannerBuffTooltips;

    [DefaultValue(true)]
    public bool ShowPotionBuffTooltips;

    [DefaultValue(true)]
    public bool ShowSelectPrefixTooltip;


    [Header("MessagesSettingsHeader")]

    [DefaultValue(true)]
    public bool ShowNewlyResearchedItems;

    [DefaultValue(true)]
    public bool ShowResearchedCraftableItems;

    [DefaultValue(true)]
    public bool ShowResearchedShimmeredItems;

    [DefaultValue(true)]
    public bool ShowResearchedDecraftItems;

    [DefaultValue(false)]
    public bool ShowSacrifices;


    [Header("MultiplayerMessagesSettingsHeader")]

    [DefaultValue(true)]
    public bool ShowSharedItems;

    [DefaultValue(true)]
    public bool ShowSharedSacrifices;

    [DefaultValue(false)]
    public bool ShowOtherPlayersResearchedItems;


    [Header("UISettingsHeader")]

    [DefaultValue(true)]
    public bool VisualizeBuffStatus;

    [DefaultValue(true)]
    public bool ShowResearchInventoryButton;

    [DefaultValue(true)]
    public bool ShowClearInventoryButton;

    [DefaultValue(true)]
    public bool ShowAutoCraftButton;

    [DefaultValue(true)]
    public bool ShowShimmerButton;

    [DefaultValue(true)]
    public bool ShowShimmerDecraftButton;

    [DefaultValue(true)]
    public bool ShowTeamShareButton;

    [Range(2, 9)]
    [Slider()]
    [DefaultValue(2)]
    public int InventoryButtonsSlotOffset;

    [DefaultValue(true)]
    public bool ShowTotalResearchedItemsCount;



    public static VisualConfig Instance { get; private set; } = null!;

    public override ConfigScope Mode => ConfigScope.ClientSide;
    public static event Action? Changed;

    public override void OnLoaded() => Instance = this;

    public override void OnChanged() => Changed?.Invoke();
}