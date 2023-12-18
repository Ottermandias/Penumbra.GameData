using Dalamud;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using Lumina.Excel.GeneratedSheets;
using OtterGui.Log;
using Penumbra.GameData.DataContainers.Bases;
using Penumbra.GameData.Structs;

namespace Penumbra.GameData.DataContainers;

public sealed class IdentificationListModels(DalamudPluginInterface pi, IDataManager gameData, Logger log)
    : KeyList<ModelChara>(pi, log, "ModelIdentification", gameData.Language, 7, () => CreateModelList(gameData), ToKey, ValidKey, ValueKeySelector)
{
    public IEnumerable<ModelChara> Between(CharacterBase.ModelType type, SetId modelId, byte modelBase = 0, Variant variant = default)
    {
        if (modelBase == 0)
            return Between(ToKey(type, modelId, 0, 0), ToKey(type, modelId, 0xFF, 0xFF));
        if (variant == 0)
            return Between(ToKey(type, modelId, modelBase, 0), ToKey(type, modelId, modelBase, 0xFF));

        return Between(ToKey(type, modelId, modelBase, variant), ToKey(type, modelId, modelBase, variant));
    }

    public static ulong ToKey(CharacterBase.ModelType type, SetId model, byte modelBase, Variant variant)
        => (ulong)type << 32 | (ulong)model.Id << 16 | (ulong)modelBase << 8 | variant.Id;

    private static ulong ToKey(ModelChara row)
        => ToKey((CharacterBase.ModelType)row.Type, row.Model, row.Base, row.Variant);

    private static bool ValidKey(ulong key)
        => key != 0;

    private static int ValueKeySelector(ModelChara data)
        => (int)data.RowId;

    private static IEnumerable<ModelChara> CreateModelList(IDataManager gameData)
        => gameData.GetExcelSheet<ModelChara>(gameData.Language)!;

    public override long ComputeMemory()
        => 24 + Value.Count * 40;
}
