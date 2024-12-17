namespace Penumbra.GameData.Files.MaterialStructs;

public interface IColorDyeTable
{
    /// <summary> The size of one row of this table, in bytes. </summary>
    int RowSize { get; }

    /// <summary> The height of this table, in rows. </summary>
    int Height { get; }

    /// <summary> The size of this table, in bytes. </summary>
    int Size { get; }

    /// <summary> Gets the contents of this table, as bytes. </summary>
    Span<byte> AsBytes();

    /// <summary> Gets the contents of a row, as <see cref="Half"/>. </summary>
    Span<byte> RowAsBytes(int i);

    /// <summary> Resets this table to default values. </summary>
    /// <returns> Whether something actually changed. </returns>
    bool SetDefault();

    /// <summary> Resets a row to default values. </summary>
    /// <returns> Whether something actually changed. </returns>
    bool SetDefaultRow(int i);

    [Flags]
    public enum ValueTypes : ulong
    {
        Diffuse       = 0x00000001,
        Specular      = 0x00000002,
        Emissive      = 0x00000004,
        Unk4          = 0x00000008,
        Metalness     = 0x00000010,
        Roughness     = 0x00000020,
        SheenRate     = 0x00000040,
        SheenTint     = 0x00000080,
        SheenApt      = 0x00000100,
        Anisotropy    = 0x00000200,
        SphereMapIdx  = 0x00000400,
        SphereMapMask = 0x00000800,
        Template      = 0x07FF0000,
        Channel       = 0x18000000,

        OldTemplate     = 0xE0,
        OldShinyness    = 0x08,
        OldSpecularMask = 0x10,

        Colors = Diffuse | Specular | Emissive,
    }

    public bool MergeSpecificValues(Span<byte> mergeInto, ReadOnlySpan<byte> mergeFrom, ValueTypes which)
    {
        if (!SpanSizeCheck(mergeInto) || !SpanSizeCheck(mergeFrom))
            return false;

        var mask = ToMask(which);
        if (mask == 0)
            return true;

        for (var i = 0; i < mergeInto.Length; ++i)
        {
            for (var j = 0; j < 8; ++j)
            {
                var flag     = 1 << j;
                var byteFlag = (ulong)(flag << (i * 8));
                if ((mask & byteFlag) == byteFlag)
                    mergeInto[i] = (byte)((mergeInto[i] & ~flag) | (mergeFrom[i] & flag));
            }
        }

        return true;
    }

    protected bool SpanSizeCheck(ReadOnlySpan<byte> span);

    protected ulong ToMask(ValueTypes mask);
}
