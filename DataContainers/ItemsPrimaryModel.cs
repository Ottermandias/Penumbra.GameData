using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Penumbra.GameData.DataContainers.Bases;
using Penumbra.GameData.Enums;

namespace Penumbra.GameData.DataContainers;

public sealed class ItemsPrimaryModel(DalamudPluginInterface pi, IPluginLog log, IDataManager gameData, ItemsByType items)
    : ItemDictionary(pi, log, "ItemDictPrimary", gameData.Language, 1, () => CreateMainItems(items), items.Awaiter)
{
    private static IReadOnlyDictionary<uint, PseudoEquipItem> CreateMainItems(ItemsByType items)
    {
        var dict = new Dictionary<uint, PseudoEquipItem>(1024 * 4);
        foreach (var type in Enum.GetValues<FullEquipType>().Where(v => !FullEquipTypeExtensions.OffhandTypes.Contains(v)))
        {
            var list = items.Value[(int)type];
            foreach (var item in list)
                dict.TryAdd((uint)item.Item2, item);
        }

        dict.TrimExcess();
        return dict;
    }
}
