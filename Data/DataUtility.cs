using Dalamud.Utility;
using Lumina.Text;

namespace Penumbra.GameData.Data;

public static class DataUtility
{
    public static string ToTitleCaseExtended(SeString s, sbyte article)
    {
        if (article == 1)
            return string.Intern(s.ToDalamudString().ToString());

        var sb = new StringBuilder(s.ToDalamudString().ToString());
        var lastSpace = true;
        for (var i = 0; i < sb.Length; ++i)
        {
            if (sb[i] == ' ')
            {
                lastSpace = true;
            }
            else if (lastSpace)
            {
                lastSpace = false;
                sb[i] = char.ToUpperInvariant(sb[i]);
            }
        }

        return string.Intern(sb.ToString());
    }

    public static int DictionaryMemory(int tupleSize, int count)
        => 64 + (tupleSize + 16) * count;
}
