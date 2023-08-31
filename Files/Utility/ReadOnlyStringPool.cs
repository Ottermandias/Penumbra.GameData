using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Penumbra.GameData.Files.Utility;

public readonly ref struct ReadOnlyStringPool
{
    public readonly ReadOnlySpan<byte> Span;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public ReadOnlyStringPool(ReadOnlySpan<byte> span)
    {
        Span = span;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly string GetString(int offset, int size)
        => Encoding.UTF8.GetString(Span.Slice(offset, size));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly string GetNullTerminatedString(int offset)
    {
        var str = Span[offset..];
        var size = str.IndexOf((byte)0);
        if (size >= 0)
            str = str[..size];
        return Encoding.UTF8.GetString(str);
    }
}
