using Dalamud;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Penumbra.GameData.Data;
using Penumbra.GameData.Structs;

namespace Penumbra.GameData.DataContainers.Bases;

public abstract class ItemDictionary(
    DalamudPluginInterface pluginInterface,
    IPluginLog log,
    string name,
    ClientLanguage language,
    int version,
    Func<IReadOnlyDictionary<uint, PseudoEquipItem>> factory,
    Task? continuation = null)
    : DataSharer<IReadOnlyDictionary<uint, PseudoEquipItem>>(pluginInterface, log, name, language, version, factory, continuation),
        IReadOnlyDictionary<uint, EquipItem>
{
    public IEnumerator<KeyValuePair<uint, EquipItem>> GetEnumerator()
        => Value.Select(kvp => new KeyValuePair<uint, EquipItem>(kvp.Key, kvp.Value)).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public int Count
        => Value.Count;

    public bool ContainsKey(uint key)
        => Value.ContainsKey(key);

    public bool TryGetValue(uint key, out EquipItem value)
    {
        if (Value.TryGetValue(key, out var v))
        {
            value = v;
            return true;
        }

        value = default;
        return false;
    }

    public EquipItem this[uint key]
        => Value[key];

    public IEnumerable<uint> Keys
        => Value.Keys;

    public IEnumerable<EquipItem> Values
        => Value.Values.Select(e => (EquipItem)e);

    public override long ComputeMemory()
        => DataUtility.DictionaryMemory(40, Count);

    public override int ComputeTotalCount()
        => Count;
}
