using System.Collections.Frozen;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OtterGui;
using OtterGui.Log;
using Penumbra.GameData.DataContainers.Bases;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace Penumbra.GameData.DataContainers;

public static class CachedDataService
{
    private static unsafe string GameVersion
    {
        get
        {
            var framework = Framework.Instance();
            return framework == null ? string.Empty : framework->GameVersion[0];
        }
    }

    public static T Create<T>(DalamudPluginInterface pi, Logger log, string fileName, int version, Func<T> factory, Func<JToken?, T?> parse,
        Func<T, JObject> write) where T : class
    {
        var path        = Path.Combine(pi.GetPluginConfigDirectory(), "DataCache", fileName + ".json");
        var gameVersion = GameVersion;

        if (!File.Exists(path))
        {
            log.Debug($"Cached game data {fileName} did not exist, creating new with Version{version}.");
            return CreateNew();
        }

        try
        {
            var text            = File.ReadAllText(path);
            var obj             = JObject.Parse(text);
            var readName        = obj["Name"]?.ToObject<string>();
            var readVersion     = obj["Version"]?.ToObject<int>() ?? 0;
            var readGameVersion = obj["GameVersion"]?.ToObject<string>() ?? string.Empty;
            if (readName != fileName || readVersion == 0 || readGameVersion.Length == 0)
            {
                log.Warning($"Cached game data {fileName} is corrupt, creating new with Version {version}.");
                return CreateNew();
            }

            if (readVersion != version)
            {
                log.Debug($"Cached game data {fileName} has version {readVersion}, creating new with Version {version}.");
                return CreateNew();
            }

            if (readGameVersion != gameVersion)
            {
                log.Debug(
                    $"Cached game data {fileName} was constructed for game version {readGameVersion}, but this game version {GameVersion}, creating new with Version {version}.");
                return CreateNew();
            }

            var data = parse(obj["Data"]);
            if (data != null)
                return data;

            log.Warning($"Cached game data {fileName} is corrupt, creating new with Version {version}.");
            return CreateNew();
        }
        catch (Exception ex)
        {
            log.Warning($"Cached game data {fileName} could not be read, creating new with Version {version}:\n{ex}");
            return CreateNew();
        }


        T CreateNew()
        {
            var ret = factory();
            try
            {
                var jObj = new JObject()
                {
                    ["Name"]        = fileName,
                    ["GameVersion"] = gameVersion,
                    ["Version"]     = version,
                    ["Data"]        = write(ret),
                };
                using var stream = File.OpenWrite(path);
                using var writer = new StreamWriter(stream);
                using var j      = new JsonTextWriter(writer);
                j.Formatting = Formatting.Indented;
                jObj.WriteTo(j);
            }
            catch (Exception ex)
            {
                log.Error($"Error creating cached game data {fileName} with version {version}:\n{ex}");
            }

            return ret;
        }
    }
}

/// <summary> A dictionary that matches action keys to their identities. </summary>
public sealed class DictAction(DalamudPluginInterface pluginInterface, Logger log, IDataManager data)
    : DictLuminaName<Action>(pluginInterface, log, "Actions", data.Language, 8,
        () => CachedDataService.Create(pluginInterface, log, "Actions", 8, () => CreateActionList(data), jTok => ReadActionList(data, jTok),
            WriteActionList))
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

    private static IReadOnlyDictionary<string, IReadOnlyList<Action>>? ReadActionList(IDataManager gameData, JToken? jObj)
    {
        if (jObj == null)
            return null;

        var sheet = gameData.GetExcelSheet<Action>(gameData.Language)!;
        try
        {
            var parsed = jObj.ToObject<Dictionary<string, uint[]>>();
            return parsed?.ToFrozenDictionary(kvp => kvp.Key,
                kvp => (IReadOnlyList<Action>)kvp.Value.Distinct().Select(v => sheet.GetRow(v)).OfType<Action>().ToArray());
        }
        catch
        {
            return null;
        }
    }

    private static JObject WriteActionList(IReadOnlyDictionary<string, IReadOnlyList<Action>> dict)
    {
        var ret = new JObject();
        foreach (var (key, array) in dict)
            ret[key] = new JArray(array.Select(a => a.RowId));
        return ret;
    }

    private static bool Validate(IReadOnlyDictionary<string, IReadOnlyList<Action>> dict)
        => dict.Count > 0;
}
