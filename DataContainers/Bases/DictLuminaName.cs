using Dalamud;
using Dalamud.Plugin;
using OtterGui.Log;
using Penumbra.GameData.Data;

namespace Penumbra.GameData.DataContainers.Bases;

public abstract class DictLuminaName<T>(
    DalamudPluginInterface pluginInterface,
    Logger log,
    string name,
    ClientLanguage language,
    int version,
    Func<IReadOnlyDictionary<string, IReadOnlyList<T>>> factory,
    Task? continuation = null)
    : DataSharer<IReadOnlyDictionary<string, IReadOnlyList<T>>>(pluginInterface, log, name, language, version, factory, continuation),
        IReadOnlyDictionary<string, IReadOnlyList<T>>
{
    protected abstract int TypeSize { get; }

    public override long ComputeMemory()
        => DataUtility.DictionaryMemory(40, Count) + Keys.Sum(s => s.Length + 1) * 2 + Values.Sum(v => v.Count) * TypeSize;

    public override int ComputeTotalCount()
        => Value.Count;

    public IEnumerator<KeyValuePair<string, IReadOnlyList<T>>> GetEnumerator()
        => Value.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public int Count
        => Value.Count;

    public bool ContainsKey(string key)
        => Value.ContainsKey(key);

    public bool TryGetValue(string key, [NotNullWhen(true)] out IReadOnlyList<T>? value)
        => Value.TryGetValue(key, out value);

    public IReadOnlyList<T> this[string key]
        => Value[key];

    public IEnumerable<string> Keys
        => Value.Keys;

    public IEnumerable<IReadOnlyList<T>> Values
        => Value.Values;
}
