using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Lumina.Excel.GeneratedSheets;
using Penumbra.GameData.Data;
using Penumbra.GameData.DataContainers.Bases;
using Penumbra.GameData.Structs;

namespace Penumbra.GameData.DataContainers;

public sealed class DictBNpc(DalamudPluginInterface pluginInterface, IPluginLog log, IDataManager gameData)
    : NameDictionary(pluginInterface, log, gameData, "BNpcs", 6, () => CreateBNpcData(gameData))
{
    private static IReadOnlyDictionary<uint, string> CreateBNpcData(IDataManager gameData)
    {
        var dict = new Dictionary<uint, string>();
        foreach (var n in gameData.GetExcelSheet<BNpcName>(gameData.Language)!.Where(n => n.Singular.RawData.Length > 0))
            dict.TryAdd(n.RowId, DataUtility.ToTitleCaseExtended(n.Singular, n.Article));
        return dict;
    }

    public bool ContainsKey(BNpcNameId key)
        => Value.ContainsKey(key.Id);

    public bool TryGetValue(BNpcNameId key, [NotNullWhen(true)] out string? value)
        => Value.TryGetValue(key.Id, out value);

    public string this[BNpcNameId key]
        => Value[key.Id];
}
