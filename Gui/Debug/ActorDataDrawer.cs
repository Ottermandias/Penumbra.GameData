using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
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

        var resetScroll = ImGui.InputTextWithHint("##filter", "Filter...", ref _bNpcFilter, 256);
        var height      = ImGui.GetTextLineHeightWithSpacing() + 2 * ImGui.GetStyle().CellPadding.Y;
        using var table = ImRaii.Table("##table", 3, ImGuiTableFlags.RowBg | ImGuiTableFlags.ScrollY | ImGuiTableFlags.BordersOuter,
            new Vector2(-1, 10 * height));
        if (!table)
            return;

        if (resetScroll)
            ImGui.SetScrollY(0);
        ImGui.TableSetupColumn("1", ImGuiTableColumnFlags.WidthFixed, 50 * ImGuiHelpers.GlobalScale);
        ImGui.TableSetupColumn("2", ImGuiTableColumnFlags.WidthStretch);
        ImGui.TableSetupColumn("3", ImGuiTableColumnFlags.WidthStretch);
        ImGui.TableNextColumn();
        var skips = ImGuiClip.GetNecessarySkips(height);
        ImGui.TableNextRow();
        var data = dicts.BNpcs.Select(kvp => (kvp.Key, kvp.Key.Id.ToString("D5"), kvp.Value));
        var remainder = ImGuiClip.FilteredClippedDraw(data, skips,
            p => p.Item2.Contains(_bNpcFilter) || p.Value.Contains(_bNpcFilter, StringComparison.OrdinalIgnoreCase),
            p =>
            {
                ImGuiUtil.DrawTableColumn(p.Item2);
                ImGuiUtil.DrawTableColumn(p.Value);
                var bNpcs = bNpcNames.GetBNpcsFromName(p.Key.BNpcNameId);
                ImGuiUtil.DrawTableColumn(string.Join(", ", bNpcs.Select(b => b.Id.ToString())));
            });
        ImGuiClip.DrawEndDummy(remainder, height);
    }

    /// <inheritdoc/>
    public bool Disabled
        => !dicts.Finished || !bNpcNames.Finished;
}
