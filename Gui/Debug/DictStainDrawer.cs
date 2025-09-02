using ImSharp;
using Penumbra.GameData.DataContainers;
using Stain = Penumbra.GameData.Structs.Stain;

namespace Penumbra.GameData.Gui.Debug;

/// <summary> Draw collected stain data. </summary>
public class DictStainDrawer(DictStain stains) : IGameDataDrawer
{
    /// <inheritdoc/>
    public ReadOnlySpan<byte> Label
        => "Stains"u8;

    /// <inheritdoc/>
    public bool Disabled
        => !stains.Finished;

    private readonly Filter _filter = new();

    /// <inheritdoc/>
    public void Draw()
    {
        const TableFlags flags       = TableFlags.RowBackground | TableFlags.ScrollY | TableFlags.BordersOuter | TableFlags.SizingFixedFit;
        var              resetScroll = _filter.DrawFilter("Filter..."u8, Im.ContentRegion.Available);
        var              height      = Im.Style.TextHeightWithSpacing + 2 * Im.Style.CellPadding.Y;
        using var        table       = Im.Table.Begin("##table"u8, 4, flags, new Vector2(Im.ContentRegion.Available.X, 10 * height));
        if (!table)
            return;

        if (resetScroll)
            Im.Scroll.Y = 0;

        var       cache   = CacheManager.Instance.GetOrCreateCache(Im.Id.Current, () => new Cache(this, stains, _filter));
        using var clipper = new Im.ListClipper(cache.Count, height);
        foreach (var (item, _) in clipper.Iterate(cache))
        {
            table.DrawColumn(item.Id);
            table.NextColumn();
            ImEx.ColorFrame(item.Stain.RgbaColor);
            table.DrawColumn(item.Name);
            table.DrawColumn(item.Data);
        }
    }

    private readonly record struct CachedStain(StringU8 Id, StringU8 Data, Stain Stain, StringU8 Name, string IdU16)
    {
        public CachedStain(Stain stain)
            : this(new StringU8($"{stain.RowIndex.Id:D3}"),
                new StringU8($"#{stain.R:X2}{stain.G:X2}{stain.B:X2}{(stain.Gloss ? ", Glossy" : string.Empty)}"),
                stain,
                new StringU8(stain.Name),
                $"{stain.RowIndex.Id:D3}")
        { }
    }

    private sealed class Filter : TextFilterBase<CachedStain>
    {
        public override bool WouldBeVisible(in CachedStain item, int globalIndex)
            => Text.Length is 0 || item.Stain.Name.Contains(Text, Comparison) || item.IdU16.Contains(Text, Comparison);

        protected override string ToFilterString(in CachedStain item, int globalIndex)
            => item.Stain.Name;
    }

    private sealed class Cache(DictStainDrawer parent, DictStain stains, Filter filter) : BasicFilterCache<CachedStain>(filter)
    {
        protected override IEnumerable<CachedStain> GetItems()
            => stains.Select(s => new CachedStain(s.Value));
    }
}
