namespace Penumbra.GameData.Files.MaterialStructs;

public interface IColorTable
{
    /// <summary> The width of this table, in vectors of 4 <see cref="Half"/> per row. </summary>
    int Width { get; }

    /// <summary> The size of one row of this table, in bytes. </summary>
    int RowSize { get; }

    /// <summary> The height of this table, in rows. </summary>
    int Height { get; }

    /// <summary> The size of this table, in bytes. </summary>
    int Size { get; }

    /// <summary>
    /// Binary logarithms of the dimensions of this table, for <see cref="TableFlags.TableDimensionLogs"/>.
    /// Low nibble is the log of <see cref="Width"/>, high nibble is the log of <see cref="Height"/>.
    /// Must be zero (instead of 0x42) for legacy tables.
    /// </summary>
    byte DimensionLogs { get; }

    /// <summary> Gets the contents of this table, as bytes. </summary>
    Span<byte> AsBytes();

    /// <summary> Gets the contents of this table, as <see cref="Half"/>. </summary>
    Span<Half> AsHalves();

    /// <summary> Gets the contents of a row, as bytes. </summary>
    Span<byte> RowAsBytes(int i);

    /// <summary> Gets the contents of a row, as <see cref="Half"/>. </summary>
    Span<Half> RowAsHalves(int i);

    /// <summary> Resets this table to default values. </summary>
    /// <returns> Whether something actually changed. </returns>
    bool SetDefault();

    /// <summary> Resets a row to default values. </summary>
    /// <returns> Whether something actually changed. </returns>
    bool SetDefaultRow(int i);

    [Flags]
    public enum ValueTypes : ulong
    {
        DiffuseR         = 0x01,
        DiffuseG         = 0x02,
        DiffuseB         = 0x04,
        Unk4             = 0x08,
        SpecularStrength = 0x08,

        SpecularR = 0x10,
        SpecularG = 0x20,
        SpecularB = 0x40,
        Unk8      = 0x80,
        Gloss     = 0x80,

        EmissiveR  = 0x100,
        EmissiveG  = 0x200,
        EmissiveB  = 0x400,
        Unk12      = 0x800,
        TileIdxOld = 0x800,

        Colors = 0x777,

        SheenRate = 0x1000,
        SheenTint = 0x2000,
        SheenApt  = 0x4000,
        SheenUnk  = 0x8000,

        TileOldXFUU = 0x10000000,
        TileOldXFUV = 0x20000000,
        TileOldXFVU = 0x40000000,
        TileOldXFVV = 0x80000000,

        Roughness  = 0x10000,
        Unk18      = 0x20000,
        Metalness  = 0x40000,
        Anisotropy = 0x80000,

        Unk20      = 0x100000,
        SphereMask = 0x200000,
        Unk22      = 0x400000,
        Unk23      = 0x800000,

        ShaderIdx  = 0x1000000,
        TileIdxNew = 0x2000000,
        TileAlpha  = 0x4000000,
        SphereIdx  = 0x8000000,

        TileNewXFUU = 0x10000000,
        TileNewXFUV = 0x20000000,
        TileNewXFVU = 0x40000000,
        TileNewXFVV = 0x80000000,
    }

    public bool MergeSpecificValues(Span<Half> mergeInto, ReadOnlySpan<Half> mergeFrom, ValueTypes which)
    {
        if (!SpanSizeCheck(mergeInto) || !SpanSizeCheck(mergeFrom))
            return false;

        var mask = ToMask(which);
        if (mask == 0)
            return true;

        for (var i = 0; i < mergeInto.Length; ++i)
        {
            if ((((ulong)which >> i) & 1ul) == 1ul)
                mergeInto[i] = mergeFrom[i];
        }

        return true;
    }

    protected bool SpanSizeCheck(ReadOnlySpan<Half> span);

    protected ulong ToMask(ValueTypes mask);
}
