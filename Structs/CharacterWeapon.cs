namespace Penumbra.GameData.Structs;

/// <summary> A packed struct representing the model data for a piece of armor or accessory. </summary>
/// <param name="class"> The primary ID. </param>
/// <param name="set"> The secondary ID. </param>
/// <param name="variant"> The variant. </param>
/// <param name="stain"> The Stain ID. </param>
[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 7)]
public struct CharacterWeapon(PrimaryId @class, SecondaryId set, Variant variant, StainId stain)
    : IEquatable<CharacterWeapon>, IComparable<CharacterWeapon>
{
    [FieldOffset(0)]
    public PrimaryId Class = @class;

    [FieldOffset(2)]
    public SecondaryId Set = set;

    [FieldOffset(4)]
    public Variant Variant = variant;

    [FieldOffset(5)]
    private readonly byte _padding = 0;

    [FieldOffset(6)]
    public StainId Stain = stain;

    /// <summary> The data as a single value. </summary>
    public readonly ulong Value
        => (ulong)Class.Id | ((ulong)Set.Id << 16) | ((ulong)Variant.Id << 32) | ((ulong)Stain.Id << 48);

    /// <inheritdoc/>
    public override readonly string ToString()
        => $"{Class},{Set},{Variant},{Stain}";

    /// <summary> Compare in inverse order. </summary>
    private readonly ulong CompareValue
        => ((ulong)Class.Id << 48) | ((ulong)Set.Id << 32) | ((ulong)Variant.Id << 16) | (ulong)Stain.Id;

    /// <summary> Create a character weapon from a single value. </summary>
    public CharacterWeapon(ulong value)
        : this((PrimaryId)value, (SecondaryId)(value >> 16), (Variant)(value >> 32), (StainId)(value >> 48))
    { }

    /// <summary> Return the same weapon with a different stain. </summary>
    public readonly CharacterWeapon With(StainId stainId)
        => new(Class, Set, Variant, stainId);

    /// <summary> Turn to a CharacterArmor by forgetting the secondary ID. </summary>
    public readonly CharacterArmor ToArmor()
        => new(Class, Variant, Stain);

    /// <summary> Turn to a CharacterArmor by forgetting the secondary ID, with a different stain. </summary>
    public readonly CharacterArmor ToArmor(StainId stainId)
        => new(Class, Variant, stainId);

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
