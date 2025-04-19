using Dalamud.Game;
using Dalamud.Utility;
using Lumina.Text.ReadOnly;

namespace Penumbra.GameData.Data;

/// <summary> Some utility functions for data containers. </summary>
public static class DataUtility
{
    /// <summary> Convert to text and replace unprintable symbols. </summary>
    public static string ExtractTextExtended(this ReadOnlySeString s)
        => s.ExtractText().Replace("\u00AD", string.Empty).Replace("\u00A0", string.Empty);

    /// <summary> Convert a given ReadonlySeString to title case and intern it. </summary>
    /// <param name="s"> The string to convert. </param>
    /// <param name="language"> The language to choose words excluded from capitalization from. </param>
    /// <returns> The interned string in title case. </returns>
    public static string ToTitleCaseExtended(in ReadOnlySeString s, ClientLanguage language)
        => string.Intern(s.ExtractTextExtended().ToUpper(true, true, false, language));

    /// <summary> Approximate the memory a dictionary needs </summary>
    /// <param name="tupleSize"> The approximate size of the KeyValuePair. </param>
    /// <param name="count"> The number of entries. </param>
    public static int DictionaryMemory(int tupleSize, int count)
        => 64 + (tupleSize + 16) * count;

    /// <summary> Approximate the memory an array needs </summary>
    /// <param name="dataSize"> The approximate size of the data. </param>
    /// <param name="count"> The number of entries. </param>
    public static int ArrayMemory(int dataSize, int count)
        => 16 + dataSize * count;

    /// <summary> Approximate the memory a list needs </summary>
    /// <param name="dataSize"> The approximate size of the data. </param>
    /// <param name="count"> The number of entries. </param>
    public static int ListMemory(int dataSize, int count)
        => 24 + dataSize * count;
}
