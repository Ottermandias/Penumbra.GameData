using ImSharp;
using Penumbra.GameData.DataContainers;

namespace Penumbra.GameData.Gui.Debug;

/// <summary> Draw either all collected or just the valid job groups. </summary>
public class DictJobGroupDrawer(DictJobGroup jobGroups, DictJob jobs) : IGameDataDrawer
{
    /// <inheritdoc/>
    public ReadOnlySpan<byte> Label
        => "Valid Job Groups"u8;

    private bool _showAll;

    /// <inheritdoc/>
    public void Draw()
    {
        if (Im.Checkbox("Show All Job Groups"u8, ref _showAll))
            CacheManager.Instance.SetCustomDirty(Im.Id.Current);

        using var table = Im.Table.Begin("##groups"u8, 4, TableFlags.SizingFixedFit | TableFlags.RowBackground);
        if (!table)
            return;

        var cache = CacheManager.Instance.GetOrCreateCache(Im.Id.Current, () => new Cache(this, jobGroups, jobs));
        foreach (var (id, group, count, jobList) in cache.Data)
        {
            table.DrawColumn(id);
            table.DrawColumn(group);
            table.DrawColumn(count);
            table.DrawColumn(jobList);
        }
    }

    private sealed class Cache(DictJobGroupDrawer parent, DictJobGroup jobGroups, DictJob jobs) : BasicCache
    {
        public readonly List<(StringU8, StringU8, StringU8, StringU8)> Data = [];

        public override void Update()
        {
            var customDirty = CustomDirty;
            Dirty = IManagedCache.DirtyFlags.Clean;
            if (!customDirty)
                return;

            Data.Clear();
            var enumerable = parent._showAll
                ? jobGroups.AllJobGroups
                : jobGroups.Values;
            foreach (var group in enumerable)
            {
                var idString    = new StringU8($"{group.Id.Id:D3}");
                var countString = new StringU8($"{group.Count}");
                var jobsString = StringU8.Join(", "u8,
                    group.Iterate().Select(j => new StringU8($"{jobs[j].Abbreviation} ({jobs[j].Id.Id})").ToList()));
                Data.Add((idString, group.Name, countString, jobsString));
            }
        }
    }

    /// <inheritdoc/>
    public bool Disabled
        => false;
}
