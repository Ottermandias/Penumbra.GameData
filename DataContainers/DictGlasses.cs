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
public sealed class DictGlasses(IDalamudPluginInterface pluginInterface, Logger log, IDataManager gameData)
    : DataSharer<IReadOnlyDictionary<ushort, (string Name, uint Icon, ushort PrimaryId, byte Variant)>>(pluginInterface, log, "Glasses",
        gameData.Language, 1,
        () => CreateGlassesData(gameData)), IReadOnlyDictionary<GlassesId, Glasses>
{
    /// <summary> Create the data. </summary>
    private static IReadOnlyDictionary<ushort, (string Name, uint Icon, ushort PrimaryId, byte Variant)> CreateGlassesData(
        IDataManager dataManager)
    {
        // TODO
        var glassesSheet = dataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets2.Glasses>(dataManager.Language)!;
        return glassesSheet.Where(s => s.Unknown0.RawData.Length > 0)
            .ToFrozenDictionary(s => (ushort)s.RowId,
                s => (s.Unknown0.ToDalamudString().ToString(), (uint)s.Unknown11, (ushort)s.Unknown10, (byte)(s.Unknown10 >> 16)));
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<GlassesId, Glasses>> GetEnumerator()
        => Value.Select(kvp
                => new KeyValuePair<GlassesId, Glasses>(new GlassesId(kvp.Key),
                    new Glasses(kvp.Value.Name, kvp.Value.Icon, kvp.Value.PrimaryId, kvp.Value.Variant)))
            .GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    /// <inheritdoc/>
    public int Count
        => Value.Count;

    /// <inheritdoc/>
    public bool ContainsKey(GlassesId key)
        => Value.ContainsKey(key.Id);

    /// <inheritdoc/>
    public bool TryGetValue(GlassesId key, out Glasses value)
    {
        if (!Value.TryGetValue(key.Id, out var data))
        {
            value = default;
            return false;
        }

        value = new Glasses(data.Name, data.Icon, data.PrimaryId, data.Variant);
        return true;
    }

    /// <inheritdoc/>
    public Glasses this[GlassesId key]
        => TryGetValue(key, out var data) ? data : throw new ArgumentOutOfRangeException(nameof(key));

    /// <inheritdoc/>
    public IEnumerable<GlassesId> Keys
        => Value.Keys.Select(k => new GlassesId(k));

    /// <inheritdoc/>
    public IEnumerable<Glasses> Values
        => Value.Select(kvp => new Glasses(kvp.Value.Name, kvp.Value.Icon, kvp.Value.PrimaryId, kvp.Value.Variant));

    /// <inheritdoc/>
    protected override long ComputeMemory()
        => DataUtility.DictionaryMemory(24, Count);

    /// <inheritdoc/>
    protected override int ComputeTotalCount()
        => Count;
}
