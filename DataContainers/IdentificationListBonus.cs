using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using OtterGui.Log;
using Penumbra.GameData.DataContainers.Bases;
using Penumbra.GameData.Enums;
using Penumbra.GameData.Structs;

namespace Penumbra.GameData.DataContainers;

/// <summary> A list to efficiently identify equipment pieces. This requires ItemsByType to be finished. </summary>
public sealed class IdentificationListBonus(IDalamudPluginInterface pi, Logger log, IDataManager gameData, DictBonusItems items)
    : KeyList<PseudoBonusItem>(pi, log, "BonusIdentification", gameData.Language, 0, () => CreateEquipmentList(items), ToKey, ValidKey,
        ValueKeySelector, items.Awaiter)
{
    /// <summary> Find all bonus item affected by the given set of input data. </summary>
    /// <param name="modelId"> The primary ID of the piece. </param>
    /// <param name="slot"> The slot. If Unknown, check all slots. </param>
    /// <param name="variant"> The variant. If 0, check all variants. </param>
    /// <returns> A list of all affected bonus items. </returns>
    public IEnumerable<BonusItem> Between(PrimaryId modelId, BonusItemFlag slot = BonusItemFlag.Unknown, Variant variant = default)
    {
        if (slot == BonusItemFlag.Unknown)
            return Between(ToKey(modelId, 0, 0), ToKey(modelId, (BonusItemFlag)0xFF, 0xFF)).Select(e => (BonusItem)e);
        if (variant == 0)
            return Between(ToKey(modelId, slot, 0), ToKey(modelId, slot, 0xFF)).Select(e => (BonusItem)e);

        return Between(ToKey(modelId, slot, variant), ToKey(modelId, slot, variant)).Select(e => (BonusItem)e);
    }

    /// <summary> Convert a set of data to its key representation. </summary>
    public static ulong ToKey(PrimaryId modelId, BonusItemFlag slot, Variant variant)
        => ((ulong)modelId.Id << 32) | ((ulong)slot << 16) | variant.Id;

    /// <summary> Turn a specific item to its key representation. </summary>
    public static ulong ToKey(BonusItem i)
        => ToKey(i.ModelId, i.Slot, i.Variant);

    /// <summary> Turn a pseudo equip item to its key representation. </summary>
    private static ulong ToKey(PseudoBonusItem i)
        => ToKey((BonusItem)i);

    /// <summary> All non-zero keys are valid. </summary>
    private static bool ValidKey(ulong key)
        => key != 0;

    /// <summary> Order by the sheet id after the keys. </summary>
    private static int ValueKeySelector(PseudoBonusItem data)
        => data.Item3;

    /// <summary> Create the key list of all bonus items.  </summary>
    private static IEnumerable<PseudoBonusItem> CreateEquipmentList(DictBonusItems items)
        => items.Values.Select(i => (PseudoBonusItem)i);

    /// <inheritdoc/>
    protected override long ComputeMemory()
        => 24 + Value.Count * 24;
}
