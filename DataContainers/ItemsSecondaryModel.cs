using System.Collections.Frozen;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using OtterGui.Log;
using Penumbra.GameData.DataContainers.Bases;
using Penumbra.GameData.Enums;

namespace Penumbra.GameData.DataContainers;

/// <summary> A dictionary mapping ItemIds to all secondary model items (offhands). This requires ItemsByType to be finished. </summary>
public sealed class ItemsSecondaryModel(DalamudPluginInterface pi, Logger log, IDataManager gameData, ItemsByType items)
    : ItemDictionary(pi, log, "ItemDictSecondary", gameData.Language, 2, () => CreateOffhands(items), items.Awaiter)
{
    /// <summary> Create data by taking only the secondary models for all items. </summary>
    private static IReadOnlyDictionary<uint, PseudoEquipItem> CreateOffhands(ItemsByType items)
    {
        var dict = new Dictionary<uint, PseudoEquipItem>(1024 * 4);
        foreach (var type in FullEquipTypeExtensions.OffhandTypes)
        {
            var list = items.Value[(int)type];
            foreach (var item in list)
                dict.TryAdd((uint)item.Item2, item);
        }

        return dict.ToFrozenDictionary();
    }
}
