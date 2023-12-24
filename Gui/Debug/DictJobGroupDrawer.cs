using ImGuiNET;
using OtterGui;
using OtterGui.Raii;
using Penumbra.GameData.DataContainers;

namespace Penumbra.GameData.Gui.Debug;

/// <summary> Draw either all collected or just the valid job groups. </summary>
public class DictJobGroupDrawer(DictJobGroup _jobGroups) : IGameDataDrawer
{
    /// <inheritdoc/>
    public string Label
        => "Valid Job Groups";

    private bool _showAll;

    /// <inheritdoc/>
    public void Draw()
    {
        ImGui.Checkbox("Show All Job Groups", ref _showAll);

        using var table = ImRaii.Table("##groups", 3, ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.RowBg);
        if (!table)
            return;

        var enumerable = _showAll
            ? _jobGroups.AllJobGroups.Select(g => (g.Id.Id.ToString("D3"), g.Name, g.Count.ToString()))
            : _jobGroups.Select(g => (g.Key.Id.ToString("D3"), g.Value.Name, g.Value.Count.ToString()));

        foreach (var (id, group, count) in enumerable)
        {
            ImGuiUtil.DrawTableColumn(id);
            ImGuiUtil.DrawTableColumn(group);
            ImGuiUtil.DrawTableColumn(group);
        }
    }

    /// <inheritdoc/>
    public bool Disabled
        => false;
}
