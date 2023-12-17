using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using Penumbra.GameData.DataContainers.Bases;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace Penumbra.GameData.DataContainers;

public sealed class DictActions(DalamudPluginInterface pluginInterface, IPluginLog log, IDataManager data)
    : DictLuminaName<Action>(pluginInterface, log, "Actions", data.Language, 7, () => CreateActionList(data))
{
    /// <summary> This is too much effort to do accurately. </summary>
    protected override int TypeSize
        => 256;

    private static IReadOnlyDictionary<string, IReadOnlyList<Action>> CreateActionList(IDataManager gameData)
    {
        var sheet   = gameData.GetExcelSheet<Action>(gameData.Language)!;
        var storage = new ConcurrentDictionary<string, ConcurrentBag<Action>>();

        void AddAction(string? key, Action action)
        {
            if (key.IsNullOrEmpty())
                return;

            key = key.ToLowerInvariant();
            if (storage.TryGetValue(key, out var actions))
                actions.Add(action);
            else
                storage[key] = new ConcurrentBag<Action> { action };
        }

        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount,
        };

        Parallel.ForEach(sheet.Where(a => !a.Name.RawData.IsEmpty), options, action =>
        {
            var startKey = action.AnimationStart?.Value?.Name?.Value?.Key.ToDalamudString().ToString();
            var endKey   = action.AnimationEnd?.Value?.Key.ToDalamudString().ToString();
            var hitKey   = action.ActionTimelineHit?.Value?.Key.ToDalamudString().ToString();
            AddAction(startKey, action);
            AddAction(endKey,   action);
            AddAction(hitKey,   action);
        });

        return storage.ToDictionary(kvp => kvp.Key, kvp => (IReadOnlyList<Action>)kvp.Value.Distinct().ToArray());
    }
}
