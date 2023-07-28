using System.Numerics;
using Penumbra.GameData.Actors;
using Penumbra.GameData.Enums;

namespace Penumbra.GameData.Structs;

public readonly record struct ModelCharaId(uint Id)
{
    public static implicit operator ModelCharaId(uint id)
        => new(id);

    public override string ToString()
        => Id.ToString();
}

public readonly record struct BnpcId(uint Id)
{
    public static implicit operator BnpcId(uint id)
        => new(id);

    public override string ToString()
        => Id.ToString();
}

public readonly record struct EnpcId(uint Id)
{
    public static implicit operator EnpcId(uint id)
        => new(id);

    public override string ToString()
        => Id.ToString();
}

public readonly record struct BnpcNameId(uint Id)
{
    public static implicit operator BnpcNameId(uint id)
        => new(id);

    public override string ToString()
        => Id.ToString();
}

public readonly record struct MountId(uint Id)
{
    public static implicit operator MountId(uint id)
        => new(id);

    public override string ToString()
        => Id.ToString();
}

public readonly record struct CompanionId(uint Id)
{
    public static implicit operator CompanionId(uint id)
        => new(id);

    public override string ToString()
        => Id.ToString();
}

public readonly record struct OrnamentId(uint Id)
{
    public static implicit operator OrnamentId(uint id)
        => new(id);

    public override string ToString()
        => Id.ToString();
}

public readonly record struct NpcId(uint Id)
{
    public static implicit operator NpcId(uint id)
        => new(id);

    public static implicit operator NpcId(EnpcId id)
        => new(id.Id);

    public static implicit operator NpcId(BnpcNameId id)
        => new(id.Id);

    public static implicit operator NpcId(MountId id)
        => new(id.Id);

    public static implicit operator NpcId(CompanionId id)
        => new(id.Id);

    public static implicit operator NpcId(OrnamentId id)
        => new(id.Id);

    public EnpcId EnpcId
        => new(Id);

    public BnpcNameId BnpcNameId
        => new(Id);

    public MountId MountId
        => new(Id);

    public CompanionId CompanionId
        => new(Id);

    public OrnamentId OrnamentId
        => new(Id);

    public override string ToString()
        => Id.ToString();
}

public readonly record struct WeaponType(ushort Id)
{
    public static implicit operator WeaponType(ushort id)
        => new(id);

    public override string ToString()
        => Id.ToString();
}

public readonly record struct SetId(ushort Id) : IComparisonOperators<SetId, SetId, bool>
{
    public static implicit operator SetId(ushort id)
        => new(id);

    public override string ToString()
        => Id.ToString();

    public static bool operator >(SetId left, SetId right)
        => left.Id > right.Id;

    public static bool operator >=(SetId left, SetId right)
        => left.Id >= right.Id;

    public static bool operator <(SetId left, SetId right)
        => left.Id < right.Id;

    public static bool operator <=(SetId left, SetId right)
        => left.Id <= right.Id;
}

public readonly record struct Variant(byte Id)
{
    public static implicit operator Variant(byte id)
        => new(id);

    public override string ToString()
        => Id.ToString();
}

public readonly record struct StainId(byte Id)
{
    public static implicit operator StainId(byte id)
        => new(id);

    public override string ToString()
        => Id.ToString();
}

public readonly record struct ItemId(uint Id) : IComparisonOperators<ItemId, ItemId, bool>
{
    public static implicit operator ItemId(uint id)
        => new(id);

    public ItemId StripModifiers
        => new(Id switch
        {
            > 1000000 => Id - 1000000,
            > 500000  => Id - 500000,
            _         => Id,
        });

    public override string ToString()
        => Id.ToString();

    public static bool operator >(ItemId left, ItemId right)
        => left.Id > right.Id;

    public static bool operator >=(ItemId left, ItemId right)
        => left.Id >= right.Id;

    public static bool operator <(ItemId left, ItemId right)
        => left.Id < right.Id;

    public static bool operator <=(ItemId left, ItemId right)
        => left.Id <= right.Id;
}

public readonly record struct CustomItemId(ulong Id) : IComparisonOperators<CustomItemId, CustomItemId, bool>
{
    public bool IsItem
        => Id < uint.MaxValue / 2;

    public ItemId Item
        => IsItem ? (ItemId)Id : 0;

    public (SetId Model, WeaponType WeaponType, Variant Variant, FullEquipType Type) Split
        => IsItem ? (0, 0, 0, FullEquipType.Unknown) : ((SetId)Id, (WeaponType)(Id >> 16), (Variant)(Id >> 32), (FullEquipType)(Id >> 40));

    public CustomItemId(SetId model, WeaponType weaponType, Variant variant, FullEquipType type)
        : this(model.Id | ((ulong)weaponType.Id << 16) | ((ulong)variant.Id << 32) | ((ulong)type << 40) | (1ul << 48))
    { }

    public static implicit operator CustomItemId(ItemId id)
        => new(id.Id);

    public static implicit operator CustomItemId(ulong id)
        => new(id);

    public override string ToString()
        => Id.ToString();

    public static bool operator >(CustomItemId left, CustomItemId right)
        => left.Id > right.Id;

    public static bool operator >=(CustomItemId left, CustomItemId right)
        => left.Id >= right.Id;

    public static bool operator <(CustomItemId left, CustomItemId right)
        => left.Id < right.Id;

    public static bool operator <=(CustomItemId left, CustomItemId right)
        => left.Id <= right.Id;
}

public readonly record struct IconId(ushort Id)
{
    public static implicit operator IconId(ushort id)
        => new(id);

    public override string ToString()
        => Id.ToString();
}

public readonly record struct WorldId(ushort Id)
{
    public static readonly WorldId AnyWorld = new(ushort.MaxValue);

    public static implicit operator WorldId(ushort id)
        => new(id);

    public override string ToString()
        => Id.ToString();
}

public readonly record struct ObjectIndex(ushort Index) : IComparisonOperators<ObjectIndex, ObjectIndex, bool>
{
    public static readonly ObjectIndex AnyIndex        = new(ushort.MaxValue);
    public static readonly ObjectIndex CutsceneStart   = new((ushort)ScreenActor.CutsceneStart);
    public static readonly ObjectIndex GPosePlayer     = new((ushort)ScreenActor.GPosePlayer);
    public static readonly ObjectIndex CharacterScreen = new((ushort)ScreenActor.CharacterScreen);
    public static readonly ObjectIndex ExamineScreen   = new((ushort)ScreenActor.ExamineScreen);
    public static readonly ObjectIndex FittingRoom     = new((ushort)ScreenActor.FittingRoom);
    public static readonly ObjectIndex DyePreview      = new((ushort)ScreenActor.DyePreview);
    public static readonly ObjectIndex Portrait        = new((ushort)ScreenActor.Portrait);
    public static readonly ObjectIndex Card6           = new((ushort)ScreenActor.Card6);
    public static readonly ObjectIndex Card7           = new((ushort)ScreenActor.Card7);
    public static readonly ObjectIndex Card8           = new((ushort)ScreenActor.Card8);

    public static implicit operator ObjectIndex(ushort index)
        => new(index);

    public override string ToString()
        => Index.ToString();

    public static bool operator >(ObjectIndex left, ObjectIndex right)
        => left.Index > right.Index;

    public static bool operator >=(ObjectIndex left, ObjectIndex right)
        => left.Index >= right.Index;

    public static bool operator <(ObjectIndex left, ObjectIndex right)
        => left.Index < right.Index;

    public static bool operator <=(ObjectIndex left, ObjectIndex right)
        => left.Index <= right.Index;
}
