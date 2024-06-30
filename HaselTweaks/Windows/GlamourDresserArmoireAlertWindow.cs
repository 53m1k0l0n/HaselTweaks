using System.Linq;
using System.Numerics;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI;
using HaselCommon.Services;
using HaselCommon.Sheets;
using HaselCommon.Utils;
using HaselCommon.Windowing;
using HaselCommon.Windowing.Interfaces;
using HaselTweaks.Tweaks;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;

namespace HaselTweaks.Windows;

public unsafe class GlamourDresserArmoireAlertWindow : SimpleWindow
{
    private const int NumPrismBoxSlots = 800;
    private static readonly Vector2 IconSize = new(34);
    private readonly TextureService TextureService;
    private readonly ExcelService ExcelService;
    private readonly TextService TextService;
    private readonly ImGuiContextMenuService ImGuiContextMenuService;

    private static AddonMiragePrismPrismBox* Addon => GetAddon<AddonMiragePrismPrismBox>("MiragePrismPrismBox");

    public GlamourDresserArmoireAlertWindow(
        IWindowManager windowManager,
        TextureService textureService,
        ExcelService excelService,
        TextService textService,
        ImGuiContextMenuService imGuiContextMenuService)
        : base(windowManager, textService.Translate("GlamourDresserArmoireAlertWindow.Title"))
    {
        TextureService = textureService;
        ExcelService = excelService;
        TextService = textService;
        ImGuiContextMenuService = imGuiContextMenuService;
        DisableWindowSounds = true;

        Flags |= ImGuiWindowFlags.NoSavedSettings;
        Flags |= ImGuiWindowFlags.NoResize;
        Flags |= ImGuiWindowFlags.NoMove;

        SizeCondition = ImGuiCond.Always;
        Size = new(360, 428);
    }

    public GlamourDresserArmoireAlert? Tweak { get; set; }

    public override bool DrawConditions()
        => Tweak != null && Addon != null && Addon->AtkUnitBase.IsVisible && Tweak!.Categories.Count != 0;

    public override void Draw()
    {
        TextService.DrawWrapped("GlamourDresserArmoireAlertWindow.Info");

        foreach (var (categoryId, categoryItems) in Tweak!.Categories.OrderBy(kv => kv.Key))
        {
            var category = ExcelService.GetRow<ItemUICategory>(categoryId)!;

            ImGui.TextUnformatted(category.Name.ToDalamudString().ToString());
            ImGuiUtils.PushCursorY(3 * ImGuiHelpers.GlobalScale);

            using var indent = ImRaii.PushIndent();

            foreach (var (itemIndex, (item, isHq)) in categoryItems)
            {
                DrawItem(itemIndex, item, isHq);
            }
        }

        Position = new(
            Addon->AtkUnitBase.X + Addon->AtkUnitBase.GetScaledWidth(true) - 12,
            Addon->AtkUnitBase.Y + 9
        );
    }

    public void DrawItem(uint itemIndex, ExtendedItem item, bool isHq)
    {
        var popupKey = $"##ItemContextMenu_{item.RowId}_Tooltip";

        using (var group = ImRaii.Group())
        {
            TextureService.DrawIcon(new GameIconLookup(item.Icon, isHq), IconSize * ImGuiHelpers.GlobalScale);

            ImGui.SameLine();

            var pos = ImGui.GetCursorPos();
            if (ImGui.Selectable(
                "##Selectable",
                false,
                Tweak!.UpdatePending
                    ? ImGuiSelectableFlags.Disabled
                    : ImGuiSelectableFlags.None,
                ImGuiHelpers.ScaledVector2(ImGui.GetContentRegionAvail().X, IconSize.Y)))
            {
                RestoreItem(itemIndex);
            }

            ImGui.SetCursorPos(pos + new Vector2(
                ImGui.GetStyle().ItemInnerSpacing.X,
                IconSize.Y * ImGuiHelpers.GlobalScale / 2f - ImGui.GetTextLineHeight() / 2f - 1));

            ImGui.TextUnformatted(TextService.GetItemName(item.RowId));
        }

        ImGuiContextMenuService.Draw(popupKey, [
            ImGuiContextMenuService.CreateTryOn(item),
            ImGuiContextMenuService.CreateItemFinder(item.RowId),
            ImGuiContextMenuService.CreateCopyItemName(item.RowId),
            ImGuiContextMenuService.CreateOpenOnGarlandTools("item", item.RowId),
            ImGuiContextMenuService.CreateItemSearch(item)
        ]);
    }

    private void RestoreItem(uint itemIndex)
    {
        Tweak!.UpdatePending = MirageManager.Instance()->RestorePrismBoxItem(itemIndex);
    }
}
