using Penumbra.GameData.Files.StainMapStructs;
using Penumbra.GameData.Files.Utility;
using Penumbra.GameData.Structs;

namespace Penumbra.GameData.Files.MaterialStructs;

public sealed class LegacyColorTable : IEnumerable<LegacyColorTable.Row>, IColorTable
{
    /// <summary>
    /// The number after the parameter is the bit in the dye flags.
    /// <code>
    /// #       |    X (+0)    |    |    Y (+1)    |    |    Z (+2)   |    |   W (+3)    |
    /// --------------------------------------------------------------------------------------
    /// 0 (+ 0) |    Diffuse.R |  0 |    Diffuse.G |  0 |   Diffuse.B |  0 | SpecularStr |  3
    /// 1 (+ 4) |   Specular.R |  1 |   Specular.G |  1 |  Specular.B |  1 |       Gloss |  4
    /// 2 (+ 8) |   Emissive.R |  2 |   Emissive.G |  2 |  Emissive.B |  2 |  Tile Index |
    /// 7 (+28) |   Tile XF UU |    |   Tile XF UV |    |  Tile XF VU |    |  Tile XF VV |
    /// </code>
    /// </summary>
    [InlineArray(NumVec4 * Halves)]
    public struct Row : IEquatable<Row>
    {
        public const int NumVec4 = 4;
        public const int Halves  = 4;
        public const int Size    = NumVec4 * Halves * 2;

        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "InlineArray")]
        private Half _element0;

        public static readonly Row Default = new()
        {
            DiffuseColor  = HalfColor.White,
            SpecularMask  = Half.One,
            SpecularColor = HalfColor.White,
            Shininess     = (Half)20.0f,
            EmissiveColor = HalfColor.Black,
            TileIndex     = 0,
            TileTransform = HalfMatrix2x2.ScaledIdentity((Half)16.0f),
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

        public Half SpecularMask
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

        public Half Shininess
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

        public ushort TileIndex
        {
            readonly get => (ushort)((float)this[11] * 64f);
            set => this[11] = (Half)((value + 0.5f) / 64f);
        }

        public HalfMatrix2x2 TileTransform
        {
            readonly get => new(this[12], this[13], this[14], this[15]);
            set
            {
                this[12] = value.UU;
                this[13] = value.UV;
                this[14] = value.VV;
                this[15] = value.VV;
            }
        }

        public bool ApplyDye(LegacyColorDyeTable.Row dyeRow, LegacyDyePack dyes)
        {
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

            if (dyeRow.SpecularMask && SpecularMask != dyes.SpecularMask)
            {
                SpecularMask = dyes.SpecularMask;
                ret          = true;
            }

            if (dyeRow.Shininess && Shininess != dyes.Shininess)
            {
                Shininess = dyes.Shininess;
                ret       = true;
            }

            return ret;
        }

        public Row(in ColorTable.Row row)
        {
            DiffuseColor  = row.DiffuseColor;
            SpecularMask  = row.Scalar7;
            SpecularColor = row.SpecularColor;
            Shininess     = row.Scalar3;
            EmissiveColor = row.EmissiveColor;
            TileIndex     = row.TileIndex;
            TileTransform = row.TileTransform;
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

    public const int NumRows = 16;
    public const int Size    = NumRows * Row.Size;

    int IColorTable.Width   => Row.NumVec4;
    int IColorTable.RowSize => Row.Size;
    int IColorTable.Height  => NumRows;
    int IColorTable.Size    => Size;
    byte IColorTable.DimensionLogs => 0;

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

    public bool ApplyDye(StmFile<LegacyDyePack> stm, ReadOnlySpan<StainId> stainIds, LegacyColorDyeTable dyeTable)
    {
        if (stainIds.Length == 0)
            return false;

        var ret = false;
        for (var rowIdx = 0; rowIdx < LegacyColorDyeTable.NumRows; ++rowIdx)
        {
            var dyeRow = dyeTable[rowIdx];
            if (stainIds[0] != 0 && stm.TryGetValue(dyeRow.Template, stainIds[0], out var dyes))
                ret |= _rowData[rowIdx].ApplyDye(dyeRow, dyes);
        }

        return ret;
    }

    public bool ApplyDyeToRow(StmFile<LegacyDyePack> stm, ReadOnlySpan<StainId> stainIds, int rowIdx, LegacyColorDyeTable.Row dyeRow)
    {
        if (rowIdx < 0 || rowIdx >= NumRows || stainIds.Length == 0)
            return false;

        var ret = false;
        if (stainIds[0] != 0 && stm.TryGetValue(dyeRow.Template, stainIds[0], out var dyes))
            ret |= _rowData[rowIdx].ApplyDye(dyeRow, dyes);

        return ret;
    }

    public LegacyColorTable()
    {
        SetDefault();
    }

    private LegacyColorTable(ref SpanBinaryReader reader)
    {
        _rowData = reader.Read<Table>();
    }

    public LegacyColorTable(ColorTable newTable)
    {
        for (var i = 0; i < NumRows; ++i)
            this[i] = new Row(newTable[i]);
    }

    public LegacyColorTable(IColorTable other)
    {
        if (other is ColorTable newTable)
        {
            for (var i = 0; i < NumRows; ++i)
                _rowData[i] = new Row(newTable[i]);
        }
        else if (other is LegacyColorTable table)
        {
            for (var i = 0; i < NumRows; ++i)
                _rowData[i] = table[i];
        }
        else
            SetDefault();
    }

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
    public static LegacyColorTable TryReadFrom(ref SpanBinaryReader reader)
        => reader.Remaining >= Size ? new(ref reader) : new();
}
