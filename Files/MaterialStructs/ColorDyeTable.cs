namespace Penumbra.GameData.Files.MaterialStructs;

// TODO those values are not correct at all, just taken from the old values for now.
public unsafe struct ColorDyeTable : IEnumerable<ColorDyeTable.Row>
{
    /// <inheritdoc cref="ColorTable.Row"/>
    public struct Row
    {
        public const int  Size = 4;
        private      uint _data;

        public ushort Template
        {
            get => (ushort)(_data >> 10);
            set => _data = (_data & 0x03FFu) | ((uint)value << 10);
        }

        public bool Diffuse
        {
            get => (_data & 0x0001) != 0;
            set => _data = value ? _data | 0x0001u : _data & ~0x0001u;
        }

        public bool Specular
        {
            get => (_data & 0x0002) != 0;
            set => _data = value ? _data | 0x0002u : _data & ~0x0002u;
        }

        public bool Emissive
        {
            get => (_data & 0x0004) != 0;
            set => _data = value ? _data | 0x0004u : _data & ~0x0004u;
        }

        public bool Gloss
        {
            get => (_data & 0x0008) != 0;
            set => _data = value ? _data | 0x0008u : _data & ~0x0008u;
        }

        public bool SpecularStrength
        {
            get => (_data & 0x0010) != 0;
            set => _data = value ? _data | 0x0010u : _data & ~0x0010u;
        }

        public bool Unk1
        {
            get => (_data & 0x0020) != 0;
            set => _data = value ? _data | 0x0020u : _data & ~0x0020u;
        }

        public bool Unk2
        {
            get => (_data & 0x0040) != 0;
            set => _data = value ? _data | 0x0040u : _data & ~0x0040u;
        }

        public bool Unk3
        {
            get => (_data & 0x0080) != 0;
            set => _data = value ? _data | 0x0080u : _data & ~0x0080u;
        }

        public bool Unk4
        {
            get => (_data & 0x0100) != 0;
            set => _data = value ? _data | 0x0100u : _data & ~0x0100u;
        }

        public bool Unk5
        {
            get => (_data & 0x0200) != 0;
            set => _data = value ? _data | 0x0200u : _data & ~0x0200u;
        }
    }

    public const  int  NumRows = 32;
    private fixed uint _rowData[NumRows];

    public ref Row this[int i]
    {
        get
        {
            fixed (uint* ptr = _rowData)
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
        fixed (uint* ptr = _rowData)
        {
            return new ReadOnlySpan<byte>(ptr, NumRows * sizeof(ushort));
        }
    }

    internal ColorDyeTable(in LegacyColorDyeTable oldTable)
    {
        for (var i = 0; i < LegacyColorDyeTable.NumRows; ++i)
        {
            var     oldRow = oldTable[i];
            ref var row    = ref this[i];
            row.Template         = oldRow.Template;
            row.Diffuse          = oldRow.Diffuse;
            row.Specular         = oldRow.Specular;
            row.Emissive         = oldRow.Emissive;
            row.Gloss            = oldRow.Gloss;
            row.SpecularStrength = oldRow.SpecularStrength;
        }
    }
}
