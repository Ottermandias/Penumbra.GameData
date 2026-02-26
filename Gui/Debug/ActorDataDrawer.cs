using ImSharp;
using Penumbra.GameData.Data;
using Penumbra.GameData.DataContainers;
using ImGuiClip = Dalamud.Interface.Utility.ImGuiClip;

namespace Penumbra.GameData.Gui.Debug;

/// <summary> Draw all collected data for actor identification management. </summary>
public class ActorDataDrawer(NameDicts dicts, DictBNpcNames bNpcNames) : IGameDataDrawer
{
    /// <inheritdoc/>
    public ReadOnlySpan<byte> Label
        => "Actor Data"u8;

    private string _bNpcFilter      = string.Empty;
    private string _eNpcFilter      = string.Empty;
    private string _companionFilter = string.Empty;
    private string _mountFilter     = string.Empty;
    private string _ornamentFilter  = string.Empty;
    private string _worldFilter     = string.Empty;

    /// <inheritdoc/>
    public void Draw()
    {
        DrawBNpcTable();
        DebugUtility.DrawNameTable("Event NPCs"u8, ref _eNpcFilter,      true, dicts.ENpcs.Select(kvp => ((ulong)kvp.Key.Id, kvp.Value)));
        DebugUtility.DrawNameTable("Companions"u8, ref _companionFilter, true, dicts.Companions.Select(kvp => ((ulong)kvp.Key.Id, kvp.Value)));
        DebugUtility.DrawNameTable("Mounts"u8,     ref _mountFilter,     true, dicts.Mounts.Select(kvp => ((ulong)kvp.Key.Id, kvp.Value)));
        DebugUtility.DrawNameTable("Ornaments"u8,  ref _ornamentFilter,  true, dicts.Ornaments.Select(kvp => ((ulong)kvp.Key.Id, kvp.Value)));
        DebugUtility.DrawNameTable("Worlds"u8,     ref _worldFilter,     true, dicts.Worlds.Select(kvp => ((ulong)kvp.Key.Id, kvp.Value)));
    }

    /// <summary> Draw a list of battle NPCs associated with names and base IDs. </summary>
    private void DrawBNpcTable()
    {
        using var tree = Im.Tree.Node("Battle NPCs"u8);
        if (!tree)
            return;

        var resetScroll = Im.Input.Text("##filter"u8, ref _bNpcFilter, "Filter..."u8);
        var height      = Im.Style.TextHeightWithSpacing + 2 * Im.Style.CellPadding.Y;
        using var table = Im.Table.Begin("##table"u8, 3, TableFlags.RowBackground | TableFlags.ScrollY | TableFlags.BordersOuter,
            new Vector2(-1, 10 * height));
        if (!table)
            return;

        if (resetScroll)
            Im.Scroll.Y = 0;
        table.SetupColumn("1"u8, TableColumnFlags.WidthFixed, 50 * Im.Style.GlobalScale);
        table.SetupColumn("2"u8, TableColumnFlags.WidthStretch);
        table.SetupColumn("3"u8, TableColumnFlags.WidthStretch);
        table.NextColumn();
        var skips = ImGuiClip.GetNecessarySkips(height);
        table.NextRow();
        var data = dicts.BNpcs.Select(kvp => (kvp.Key, kvp.Key.Id.ToString("D5"), kvp.Value));
        var remainder = ImGuiClip.FilteredClippedDraw(data, skips,
            p => p.Item2.Contains(_bNpcFilter) || p.Value.Contains(_bNpcFilter, StringComparison.OrdinalIgnoreCase),
            p =>
            {
                Im.Table.DrawColumn(p.Item2);
                Im.Table.DrawColumn(p.Value);
                var bNpcs = bNpcNames.GetBNpcsFromName(p.Key.BNpcNameId);
                Im.Table.DrawColumn(string.Join(", ", bNpcs.Select(b => b.Id.ToString())));
            });
        ImGuiClip.DrawEndDummy(remainder, height);
    }

    /// <inheritdoc/>
    public bool Disabled
        => !dicts.Finished || !bNpcNames.Finished;
}
