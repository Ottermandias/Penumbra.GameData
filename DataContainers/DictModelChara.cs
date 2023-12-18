using Dalamud;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Lumina.Excel.GeneratedSheets;
using OtterGui.Log;
using Penumbra.GameData.Data;
using Penumbra.GameData.DataContainers.Bases;
using Penumbra.GameData.Structs;

namespace Penumbra.GameData.DataContainers;

public sealed class DictModelChara(
    DalamudPluginInterface pluginInterface,
    Logger log,
    IDataManager gameData,
    DictBNpcNames bNpcNames,
    NameDicts nameDicts)
    : DataSharer<IReadOnlyList<IReadOnlyList<(string Name, ObjectKind Kind, uint Id)>>>(pluginInterface, log, "ModelObjectDict",
            ClientLanguage.English, 1,
            () => CreateModelObjects(gameData, bNpcNames, nameDicts),
            System.Threading.Tasks.Task.WhenAll(nameDicts.Awaiter, bNpcNames.Awaiter)),
        IReadOnlyDictionary<ModelCharaId, IReadOnlyList<(string Name, ObjectKind Kind, uint Id)>>
{
    public override long ComputeMemory()
        => 16 * (Count + 1) + TotalCount * 4;

    public override int ComputeTotalCount()
        => Value.Sum(l => l.Count);

    public IEnumerator<KeyValuePair<ModelCharaId, IReadOnlyList<(string Name, ObjectKind Kind, uint Id)>>> GetEnumerator()
        => Value.Select((list, idx) => new KeyValuePair<ModelCharaId, IReadOnlyList<(string Name, ObjectKind Kind, uint Id)>>((uint)idx, list))
            .GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public int Count
        => Value.Count;

    public bool ContainsKey(ModelCharaId key)
        => key.Id < Count;

    public bool TryGetValue(ModelCharaId key, out IReadOnlyList<(string Name, ObjectKind Kind, uint Id)> value)
    {
        if (key.Id >= Count)
        {
            value = Array.Empty<(string Name, ObjectKind Kind, uint Id)>();
            return false;
        }

        value = Value[(int)key.Id];
        return true;
    }

    public IReadOnlyList<(string Name, ObjectKind Kind, uint Id)> this[ModelCharaId key]
        => TryGetValue(key, out var value) ? value : Array.Empty<(string Name, ObjectKind Kind, uint Id)>();

    public IEnumerable<ModelCharaId> Keys
        => Enumerable.Range(0, Value.Count).Select(v => new ModelCharaId((uint)v));

    public IEnumerable<IReadOnlyList<(string Name, ObjectKind Kind, uint Id)>> Values
        => Value;

    private static IReadOnlyList<IReadOnlyList<(string Name, ObjectKind Kind, uint Id)>> CreateModelObjects(IDataManager gameData, DictBNpcNames bNpcNames, NameDicts nameDicts)
    {
        var modelSheet = gameData.GetExcelSheet<ModelChara>(gameData.Language)!;
        var bag        = new ConcurrentBag<(int ModelID, string Name, ObjectKind Kind, uint Id)>();
        var ret        = Enumerable.Repeat((IReadOnlyList<(string Name, ObjectKind Kind, uint Id)>)Array.Empty<(string Name, ObjectKind Kind, uint Id)>(), (int) modelSheet.RowCount).ToArray();

        var oTask = System.Threading.Tasks.Task.Run(() =>
        {
            foreach (var ornament in gameData.GetExcelSheet<Ornament>(gameData.Language)!)
                AddChara(ornament.Model, ObjectKind.Ornament, ornament.RowId, ornament.RowId);
        });

        var mTask = System.Threading.Tasks.Task.Run(() =>
        {
            foreach (var mount in gameData.GetExcelSheet<Mount>(gameData.Language)!)
                AddChara((int)mount.ModelChara.Row, ObjectKind.MountType, mount.RowId, mount.RowId);
        });

        var cTask = System.Threading.Tasks.Task.Run(() =>
        {
            foreach (var companion in gameData.GetExcelSheet<Companion>(gameData.Language)!)
                AddChara((int)companion.Model.Row, ObjectKind.Companion, companion.RowId, companion.RowId);
        });

        var eTask = System.Threading.Tasks.Task.Run(() =>
        {
            foreach (var eNpc in gameData.GetExcelSheet<ENpcBase>(gameData.Language)!)
                AddChara((int)eNpc.ModelChara.Row, ObjectKind.EventNpc, eNpc.RowId, eNpc.RowId);
        });

        var options = new ParallelOptions()
        {
            MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount / 2),
        };

        Parallel.ForEach(gameData.GetExcelSheet<BNpcBase>(gameData.Language)!.Where(b => b.RowId < bNpcNames.Count), options, bNpc =>
        {
            foreach (var name in bNpcNames[bNpc.RowId])
                AddChara((int)bNpc.ModelChara.Row, ObjectKind.BattleNpc, name.Id, bNpc.RowId);
        });

        System.Threading.Tasks.Task.WaitAll(oTask, mTask, cTask, eTask);

        foreach (var group in bag.GroupBy(t => t.ModelID))
            ret[group.Key] = group.Select(t => (t.Name, t.Kind, t.Id)).ToArray();

        return ret;

        void AddChara(int modelChara, ObjectKind kind, uint dataId, uint displayId)
        {
            if (modelChara >= modelSheet.RowCount)
                return;

            if (nameDicts.TryGetName(kind, dataId, out var name))
                bag.Add((modelChara, name, kind, displayId));
        }
    }
}
