namespace Penumbra.GameData.Files.MaterialStructs;

public unsafe struct LegacyColorDyeTable : IEnumerable<LegacyColorDyeTable.Row>
{
    public struct Row
    {
        public const int Size = 2;

        private ushort _data;

        public ushort Template
        {
            get => (ushort)(_data >> 5);
            set => _data = (ushort)((_data & 0x1F) | (value << 5));
        }

        public bool Diffuse
        {
            get => (_data & 0x01) != 0;
            set => _data = (ushort)(value ? _data | 0x01 : _data & 0xFFFE);
        }

        public bool Specular
        {
            get => (_data & 0x02) != 0;
            set => _data = (ushort)(value ? _data | 0x02 : _data & 0xFFFD);
        }

        public bool Emissive
        {
            get => (_data & 0x04) != 0;
            set => _data = (ushort)(value ? _data | 0x04 : _data & 0xFFFB);
        }

        public bool Gloss
        {
            get => (_data & 0x08) != 0;
            set => _data = (ushort)(value ? _data | 0x08 : _data & 0xFFF7);
        }

        public bool SpecularStrength
        {
            get => (_data & 0x10) != 0;
            set => _data = (ushort)(value ? _data | 0x10 : _data & 0xFFEF);
        }

        public Row(in ColorDyeTable.Row row)
        {
            Template         = row.Template;
            Diffuse          = row.Diffuse;
            Specular         = row.Specular;
            Emissive         = row.Emissive;
            Gloss            = row.Gloss;
            SpecularStrength = row.SpecularStrength;
        }
    }

    public const  int    NumRows     = 16;
    public const  int    NumUsedRows = 16;
    private fixed ushort _rowData[NumRows];

    public ref Row this[int i]
    {
        get
        {
            fixed (ushort* ptr = _rowData)
            {
                return ref ((Row*)ptr)[i];
            }
        }
    }

    public IEnumerator<Row> GetEnumerator()
    {
        for (var i = 0; i < NumRows; ++i)
            yield return this[i];
    }

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public ReadOnlySpan<byte> AsBytes()
    {
        fixed (ushort* ptr = _rowData)
        {
            return new ReadOnlySpan<byte>(ptr, NumRows * sizeof(ushort));
        }
    }

    public LegacyColorDyeTable(in ColorDyeTable newTable)
    {
        for (var i = 0; i < NumRows; ++i)
            this[i] = new Row(newTable[i]);
    }
}
