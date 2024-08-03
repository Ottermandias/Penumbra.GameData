using Penumbra.GameData.Files.Utility;

namespace Penumbra.GameData.Files.MaterialStructs;

public sealed class ColorDyeTable : IEnumerable<ColorDyeTable.Row>, IColorDyeTable
{
    /// <inheritdoc cref="ColorTable.Row"/>
    public struct Row : IEquatable<Row>
    {
        public const int  Size = 4;
        private      uint _data;

        public ushort Template
        {
            readonly get => (ushort)((_data >> 16) & 0x7FF);
            set => _data = (_data & ~0x7FF0000u) | ((uint)(value & 0x7FF) << 16);
        }

        public byte Channel
        {
            readonly get => (byte)((_data >> 27) & 0x3);
            set => _data = (_data & ~0x18000000u) | ((uint)(value & 0x3) << 27);
        }

        public bool DiffuseColor
        {
            readonly get => (_data & 0x0001) != 0;
            set => _data = value ? _data | 0x0001u : _data & ~0x0001u;
        }

        public bool SpecularColor
        {
            readonly get => (_data & 0x0002) != 0;
            set => _data = value ? _data | 0x0002u : _data & ~0x0002u;
        }

        public bool EmissiveColor
        {
            readonly get => (_data & 0x0004) != 0;
            set => _data = value ? _data | 0x0004u : _data & ~0x0004u;
        }

        public bool Scalar3
        {
            readonly get => (_data & 0x0008) != 0;
            set => _data = value ? _data | 0x0008u : _data & ~0x0008u;
        }

        public bool Metalness
        {
            readonly get => (_data & 0x0010) != 0;
            set => _data = value ? _data | 0x0010u : _data & ~0x0010u;
        }

        public bool Roughness
        {
            readonly get => (_data & 0x0020) != 0;
            set => _data = value ? _data | 0x0020u : _data & ~0x0020u;
        }

        public bool SheenRate
        {
            readonly get => (_data & 0x0040) != 0;
            set => _data = value ? _data | 0x0040u : _data & ~0x0040u;
        }

        public bool SheenTintRate
        {
            readonly get => (_data & 0x0080) != 0;
            set => _data = value ? _data | 0x0080u : _data & ~0x0080u;
        }

        public bool SheenAperture
        {
            readonly get => (_data & 0x0100) != 0;
            set => _data = value ? _data | 0x0100u : _data & ~0x0100u;
        }

        public bool Anisotropy
        {
            readonly get => (_data & 0x0200) != 0;
            set => _data = value ? _data | 0x0200u : _data & ~0x0200u;
        }

        public bool SphereMapIndex
        {
            readonly get => (_data & 0x0400) != 0;
            set => _data = value ? _data | 0x0400u : _data & ~0x0400u;
        }

        public bool SphereMapMask
        {
            readonly get => (_data & 0x0800) != 0;
            set => _data = value ? _data | 0x0800u : _data & ~0x0200u;
        }

        public Row(in LegacyColorDyeTable.Row oldRow)
        {
            Template      = oldRow.Template;
            DiffuseColor  = oldRow.DiffuseColor;
            SpecularColor = oldRow.SpecularColor;
            EmissiveColor = oldRow.EmissiveColor;
            Scalar3       = oldRow.Shininess;
            Metalness     = oldRow.SpecularMask;
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

    public const int NumRows = 32;
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

    public ColorDyeTable()
    {
        SetDefault();
    }

    private ColorDyeTable(ref SpanBinaryReader reader)
    {
        reader.Read<Row>(NumRows).CopyTo(_rowData);
    }

    public ColorDyeTable(IColorDyeTable other)
    {
        if (other is LegacyColorDyeTable oldTable)
        {
            for (var i = 0; i < LegacyColorDyeTable.NumRows; ++i)
                _rowData[i] = new Row(oldTable[i]);
            for (var i = LegacyColorDyeTable.NumRows; i < NumRows; ++i)
                SetDefaultRow(i);
        }
        else if (other is ColorDyeTable table)
        {
            for (var i = 0; i < NumRows; ++i)
                _rowData[i] = table[i];
        }
        else
            SetDefault();
    }

    public static ColorDyeTable CastOrConvert(IColorDyeTable other)
        => other is ColorDyeTable table ? table : new(other);

    /// <summary>
    /// Attempts to read a color dye table from the given reader.
    /// If the reader doesn't hold enough data, nothing will be read, and this will return a default table.
    /// </summary>
    public static ColorDyeTable TryReadFrom(ref SpanBinaryReader reader)
        => reader.Remaining >= Size ? new(ref reader) : new();
}
