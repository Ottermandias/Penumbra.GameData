using System.Collections.Frozen;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using OtterGui.Log;
using Penumbra.GameData.DataContainers.Bases;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace Penumbra.GameData.DataContainers;

/// <summary> A dictionary that matches action keys to their identities. </summary>
public sealed class DictAction(DalamudPluginInterface pluginInterface, Logger log, IDataManager data)
    : DictLuminaName<Action>(pluginInterface, log, "Actions", data.Language, 8, () => CreateActionList(data))
{
    /// <remarks>This is too much effort to do accurately.</remarks>>
    protected override int TypeSize
        => 128;

    /// <summary> Create the list. </summary>
    private static IReadOnlyDictionary<string, IReadOnlyList<Action>> CreateActionList(IDataManager gameData)
    {
        var sheet   = gameData.GetExcelSheet<Action>(gameData.Language)!;
        var storage = new ConcurrentDictionary<string, ConcurrentBag<Action>>();

        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount,
        };

        // Iterate through all actions and add start, end and hit keys.
        Parallel.ForEach(sheet.Where(a => !a.Name.RawData.IsEmpty), options, action =>
        {
            var startKey = action.AnimationStart?.Value?.Name?.Value?.Key.ToDalamudString().ToString();
            var endKey   = action.AnimationEnd?.Value?.Key.ToDalamudString().ToString();
            var hitKey   = action.ActionTimelineHit?.Value?.Key.ToDalamudString().ToString();
            AddAction(startKey, action);
            AddAction(endKey,   action);
            AddAction(hitKey,   action);
        });

        return storage.ToFrozenDictionary(kvp => kvp.Key, kvp => (IReadOnlyList<Action>)kvp.Value.Distinct().ToArray());

        void AddAction(string? key, Action action)
        {
            if (key.IsNullOrEmpty())
                return;

            key = key.ToLowerInvariant();
            if (storage.TryGetValue(key, out var actions))
                actions.Add(action);
            else
                storage[key] = [action];
        }
    }
}
