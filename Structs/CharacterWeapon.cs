namespace Penumbra.GameData.Structs;

/// <summary> A packed struct representing the model data for a piece of armor or accessory. </summary>
/// <param name="skeleton"> The primary ID. </param>
/// <param name="set"> The secondary ID. </param>
/// <param name="variant"> The variant. </param>
/// <param name="stain"> The Stain ID. </param>
[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 7)]
public struct CharacterWeapon(PrimaryId skeleton, SecondaryId set, Variant variant, StainId stain)
    : IEquatable<CharacterWeapon>, IComparable<CharacterWeapon>
{
    [FieldOffset(0)]
    public PrimaryId Skeleton = skeleton;

    [FieldOffset(2)]
    public SecondaryId Weapon = set;

    [FieldOffset(4)]
    public Variant Variant = variant;

    [FieldOffset(5)]
    private readonly byte _padding = 0;

    [FieldOffset(6)]
    public StainId Stain = stain;

    /// <summary> The data as a single value. </summary>
    public readonly ulong Value
        => (ulong)Skeleton.Id | ((ulong)Weapon.Id << 16) | ((ulong)Variant.Id << 32) | ((ulong)Stain.Id << 48);

    /// <inheritdoc/>
    public override readonly string ToString()
        => $"{Skeleton},{Weapon},{Variant},{Stain}";

    /// <summary> Compare in inverse order. </summary>
    private readonly ulong CompareValue
        => ((ulong)Skeleton.Id << 48) | ((ulong)Weapon.Id << 32) | ((ulong)Variant.Id << 16) | (ulong)Stain.Id;

    /// <summary> Create a character weapon from a single value. </summary>
    public CharacterWeapon(ulong value)
        : this((PrimaryId)value, (SecondaryId)(value >> 16), (Variant)(value >> 32), (StainId)(value >> 48))
    { }

    /// <summary> Return the same weapon with a different stain. </summary>
    public readonly CharacterWeapon With(StainId stainId)
        => new(Skeleton, Weapon, Variant, stainId);

    /// <summary> Turn to a CharacterArmor by forgetting the secondary ID. </summary>
    public readonly CharacterArmor ToArmor()
        => new(Skeleton, Variant, Stain);

    /// <summary> Turn to a CharacterArmor by forgetting the secondary ID, with a different stain. </summary>
    public readonly CharacterArmor ToArmor(StainId stainId)
        => new(Skeleton, Variant, stainId);

    /// <summary> The empty CharacterWeapon. </summary>
    public static readonly CharacterWeapon Empty = new(0, 0, 0, 0);

    /// <inheritdoc/>
    public readonly bool Equals(CharacterWeapon other)
        => Value == other.Value;

    /// <inheritdoc/>
    public override readonly bool Equals(object? obj)
        => obj is CharacterWeapon other && Equals(other);

    /// <inheritdoc/>
    public override readonly int GetHashCode()
        => Value.GetHashCode();

    /// <inheritdoc/>
    public readonly int CompareTo(CharacterWeapon rhs)
        => CompareValue.CompareTo(rhs.CompareValue);

    public static bool operator ==(CharacterWeapon left, CharacterWeapon right)
        => left.Value == right.Value;

    public static bool operator !=(CharacterWeapon left, CharacterWeapon right)
        => left.Value != right.Value;

    /// <summary> A helper to create CharacterWeapons without casting from integers. </summary>
    public static CharacterWeapon Int(int set, int type, int variant)
        => new((PrimaryId)set, (SecondaryId)type, (Variant)variant, 0);
}
