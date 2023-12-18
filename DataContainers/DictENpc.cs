using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Lumina.Excel.GeneratedSheets;
using OtterGui.Log;
using Penumbra.GameData.Data;
using Penumbra.GameData.DataContainers.Bases;
using Penumbra.GameData.Structs;

namespace Penumbra.GameData.DataContainers;

public sealed class DictENpc(DalamudPluginInterface pluginInterface, Logger log, IDataManager gameData)
    : NameDictionary(pluginInterface, log, gameData, "ENpcs", 6, () => CreateENpcData(gameData))
{
    private static IReadOnlyDictionary<uint, string> CreateENpcData(IDataManager gameData)
    {
        var dict = new Dictionary<uint, string>();
        foreach (var n in gameData.GetExcelSheet<ENpcResident>(gameData.Language)!.Where(e => e.Singular.RawData.Length > 0))
            dict.TryAdd(n.RowId, DataUtility.ToTitleCaseExtended(n.Singular, n.Article));
        return dict;
    }

    public bool ContainsKey(ENpcId key)
        => Value.ContainsKey(key.Id);

    public bool TryGetValue(ENpcId key, [NotNullWhen(true)] out string? value)
        => Value.TryGetValue(key.Id, out value);

    public string this[ENpcId key]
        => Value[key.Id];
}
