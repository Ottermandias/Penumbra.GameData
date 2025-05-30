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

/// <summary> A dictionary that matches CompanionId to names. </summary>
public sealed class DictCompanion(IDalamudPluginInterface pluginInterface, Logger log, IDataManager gameData, ISeStringEvaluator evaluator)
    : NameDictionary(pluginInterface, log, gameData, "Companions", Version.DictCompanion, () => CreateCompanionData(gameData, evaluator))
{
    /// <summary> Create the data. </summary>
    private static IReadOnlyDictionary<uint, string> CreateCompanionData(IDataManager gameData, ISeStringEvaluator evaluator)
    {
        var sheet = gameData.GetExcelSheet<Companion>(gameData.Language)!;
        var dict = new Dictionary<uint, string>(sheet.Count);
        foreach (var c in sheet.Where(c => c.Singular.ByteLength > 0 && c.Order < ushort.MaxValue))
            dict.TryAdd(c.RowId, evaluator.EvaluateObjStr(ObjectKind.Companion, c.RowId, gameData.Language));
        return dict.ToFrozenDictionary();
    }

    /// <inheritdoc cref="NameDictionary.ContainsKey"/>
    public bool ContainsKey(CompanionId key)
        => Value.ContainsKey(key.Id);

    /// <inheritdoc cref="NameDictionary.TryGetValue"/>
    public bool TryGetValue(CompanionId key, [NotNullWhen(true)] out string? value)
        => Value.TryGetValue(key.Id, out value);

    /// <inheritdoc cref="NameDictionary.this"/>
    public string this[CompanionId key]
        => Value[key.Id];
}
