using ImSharp;
using Penumbra.GameData.DataContainers;
using Penumbra.GameData.Structs;

namespace Penumbra.GameData.Gui;

/// <summary> A combo for selecting worlds by name, or the 'Any World' entry. </summary>
public sealed class WorldCombo(DictWorld worlds) : SimpleFilterCombo<KeyValuePair<WorldId, string>>(SimpleFilterType.Text)
{
    /// <summary> Always the first entry that can be selected. </summary>
    private const string AnyWorldString = "Any World";

    public override StringU8 DisplayString(in KeyValuePair<WorldId, string> value)
        => new(value.Value);

    public override string FilterString(in KeyValuePair<WorldId, string> value)
        => value.Value;

    public override IEnumerable<KeyValuePair<WorldId, string>> GetBaseItems()
        => worlds.OrderBy(kvp => kvp.Value).Prepend(new KeyValuePair<WorldId, string>(WorldId.AnyWorld, AnyWorldString));
}
