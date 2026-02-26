using ImSharp;
using ImGuiClip = Dalamud.Interface.Utility.ImGuiClip;

namespace Penumbra.GameData.Gui.Debug;

/// <summary> Utility for drawing debug data. </summary>
public static class DebugUtility
{
    /// <summary> Draw a filtered table for a set of IDs and names. </summary>
    /// <param name="label"> The label for the object. </param>
    /// <param name="filter"> A filter for the displayed names. Can also filter for the IDs. </param>
    /// <param name="withTree"> Whether to wrap the table into a tree node. </param>
    /// <param name="names"> The enumerable list of names and IDs. </param>
    public static void DrawNameTable(Utf8StringHandler<LabelStringHandlerBuffer> label, ref string filter, bool withTree, IEnumerable<(ulong, string)> names)
    {
        using var tree = withTree ? Im.Tree.Node(label) : new Im.TreeNodeDisposable();
        if (!tree.Success)
            return;

        using var _           = Im.Id.Push(ref label);
        var       resetScroll = Im.Input.Text("##filter"u8, ref filter, "Filter..."u8);
        var       height      = Im.Style.TextHeightWithSpacing+ 2 * Im.Style.CellPadding.Y;
        using var table = Im.Table.Begin("##table"u8, 2, TableFlags.RowBackground | TableFlags.ScrollY | TableFlags.BordersOuter,
            new Vector2(-1, 10 * height));
        if (!table)
            return;

        if (resetScroll)
            Im.Scroll.Y = 0;
        table.SetupColumn("1"u8, TableColumnFlags.WidthFixed, 50 * Im.Style.GlobalScale);
        table.SetupColumn("2"u8, TableColumnFlags.WidthStretch);
        table.NextColumn();
        var skips = ImGuiClip.GetNecessarySkips(height);
        table.NextColumn();
        var f = filter;
        var remainder = ImGuiClip.FilteredClippedDraw(names.Select(p => (p.Item1.ToString("D5"), p.Item2)), skips,
            p => p.Item1.Contains(f) || p.Item2.Contains(f, StringComparison.OrdinalIgnoreCase),
            p =>
            {
                Im.Table.DrawColumn(p.Item1);
                Im.Table.DrawColumn(p.Item2);
            });
        ImGuiClip.DrawEndDummy(remainder, height);
    }
}
