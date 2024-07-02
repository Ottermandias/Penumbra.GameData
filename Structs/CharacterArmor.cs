namespace Penumbra.GameData.Structs;

/// <summary> A packed struct representing the model data for a piece of armor or accessory. </summary>
/// <param name="set"> The primary ID. </param>
/// <param name="variant"> The variant. </param>
/// <param name="stain1"> The Stain ID. </param>
[StructLayout(LayoutKind.Explicit, Size = 8)]
public struct CharacterArmor(PrimaryId set, Variant variant, StainId stain1, StainId stain2) : IEquatable<CharacterArmor>
{
    /// <summary> The size of the struct. </summary>
    public const int Size = 8;

    /// <summary> The combined value. </summary>
    [FieldOffset(0)]
    public readonly ulong Value = 0;

    /// <summary> The primary ID </summary>
    [FieldOffset(0)]
    public PrimaryId Set = set;

    /// <summary> The variant. </summary>
    [FieldOffset(2)]
    public Variant Variant = variant;

    /// <summary> The first Stain ID. </summary>
    [FieldOffset(3)]
    public StainId Stain1 = stain1;

    /// <summary> The first Stain ID. </summary>
    [FieldOffset(4)]
    public StainId Stain2 = stain2;


    /// <summary> Return the same piece of equipment with a different stain. </summary>
    public readonly CharacterArmor With(StainId stainId1, StainId stainId2)
        => new(Set, Variant, stainId1, stainId2);

    /// <summary> Return the same piece of equipment as a CharacterWeapon without secondary ID. </summary>
    public readonly CharacterWeapon ToWeapon(SecondaryId set)
        => new(Set, set, Variant, Stain1, Stain2);

    /// <summary> Return the same piece of equipment as a CharacterWeapon without secondary ID and with a different stain. </summary>
    public readonly CharacterWeapon ToWeapon(SecondaryId set, StainId stainId1, StainId stainId2)
        => new(Set, set, Variant, stainId1, stainId2);

    /// <inheritdoc/>
    public override readonly string ToString()
        => $"{Set},{Variant},{Stain1},{Stain2}";

    /// <summary> The empty model. </summary>
    public static readonly CharacterArmor Empty;

    /// <inheritdoc/>
    public readonly bool Equals(CharacterArmor other)
        => Value == other.Value;

    /// <inheritdoc/>
    public override readonly bool Equals(object? obj)
        => obj is CharacterArmor other && Equals(other);

    /// <inheritdoc/>
    public override readonly int GetHashCode()
        => (int)Value;

    public static bool operator ==(CharacterArmor left, CharacterArmor right)
        => left.Value == right.Value;

    public static bool operator !=(CharacterArmor left, CharacterArmor right)
        => left.Value != right.Value;
}
