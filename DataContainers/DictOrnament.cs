using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Lumina.Excel.GeneratedSheets;
using OtterGui.Log;
using Penumbra.GameData.Data;
using Penumbra.GameData.DataContainers.Bases;
using Penumbra.GameData.Structs;

namespace Penumbra.GameData.DataContainers;

/// <summary> A dictionary that matches OrnamentId to names. </summary>
public sealed class DictOrnament(DalamudPluginInterface pluginInterface, Logger log, IDataManager gameData)
    : NameDictionary(pluginInterface, log, gameData, "Ornaments", 6, () => CreateOrnamentData(gameData))
{
    /// <summary> Create the data. </summary>
    private static IReadOnlyDictionary<uint, string> CreateOrnamentData(IDataManager gameData)
    {
        var dict = new Dictionary<uint, string>();
        foreach (var o in gameData.GetExcelSheet<Ornament>(gameData.Language)!
                     .Where(o => o.Singular.RawData.Length > 0))
            dict.TryAdd(o.RowId, DataUtility.ToTitleCaseExtended(o.Singular, o.Article));
        // TODO: FrozenDictionary
        return dict;
    }

    /// <inheritdoc cref="NameDictionary.ContainsKey"/>
    public bool ContainsKey(OrnamentId key)
        => Value.ContainsKey(key.Id);

    /// <inheritdoc cref="NameDictionary.TryGetValue"/>
    public bool TryGetValue(OrnamentId key, [NotNullWhen(true)] out string? value)
        => Value.TryGetValue(key.Id, out value);

    /// <inheritdoc cref="NameDictionary.this"/>
    public string this[OrnamentId key]
        => Value[key.Id];
}
