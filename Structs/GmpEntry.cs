using Newtonsoft.Json;

namespace Penumbra.GameData.Structs;

/// <summary>
/// Gimmick Parameter entries contain information about visors and exist for all primary equipment IDs for head slots only.
/// It is 5 bytes per entry.
/// </summary>
public struct GmpEntry : IEquatable<GmpEntry>
{
    /// <summary> The default GMP entry is empty. </summary>
    public static readonly GmpEntry Default = new();

    private byte _value1;
    private byte _value2;
    private byte _value3;
    private byte _value4;
    private byte _value5;

    /// <summary> Whether the visor is enabled at all. Bit 1. </summary>
    public bool Enabled
    {
        readonly get => (_value1 & 1) == 1;
        set
        {
            if (value)
                _value1 = (byte)(_value1 | 1);
            else
                _value1 = (byte)(_value1 & ~1);
        }
    }

    /// <summary> Whether toggling the visor is animated. Bit 2.</summary>
    public bool Animated
    {
        readonly get => (_value1 & 2) == 2;
        set
        {
            if (value)
                _value1 = (byte)(_value1 | 2);
            else
                _value1 = (byte)(_value1 & ~2);
        }
    }

    /// <summary> Rotation a in degrees for a toggled visor. Bit 3-12. </summary>
    public ushort RotationA
    {
        readonly get => (ushort)((_value1 >> 2) | ((_value2 & 0x0F) << 6));
        set
        {
            _value1 = (byte)((_value1 & 0x03) | ((value << 2) & 0xFC));
            _value2 = (byte)((_value2 & 0xF0) | ((value >> 6) & 0x0F));
        }
    }

    /// <summary> Rotation b in degrees for a toggled visor. Bit 13-22</summary>
    public ushort RotationB
    {
        readonly get => (ushort)((_value2 >> 4) | ((_value3 & 0x3F) << 4));
        set
        {
            _value2 = (byte)((_value2 & 0x0F) | ((value << 4) & 0xF0));
            _value3 = (byte)((_value3 & 0xC0) | ((value >> 4) & 0x3F));
        }
    }

    /// <summary> Rotation c in degrees for a toggled visor. Bit 23-32</summary>
    public ushort RotationC
    {
        readonly get => (ushort)((_value3 >> 6) | (_value4 << 2));
        set
        {
            _value3 = (byte)((_value3 & 0x3F) | ((value << 6) & 0xC0));
            _value4 = (byte)(value >> 2);
        }
    }

    /// <summary> Unknown parameter 1. Bit 33-36</summary>
    public byte UnknownA
    {
        readonly get => (byte)(_value5 & 0x0F);
        set => _value5 = (byte)((_value5 & 0xF0) | (value & 0x0F));
    }

    /// <summary> Unknown parameter 2. Bit 37-40</summary>
    public byte UnknownB
    {
        readonly get => (byte)(_value5 >> 4);
        set => _value5 = (byte)((_value5 & 0x0F) | (value << 4));
    }

    /// <summary> Both unknown parameters together. Bits 32-40 or byte 5. </summary>
    [JsonIgnore]
    public byte UnknownTotal
    {
        get => _value5;
        set => _value5 = value;
    }

    /// <summary> The total value of the parameter as single 8 byte integer, the upper 3 bytes are always 0. </summary>
    [JsonIgnore]
    public ulong Value
    {
        readonly get => _value1 | ((ulong)_value2 << 8) | ((ulong)_value3 << 16) | ((ulong)_value4 << 24) | ((ulong)_value5 << 32);
        set
        {
            _value1 = (byte)(value & 0xFF);
            _value2 = (byte)((value >> 8) & 0xFF);
            _value3 = (byte)((value >> 16) & 0xFF);
            _value4 = (byte)((value >> 24) & 0xFF);
            _value5 = (byte)((value >> 32) & 0xFF);
        }
    }


    /// <summary> Read 5 bytes from the array. </summary>
    public static GmpEntry FromTexToolsMeta(ReadOnlySpan<byte> data)
        => new()
        {
            _value1 = data[0],
            _value2 = data[1],
            _value3 = data[2],
            _value4 = data[3],
            _value5 = data[4],
        };

    /// <inheritdoc/>
    public readonly bool Equals(GmpEntry other)
        => _value1 == other._value1
         && _value2 == other._value2
         && _value3 == other._value3
         && _value4 == other._value4
         && _value5 == other._value5;

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
