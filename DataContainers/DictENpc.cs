using System.Collections.Frozen;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;
using OtterGui.Log;
using Penumbra.GameData.DataContainers.Bases;
using Penumbra.GameData.Structs;

namespace Penumbra.GameData.DataContainers;

#pragma warning disable SeStringEvaluator

/// <summary> A dictionary that matches ENpcId to names. </summary>
public sealed class DictENpc(IDalamudPluginInterface pluginInterface, Logger log, IDataManager gameData, ISeStringEvaluator evaluator)
    : NameDictionary(pluginInterface, log, gameData, "ENpcs", Version.DictENpc, () => CreateENpcData(gameData, evaluator))
{
    /// <summary> Create the data. </summary>
    private static IReadOnlyDictionary<uint, string> CreateENpcData(IDataManager gameData, ISeStringEvaluator evaluator)
    {
        var sheet = gameData.GetExcelSheet<ENpcResident>(gameData.Language);
        var dict = new Dictionary<uint, string>(sheet.Count);
        foreach (var n in sheet.Where(e => e.Singular.ByteLength > 0))
            dict.TryAdd(n.RowId, evaluator.EvaluateObjStr(ObjectKind.EventNpc, n.RowId, gameData.Language));
        return dict.ToFrozenDictionary();
    }

    /// <inheritdoc cref="NameDictionary.ContainsKey"/>
    public bool ContainsKey(ENpcId key)
        => Value.ContainsKey(key.Id);

    /// <inheritdoc cref="NameDictionary.TryGetValue"/>
    public bool TryGetValue(ENpcId key, [NotNullWhen(true)] out string? value)
        => Value.TryGetValue(key.Id, out value);

    /// <inheritdoc cref="NameDictionary.this"/>
    public string this[ENpcId key]
        => Value[key.Id];
}
