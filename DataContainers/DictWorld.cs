using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using Lumina.Excel.GeneratedSheets;
using OtterGui.Log;
using Penumbra.GameData.Data;
using Penumbra.GameData.DataContainers.Bases;
using Penumbra.GameData.Structs;

namespace Penumbra.GameData.DataContainers;

public sealed class DictWorld(DalamudPluginInterface pluginInterface, Logger log, IDataManager gameData)
    : DataSharer<IReadOnlyDictionary<ushort, string>>(pluginInterface, log, "Worlds", gameData.Language, 6, () => CreateWorldData(gameData)),
        IReadOnlyDictionary<WorldId, string>
{
    private static IReadOnlyDictionary<ushort, string> CreateWorldData(IDataManager gameData)
    {
        var dict = new Dictionary<ushort, string>();
        foreach (var w in gameData.GetExcelSheet<World>()!.Where(w => w.IsPublic && !w.Name.RawData.IsEmpty))
            dict.TryAdd((ushort)w.RowId, string.Intern(w.Name.ToDalamudString().TextValue));
        return dict;
    }

    public IEnumerator<KeyValuePair<WorldId, string>> GetEnumerator()
        => Value.Select(kvp => new KeyValuePair<WorldId, string>(new WorldId(kvp.Key), kvp.Value)).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public int Count
        => Value.Count;

    public bool ContainsKey(WorldId key)
        => Value.ContainsKey(key.Id);

    public bool TryGetValue(WorldId key, [NotNullWhen(true)] out string? value)
        => Value.TryGetValue(key.Id, out value);

    public string this[WorldId key]
        => Value[key.Id];

    public IEnumerable<WorldId> Keys
        => Value.Keys.Select(k => new WorldId(k));

    public IEnumerable<string> Values
        => Value.Values;

    public override long ComputeMemory()
        => DataUtility.DictionaryMemory(16, Count) + Values.Sum(v => v.Length * 2);

    public override int ComputeTotalCount()
        => Count;
}
