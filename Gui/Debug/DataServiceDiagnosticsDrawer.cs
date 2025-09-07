using ImSharp;
using Luna;

namespace Penumbra.GameData.Gui.Debug;

/// <summary> Draws a table of all data containers and their diagnostic data. </summary>
public class DataServiceDiagnosticsDrawer(ServiceManager manager) : IGameDataDrawer
{
    /// <inheritdoc/>
    public ReadOnlySpan<byte> Label
        => "Diagnostics"u8;

    private string _filter = string.Empty;
    private int    _orderBy;

    /// <inheritdoc/>
    public void Draw()
    {
        Im.Input.Text("##filter"u8, ref _filter, "Filter..."u8);
        DrawSortCombo();
        using var table = Im.Table.Begin("services"u8, 5, TableFlags.SizingFixedFit | TableFlags.RowBackground);
        if (!table)
            return;

        table.SetupScrollFreeze(0, 1);
        table.SetupColumn("Name"u8,           TableColumnFlags.WidthFixed, 300 * Im.Style.GlobalScale);
        table.SetupColumn("Available"u8,      TableColumnFlags.WidthFixed, 55 * Im.Style.GlobalScale);
        table.SetupColumn("Setup Time"u8,     TableColumnFlags.WidthFixed, 100 * Im.Style.GlobalScale);
        table.SetupColumn("Total Items"u8,    TableColumnFlags.WidthFixed, 100 * Im.Style.GlobalScale);
        table.SetupColumn("Approx. Memory"u8, TableColumnFlags.WidthFixed, 100 * Im.Style.GlobalScale);

        table.HeaderRow();
        var services = manager.GetServicesImplementing<IDataContainer>();
        services = _orderBy switch
        {
            1 => services.OrderByDescending(c => c.Time),
            2 => services.OrderByDescending(c => c.Memory),
            3 => services.OrderByDescending(c => c.TotalCount),
            _ => services.OrderBy(c => c.Name),
        };

        foreach (var c in services)
        {
            if (!c.Name.Contains(_filter, StringComparison.OrdinalIgnoreCase))
                continue;

            var finished = c is not IAsyncService a || a.Finished;
            table.DrawColumn(c.Name);
            table.DrawColumn(finished.ToString());
            if (!finished)
            {
                table.NextRow();
            }
            else
            {
                table.DrawColumn($"{c.Time / 1000}.{c.Time % 1000:D3} s");
                table.DrawColumn(c.TotalCount.ToString());
                table.DrawColumn(FormattingFunctions.HumanReadableSize(c.Memory));
            }
        }
    }

    /// <inheritdoc/>
    public bool Disabled
        => false;

    /// <summary> Sort the table. </summary>
    private void DrawSortCombo()
    {
        using var combo = Im.Combo.Begin("Sort Order"u8, ToName(_orderBy));
        if (!combo)
            return;

        foreach (var idx in Enumerable.Range(0, 4))
        {
            if (Im.Selectable(ToName(idx), _orderBy == idx))
                _orderBy = idx;
        }

        return;

        static ReadOnlySpan<byte> ToName(int idx)
            => idx switch
            {
                0 => "Name"u8,
                1 => "Time"u8,
                2 => "Memory"u8,
                3 => "Items"u8,
                _ => ""u8,
            };
    }
}
