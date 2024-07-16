using System.Collections.Frozen;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using OtterGui.Log;
using Penumbra.GameData.Data;
using Penumbra.GameData.DataContainers.Bases;
using Penumbra.GameData.Structs;

namespace Penumbra.GameData.DataContainers;

/// <summary> A dictionary that maps GlassesIds to Glasses. </summary>
public sealed class DictBonusItems(IDalamudPluginInterface pluginInterface, Logger log, IDataManager gameData)
    : DataSharer<IReadOnlyDictionary<ushort, PseudoBonusItem>>(pluginInterface, log, "BonusItems",
        gameData.Language, 1,
        () => CreateGlassesData(gameData)), IReadOnlyDictionary<BonusItemId, BonusItem>
{
    /// <summary> Create the data. </summary>
    private static IReadOnlyDictionary<ushort, PseudoBonusItem> CreateGlassesData(
        IDataManager dataManager)
    {
        // TODO
        var glassesSheet = dataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets2.Glasses>(dataManager.Language)!;
        return glassesSheet.Where(s => s.Name.RawData.Length > 0)
            .ToFrozenDictionary(s => (ushort)s.RowId,
                s => (s.Name.ToDalamudString().ToString(), (uint)s.Icon, (ushort)s.RowId, (ushort)s.Unknown_70_7,
                    (byte)(s.Unknown_70_7 >> 16), (byte)1)); // TODO slot other than glasses
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<BonusItemId, BonusItem>> GetEnumerator()
        => Value.Select(kvp
                => new KeyValuePair<BonusItemId, BonusItem>(new BonusItemId(kvp.Key), (BonusItem)kvp.Value))
            .GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    /// <inheritdoc/>
    public int Count
        => Value.Count;

    /// <inheritdoc/>
    public bool ContainsKey(BonusItemId key)
        => Value.ContainsKey(key.Id);

    /// <inheritdoc/>
    public bool TryGetValue(BonusItemId key, out BonusItem value)
    {
        if (!Value.TryGetValue(key.Id, out var data))
        {
            value = default;
            return false;
        }

        value = (BonusItem)data;
        return true;
    }

    /// <inheritdoc/>
    public BonusItem this[BonusItemId key]
        => TryGetValue(key, out var data) ? data : throw new ArgumentOutOfRangeException(nameof(key));

    /// <inheritdoc/>
    public IEnumerable<BonusItemId> Keys
        => Value.Keys.Select(k => new BonusItemId(k));

    /// <inheritdoc/>
    public IEnumerable<BonusItem> Values
        => Value.Select(kvp => (BonusItem)kvp.Value);

    /// <inheritdoc/>
    protected override long ComputeMemory()
        => DataUtility.DictionaryMemory(24, Count);

    /// <inheritdoc/>
    protected override int ComputeTotalCount()
        => Count;
}
