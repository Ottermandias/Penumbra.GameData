using System.Collections.Frozen;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;
using Luna;
using Penumbra.GameData.DataContainers.Bases;
using Penumbra.GameData.Structs;

namespace Penumbra.GameData.DataContainers;

#pragma warning disable SeStringEvaluator

/// <summary> A dictionary that matches BNpcNameId to names. </summary>
public sealed class DictBNpc(IDalamudPluginInterface pluginInterface, Logger log, IDataManager gameData, ISeStringEvaluator evaluator)
    : NameDictionary(pluginInterface, log, gameData, "BNpcs", Version.DictBNpc, () => CreateBNpcData(gameData, evaluator))
{
    /// <summary> Create the data. </summary>
    private static IReadOnlyDictionary<uint, string> CreateBNpcData(IDataManager gameData, ISeStringEvaluator evaluator)
    {
        var sheet = gameData.GetExcelSheet<BNpcName>(gameData.Language)!;
        var dict = new Dictionary<uint, string>(sheet.Count);
        foreach (var n in sheet.Where(n => n.Singular.ByteLength > 0))
            dict.TryAdd(n.RowId, evaluator.EvaluateObjStr(ObjectKind.BattleNpc, n.RowId, gameData.Language));
        return dict.ToFrozenDictionary();
    }

    /// <inheritdoc cref="NameDictionary.ContainsKey"/>
    public bool ContainsKey(BNpcNameId key)
        => Value.ContainsKey(key.Id);

    /// <inheritdoc cref="NameDictionary.TryGetValue"/>
    public bool TryGetValue(BNpcNameId key, [NotNullWhen(true)] out string? value)
        => Value.TryGetValue(key.Id, out value);

    /// <inheritdoc cref="NameDictionary.this"/>
    public string this[BNpcNameId key]
        => Value[key.Id];
}
