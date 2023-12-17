namespace Penumbra.GameData.Structs;

[StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct CharacterArmor(SetId set, Variant variant, StainId stain) : IEquatable<CharacterArmor>
{
    public const int Size = 4;

    [FieldOffset(0)]
    public readonly uint Value = 0;

    [FieldOffset(0)]
    public SetId Set = set;

    [FieldOffset(2)]
    public Variant Variant = variant;

    [FieldOffset(3)]
    public StainId Stain = stain;

    public readonly CharacterArmor With(StainId stainId)
        => new(Set, Variant, stainId);

    public readonly CharacterWeapon ToWeapon(WeaponType type)
        => new(Set, type, Variant, Stain);

    public readonly CharacterWeapon ToWeapon(WeaponType type, StainId stainId)
        => new(Set, type, Variant, stainId);

    public override readonly string ToString()
        => $"{Set},{Variant},{Stain}";

    public static readonly CharacterArmor Empty;

    public readonly bool Equals(CharacterArmor other)
        => Value == other.Value;

    public override readonly bool Equals(object? obj)
        => obj is CharacterArmor other && Equals(other);

    public override readonly int GetHashCode()
        => (int)Value;

    public static bool operator ==(CharacterArmor left, CharacterArmor right)
        => left.Value == right.Value;

    public static bool operator !=(CharacterArmor left, CharacterArmor right)
        => left.Value != right.Value;
}
