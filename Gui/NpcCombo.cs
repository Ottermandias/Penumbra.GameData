using Dalamud.Utility;
using ImSharp;
using Penumbra.GameData.DataContainers.Bases;
using Penumbra.GameData.Structs;

namespace Penumbra.GameData.Gui;

/// <summary> A combo to select an NPC of a specific type. </summary>
public sealed class NpcCombo : SimpleFilterCombo<(string Name, NpcId[] Ids)>
{
    public readonly  StringU8                   Label;
    private          (string Name, NpcId[] Ids) _selected = (string.Empty, []);
    private readonly NameDictionary             _names;

    /// <summary> A combo to select an NPC of a specific type. </summary>
    public NpcCombo(StringU8 label, NameDictionary names)
        : base(SimpleFilterType.Partwise)
    {
        Label           = label;
        _names          = names;
        AllowMouseWheel = MouseWheelType.Control;
    }

    public (string Name, NpcId[] Ids) Selected
        => _selected;

    /// <inheritdoc/>
    public override StringU8 DisplayString(in (string, NpcId[]) value)
        => new(value.Item1);

    /// <inheritdoc/>
    public override string FilterString(in (string, NpcId[]) value)
        => value.Item1 + '\0' + string.Join('\0', value.Item2);

    /// <inheritdoc/>
    public override StringU8 Tooltip(in (string, NpcId[]) value)
    {
        if (value.Item2.Length <= 16)
            return StringU8.Join((byte)'\n', value.Item2);

        return StringU8.Join((byte)'\n', value.Item2.Take(16)) + new StringU8($"\nAnd {value.Item2.Length - 16} Others...");
    }

    protected override bool IsSelected(SimpleCacheItem<(string Name, NpcId[] Ids)> item, int globalIndex)
        => item.Item.Name == _selected.Name;

    public bool Draw(float width)
        => Draw(Label, ref _selected, ""u8, width);

    /// <summary>
    /// On creation, group NPCs by their name so that every name represents all IDs that share it.
    /// Then sort by that name using the comparer that prioritizes alphanumerics before special symbols.
    /// </summary>
    public override IEnumerable<(string, NpcId[])> GetBaseItems()
        => _names.GroupBy(kvp => kvp.Value)
            .Select(g => (g.Key, g.Select(g => g.Key).ToArray()))
            .OrderBy(g => g.Key, Comparer)
            .ToList();


    /// <summary> Compare strings in a way that letters and numbers are sorted before any special symbols. </summary>
    private class NameComparer : IComparer<string>
    {
        /// <inheritdoc/>
        public int Compare(string? x, string? y)
        {
            if (x.IsNullOrEmpty() || y.IsNullOrEmpty())
                return StringComparer.OrdinalIgnoreCase.Compare(x, y);

            return (char.IsAsciiLetterOrDigit(x[0]), char.IsAsciiLetterOrDigit(y[0])) switch
            {
                (true, false) => -1,
                (false, true) => 1,
                _             => StringComparer.OrdinalIgnoreCase.Compare(x, y),
            };
        }
    }

    /// <summary> The comparer we use. </summary>
    private static readonly NameComparer Comparer = new();
}
