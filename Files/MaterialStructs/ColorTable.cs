using Penumbra.GameData.Files.StainMapStructs;
using Penumbra.GameData.Files.Utility;
using Penumbra.GameData.Structs;

namespace Penumbra.GameData.Files.MaterialStructs;

// TODO: those values are not correct at all, just taken from legacy for now.
public sealed class ColorTable : IEnumerable<ColorTable.Row>, IColorTable
{
    /// <summary>
    /// The number after the parameter is the bit in the dye flags.
    /// <code>
    /// #       |    X (+0)    |    |    Y (+1)    |    |    Z (+2)   |    |   W (+3)    |
    /// --------------------------------------------------------------------------------------
    /// 0 (+ 0) |    Diffuse.R |  0 |    Diffuse.G |  0 |   Diffuse.B |  0 |         Unk |  
    /// 1 (+ 4) |   Specular.R |  1 |   Specular.G |  1 |  Specular.B |  1 |         Unk |
    /// 2 (+ 8) |   Emissive.R |  2 |   Emissive.G |  2 |  Emissive.B |  2 |         Unk |  3
    /// 3 (+12) |   Sheen Rate |  6 |   Sheen Tint |  7 |  Sheen Apt. |  8 |         Unk |
    /// 4 (+16) |   Rougnhess? |  5 |              |    |  Metalness? |  4 |  Anisotropy |  9
    /// 5 (+20) |          Unk |    |  Sphere Mask | 11 |         Unk |    |         Unk |   
    /// 6 (+24) |   Shader Idx |    |   Tile Index |    |  Tile Alpha |    |  Sphere Idx | 10
    /// 7 (+28) |   Tile XF UU |    |   Tile XF UV |    |  Tile XF VU |    |  Tile XF VV |
    /// </code>
    /// </summary>
    [InlineArray(NumVec4 * Halves)]
    public struct Row : IEquatable<Row>
    {
        public const int NumVec4 = 8;
        public const int Halves  = 4;
        public const int Size    = NumVec4 * Halves * 2;

        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "InlineArray")]
        private Half _element0;

        public static readonly Row LegacyDefault = new(LegacyColorTable.Row.Default);
        public static readonly Row Default       = new()
        {
            DiffuseColor   = HalfColor.White,
            Scalar3        = Half.One,
            SpecularColor  = HalfColor.White,
            Scalar7        = Half.Zero,
            EmissiveColor  = HalfColor.Black,
            Scalar11       = Half.One,
            SheenRate      = (Half)0.1f,
            SheenTintRate  = (Half)0.2f,
            SheenAperture  = (Half)5.0f,
            Scalar15       = Half.Zero,
            Roughness      = (Half)0.5f,
            Scalar17       = Half.Zero,
            Metalness      = Half.Zero,
            Anisotropy     = Half.Zero,
            Scalar20       = Half.Zero,
            SphereMapMask  = Half.Zero,
            Scalar22       = Half.Zero,
            Scalar23       = Half.Zero,
            ShaderId       = 0,
            TileIndex      = 0,
            TileAlpha      = Half.One,
            SphereMapIndex = 0,
            TileTransform  = HalfMatrix2x2.ScaledIdentity((Half)16.0f),
        };

        public HalfColor DiffuseColor
        {
            readonly get => new(this[0], this[1], this[2]);
            set
            {
                this[0] = value.Red;
                this[1] = value.Green;
                this[2] = value.Blue;
            }
        }

        public Half Scalar3
        {
            readonly get => this[3];
            set => this[3] = value;
        }

        public HalfColor SpecularColor
        {
            readonly get => new(this[4], this[5], this[6]);
            set
            {
                this[4] = value.Red;
                this[5] = value.Green;
                this[6] = value.Blue;
            }
        }

        public Half Scalar7
        {
            readonly get => this[7];
            set => this[7] = value;
        }

        public HalfColor EmissiveColor
        {
            readonly get => new(this[8], this[9], this[10]);
            set
            {
                this[8]  = value.Red;
                this[9]  = value.Green;
                this[10] = value.Blue;
            }
        }

        public Half Scalar11
        {
            readonly get => this[11];
            set => this[11] = value;
        }

        public Half SheenRate
        {
            readonly get => this[12];
            set => this[12] = value;
        }

        public Half SheenTintRate
        {
            readonly get => this[13];
            set => this[13] = value;
        }

        public Half SheenAperture
        {
            readonly get => this[14];
            set => this[14] = value;
        }

        public Half Scalar15
        {
            readonly get => this[15];
            set => this[15] = value;
        }

        public Half Roughness
        {
            readonly get => this[16];
            set => this[16] = value;
        }

        public Half Scalar17
        {
            readonly get => this[17];
            set => this[17] = value;
        }

        public Half Metalness
        {
            readonly get => this[18];
            set => this[18] = value;
        }

        public Half Anisotropy
        {
            readonly get => this[19];
            set => this[19] = value;
        }

        public Half Scalar20
        {
            readonly get => this[20];
            set => this[20] = value;
        }

        public Half SphereMapMask
        {
            readonly get => this[21];
            set => this[21] = value;
        }

        public Half Scalar22
        {
            readonly get => this[22];
            set => this[22] = value;
        }

        public Half Scalar23
        {
            readonly get => this[23];
            set => this[23] = value;
        }

        public ushort ShaderId
        {
            readonly get => (ushort)this[24];
            set => this[24] = (Half)value;
        }

        public ushort TileIndex
        {
            readonly get => (ushort)((float)this[25] * 64f);
            set => this[25] = (Half)((value + 0.5f) / 64f);
        }

        public Half TileAlpha
        {
            readonly get => this[26];
            set => this[26] = value;
        }

        public ushort SphereMapIndex
        {
            readonly get => (ushort)this[27];
            set => this[27] = (Half)value;
        }

        public HalfMatrix2x2 TileTransform
        {
            readonly get => new(this[28], this[29], this[30], this[31]);
            set
            {
                this[28] = value.UU;
                this[29] = value.UV;
                this[30] = value.VU;
                this[31] = value.VV;
            }
        }

        public bool ApplyDye(ColorDyeTable.Row dyeRow, LegacyDyePack dyes)
        {
            // This does a legacy interpretation of the new structures.

            var ret = false;

            if (dyeRow.DiffuseColor && DiffuseColor != dyes.DiffuseColor)
            {
                DiffuseColor = dyes.DiffuseColor;
                ret          = true;
            }

            if (dyeRow.SpecularColor && SpecularColor != dyes.SpecularColor && dyes.SpecularColor != HalfColor.Black)
            {
                SpecularColor = dyes.SpecularColor;
                ret           = true;
            }

            if (dyeRow.EmissiveColor && EmissiveColor != dyes.EmissiveColor)
            {
                EmissiveColor = dyes.EmissiveColor;
                ret           = true;
            }

            if (dyeRow.Scalar3 && Scalar3 != dyes.Shininess)
            {
                Scalar3 = dyes.Shininess;
                ret     = true;
            }

            if (dyeRow.Metalness && Scalar7 != dyes.SpecularMask)
            {
                Scalar7 = dyes.SpecularMask;
                ret     = true;
            }

            return ret;
        }

        public bool ApplyDye(ColorDyeTable.Row dyeRow, DyePack dye)
        {
            var ret = false;

            if (dyeRow.DiffuseColor && DiffuseColor != dye.DiffuseColor)
            {
                DiffuseColor = dye.DiffuseColor;
                ret          = true;
            }

            if (dyeRow.SpecularColor && SpecularColor != dye.SpecularColor && dye.SpecularColor != HalfColor.Black)
            {
                SpecularColor = dye.SpecularColor;
                ret           = true;
            }

            if (dyeRow.EmissiveColor && EmissiveColor != dye.EmissiveColor)
            {
                EmissiveColor = dye.EmissiveColor;
                ret           = true;
            }

            if (dyeRow.Scalar3 && Scalar11 != dye.Scalar3)
            {
                Scalar11 = dye.Scalar3;
                ret      = true;
            }

            if (dyeRow.Metalness && Metalness != dye.Metalness)
            {
                Metalness = dye.Metalness;
                ret       = true;
            }

            if (dyeRow.Roughness && Roughness != dye.Roughness)
            {
                Roughness = dye.Roughness;
                ret       = true;
            }

            if (dyeRow.SheenRate && SheenRate != dye.SheenRate)
            {
                SheenRate = dye.SheenRate;
                ret       = true;
            }

            if (dyeRow.SheenTintRate && SheenTintRate != dye.SheenTintRate)
            {
                SheenTintRate = dye.SheenTintRate;
                ret           = true;
            }

            if (dyeRow.SheenAperture && SheenAperture != dye.SheenAperture)
            {
                SheenAperture = dye.SheenAperture;
                ret           = true;
            }

            if (dyeRow.Anisotropy && Anisotropy != dye.Anisotropy)
            {
                Anisotropy = dye.Anisotropy;
                ret        = true;
            }

            if (dyeRow.SphereMapIndex && this[27] != dye.RawSphereMapIndex)
            {
                this[27] = dye.RawSphereMapIndex;
                ret      = true;
            }

            if (dyeRow.SphereMapMask && SphereMapMask != dye.SphereMapMask)
            {
                SphereMapMask = dye.SphereMapMask;
                ret           = true;
            }

            return ret;
        }

        public Row(in LegacyColorTable.Row oldRow)
        {
            DiffuseColor     = oldRow.DiffuseColor;
            Scalar3          = oldRow.Shininess;
            SpecularColor    = oldRow.SpecularColor;
            Scalar7          = oldRow.SpecularMask;
            EmissiveColor    = oldRow.EmissiveColor;
            TileIndex        = oldRow.TileIndex;
            TileAlpha        = Half.One;
            TileTransform    = oldRow.TileTransform;
        }

        public override readonly bool Equals([NotNullWhen(true)] object? obj)
            => obj is Row other && Equals(other);

        public readonly bool Equals(Row other)
            => this[..].SequenceEqual(other[..]);

        public override readonly int GetHashCode()
        {
            var hc = new HashCode();
            hc.AddBytes(MemoryMarshal.AsBytes(new ReadOnlySpan<Row>(in this)));
            return hc.ToHashCode();
        }

        public static bool operator ==(Row row1, Row row2)
            => row1.Equals(row2);

        public static bool operator !=(Row row1, Row row2)
            => !row1.Equals(row2);
    }

    [InlineArray(NumRows)]
    private struct Table
    {
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "InlineArray")]
        private Row _element0;
    }

    public const int NumRows = 32;
    public const int Size    = NumRows * Row.Size;

    public const byte DimensionLogs = 0x53;

    int IColorTable.Width   => Row.NumVec4;
    int IColorTable.RowSize => Row.Size;
    int IColorTable.Height  => NumRows;
    int IColorTable.Size    => Size;
    byte IColorTable.DimensionLogs => DimensionLogs;

    private Table _rowData;

    public ref Row this[int i]
        => ref _rowData[i];

    public IEnumerator<Row> GetEnumerator()
    {
        for (var i = 0; i < NumRows; ++i)
            yield return _rowData[i];
    }

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public Span<byte> AsBytes()
        => MemoryMarshal.AsBytes(_rowData[..]);

    public Span<Half> AsHalves()
        => MemoryMarshal.Cast<Row, Half>(_rowData);

    public Span<byte> RowAsBytes(int i)
        => MemoryMarshal.AsBytes(_rowData[i][..]);

    public Span<Half> RowAsHalves(int i)
        => _rowData[i];

    public Span<Row> AsRows()
        => _rowData;

    public bool SetDefault()
    {
        var ret = false;
        for (var i = 0; i < NumRows; ++i)
            ret |= SetDefaultRow(i);

        return ret;
    }

    public bool SetDefaultRow(int i)
    {
        if (_rowData[i] == Row.Default)
            return false;

        _rowData[i] = Row.Default;
        return true;
    }

    public bool ApplyDye(StmFile<DyePack> stm, ReadOnlySpan<StainId> stainIds, ColorDyeTable dyeTable)
    {
        if (stainIds.Length == 0)
            return false;

        var ret = false;
        for (var rowIdx = 0; rowIdx < ColorDyeTable.NumRows; ++rowIdx)
        {
            var dyeRow  = dyeTable[rowIdx];
            var stainId = dyeRow.Channel < stainIds.Length ? stainIds[dyeRow.Channel] : 0;
            if (stainId != 0 && stm.TryGetValue(dyeRow.Template, stainId, out var dyes))
                ret |= _rowData[rowIdx].ApplyDye(dyeRow, dyes);
        }

        return ret;
    }

    public bool ApplyDye(StmFile<LegacyDyePack> stm, ReadOnlySpan<StainId> stainIds, ColorDyeTable dyeTable)
    {
        if (stainIds.Length == 0)
            return false;

        var ret = false;
        for (var rowIdx = 0; rowIdx < ColorDyeTable.NumRows; ++rowIdx)
        {
            var dyeRow  = dyeTable[rowIdx];
            var stainId = dyeRow.Channel < stainIds.Length ? stainIds[dyeRow.Channel] : 0;
            if (stainId != 0 && stm.TryGetValue(dyeRow.Template, stainId, out var dyes))
                ret |= _rowData[rowIdx].ApplyDye(dyeRow, dyes);
        }

        return ret;
    }

    public bool ApplyDyeToRow(StmFile<DyePack> stm, ReadOnlySpan<StainId> stainIds, int rowIdx, ColorDyeTable.Row dyeRow)
    {
        if (rowIdx < 0 || rowIdx >= NumRows || stainIds.Length == 0)
            return false;

        var ret     = false;
        var stainId = dyeRow.Channel < stainIds.Length ? stainIds[dyeRow.Channel] : 0;
        if (stainId != 0 && stm.TryGetValue(dyeRow.Template, stainId, out var dyes))
            ret |= _rowData[rowIdx].ApplyDye(dyeRow, dyes);

        return ret;
    }

    public bool ApplyDyeToRow(StmFile<LegacyDyePack> stm, ReadOnlySpan<StainId> stainIds, int rowIdx, ColorDyeTable.Row dyeRow)
    {
        if (rowIdx < 0 || rowIdx >= NumRows || stainIds.Length == 0)
            return false;

        var ret     = false;
        var stainId = dyeRow.Channel < stainIds.Length ? stainIds[dyeRow.Channel] : 0;
        if (stainId != 0 && stm.TryGetValue(dyeRow.Template, stainId, out var dyes))
            ret |= _rowData[rowIdx].ApplyDye(dyeRow, dyes);

        return ret;
    }

    public ColorTable()
    {
        SetDefault();
    }

    private ColorTable(ref SpanBinaryReader reader)
    {
        _rowData = reader.Read<Table>();
    }

    public ColorTable(IColorTable other)
    {
        if (other is LegacyColorTable oldTable)
        {
            for (var i = 0; i < LegacyColorTable.NumRows; ++i)
                _rowData[i] = new Row(oldTable[i]);
            for (var i = LegacyColorTable.NumRows; i < NumRows; ++i)
                _rowData[i] = Row.LegacyDefault;
        }
        else if (other is ColorTable table)
        {
            for (var i = 0; i < NumRows; ++i)
                _rowData[i] = table[i];
        }
        else
            SetDefault();
    }

    public static ColorTable CastOrConvert(IColorTable other)
        => other is ColorTable table ? table : new(other);

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (var i = 0; i < NumRows; ++i)
        {
            var row = this[i];
            for (var j = 0; j < Row.NumVec4 * Row.Halves; ++j)
                sb.Append($"{row[j]:F1} ");
            sb[^1] = '\n';
        }

        return sb.ToString();
    }

    /// <summary>
    /// Attempts to read a legacy color table from the given reader.
    /// If the reader doesn't hold enough data, nothing will be read, and this will return a default table.
    /// </summary>
    public static ColorTable TryReadFrom(ref SpanBinaryReader reader)
        => reader.Remaining >= Size ? new(ref reader) : new();
}
