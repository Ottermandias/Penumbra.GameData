using System.Collections.Frozen;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using Lumina.Excel.GeneratedSheets;
using OtterGui.Log;
using Penumbra.GameData.Data;
using Penumbra.GameData.DataContainers.Bases;
using Penumbra.GameData.Structs;

namespace Penumbra.GameData.DataContainers;

/// <summary> A dictionary that matches MountId to names. </summary>
public sealed class DictMount(IDalamudPluginInterface pluginInterface, Logger log, IDataManager gameData)
    : NameDictionary(pluginInterface, log, gameData, "Mounts", 7, () => CreateMountData(gameData))
{
    /// <summary> Create the data. </summary>
    private static IReadOnlyDictionary<uint, string> CreateMountData(IDataManager gameData)
    {
        var sheet = gameData.GetExcelSheet<Mount>(gameData.Language)!;
        var dict  = new Dictionary<uint, string>((int) sheet.RowCount);
        // Add some custom data.
        dict.TryAdd(119, "Falcon (Porter)");
        dict.TryAdd(295, "Hippo Cart (Quest)");
        dict.TryAdd(296, "Hippo Cart (Quest)");
        dict.TryAdd(298, "Miw Miisv (Quest)");
        dict.TryAdd(309, "Moon-hopper (Quest)");
        foreach (var m in sheet)
        {
            if (m.Singular.RawData.Length > 0 && m.Order >= 0)
            {
                dict.TryAdd(m.RowId, DataUtility.ToTitleCaseExtended(m.Singular, m.Article));
            }
            else if (m.Unknown18.RawData.Length > 0)
            {
                // Try to transform some file names into category names.
                var whistle = m.Unknown18.ToDalamudString().ToString();
                whistle = whistle.Replace("SE_Bt_Etc_", string.Empty)
                    .Replace("Mount_", string.Empty)
                    .Replace("_call", string.Empty)
                    .Replace("Whistle", string.Empty);
                dict.TryAdd(m.RowId, $"? {whistle} #{m.RowId}");
            }
        }

        return dict.ToFrozenDictionary();
    }

    /// <inheritdoc cref="NameDictionary.ContainsKey"/>
    public bool ContainsKey(MountId key)
        => Value.ContainsKey(key.Id);

    /// <inheritdoc cref="NameDictionary.TryGetValue"/>
    public bool TryGetValue(MountId key, [NotNullWhen(true)] out string? value)
        => Value.TryGetValue(key.Id, out value);

    /// <inheritdoc cref="NameDictionary.this"/>
    public string this[MountId key]
        => Value[key.Id];
}
