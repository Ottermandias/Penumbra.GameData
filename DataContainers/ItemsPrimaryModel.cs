using System.Collections.Frozen;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using OtterGui.Log;
using Penumbra.GameData.DataContainers.Bases;
using Penumbra.GameData.Enums;

namespace Penumbra.GameData.DataContainers;

/// <summary> A dictionary mapping ItemIds to all primary model items. This requires ItemsByType to be finished. </summary>
public sealed class ItemsPrimaryModel(DalamudPluginInterface pi, Logger log, IDataManager gameData, ItemsByType items)
    : ItemDictionary(pi, log, "ItemDictPrimary", gameData.Language, 3, () => CreateMainItems(items), items.Awaiter)
{
    /// <summary> Create data by taking only the primary models for all items. </summary>
    private static IReadOnlyDictionary<uint, PseudoEquipItem> CreateMainItems(ItemsByType items)
    {
        var dict = new Dictionary<uint, PseudoEquipItem>(1024 * 16);
        foreach (var type in Enum.GetValues<FullEquipType>().Where(v => !FullEquipTypeExtensions.OffhandTypes.Contains(v)))
        {
            var list = items.Value[(int)type];
            if (type is FullEquipType.Hands)
            {
                foreach (var item in list.Where(i => !i.Item1.EndsWith(" (Gauntlets)")))
                    dict.TryAdd((uint)item.Item2, item);
            }
            else
            {
                foreach (var item in list)
                    dict.TryAdd((uint)item.Item2, item);
            }
        }

        return dict.ToFrozenDictionary();
    }
}
