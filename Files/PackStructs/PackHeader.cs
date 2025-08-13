namespace Penumbra.GameData.Files.PackStructs;

public struct PackHeader
{
    /// <summary> The type of the pack struct. Generally an ASCII representation of 4 bytes of the type, like 'PACK' or 'EPBD'. </summary>
    public uint Type;

    /// <summary> Probably the version, currently always 1. </summary>
    public ushort Version;

    /// <summary> The number of remaining packs before this one, currently generally 1 (for <see cref="PackFooter"/>) or 0 for everything else. </summary>
    public ushort PackCount;

    /// <summary> The negative offset to the footer of the next pack structure (i.e. added to the start position of the current structure), sometimes the positive size of the data for the last pack struct. </summary>
    public long PriorOffset;
}
