using System.Text.Json;
using Luna;
using Penumbra.String;

namespace Penumbra.GameData.Files.AtchStructs;

public unsafe struct AtchEntry : IEquatable<AtchEntry>
{
    public const int MaxBoneNameLength = 34;

    private fixed byte  _boneName[MaxBoneNameLength + 1];
    private       byte  _boneNameLength;
    public        float Scale;
    public        float OffsetX;
    public        float OffsetY;
    public        float OffsetZ;
    public        float RotationX;
    public        float RotationY;
    public        float RotationZ;

    public Span<byte> FullSpan
    {
        get
        {
            fixed (byte* ptr = _boneName)
            {
                return new Span<byte>(ptr, MaxBoneNameLength);
            }
        }
    }


    public ReadOnlySpan<byte> Bone
    {
        get
        {
            fixed (byte* ptr = _boneName)
            {
                return new ReadOnlySpan<byte>(ptr, _boneNameLength);
            }
        }
    }

    public void WriteJson(Utf8JsonWriter j)
    {
        j.WriteStartObject();
        AddToJson(j);
        j.WriteEndObject();
    }

    public void AddToJson(Utf8JsonWriter j)
    {
        j.WriteString("Bone"u8, Bone);
        j.WriteNumber("Scale"u8, Scale);
        j.WriteNumber("OffsetX"u8, OffsetX);
        j.WriteNumber("OffsetY"u8, OffsetY);
        j.WriteNumber("OffsetZ"u8, OffsetZ);
        j.WriteNumber("RotationX"u8, RotationX);
        j.WriteNumber("RotationY"u8, RotationY);
        j.WriteNumber("RotationZ"u8, RotationZ);
    }

    public bool SetBoneName(ReadOnlySpan<byte> text)
    {
        if (text.Length > MaxBoneNameLength)
            return false;

        fixed (byte* ptr = _boneName)
        {
            text.CopyTo(new Span<byte>(ptr, MaxBoneNameLength));
        }

        _boneNameLength            = (byte)text.Length;
        _boneName[_boneNameLength] = 0;
        return true;
    }

    public bool SetBoneName(string text)
    {
        try
        {
            var utf8 = Encoding.UTF8.GetBytes(text);
            return SetBoneName(utf8);
        }
        catch
        {
            return false;
        }
    }

    public Vector3 Offset
    {
        get => new(OffsetX, OffsetY, OffsetZ);
        set
        {
            OffsetX = value.X;
            OffsetY = value.Y;
            OffsetZ = value.Z;
        }
    }

    public Vector3 Rotation
    {
        get => new(RotationX, RotationY, RotationZ);
        set
        {
            RotationX = value.X;
            RotationY = value.Y;
            RotationZ = value.Z;
        }
    }


    public static AtchEntry Read(ref SpanBinaryReader reader)
    {
        var stringOffset = reader.ReadInt32();
        var boneName     = reader.ReadByteString(stringOffset);
        var ret          = new AtchEntry();
        if (!ret.SetBoneName(boneName))
            throw new Exception(
                $"Invalid Atch Entry with bone name {ByteString.FromSpanUnsafe(boneName, true, null, true)} longer than max length.");

        ret.Scale     = reader.ReadSingle();
        ret.OffsetX   = reader.ReadSingle();
        ret.OffsetY   = reader.ReadSingle();
        ret.OffsetZ   = reader.ReadSingle();
        ret.RotationX = reader.ReadSingle();
        ret.RotationY = reader.ReadSingle();
        ret.RotationZ = reader.ReadSingle();
        return ret;
    }

    public bool Equals(AtchEntry other)
        => Scale.Equals(other.Scale)
         && OffsetX.Equals(other.OffsetX)
         && OffsetY.Equals(other.OffsetY)
         && OffsetZ.Equals(other.OffsetZ)
         && RotationX.Equals(other.RotationX)
         && RotationY.Equals(other.RotationY)
         && RotationZ.Equals(other.RotationZ)
         && Bone.SequenceEqual(other.Bone);

    public override bool Equals(object? obj)
        => obj is AtchEntry other && Equals(other);

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(_boneNameLength);
        hashCode.Add(_boneName[0]);
        hashCode.Add(Scale);
        hashCode.Add(OffsetX);
        hashCode.Add(OffsetY);
        hashCode.Add(OffsetZ);
        hashCode.Add(RotationX);
        hashCode.Add(RotationY);
        hashCode.Add(RotationZ);
        return hashCode.ToHashCode();
    }

    public static bool operator ==(AtchEntry left, AtchEntry right)
        => left.Equals(right);

    public static bool operator !=(AtchEntry left, AtchEntry right)
        => !left.Equals(right);
}
