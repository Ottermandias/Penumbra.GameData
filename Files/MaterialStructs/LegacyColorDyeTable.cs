using Penumbra.GameData.Files.Utility;

namespace Penumbra.GameData.Files.MaterialStructs;

public unsafe sealed class LegacyColorDyeTable : IEnumerable<LegacyColorDyeTable.Row>, IColorDyeTable
{
    public struct Row : IEquatable<Row>
    {
        public const int Size = 2;

        private ushort _data;

        public ushort Template
        {
            readonly get => (ushort)(_data >> 5);
            set => _data = (ushort)((_data & 0x1F) | (value << 5));
        }

        public bool DiffuseColor
        {
            readonly get => (_data & 0x01) != 0;
            set => _data = (ushort)(value ? _data | 0x01 : _data & 0xFFFE);
        }

        public bool SpecularColor
        {
            readonly get => (_data & 0x02) != 0;
            set => _data = (ushort)(value ? _data | 0x02 : _data & 0xFFFD);
        }

        public bool EmissiveColor
        {
            readonly get => (_data & 0x04) != 0;
            set => _data = (ushort)(value ? _data | 0x04 : _data & 0xFFFB);
        }

        public bool Shininess
        {
            readonly get => (_data & 0x08) != 0;
            set => _data = (ushort)(value ? _data | 0x08 : _data & 0xFFF7);
        }

        public bool SpecularMask
        {
            readonly get => (_data & 0x10) != 0;
            set => _data = (ushort)(value ? _data | 0x10 : _data & 0xFFEF);
        }

        public Row(in ColorDyeTable.Row row)
        {
            Template      = row.Template;
            DiffuseColor  = row.DiffuseColor;
            SpecularColor = row.SpecularColor;
            EmissiveColor = row.EmissiveColor;
            Shininess     = row.Scalar3;
            SpecularMask  = row.Metalness;
        }

        public override readonly bool Equals([NotNullWhen(true)] object? obj)
            => obj is Row other && Equals(other);

        public readonly bool Equals(Row other)
            => _data == other._data;

        public override readonly int GetHashCode()
            => _data.GetHashCode();

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

    int IColorDyeTable.RowSize => Row.Size;
    int IColorDyeTable.Height  => NumRows;
    int IColorDyeTable.Size    => Size;

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

    public Span<byte> RowAsBytes(int i)
        => MemoryMarshal.AsBytes(new Span<Row>(ref _rowData[i]));

    public bool SetDefault()
    {
        var ret = false;
        for (var i = 0; i < NumRows; ++i)
            ret |= SetDefaultRow(i);

        return ret;
    }

    public bool SetDefaultRow(int i)
    {
        if (_rowData[i] == default)
            return false;

        _rowData[i] = default;
        return true;
    }

    public LegacyColorDyeTable()
    {
        SetDefault();
    }

    private LegacyColorDyeTable(ref SpanBinaryReader reader)
    {
        reader.Read<Row>(NumRows).CopyTo(_rowData);
    }

    public LegacyColorDyeTable(IColorDyeTable other)
    {
        if (other is ColorDyeTable newTable)
        {
            for (var i = 0; i < NumRows; ++i)
                _rowData[i] = new Row(newTable[i]);
        }
        else if (other is LegacyColorDyeTable table)
        {
            for (var i = 0; i < NumRows; ++i)
                _rowData[i] = table[i];
        }
        else
            SetDefault();
    }

    /// <summary>
    /// Attempts to read a legacy color dye table from the given reader.
    /// If the reader doesn't hold enough data, nothing will be read, and this will return a default table.
    /// </summary>
    public static LegacyColorDyeTable TryReadFrom(ref SpanBinaryReader reader)
        => reader.Remaining >= Size ? new(ref reader) : new();
}
