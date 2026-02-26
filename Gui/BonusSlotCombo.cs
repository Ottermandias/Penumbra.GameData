using ImSharp;
using Penumbra.GameData.Enums;

namespace Penumbra.GameData.Gui;

/// <summary> A simple combo wrapper to select a bonus item slot. </summary>
public static class BonusSlotCombo
{
    /// <summary> Draw a combo selecting among actual bonus item slots. </summary>
    /// <param name="label"> The label for the combo. If this is UTF8, HAS to be null-terminated. </param>
    /// <param name="tooltip"> A hover tooltip for the combo. </param>
    /// <param name="slot"> The current and returned slot. </param>
    /// <param name="width"> The width of the selection preview, if 0, it is sized to fit the longest value. </param>
    /// <returns> True on change, false otherwise. </returns>
    public static bool Draw(Utf8StringHandler<LabelStringHandlerBuffer> label, ReadOnlySpan<byte> tooltip, ref BonusItemFlag slot, float width = 0)
    {
        if (width == 0)
            width = Im.Font.CalculateSize(BonusItemFlag.Glasses.ToNameU8()).X + Im.Style.FrameHeightWithSpacing;
        Im.Item.SetNextWidth(width);
        using var combo = Im.Combo.Begin(label, slot.ToNameU8());
        var       ret   = false;
        if (combo)
            foreach (var tmpSlot in BonusExtensions.AllFlags)
            {
                if (Im.Selectable(tmpSlot.ToNameU8(), tmpSlot == slot) && slot != tmpSlot)
                {
                    slot = tmpSlot;
                    ret  = true;
                }
            }

        Im.Tooltip.OnHover(tooltip);
        return ret;
    }
}
