using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Penumbra.GameData.Data;
using Penumbra.GameData.DataContainers.Bases;
using Penumbra.GameData.Structs;

namespace Penumbra.GameData.DataContainers;

public sealed class DictStains(DalamudPluginInterface pluginInterface, IPluginLog log, IDataManager gameData)
    : DataSharer<IReadOnlyDictionary<byte, (string Name, uint Dye, bool Gloss)>>(pluginInterface, log, "Stains", gameData.Language, 3,
        () => CreateStainData(gameData)), IReadOnlyDictionary<StainId, Stain>
{
    private static IReadOnlyDictionary<byte, (string Name, uint Dye, bool Gloss)> CreateStainData(IDataManager dataManager)
    {
        var stainSheet = dataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Stain>(dataManager.Language)!;
        return stainSheet.Where(s => s.Color != 0 && s.Name.RawData.Length > 0)
            .ToDictionary(s => (byte)s.RowId, s =>
            {
                var stain = new Stain(s);
                return (stain.Name, stain.RgbaColor, stain.Gloss);
            });
    }

    public IEnumerator<KeyValuePair<StainId, Stain>> GetEnumerator()
        => Value.Select(kvp
                => new KeyValuePair<StainId, Stain>(new StainId(kvp.Key), new Stain(kvp.Value.Name, kvp.Value.Dye, kvp.Key, kvp.Value.Gloss)))
            .GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public int Count
        => Value.Count;

    public bool ContainsKey(StainId key)
        => Value.ContainsKey(key.Id);

    public bool TryGetValue(StainId key, out Stain value)
    {
        if (!Value.TryGetValue(key.Id, out var data))
        {
            value = default;
            return false;
        }

        value = new Stain(data.Name, data.Dye, key.Id, data.Gloss);
        return true;
    }

    public Stain this[StainId key]
        => TryGetValue(key, out var data) ? data : throw new ArgumentOutOfRangeException(nameof(key));

    public IEnumerable<StainId> Keys
        => Value.Keys.Select(k => new StainId(k));

    public IEnumerable<Stain> Values
        => Value.Select(kvp => new Stain(kvp.Value.Name, kvp.Value.Dye, kvp.Key, kvp.Value.Gloss));

    public override long ComputeMemory()
        => DataUtility.DictionaryMemory(24, Count);

    public override int ComputeTotalCount()
        => Count;
}
