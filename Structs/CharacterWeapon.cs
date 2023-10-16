namespace Penumbra.GameData.Structs;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 7)]
public struct CharacterWeapon : IEquatable<CharacterWeapon>, IComparable<CharacterWeapon>
{
    [FieldOffset(0)]
    public SetId Set;

    [FieldOffset(2)]
    public WeaponType Type;

    [FieldOffset(4)]
    public Variant Variant;

    [FieldOffset(5)]
    private readonly byte _padding = 0;

    [FieldOffset(6)]
    public StainId Stain;

    public readonly ulong Value
        => (ulong)Set.Id | ((ulong)Type.Id << 16) | ((ulong)Variant.Id << 32) | ((ulong)Stain.Id << 48);

    public override readonly string ToString()
        => $"{Set},{Type},{Variant},{Stain}";

    private readonly ulong CompareValue
        => ((ulong)Set.Id << 48) | ((ulong)Type.Id << 32) | ((ulong)Variant.Id << 16) | (ulong)Stain.Id;

    public CharacterWeapon(SetId set, WeaponType type, Variant variant, StainId stain)
    {
        Set      = set;
        Type     = type;
        Variant  = variant;
        Stain    = stain;
        _padding = 0;
    }

    public CharacterWeapon(ulong value)
    {
        Set      = (SetId)value;
        Type     = (WeaponType)(value >> 16);
        Variant  = (Variant)(value >> 32);
        Stain    = (StainId)(value >> 48);
        _padding = 0;
    }

    public readonly CharacterWeapon With(StainId stain)
        => new(Set, Type, Variant, stain);

    public readonly CharacterArmor ToArmor()
        => new(Set, Variant, Stain);

    public readonly CharacterArmor ToArmor(StainId stain)
        => new(Set, Variant, stain);

    public static readonly CharacterWeapon Empty = new(0, 0, 0, 0);

    public readonly bool Equals(CharacterWeapon other)
        => Value == other.Value;

    public override readonly bool Equals(object? obj)
        => obj is CharacterWeapon other && Equals(other);

    public override readonly int GetHashCode()
        => Value.GetHashCode();

    public readonly int CompareTo(CharacterWeapon rhs)
        => CompareValue.CompareTo(rhs.CompareValue);

    public static bool operator ==(CharacterWeapon left, CharacterWeapon right)
        => left.Value == right.Value;

    public static bool operator !=(CharacterWeapon left, CharacterWeapon right)
        => left.Value != right.Value;

    public static CharacterWeapon Int(int set, int type, int variant)
        => new((SetId)set, (WeaponType)type, (Variant)variant, 0);
}
