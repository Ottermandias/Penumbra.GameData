using Penumbra.GameData.Files.Utility;

namespace Penumbra.GameData.Files.PackStructs;

/// <summary> The base pack structure that signifies existence of pack structures. </summary>
public unsafe struct PackFooter
{
    /// <summary> The type. </summary>
    public const uint PackType = 'P' | ((uint)'A' << 8) | ((uint)'C' << 16) | ((uint)'K' << 24);

    /// <summary> The parsed footer. </summary>
    public PackHeader Header;

    /// <summary> The total size of the pack data, i.e. FileSize - this is the starting point of the pack data. </summary>
    public ulong TotalSize;

    /// <summary> Try to read a pack base from the full data of a file. </summary>
    public static bool TryRead(ReadOnlySpan<byte> data, out PackFooter packFooter)
    {
        var requiredSize = sizeof(PackFooter);
        if (data.Length < requiredSize)
        {
            packFooter = default;
            return false;
        }

        packFooter = new SpanBinaryReader(data[^requiredSize..]).Read<PackFooter>();
        return packFooter.Header.Type == PackType;
    }

    
}
