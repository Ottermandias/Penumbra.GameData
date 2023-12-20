using Penumbra.String.Functions;

namespace Penumbra.GameData.Structs;

/// <summary> A struct containing all the customize data for a character model. </summary>
[StructLayout(LayoutKind.Sequential, Size = Size)]
public unsafe struct CustomizeArray : IEquatable<CustomizeArray>, IReadOnlyCollection<byte>
{
    /// <summary> The size of the customize array. </summary>
    public const int Size = 26;

    /// <summary> The data.  </summary>
    public fixed byte Data[Size];

    /// <summary> The size of the customize array. </summary>
    public readonly int Count
        => Size;

    /// <summary> Iterate over all bytes in the data. </summary>
    public readonly IEnumerator<byte> GetEnumerator()
    {
        for (var i = 0; i < Size; ++i)
            yield return At(i);
    }

    /// <summary> Iterate over all bytes in the data. </summary>
    readonly IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    /// <summary> Get the byte at the given index without range-checking. </summary>
    public readonly byte At(int index)
        => Data[index];

    /// <summary> Set the byte at the given index without range-checking. </summary>
    public void Set(int index, byte value)
        => Data[index] = value;


    /// <summary> Read a pointer to another Customize array and copy its data. </summary>
    public void Read(void* source)
    {
        fixed (byte* ptr = Data)
        {
            MemoryUtility.MemCpyUnchecked(ptr, source, Size);
        }
    }

    /// <summary> Write this data to another Customize array given by a pointer. </summary>
    public readonly void Write(void* target)
    {
        fixed (byte* ptr = Data)
        {
            MemoryUtility.MemCpyUnchecked(target, ptr, Size);
        }
    }

    /// <summary> Clone this data to another Customize array. </summary>
    public readonly CustomizeArray Clone()
    {
        var ret = new CustomizeArray();
        Write(ret.Data);
        return ret;
    }

    /// <summary> Compare two Customize arrays for equality. </summary>
    public readonly bool Equals(CustomizeArray other)
    {
        fixed (byte* ptr = Data)
        {
            return MemoryUtility.MemCmpUnchecked(ptr, other.Data, Size) == 0;
        }
    }

    /// <inheritdoc/>
    public override readonly bool Equals(object? obj)
        => obj is CustomizeArray other && Equals(other);

    /// <summary> Compare two Customize arrays in unmanaged memory for equality. </summary>
    public static bool Equals(CustomizeArray* lhs, CustomizeArray* rhs)
        => MemoryUtility.MemCmpUnchecked(lhs, rhs, Size) == 0;

    /// <remarks>
    /// Compare Gender and then only from Height onwards, because all screen actors are set to Height 50,
    /// the Race is implicitly included in the subrace (after height),
    /// and the body type is irrelevant for players.
    /// </remarks>>
    public static bool ScreenActorEquals(CustomizeArray* lhs, CustomizeArray* rhs)
        => lhs->Data[1] == rhs->Data[1] && MemoryUtility.MemCmpUnchecked(lhs->Data + 4, rhs->Data + 4, Size - 4) == 0;

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        fixed (byte* ptr = Data)
        {
            var p = (int*)ptr;
            var u = *(ushort*)(p + 6);
            return HashCode.Combine(*p, p[1], p[2], p[3], p[4], p[5], u);
        }
    }

    /// <summary> Write a Customize array as Base64 string. </summary>
    public readonly string WriteBase64()
    {
        fixed (byte* ptr = Data)
        {
            var data = new ReadOnlySpan<byte>(ptr, Size);
            return Convert.ToBase64String(data);
        }
    }

    /// <summary> Write this Customize array byte-wise to string. </summary>
    public override string ToString()
    {
        var sb = new StringBuilder(Size * 3);
        for (var i = 0; i < Size - 1; ++i)
            sb.Append($"{Data[i]:X2} ");
        sb.Append($"{Data[Size - 1]:X2}");
        return sb.ToString();
    }


    /// <summary> Try to load a Base64 string into this Customize array. Returns true on success. </summary>
    public bool LoadBase64(string base64)
    {
        var buffer = stackalloc byte[Size];
        var span   = new Span<byte>(buffer, Size);
        if (!Convert.TryFromBase64String(base64, span, out var written) || written != Size)
            return false;

        Read(buffer);
        return true;
    }
}
