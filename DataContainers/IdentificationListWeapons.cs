using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using OtterGui.Log;
using Penumbra.GameData.DataContainers.Bases;
using Penumbra.GameData.Enums;
using Penumbra.GameData.Structs;

namespace Penumbra.GameData.DataContainers;

public sealed class IdentificationListWeapons(DalamudPluginInterface pi, Logger log, IDataManager gameData, ItemsByType data)
    : KeyList<PseudoEquipItem>(pi, log, "WeaponIdentification", gameData.Language, 7, () => CreateWeaponList(data), ToKey, ValidKey, ValueKeySelector,
        data.Awaiter)
{
    public IEnumerable<EquipItem> Between(SetId modelId)
        => Between(ToKey(modelId, 0, 0), ToKey(modelId, 0xFFFF, 0xFF)).Select(e => (EquipItem)e);

    public IEnumerable<EquipItem> Between(SetId modelId, WeaponType type, Variant variant = default)
    {
        if (type == 0)
            return Between(ToKey(modelId, 0, 0), ToKey(modelId, 0xFFFF, 0xFF)).Select(e => (EquipItem)e);
        if (variant == 0)
            return Between(ToKey(modelId, type, 0), ToKey(modelId, type, 0xFF)).Select(e => (EquipItem)e);

        return Between(ToKey(modelId, type, variant), ToKey(modelId, type, variant)).Select(e => (EquipItem)e);
    }

    public static ulong ToKey(SetId modelId, WeaponType type, Variant variant)
        => (ulong)modelId.Id << 32 | (ulong)type.Id << 16 | variant.Id;

    public static ulong ToKey(EquipItem i)
        => ToKey(i.ModelId, i.WeaponType, i.Variant);

    private static ulong ToKey(PseudoEquipItem data)
        => ToKey((EquipItem)data);

    private static bool ValidKey(ulong key)
        => key != 0;

    private static int ValueKeySelector(PseudoEquipItem data)
        => (int)data.Item2;

    private static IEnumerable<PseudoEquipItem> CreateWeaponList(ItemsByType data)
        => data.Where(kvp => !kvp.Key.IsEquipment() && !kvp.Key.IsAccessory())
            .SelectMany(kvp => kvp.Value)
            .Select(i => (PseudoEquipItem)i);

    public override long ComputeMemory()
        => 24 + Value.Count * 40;
}
