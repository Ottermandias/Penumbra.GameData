using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using Lumina.Excel.GeneratedSheets;
using OtterGui.Log;
using Penumbra.GameData.Data;
using Penumbra.GameData.DataContainers.Bases;
using Penumbra.GameData.Files;

namespace Penumbra.GameData.DataContainers;

public sealed class DictEmotes(DalamudPluginInterface pluginInterface, Logger log, IDataManager data)
    : DictLuminaName<Emote>(pluginInterface, log, "Emotes", data.Language, 7, () => CreateEmoteList(log, data))
{
    private static IReadOnlyDictionary<string, IReadOnlyList<Emote>> CreateEmoteList(Logger log, IDataManager gameData)
    {
        var sheet   = gameData.GetExcelSheet<Emote>(gameData.Language)!;
        var storage = new ConcurrentDictionary<string, ConcurrentBag<Emote>>();

        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount,
        };
        var seenTmbs = new ConcurrentDictionary<string, TmbFile>();

        Parallel.ForEach(sheet.Where(n => n.Name.RawData.Length > 0), options, ProcessEmote);

        var sit = sheet.GetRow(50)!;
        AddEmote("s_pose01_loop.pap", sit);
        AddEmote("s_pose02_loop.pap", sit);
        AddEmote("s_pose03_loop.pap", sit);
        AddEmote("s_pose04_loop.pap", sit);
        AddEmote("s_pose05_loop.pap", sit);

        var sitOnGround = sheet.GetRow(52)!;
        AddEmote("j_pose01_loop.pap", sitOnGround);
        AddEmote("j_pose02_loop.pap", sitOnGround);
        AddEmote("j_pose03_loop.pap", sitOnGround);
        AddEmote("j_pose04_loop.pap", sitOnGround);

        var doze = sheet.GetRow(13)!;
        AddEmote("l_pose01_loop.pap", doze);
        AddEmote("l_pose02_loop.pap", doze);
        AddEmote("l_pose03_loop.pap", doze);

        return storage.ToDictionary(kvp => kvp.Key, kvp => (IReadOnlyList<Emote>)kvp.Value.Distinct().ToArray());

        void ProcessEmote(Emote emote)
        {
            var emoteTmbs = new HashSet<string>(8);
            var tmbs      = new Queue<string>(8);

            foreach (var timeline in emote.ActionTimeline.Where(t => t.Row != 0 && t.Value != null).Select(t => t.Value!))
            {
                var key = timeline.Key.ToDalamudString().TextValue;
                tmbs.Enqueue(GamePaths.Vfx.ActionTmb(key));
                AddEmote(Path.GetFileName(key) + ".pap", emote);
            }

            while (tmbs.TryDequeue(out var tmbPath))
            {
                if (!emoteTmbs.Add(tmbPath))
                    continue;

                AddEmote(Path.GetFileName(tmbPath), emote);

                try
                {
                    var file = gameData.GetFile(tmbPath);
                    if (file != null)
                    {
                        if (!seenTmbs.TryGetValue(tmbPath, out var tmb))
                        {
                            tmb = new TmbFile(file.DataSpan);
                            seenTmbs.TryAdd(tmbPath, tmb);
                        }

                        foreach (var subfile in tmb.Paths)
                        {
                            AddEmote(Path.GetFileName(subfile), emote);
                            if (Path.GetExtension(subfile) == ".tmb")
                                tmbs.Enqueue($"chara/action/{subfile}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Warning($"Unknown Error while creating data:\n{ex}");
                }
            }
        }

        void AddEmote(string? key, Emote emote)
        {
            if (key.IsNullOrEmpty())
                return;

            key = key.ToLowerInvariant();
            if (storage.TryGetValue(key, out var emotes))
                emotes.Add(emote);
            else
                storage[key] = [emote];
        }
    }

    /// <summary> This is too much effort to do accurately. </summary>
    protected override int TypeSize
        => 128;
}
