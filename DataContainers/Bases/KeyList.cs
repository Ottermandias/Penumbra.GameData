using Dalamud;
using Dalamud.Plugin;
using OtterGui.Log;

namespace Penumbra.GameData.DataContainers.Bases;

/// <summary>
/// A list sorting objects based on a key which then allows efficiently finding all objects between a pair of keys via binary search.
/// </summary>
public abstract class KeyList<T> : DataSharer<List<(ulong Key, T Data)>>
{
    private List<(ulong Key, T Data)> InternalValue
        => base.Value;

    /// <summary> Do not expose the modifiable list. </summary>
    public new IReadOnlyList<(ulong Key, T Data)> Value
        => base.Value;

    /// <summary>
    /// Iterate over all objects between the given minimal and maximal keys (inclusive).
    /// </summary>
    protected IEnumerable<T> Between(ulong minKey, ulong maxKey)
    {
        var (minIdx, maxIdx) = GetMinMax(minKey, maxKey);
        if (minIdx < 0)
            yield break;

        for (var i = minIdx; i <= maxIdx; ++i)
            yield return Value[i].Data;
    }

    private (int MinIdx, int MaxIdx) GetMinMax(ulong minKey, ulong maxKey)
    {
        var idx = InternalValue.BinarySearch((minKey, default!), ListComparer);
        var minIdx = idx;
        if (minIdx < 0)
        {
            minIdx = ~minIdx;
            if (minIdx == InternalValue.Count || InternalValue[minIdx].Key > maxKey)
                return (-1, -1);

            idx = minIdx;
        }
        else
        {
            while (minIdx > 0 && InternalValue[minIdx - 1].Key >= minKey)
                --minIdx;
        }

        if (InternalValue[minIdx].Key < minKey || InternalValue[minIdx].Key > maxKey)
            return (-1, -1);


        var maxIdx = InternalValue.BinarySearch(idx, InternalValue.Count - idx, (maxKey, default!), ListComparer);
        if (maxIdx < 0)
        {
            maxIdx = ~maxIdx;
            return maxIdx > minIdx ? (minIdx, maxIdx - 1) : (-1, -1);
        }

        while (maxIdx < InternalValue.Count - 1 && InternalValue[maxIdx + 1].Key <= maxKey)
            ++maxIdx;

        if (InternalValue[maxIdx].Key < minKey || InternalValue[maxIdx].Key > maxKey)
            return (-1, -1);

        return (minIdx, maxIdx);
    }

    protected KeyList(DalamudPluginInterface pi, Logger log, string name, ClientLanguage language, int version, Func<IEnumerable<T>> data,
        Func<T, IEnumerable<ulong>> toKeys, Func<ulong, bool> validKey, Func<T, int> valueKeySelector, Task? continuation = null)
        : base(pi, log, name, language, version,
            () => data().SelectMany(d => toKeys(d).Select(k => (k, d))).Where(p => validKey(p.k)).OrderBy(p => p.k)
                .ThenBy(p => valueKeySelector(p.d)).ToList(), continuation)
    { }

    protected KeyList(DalamudPluginInterface pi, Logger log, string name, ClientLanguage language, int version, Func<IEnumerable<T>> data,
        Func<T, ulong> toKey, Func<ulong, bool> validKey, Func<T, int> valueKeySelector, Task? continuation = null)
        : base(pi, log, name, language, version,
            () => data().Select(d => (toKey(d), d)).Where(p => validKey(p.Item1)).OrderBy(p => p.Item1)
                .ThenBy(p => valueKeySelector(p.d)).ToList(), continuation)
    { }

    private class Comparer : IComparer<(ulong, T)>
    {
        public int Compare((ulong, T) x, (ulong, T) y)
            => x.Item1.CompareTo(y.Item1);
    }

    private static readonly Comparer ListComparer = new();

    public override int ComputeTotalCount()
        => base.Value.Count;
}
