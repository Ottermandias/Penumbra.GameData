using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Penumbra.GameData.DataContainers.Bases;
using Penumbra.GameData.Enums;
using Penumbra.GameData.Structs;

namespace Penumbra.GameData.DataContainers;

public sealed class IdentificationListEquipment(DalamudPluginInterface pi, IPluginLog log, IDataManager gameData, ItemsByType items)
    : KeyList<PseudoEquipItem>(pi, log, "EquipmentIdentification", gameData.Language, 7, () => CreateEquipmentList(items), ToKey, ValidKey,
        ValueKeySelector, items.Awaiter)
{
    public IEnumerable<EquipItem> Between(SetId modelId, EquipSlot slot = EquipSlot.Unknown, Variant variant = default)
    {
        if (slot == EquipSlot.Unknown)
            return Between(ToKey(modelId, 0, 0), ToKey(modelId, (EquipSlot)0xFF, 0xFF)).Select(e => (EquipItem)e);
        if (variant == 0)
            return Between(ToKey(modelId, slot, 0), ToKey(modelId, slot, 0xFF)).Select(e => (EquipItem)e);

        return Between(ToKey(modelId, slot, variant), ToKey(modelId, slot, variant)).Select(e => (EquipItem)e);
    }

    public static ulong ToKey(SetId modelId, EquipSlot slot, Variant variant)
        => ((ulong)modelId.Id << 32) | ((ulong)slot << 16) | variant.Id;

    public static ulong ToKey(EquipItem i)
        => ToKey(i.ModelId, i.Type.ToSlot(), i.Variant);

    private static ulong ToKey(PseudoEquipItem i)
        => ToKey((EquipItem)i);

    private static bool ValidKey(ulong key)
        => key != 0;

    private static int ValueKeySelector(PseudoEquipItem data)
        => (int)data.Item2;

    private static IEnumerable<PseudoEquipItem> CreateEquipmentList(ItemsByType items)
    {
        return items.Where(kvp => kvp.Key.IsEquipment() || kvp.Key.IsAccessory())
            .SelectMany(kvp => kvp.Value)
            .Select(i => (PseudoEquipItem)i)
            .Concat(CustomList);
    }

    private static IEnumerable<PseudoEquipItem> CustomList
        => new[]
        {
            // @formatter:off
            (PseudoEquipItem)EquipItem.FromIds(0, 0, (SetId)8100, (WeaponType)0, 01, FullEquipType.Body, name:"Reaper Shroud"),
            (PseudoEquipItem)EquipItem.FromIds(0, 0, (SetId)9041, (WeaponType)0, 01, FullEquipType.Head,  name:"Cid's Bandana (9041)"),
            (PseudoEquipItem)EquipItem.FromIds(0, 0, (SetId)9041, (WeaponType)0, 01, FullEquipType.Body,  name:"Cid's Body (9041)"),
            (PseudoEquipItem)EquipItem.FromIds(0, 0, (SetId)9903, (WeaponType)0, 01, FullEquipType.Head,  name:"Smallclothes (NPC, 9903)"),
            (PseudoEquipItem)EquipItem.FromIds(0, 0, (SetId)9903, (WeaponType)0, 01, FullEquipType.Body,  name:"Smallclothes (NPC, 9903)"),
            (PseudoEquipItem)EquipItem.FromIds(0, 0, (SetId)9903, (WeaponType)0, 01, FullEquipType.Hands, name:"Smallclothes (NPC, 9903)"),
            (PseudoEquipItem)EquipItem.FromIds(0, 0, (SetId)9903, (WeaponType)0, 01, FullEquipType.Legs,  name:"Smallclothes (NPC, 9903)"),
            (PseudoEquipItem)EquipItem.FromIds(0, 0, (SetId)9903, (WeaponType)0, 01, FullEquipType.Feet,  name:"Smallclothes (NPC, 9903)"),
            (PseudoEquipItem)EquipItem.FromIds(0, 0, (SetId)9212, (WeaponType)0, 12, FullEquipType.Body,  name:"Ancient Robes (Lahabrea)"),
            (PseudoEquipItem)EquipItem.FromIds(0, 0, (SetId)9212, (WeaponType)0, 01, FullEquipType.Legs,  name:"Ancient Legs"),
            (PseudoEquipItem)EquipItem.FromIds(0, 0, (SetId)9212, (WeaponType)0, 01, FullEquipType.Feet,  name:"Ancient Shoes"),
            (PseudoEquipItem)EquipItem.FromIds(0, 40672, (SetId)0199, (WeaponType)0, 01, FullEquipType.Head, name:"Veil of Eternal Innocence (Long)"),
            (PseudoEquipItem)EquipItem.FromIds(0, 40673, (SetId)0199, (WeaponType)0, 01, FullEquipType.Head, name:"Veil of Eternal Passion (Long)"),
            (PseudoEquipItem)EquipItem.FromIds(0, 40674, (SetId)0199, (WeaponType)0, 01, FullEquipType.Head, name:"Veil of Eternal Devotion (Long)"),
            // @formatter:on
        };

    public override long ComputeMemory()
        => 24 + Value.Count * 40;
}
