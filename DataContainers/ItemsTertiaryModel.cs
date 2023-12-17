using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Penumbra.GameData.DataContainers.Bases;
using Penumbra.GameData.Enums;

namespace Penumbra.GameData.DataContainers;

/// <summary> Currently only used by fist weapon gauntlets for fist weapons in range 1601-1650. </summary>
public sealed class ItemsTertiaryModel(
    DalamudPluginInterface pi,
    IPluginLog log,
    IDataManager gameData,
    ItemsByType items,
    ItemsSecondaryModel itemsSecondaries)
    : ItemDictionary(pi, log, "ItemDictTertiary", gameData.Language, 1, () => CreateGauntlets(items, itemsSecondaries),
        itemsSecondaries.Awaiter)
{
    private static IReadOnlyDictionary<uint, PseudoEquipItem> CreateGauntlets(ItemsByType items,
        ItemsSecondaryModel itemsSecondaries)
    {
        var gauntlets = items.Value[(int)FullEquipType.Hands]
            .Where(g => itemsSecondaries.Value
                .ContainsKey((uint)g.Item2))
            .ToDictionary(g => (uint)g.Item2, g => g);
        gauntlets.TrimExcess();
        return gauntlets;
    }
}
