using Newtonsoft.Json;

namespace Penumbra.GameData.Structs;

/// <summary> An Image Change entry associates a variant with a specific material, effects and decals. </summary>
public readonly struct ImcEntry : IEquatable<ImcEntry>
{
    /// <summary> The material to use with this variant. </summary>
    public byte MaterialId { get; init; }

    /// <summary> An optional decal to use with this variant. 0 means none. </summary>
    public byte DecalId { get; init; }

    /// <summary> Additional attributes and a sound ID packed. </summary>
    public readonly ushort AttributeAndSound;

    /// <summary> An optional VFX to use with this variant. 0 means none. </summary>
    public byte VfxId { get; init; }

    /// <summary> An optional animation to use with this variant. 0 means none. </summary>
    public byte MaterialAnimationId { get; init; }

    /// <summary> Additional attributes for this variant. Bit 1-10. </summary>
    public ushort AttributeMask
    {
        get => (ushort)(AttributeAndSound & 0x3FF);
        init => AttributeAndSound = (ushort)((AttributeAndSound & ~0x3FF) | (value & 0x3FF));
    }

    /// <summary> An optional sound ID to use with this variant. Bit 11-16. </summary>
    public byte SoundId
    {
        get => (byte)(AttributeAndSound >> 10);
        init => AttributeAndSound = (ushort)(AttributeMask | (value << 10));
    }

    /// <inheritdoc/>
    public bool Equals(ImcEntry other)
        => MaterialId == other.MaterialId
         && DecalId == other.DecalId
         && AttributeAndSound == other.AttributeAndSound
         && VfxId == other.VfxId
         && MaterialAnimationId == other.MaterialAnimationId;

    /// <inheritdoc/>
    public override bool Equals(object? obj)
        => obj is ImcEntry other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
        => HashCode.Combine(MaterialId, DecalId, AttributeAndSound, VfxId, MaterialAnimationId);

    /// <summary> A constructor to use when deserializing from JSON. </summary>
    [JsonConstructor]
    public ImcEntry(byte materialId, byte decalId, ushort attributeMask, byte soundId, byte vfxId, byte materialAnimationId)
    {
        MaterialId          = materialId;
        DecalId             = decalId;
        AttributeAndSound   = 0;
        VfxId               = vfxId;
        MaterialAnimationId = materialAnimationId;
        AttributeMask       = attributeMask;
        SoundId             = soundId;
    }

    public static bool operator ==(ImcEntry left, ImcEntry right)
        => left.Equals(right);

    public static bool operator !=(ImcEntry left, ImcEntry right)
        => !(left == right);
}
