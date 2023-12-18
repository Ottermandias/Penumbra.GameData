using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using OtterGui.Log;
using Penumbra.GameData.Data;
using Penumbra.GameData.Structs;

namespace Penumbra.GameData.DataContainers.Bases;

public abstract class NameDictionary(
    DalamudPluginInterface pluginInterface,
    Logger log,
    IDataManager gameData,
    string name,
    int version,
    Func<IReadOnlyDictionary<uint, string>> factory)
    : DataSharer<IReadOnlyDictionary<uint, string>>(pluginInterface, log, name, gameData.Language, version, factory),
        IReadOnlyDictionary<NpcId, string>
{
    public IEnumerator<KeyValuePair<NpcId, string>> GetEnumerator()
        => Value.Select(kvp => new KeyValuePair<NpcId, string>(new NpcId(kvp.Key), kvp.Value)).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public int Count
        => Value.Count;

    public bool ContainsKey(NpcId key)
        => Value.ContainsKey(key.Id);

    public bool TryGetValue(NpcId key, [NotNullWhen(true)] out string? value)
        => Value.TryGetValue(key.Id, out value);

    public string this[NpcId key]
        => Value[key.Id];

    public IEnumerable<NpcId> Keys
        => Value.Keys.Select(k => new NpcId(k));

    public IEnumerable<string> Values
        => Value.Values;

    public override long ComputeMemory()
        => DataUtility.DictionaryMemory(16, Count) + Values.Sum(v => v.Length * 2);

    public override int ComputeTotalCount()
        => Count;
}
