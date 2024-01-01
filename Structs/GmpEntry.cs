namespace Penumbra.GameData.Structs;

/// <summary>
/// Gimmick Parameter entries contain information about visors and exist for all primary equipment IDs for head slots only.
/// It is 5 bytes per entry.
/// </summary>
public struct GmpEntry : IEquatable<GmpEntry>
{
    /// <summary> The default GMP entry is empty. </summary>
    public static readonly GmpEntry Default = new();

    /// <summary> Whether the visor is enabled at all. Bit 1. </summary>
    public bool Enabled
    {
        readonly get => (Value & 1) == 1;
        set
        {
            if (value)
                Value |= 1ul;
            else
                Value &= ~1ul;
        }
    }

    /// <summary> Whether toggling the visor is animated. Bit 2.</summary>
    public bool Animated
    {
        readonly get => (Value & 2) == 2;
        set
        {
            if (value)
                Value |= 2ul;
            else
                Value &= ~2ul;
        }
    }

    /// <summary> Rotation a in degrees for a toggled visor. Bit 3-12. </summary>
    public ushort RotationA
    {
        readonly get => (ushort)((Value >> 2) & 0x3FF);
        set => Value = (Value & ~0xFFCul) | ((value & 0x3FFul) << 2);
    }

    /// <summary> Rotation b in degrees for a toggled visor. Bit 13-22</summary>
    public ushort RotationB
    {
        readonly get => (ushort)((Value >> 12) & 0x3FF);
        set => Value = (Value & ~0x3FF000ul) | ((value & 0x3FFul) << 12);
    }

    /// <summary> Rotation c in degrees for a toggled visor. Bit 23-32</summary>
    public ushort RotationC
    {
        readonly get => (ushort)((Value >> 22) & 0x3FF);
        set => Value = (Value & ~0xFFC00000ul) | ((value & 0x3FFul) << 22);
    }

    /// <summary> Unknown parameter 1. Bit 33-36</summary>
    public byte UnknownA
    {
        readonly get => (byte)((Value >> 32) & 0x0F);
        set => Value = (Value & ~0x0F00000000ul) | ((value & 0x0Ful) << 32);
    }

    /// <summary> Unknown parameter 2. Bit 37-40</summary>
    public byte UnknownB
    {
        readonly get => (byte)((Value >> 36) & 0x0F);
        set => Value = (Value & ~0xF000000000ul) | ((value & 0x0Ful) << 36);
    }

    /// <summary> Both unknown parameters together. Bits 32-40 or byte 5. </summary>
    public byte UnknownTotal
    {
        get => (byte)((Value >> 32) & 0xFF);
        set => Value = (Value & ~0xFF00000000ul) | ((value & 0xFFul) << 32);
    }

    /// <summary> The total value of the parameter as single 8 byte integer, the upper 3 bytes are always 0. </summary>
    public ulong Value { get; set; }


    /// <summary> Read 5 bytes from the array.
    public static GmpEntry FromTexToolsMeta(ReadOnlySpan<byte> data)
        => new()
        {
            Value    = MemoryMarshal.Read<uint>(data),
            UnknownA = data[4],
        };

    /// <summary> Convert a GmpEntry to ulong. </summary>
    public static implicit operator ulong(GmpEntry entry)
        => entry.Value;

    /// <summary> Convert an ulong to GmpEntry. </summary>
    public static explicit operator GmpEntry(ulong entry)
        => new() { Value = entry };

    /// <inheritdoc/>
    public readonly bool Equals(GmpEntry other)
        => Value == other.Value;

    /// <inheritdoc/>
    public override readonly bool Equals(object? obj)
        => obj is GmpEntry other && Equals(other);

    /// <inheritdoc/>
    public override readonly int GetHashCode()
        => Value.GetHashCode();

    public static bool operator ==(GmpEntry left, GmpEntry right)
        => left.Equals(right);

    public static bool operator !=(GmpEntry left, GmpEntry right)
        => !(left == right);
}
