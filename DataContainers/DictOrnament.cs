using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Lumina.Excel.GeneratedSheets;
using OtterGui.Log;
using Penumbra.GameData.Data;
using Penumbra.GameData.DataContainers.Bases;
using Penumbra.GameData.Structs;

namespace Penumbra.GameData.DataContainers;

public sealed class DictOrnament(DalamudPluginInterface pluginInterface, Logger log, IDataManager gameData)
    : NameDictionary(pluginInterface, log, gameData, "Ornaments", 6, () => CreateOrnamentData(gameData))
{
    private static IReadOnlyDictionary<uint, string> CreateOrnamentData(IDataManager gameData)
    {
        var dict = new Dictionary<uint, string>();
        foreach (var o in gameData.GetExcelSheet<Ornament>(gameData.Language)!
                     .Where(o => o.Singular.RawData.Length > 0))
            dict.TryAdd(o.RowId, DataUtility.ToTitleCaseExtended(o.Singular, o.Article));
        return dict;
    }

    public bool ContainsKey(OrnamentId key)
        => Value.ContainsKey(key.Id);

    public bool TryGetValue(OrnamentId key, [NotNullWhen(true)] out string? value)
        => Value.TryGetValue(key.Id, out value);

    public string this[OrnamentId key]
        => Value[key.Id];
}
