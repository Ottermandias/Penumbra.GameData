using ImGuiNET;
using OtterGui.Log;
using OtterGui.Widgets;
using Penumbra.GameData.Structs;

namespace Penumbra.GameData.Gui;

public sealed class WorldCombo : FilterComboCache<KeyValuePair<WorldId, string>>
{
    private const string AnyWorldString = "Any World";

    public WorldCombo(IReadOnlyDictionary<WorldId, string> worlds, Logger log, WorldId allWorldValue)
        : base(worlds.OrderBy(kvp => kvp.Value).Prepend(new KeyValuePair<WorldId, string>(allWorldValue, AnyWorldString)), log)
    {
        CurrentSelection    = new KeyValuePair<WorldId, string>(allWorldValue, AnyWorldString);
        CurrentSelectionIdx = 0;
    }

    protected override string ToString(KeyValuePair<WorldId, string> obj)
        => obj.Value;

    public bool Draw(float width)
        => Draw("##worldCombo", CurrentSelection.Value, string.Empty, width, ImGui.GetTextLineHeightWithSpacing());
}
