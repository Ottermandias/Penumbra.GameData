using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using OtterGui.Log;
using Penumbra.GameData.DataContainers.Bases;
using Penumbra.GameData.Enums;

namespace Penumbra.GameData.DataContainers;

public sealed class ItemsSecondaryModel(DalamudPluginInterface pi, Logger log, IDataManager gameData, ItemsByType items)
    : ItemDictionary(pi, log, "ItemDictSecondary", gameData.Language, 1, () => CreateOffhands(items), items.Awaiter)
{
    private static IReadOnlyDictionary<uint, PseudoEquipItem> CreateOffhands(ItemsByType items)
    {
        var dict = new Dictionary<uint, PseudoEquipItem>(1024);
        foreach (var type in FullEquipTypeExtensions.OffhandTypes)
        {
            var list = items.Value[(int)type];
            foreach (var item in list)
                dict.TryAdd((uint)item.Item2, item);
        }

        dict.TrimExcess();
        return dict;
    }
}
