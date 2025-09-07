using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using Luna.Generators;
using Newtonsoft.Json.Linq;
using Penumbra.GameData.Enums;

namespace Penumbra.GameData.Structs;

internal static class IdTypes
{
    public const string         IdName = "Id";
    public const StrongTypeFlag Flags  = StrongTypeFlag.Default | StrongTypeFlag.NewtonsoftConverter;
}

[StrongType<uint>(IdTypes.IdName, IdTypes.Flags)]
public readonly partial struct ModelCharaId;

[StrongType<uint>(IdTypes.IdName, IdTypes.Flags)]
public readonly partial struct BNpcId;

[StrongType<uint>(IdTypes.IdName, IdTypes.Flags)]
public readonly partial struct ENpcId;

[StrongType<uint>(IdTypes.IdName, IdTypes.Flags)]
public readonly partial struct BNpcNameId;

[StrongType<uint>(IdTypes.IdName, IdTypes.Flags)]
public readonly partial struct MountId;

[StrongType<uint>(IdTypes.IdName, IdTypes.Flags)]
public readonly partial struct CompanionId;

[StrongType<uint>(IdTypes.IdName, IdTypes.Flags)]
public readonly partial struct OrnamentId;

[StrongType<uint>(IdTypes.IdName, IdTypes.Flags)]
public readonly partial struct NpcId
{
    public static implicit operator NpcId(ENpcId id)
        => new(id.Id);

    public static implicit operator NpcId(BNpcNameId id)
        => new(id.Id);

    public static implicit operator NpcId(MountId id)
        => new(id.Id);

    public static implicit operator NpcId(CompanionId id)
        => new(id.Id);

    public static implicit operator NpcId(OrnamentId id)
        => new(id.Id);

    public ENpcId ENpcId
        => new(Id);

    public BNpcNameId BNpcNameId
        => new(Id);

    public MountId MountId
        => new(Id);

    public CompanionId CompanionId
        => new(Id);

    public OrnamentId OrnamentId
        => new(Id);
}

[StrongType<ushort>(IdTypes.IdName, IdTypes.Flags)]
public readonly partial struct SecondaryId;

[StrongType<ushort>(IdTypes.IdName, IdTypes.Flags)]
public readonly partial struct PrimaryId;

[StrongType<byte>(IdTypes.IdName, IdTypes.Flags)]
public readonly partial struct Variant
{
    public static readonly Variant None = new(0);
}

[StrongType<byte>(IdTypes.IdName, IdTypes.Flags)]
public readonly partial struct StainId
{
    public const int NumStains = 2;
}

public readonly record struct StainIds(StainId Stain1, StainId Stain2) : IReadOnlyList<StainId>
{
    public static readonly StainIds None = new();

    public StainIds(StainId stain1)
        : this(stain1, 0)
    { }

    public StainIds ChangeSecond(StainId stain2)
        => this with { Stain2 = stain2 };

    public static StainIds Second(StainId stain2)
        => new(0, stain2);

    public static implicit operator StainIds(StainId stain1)
        => new(stain1);

    public StainIds With(int idx, StainId stain)
    {
        return idx switch
        {
            0 => this with { Stain1 = stain },
            1 => this with { Stain2 = stain },
            _ => this,
        };
    }

    public IEnumerator<StainId> GetEnumerator()
    {
        yield return Stain1;
        yield return Stain2;
    }

    public override string ToString()
        => $"{Stain1.Id},{Stain2.Id}";

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public static StainIds FromUShort(ushort value)
        => new((StainId)(value & 0xFF), (StainId)(value >> 8));

    public int Count
        => 2;

    public StainId this[int index]
        => index == 0 ? Stain1 : Stain2;

    public JObject AddToObject(JObject obj)
    {
        obj["Stain"] = Stain1.Id;
        var i = 2;
        foreach (var stain in this.Skip(1))
            obj[$"Stain{i++}"] = stain.Id;
        return obj;
    }

    public static StainIds ParseFromObject(JObject? obj)
    {
        if (obj == null)
            return None;

        var stain  = (StainId)(obj["Stain"]?.ToObject<byte>() ?? 0);
        var stain2 = (StainId)(obj["Stain2"]?.ToObject<byte>() ?? 0);
        return new StainIds(stain, stain2);
    }

    public static StainIds All(StainId stain)
        => new(stain, stain);

    public StainIds(IReadOnlyList<StainId> stains)
        : this(stains.Count > 0 ? stains[0] : StainId.Zero, stains.Count > 1 ? stains[1] : StainId.Zero)
    { }

    public StainIds(IReadOnlyList<byte> stains)
        : this(stains.Count > 0 ? stains[0] : StainId.Zero, stains.Count > 1 ? stains[1] : StainId.Zero)
    { }

    public StainIds(ReadOnlySpan<byte> stains)
        : this(stains.Length > 0 ? (StainId)stains[0] : StainId.Zero, stains.Length > 1 ? stains[1] : StainId.Zero)
    { }

    public static StainIds FromGearsetItem(in RaptureGearsetModule.GearsetItem item)
        => new(item.Stain0Id, item.Stain1Id);

    public static StainIds FromGlamourPlate(in MirageManager.GlamourPlate plate, int idx)
        => new(plate.Stain0Ids[idx], plate.Stain1Ids[idx]);

    public static StainIds FromWeapon(in Weapon weapon)
        => new(weapon.Stain0, weapon.Stain1);
}

