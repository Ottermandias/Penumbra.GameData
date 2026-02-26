using ImSharp;
using Penumbra.GameData.DataContainers;

namespace Penumbra.GameData.Gui.Debug;

/// <summary> Draw all collected jobs. </summary>
public class DictJobDrawer(DictJob jobs) : IGameDataDrawer
{
    /// <inheritdoc/>
    public ReadOnlySpan<byte> Label
        => "Jobs"u8;

    /// <inheritdoc/>
    public void Draw()
    {
        using var table = Im.Table.Begin("##jobs"u8, 3, TableFlags.SizingFixedFit | TableFlags.RowBackground);
        if (!table)
            return;

        foreach (var (id, job) in jobs)
        {
            table.DrawColumn($"{id.Id:D3}");
            table.DrawColumn(job.Name);
            table.DrawColumn(job.Abbreviation);
        }
    }

    /// <inheritdoc/>
    public bool Disabled
        => false;
}
