using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using Lumina.Excel.GeneratedSheets;
using Penumbra.GameData.DataContainers.Bases;
using Penumbra.GameData.Structs;

namespace Penumbra.GameData.DataContainers;

public sealed class HumanModelList(DalamudPluginInterface pluginInterface, IPluginLog log, IDataManager gameData)
    : DataSharer<Tuple<BitArray, int>>(pluginInterface, log, "HumanModels", gameData.Language, 3, () => GetValidHumanModels(gameData))
{
    public bool IsHuman(ModelCharaId modelId)
        => modelId.Id < Count && Value.Item1[(int)modelId.Id];

    public int Count
        => Value.Item1.Count;

    public int HumanCount
        => Value.Item2;

    /// <summary>
    /// Go through all ModelChara rows and return a bitfield of those that resolve to human models.
    /// </summary>
    private static Tuple<BitArray, int> GetValidHumanModels(IDataManager gameData)
    {
        var sheet = gameData.GetExcelSheet<ModelChara>()!;
        var ret = new BitArray((int)sheet.RowCount, false);
        var count = 0;
        foreach (var (_, idx) in sheet.Select((m, i) => (m, i)).Where(p => p.m.Type == (byte)CharacterBase.ModelType.Human))
        {
            ret[idx] = true;
            ++count;
        }

        return new Tuple<BitArray, int>(ret, count);
    }

    public override long ComputeMemory()
        => Value.Item1.Length / 8 + 16;

    public override int ComputeTotalCount()
        => Count;
}
