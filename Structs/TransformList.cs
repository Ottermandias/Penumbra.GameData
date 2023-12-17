namespace Penumbra.GameData.Structs;

/// <summary>
/// A IReadOnlyList based on any other IReadOnlyList that applies a transforming step before fetching.
/// </summary>
public readonly struct TransformList<TIn, TOut>(IReadOnlyList<TIn> items, Func<TIn, TOut> transform) : IReadOnlyList<TOut>
{
    public IEnumerator<TOut> GetEnumerator()
        => items.Select(transform).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public int Count
        => items.Count;

    public TOut this[int index]
        => transform(items[index]);
}
