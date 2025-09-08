using ImSharp;
using Penumbra.GameData.DataContainers;
using Penumbra.GameData.Structs;

namespace Penumbra.GameData.Gui;

/// <summary> A combo for selecting worlds by name, or the 'Any World' entry. </summary>
public sealed class WorldCombo(DictWorld worlds) : SimpleFilterCombo<KeyValuePair<WorldId, string>>(SimpleFilterType.Text)
{
    /// <summary> Always the first entry that can be selected. </summary>
    private const string AnyWorldString = "Any World";

    private KeyValuePair<WorldId, string> _selected = new(WorldId.AnyWorld, AnyWorldString);

    public KeyValuePair<WorldId, string> Selected
        => _selected;

    public override StringU8 DisplayString(in KeyValuePair<WorldId, string> value)
        => new(value.Value);

    public override string FilterString(in KeyValuePair<WorldId, string> value)
        => value.Value;

    public override IEnumerable<KeyValuePair<WorldId, string>> GetBaseItems()
        => worlds.OrderBy(kvp => kvp.Value).Prepend(new KeyValuePair<WorldId, string>(WorldId.AnyWorld, AnyWorldString));

    protected override bool IsSelected(SimpleCacheItem<KeyValuePair<WorldId, string>> item, int globalIndex)
        => item.Item.Key == Selected.Key;

    public bool Draw(float width)
        => Draw("##world"u8, ref _selected, ""u8, width);
}
