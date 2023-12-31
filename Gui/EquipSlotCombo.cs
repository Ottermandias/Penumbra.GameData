using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using OtterGui;
using Penumbra.GameData.Enums;

namespace Penumbra.GameData.Gui;

/// <summary> A simple combo wrapper to select an equip slot. </summary>
public static class EquipSlotCombo
{
    /// <summary> Draw a combo selecting among actual equip slots. </summary>
    /// <param name="label"> The label for the combo. </param>
    /// <param name="tooltip"> A hover tooltip for the combo. </param>
    /// <param name="slot"> The current and returned slot. </param>
    /// /// <param name="width"> The width of the selection preview, if 0, it is sized to fit the longest value. </param>
    /// <returns> True on change, false otherwise. </returns>
    /// <remarks> This only contains the 12 slots visible on the model, i.e. weapons, equipment and accessories. </remarks>
    public static bool Draw(string label, string tooltip, ref EquipSlot slot, float width = 0)
    {
        if (width == 0)
            width = ImGui.CalcTextSize(EquipSlot.OffHand.ToName()).X + ImGui.GetFrameHeightWithSpacing();
        ImGui.SetNextItemWidth(width);
        using var combo = ImRaii.Combo(label, slot.ToName());
        var       ret   = false;
        if (combo)
            foreach (var tmpSlot in EquipSlotExtensions.FullSlots)
            {
                if (ImGui.Selectable(tmpSlot.ToName(), tmpSlot == slot) && slot != tmpSlot)
                {
                    slot = tmpSlot;
                    ret  = true;
                }
            }

        ImGuiUtil.HoverTooltip(tooltip);
        return ret;
    }
}
