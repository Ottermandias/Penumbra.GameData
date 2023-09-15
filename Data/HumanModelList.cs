using Dalamud;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using Lumina.Excel.GeneratedSheets;
using Penumbra.GameData.Structs;

namespace Penumbra.GameData.Data;

public sealed class HumanModelList : DataSharer
{
    public const string Tag            = "HumanModels";
    public const int    CurrentVersion = 2;

    private readonly BitArray _humanModels;

    public HumanModelList(DalamudPluginInterface pluginInterface, IDataManager gameData)
        : base(pluginInterface, ClientLanguage.English, CurrentVersion)
    {
        _humanModels = TryCatchData(Tag, () => GetValidHumanModels(gameData));
    }

    public bool IsHuman(ModelCharaId modelId)
        => modelId.Id < _humanModels.Count && _humanModels[(int)modelId.Id];

    public int Count
        => _humanModels.Count;

    protected override void DisposeInternal()
    {
        DisposeTag(Tag);
    }

    /// <summary>
    /// Go through all ModelChara rows and return a bitfield of those that resolve to human models.
    /// </summary>
    private static BitArray GetValidHumanModels(IDataManager gameData)
    {
        var sheet = gameData.GetExcelSheet<ModelChara>()!;
        var ret   = new BitArray((int)sheet.RowCount, false);
        foreach (var (_, idx) in sheet.Select((m, i) => (m, i)).Where(p => p.m.Type == (byte)CharacterBase.ModelType.Human))
            ret[idx] = true;

        return ret;
    }
}
