using System.Collections.Frozen;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using Lumina.Excel.GeneratedSheets;
using OtterGui.Log;
using Penumbra.GameData.Data;
using Penumbra.GameData.DataContainers.Bases;
using Penumbra.GameData.Structs;

namespace Penumbra.GameData.DataContainers;

/// <summary> A dictionary that maps WorldIds to their names. </summary>
public sealed class DictWorld(DalamudPluginInterface pluginInterface, Logger log, IDataManager gameData)
    : DataSharer<IReadOnlyDictionary<ushort, string>>(pluginInterface, log, "Worlds", gameData.Language, 7, () => CreateWorldData(gameData)),
        IReadOnlyDictionary<WorldId, string>
{
    /// <summary> Create the data. </summary>
    private static IReadOnlyDictionary<ushort, string> CreateWorldData(IDataManager gameData)
    {
        var sheet = gameData.GetExcelSheet<World>()!;
        var dict  = new Dictionary<ushort, string>((int)sheet.RowCount);
        foreach (var w in sheet.Where(w => w.IsPublic && !w.Name.RawData.IsEmpty))
            dict.TryAdd((ushort)w.RowId, string.Intern(w.Name.ToDalamudString().TextValue));
        return dict.ToFrozenDictionary();
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<WorldId, string>> GetEnumerator()
        => Value.Select(kvp => new KeyValuePair<WorldId, string>(new WorldId(kvp.Key), kvp.Value)).GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    /// <inheritdoc/>
    public int Count
        => Value.Count;

    /// <inheritdoc/>
    public bool ContainsKey(WorldId key)
        => Value.ContainsKey(key.Id);

    /// <inheritdoc/>
    public bool TryGetValue(WorldId key, [NotNullWhen(true)] out string? value)
        => Value.TryGetValue(key.Id, out value);

    /// <inheritdoc/>
    public string this[WorldId key]
        => Value[key.Id];

    /// <inheritdoc/>
    public IEnumerable<WorldId> Keys
        => Value.Keys.Select(k => new WorldId(k));

    /// <inheritdoc/>
    public IEnumerable<string> Values
        => Value.Values;

    /// <inheritdoc/>
    protected override long ComputeMemory()
        => DataUtility.DictionaryMemory(16, Count) + Values.Sum(v => v.Length * 2);

    /// <inheritdoc/>
    protected override int ComputeTotalCount()
        => Count;
}
