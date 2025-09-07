using Dalamud.Utility;
using ImSharp;
using Penumbra.GameData.DataContainers.Bases;
using Penumbra.GameData.Structs;

namespace Penumbra.GameData.Gui;

/// <summary> A combo to select an NPC of a specific type. </summary>
public sealed class NpcCombo(NameDictionary names) : SimpleFilterCombo<(string, NpcId[])>(SimpleFilterType.Partwise)
{
    /// <inheritdoc/>
    public override StringU8 DisplayString(in (string, NpcId[]) value)
        => new(value.Item1);

    /// <inheritdoc/>
    public override string FilterString(in (string, NpcId[]) value)
        => value.Item1 + '\0' + string.Join('\0', value.Item2);

    /// <inheritdoc/>
    public override StringU8 Tooltip(in (string, NpcId[]) value)
        => StringU8.Join((byte)'\n', value.Item2);

    /// <summary>
    /// On creation, group NPCs by their name so that every name represents all IDs that share it.
    /// Then sort by that name using the comparer that prioritizes alphanumerics before special symbols.
    /// </summary>
    public override IEnumerable<(string, NpcId[])> GetBaseItems()
        => names.GroupBy(kvp => kvp.Value)
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
