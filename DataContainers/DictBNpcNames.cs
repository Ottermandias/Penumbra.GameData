using Dalamud;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Penumbra.GameData.Data;
using Penumbra.GameData.DataContainers.Bases;
using Penumbra.GameData.Structs;

namespace Penumbra.GameData.DataContainers;

public sealed class DictBNpcNames(DalamudPluginInterface pluginInterface, IPluginLog log)
    : DataSharer<IReadOnlyList<IReadOnlyList<uint>>>(pluginInterface, log, "BNpcNameDict", ClientLanguage.English, 1, NpcNames.CreateNames),
        IReadOnlyDictionary<BNpcId, IReadOnlyList<BNpcNameId>>
{
    public override long ComputeMemory()
        => 16 * (Count + 1) + TotalCount * 4;

    public override int ComputeTotalCount()
        => Value.Sum(l => l.Count);

    public IEnumerator<KeyValuePair<BNpcId, IReadOnlyList<BNpcNameId>>> GetEnumerator()
        => Value.Select((list, idx) => new KeyValuePair<BNpcId, IReadOnlyList<BNpcNameId>>((uint)idx, BNpcNameList.Create(list)))
            .GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public int Count
        => Value.Count;

    public bool ContainsKey(BNpcId key)
        => key.Id < Count;

    public bool TryGetValue(BNpcId key, out IReadOnlyList<BNpcNameId> value)
    {
        if (key.Id >= Count)
        {
            value = Array.Empty<BNpcNameId>();
            return false;
        }

        value = BNpcNameList.Create(Value[(int)key.Id]);
        return true;
    }

    public IReadOnlyList<BNpcNameId> this[BNpcId key]
        => TryGetValue(key, out var value) ? value : Array.Empty<BNpcNameId>();

    public IEnumerable<BNpcId> Keys
        => Enumerable.Range(0, Value.Count).Select(v => new BNpcId((uint)v));

    public IEnumerable<IReadOnlyList<BNpcNameId>> Values
        => Value.Select(BNpcNameList.Create);

    private readonly struct BNpcNameList(IReadOnlyList<uint> items) : IReadOnlyList<BNpcNameId>
    {
        public static IReadOnlyList<BNpcNameId> Create(IReadOnlyList<uint> items)
            => items.Count > 0 ? new BNpcNameList(items) : Array.Empty<BNpcNameId>();

        public IEnumerator<BNpcNameId> GetEnumerator()
            => items.Select(i => (BNpcNameId)i).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public int Count
            => items.Count;

        public BNpcNameId this[int index]
            => items[index];
    }
}
