using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using Lumina.Excel.GeneratedSheets;
using Penumbra.GameData.Data;
using Penumbra.GameData.DataContainers.Bases;
using Penumbra.GameData.Structs;

namespace Penumbra.GameData.DataContainers;

public sealed class DictMount(DalamudPluginInterface pluginInterface, IPluginLog log, IDataManager gameData)
    : NameDictionary(pluginInterface, log, gameData, "Mounts", 6, () => CreateMountData(gameData))
{
    private static IReadOnlyDictionary<uint, string> CreateMountData(IDataManager gameData)
    {
        var dict = new Dictionary<uint, string>();
        dict.TryAdd(119, "Falcon (Porter)");
        dict.TryAdd(295, "Hippo Cart (Quest)");
        dict.TryAdd(296, "Hippo Cart (Quest)");
        dict.TryAdd(298, "Miw Miisv (Quest)");
        dict.TryAdd(309, "Moon-hopper (Quest)");
        foreach (var m in gameData.GetExcelSheet<Mount>(gameData.Language)!)
        {
            if (m.Singular.RawData.Length > 0 && m.Order >= 0)
            {
                dict.TryAdd(m.RowId, DataUtility.ToTitleCaseExtended(m.Singular, m.Article));
            }
            else if (m.Unknown18.RawData.Length > 0)
            {
                var whistle = m.Unknown18.ToDalamudString().ToString();
                whistle = whistle.Replace("SE_Bt_Etc_", string.Empty)
                    .Replace("Mount_", string.Empty)
                    .Replace("_call", string.Empty)
                    .Replace("Whistle", string.Empty);
                dict.TryAdd(m.RowId, $"? {whistle} #{m.RowId}");
            }
        }

        return dict;
    }

    public bool ContainsKey(MountId key)
        => Value.ContainsKey(key.Id);

    public bool TryGetValue(MountId key, [NotNullWhen(true)] out string? value)
        => Value.TryGetValue(key.Id, out value);

    public string this[MountId key]
        => Value[key.Id];
}
