using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Lumina.Excel.GeneratedSheets;
using Penumbra.GameData.Data;
using Penumbra.GameData.DataContainers.Bases;
using Penumbra.GameData.Structs;

namespace Penumbra.GameData.DataContainers;

public sealed class DictCompanion(DalamudPluginInterface pluginInterface, IPluginLog log, IDataManager gameData)
    : NameDictionary(pluginInterface, log, gameData, "Companions", 6, () => CreateCompanionData(gameData))
{
    private static IReadOnlyDictionary<uint, string> CreateCompanionData(IDataManager gameData)
    {
        var dict = new Dictionary<uint, string>();
        foreach (var c in gameData.GetExcelSheet<Companion>(gameData.Language)!
                     .Where(c => c.Singular.RawData.Length > 0 && c.Order < ushort.MaxValue))
            dict.TryAdd(c.RowId, DataUtility.ToTitleCaseExtended(c.Singular, c.Article));
        return dict;
    }

    public bool ContainsKey(CompanionId key)
        => Value.ContainsKey(key.Id);

    public bool TryGetValue(CompanionId key, [NotNullWhen(true)] out string? value)
        => Value.TryGetValue(key.Id, out value);

    public string this[CompanionId key]
        => Value[key.Id];
}