[StrongType<ushort>(IdTypes.IdName, IdTypes.Flags)]
public readonly partial struct BonusItemId
{
    public static readonly BonusItemId Invalid = new(ushort.MaxValue);
}

[StrongType<uint>(IdTypes.IdName, IdTypes.Flags)]
public readonly partial struct ItemId
{
    public ItemId StripModifiers
        => new(Id switch
        {
            > 1000000 => Id - 1000000,
            > 500000  => Id - 500000,
            _         => Id,
        });
}

[StrongType<ulong>(IdTypes.IdName, IdTypes.Flags)]
public readonly partial struct CustomItemId
{
    private const ulong CustomFlag    = 1ul << 48;
    private const ulong BonusItemFlag = 1ul << 49;

    public bool IsBonusItem
        => (Id & BonusItemFlag) == BonusItemFlag;

    public bool IsItem
        => Id < CustomFlag;

    public bool IsCustom
        => (Id & CustomFlag) == CustomFlag;

    public ItemId Item
        => IsItem ? (ItemId)Id : 0;

    public BonusItemId BonusItem
        => IsBonusItem ? (BonusItemId)Id : BonusItemId.Invalid;

    public (PrimaryId Model, SecondaryId WeaponType, Variant Variant, FullEquipType Type) Split
        => IsItem ? (0, 0, 0, FullEquipType.Unknown) : ((PrimaryId)Id, (SecondaryId)(Id >> 16), (Variant)(Id >> 32), (FullEquipType)(Id >> 40));

    public (PrimaryId Model, Variant Variant, BonusItemFlag Slot) SplitBonus
        => (Id & (CustomFlag | BonusItemFlag)) == (CustomFlag | BonusItemFlag)
            ? ((PrimaryId)Id, (Variant)(Id >> 16), (BonusItemFlag)(Id >> 24))
            : (0, 0, Enums.BonusItemFlag.Unknown);

    public CustomItemId(PrimaryId model, SecondaryId secondaryId, Variant variant, FullEquipType type)
        : this(model.Id | ((ulong)secondaryId.Id << 16) | ((ulong)variant.Id << 32) | ((ulong)type << 40) | CustomFlag)
    { }

    public CustomItemId(PrimaryId model, Variant variant, BonusItemFlag slot)
        : this(model.Id | ((ulong)variant.Id << 16) | ((ulong)slot << 24) | CustomFlag | BonusItemFlag)
    { }

    public static implicit operator CustomItemId(BonusItemId id)
        => new(id.Id | BonusItemFlag);

    public static implicit operator CustomItemId(ItemId id)
        => new(id.Id);

    public string ToDiscriminatingString()
    {
        if (IsItem)
            return Id.ToString();

        if (IsBonusItem)
        {
            if (IsCustom)
            {
                var (primary, variant, type) = SplitBonus;
                return $"{primary}-{variant}-{type.ToName()}";
            }

            return $"{BonusItem}B";
        }

        if (IsCustom)
        {
            var (primary, secondary, variant, type) = Split;
            return $"{primary}-{secondary}-{variant}-{type.ToName()}";
        }

        return Id.ToString();
    }
}

[StrongType<uint>(IdTypes.IdName, IdTypes.Flags)]
public readonly partial struct IconId;

[StrongType<ushort>(IdTypes.IdName, IdTypes.Flags)]
public readonly partial struct WorldId
{
    public static readonly WorldId AnyWorld = new(ushort.MaxValue);
}

[StrongType<byte>(IdTypes.IdName, IdTypes.Flags)]
public readonly partial struct JobId;

[StrongType<byte>(IdTypes.IdName, IdTypes.Flags)]
public readonly partial struct JobGroupId;

[StrongType<byte>("Value", IdTypes.Flags)]
public readonly partial struct CharacterLevel;

[StrongType<ushort>("Index", IdTypes.Flags)]
public readonly partial struct ObjectIndex
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
    public static readonly ObjectIndex IslandStart     = new(729);
}
