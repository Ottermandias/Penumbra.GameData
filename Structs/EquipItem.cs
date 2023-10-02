using Dalamud.Utility;
using Lumina.Excel.GeneratedSheets;
using Penumbra.GameData.Data;
using Penumbra.GameData.Enums;
using PseudoEquipItem = System.ValueTuple<string, ulong, ushort, ushort, ushort, byte, uint>;

namespace Penumbra.GameData.Structs;

[Flags]
public enum ItemFlags : byte
{
    IsDyable      = 0x01,
    IsTradable    = 0x02,
    IsCrestWorthy = 0x04,
}

[StructLayout(LayoutKind.Sequential)]
public readonly struct EquipItem
{
    public readonly string         Name;
    public readonly CustomItemId   Id;
    public readonly IconId         IconId;
    public readonly SetId          ModelId;
    public readonly WeaponType     WeaponType;
    public readonly Variant        Variant;
    public readonly FullEquipType  Type;
    public readonly ItemFlags      Flags;
    public readonly CharacterLevel Level;
    public readonly JobGroupId     JobRestrictions;

    public ItemId ItemId
        => Id.Item;

    public bool Valid
        => Type != FullEquipType.Unknown;

    public CharacterArmor Armor()
        => new(ModelId, Variant, 0);

    public CharacterArmor Armor(StainId stain)
        => new(ModelId, Variant, stain);

    public CharacterWeapon Weapon()
        => new(ModelId, WeaponType, Variant, 0);

    public CharacterWeapon Weapon(StainId stain)
        => new(ModelId, WeaponType, Variant, stain);

    public EquipItem()
        => Name = string.Empty;

    public EquipItem(string name, CustomItemId id, IconId iconId, SetId modelId, WeaponType weaponType, Variant variant, FullEquipType type,
        ItemFlags flags, CharacterLevel level, JobGroupId restrictions)
    {
        Name            = string.Intern(name);
        Id              = id;
        IconId          = iconId;
        ModelId         = modelId;
        WeaponType      = weaponType;
        Variant         = variant;
        Type            = type;
        Flags           = flags;
        Level           = level;
        JobRestrictions = restrictions;
    }

    public string ModelString
        => WeaponType == 0 ? $"{ModelId}-{Variant}" : $"{ModelId}-{WeaponType}-{Variant}";

    public static implicit operator EquipItem(PseudoEquipItem it)
    {
        var (type, flags, level, restrictions) = SplitInt(it.Item7);
        return new EquipItem(it.Item1, it.Item2, it.Item3, it.Item4, it.Item5, it.Item6, type, flags, level, restrictions);
    }

    public static explicit operator PseudoEquipItem(EquipItem it)
        => (it.Name, it.Id.Id, it.IconId.Id, it.ModelId.Id, it.WeaponType.Id, it.Variant.Id,
            PackBytes(it.Type, it.Flags, it.Level, it.JobRestrictions));

    public static EquipItem FromArmor(Item item)
    {
        var type            = item.ToEquipType();
        var name            = item.Name.ToDalamudString().TextValue;
        var id              = item.RowId;
        var icon            = item.Icon;
        var model           = (SetId)item.ModelMain;
        var weapon          = (WeaponType)0;
        var variant         = (Variant)(item.ModelMain >> 16);
        var flags           = GetFlags(item);
        var level           = item.LevelEquip;
        var jobRestrictions = (byte)item.ClassJobCategory.Row;
        return new EquipItem(name, id, icon, model, weapon, variant, type, flags, level, jobRestrictions);
    }

    private static ItemFlags GetFlags(Item item)
        => (item.IsDyeable ? ItemFlags.IsDyable : 0)
          | (item.IsCrestWorthy ? ItemFlags.IsCrestWorthy : 0)
          | (item.IsUntradable ? 0 : ItemFlags.IsTradable);

    public static EquipItem FromMainhand(Item item)
    {
        var type            = item.ToEquipType();
        var name            = item.Name.ToDalamudString().TextValue;
        var id              = item.RowId;
        var icon            = item.Icon;
        var model           = (SetId)item.ModelMain;
        var weapon          = (WeaponType)(item.ModelMain >> 16);
        var variant         = (Variant)(item.ModelMain >> 32);
        var flags           = GetFlags(item);
        var level           = item.LevelEquip;
        var jobRestrictions = (byte)item.ClassJobCategory.Row;
        return new EquipItem(name, id, icon, model, weapon, variant, type, flags, level, jobRestrictions);
    }

    public static EquipItem FromOffhand(Item item)
    {
        var type            = item.ToEquipType().ValidOffhand();
        var name            = item.Name.ToDalamudString().TextValue + type.OffhandTypeSuffix();
        var id              = item.RowId;
        var icon            = item.Icon;
        var model           = (SetId)item.ModelSub;
        var weapon          = (WeaponType)(item.ModelSub >> 16);
        var variant         = (Variant)(item.ModelSub >> 32);
        var flags           = GetFlags(item);
        var level           = item.LevelEquip;
        var jobRestrictions = (byte)item.ClassJobCategory.Row;
        return new EquipItem(name, id, icon, model, weapon, variant, type, flags, level, jobRestrictions);
    }

    public static EquipItem FromIds(ItemId itemId, IconId iconId, SetId modelId, WeaponType type, Variant variant,
        FullEquipType equipType = FullEquipType.Unknown, ItemFlags flags = 0, string? name = null)
        => FromIds(itemId, iconId, modelId, type, variant, 0, 0, equipType, flags, name);

    public static EquipItem FromIds(ItemId itemId, IconId iconId, SetId modelId, WeaponType type, Variant variant,
        CharacterLevel level, JobGroupId restrictions, FullEquipType equipType = FullEquipType.Unknown, ItemFlags flags = 0,
        string? name = null)
    {
        name ??= $"Unknown ({modelId}-{(type.Id != 0 ? $"{type}-" : string.Empty)}{variant})";
        if (equipType is FullEquipType.Unknown && type.Id != 0)
            equipType = ItemData.ConvertWeaponId(modelId);
        var fullId = itemId.Id is 0
            ? new CustomItemId(modelId, type, variant, equipType)
            : (CustomItemId)itemId;
        return new EquipItem(name, fullId, iconId, modelId, type, variant, equipType, flags, level, restrictions);
    }


    public static EquipItem FromId(CustomItemId id)
    {
        var (setId, weaponType, variant, equipType) = id.Split;
        if (equipType == FullEquipType.Unknown && weaponType.Id != 0)
            equipType = ItemData.ConvertWeaponId(setId);

        return new EquipItem($"Unknown ({setId}-{(weaponType.Id != 0 ? $"{weaponType}-" : string.Empty)}{variant})", id, 0, setId, weaponType,
            variant, equipType, 0, 0, 0);
    }

    public override string ToString()
        => Name;

    private static uint PackBytes(FullEquipType type, ItemFlags flags, CharacterLevel level, JobGroupId restrictions)
        => (uint)type | ((uint)flags << 8) | ((uint)level.Value << 16) | ((uint)restrictions.Id << 24);

    private static (FullEquipType, ItemFlags, CharacterLevel, JobGroupId) SplitInt(uint data)
        => ((FullEquipType)(data & 0xFF), (ItemFlags)((data >> 8) & 0xFF), (CharacterLevel)((data >> 16) & 0xFF), (JobGroupId)(data >> 24));
}
