using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Penumbra.GameData.Files.Utility;

/// <summary>
/// Equivalent to <see cref="BinaryReader"/>, but for <see cref="ReadOnlySpan{Byte}"/>.
/// </summary>
/// <remarks>
/// This differs from <see cref="BinaryReader"/> in that "array" reads will throw if the requested amount of data is not fully available,
/// and that it only works in terms of <see cref="ReadOnlySpan{T}"/> (use <see cref="ReadOnlySpan{T}.ToArray"/> if needed).
/// </remarks>
public ref struct SpanBinaryReader
{
    public readonly ReadOnlySpan<byte> Span;

    public int Position;

    public readonly int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => Span.Length;
    }

    public readonly int Remaining
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => Span.Length - Position;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public SpanBinaryReader(ReadOnlySpan<byte> span)
    {
        Span     = span;
        Position = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public byte ReadByte()
    {
        if (Position >= Span.Length)
            throw new EndOfStreamException();

        return Span[Position++];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public ReadOnlySpan<byte> ReadBytes(int count)
    {
        if (Position + count > Span.Length)
            throw new EndOfStreamException();

        var result = Span.Slice(Position, count);

        Position += count;

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public SpanBinaryReader ReadSlice(int count)
        => new(ReadBytes(count));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public ReadOnlyStringPool ReadStringPool(int size)
        => new(ReadBytes(size));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public unsafe T ReadStructure<T>() where T : unmanaged
    {
        if (Position + sizeof(T) > Span.Length)
            throw new EndOfStreamException();

        T result;
        fixed (byte* ptr = &Span[Position])
        {
            result = *(T*)ptr;
        }
        Position += sizeof(T);

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public unsafe ReadOnlySpan<T> ReadStructures<T>(int count) where T : unmanaged
    {
        if (Position + sizeof(T) * count > Span.Length)
            throw new EndOfStreamException();

        ReadOnlySpan<T> result;
        fixed (byte* ptr = &Span[Position])
        {
            result = new(ptr, count);
        }
        Position += sizeof(T) * count;

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public ushort ReadUInt16()
        => ReadStructure<ushort>();

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public uint ReadUInt32()
        => ReadStructure<uint>();
}
