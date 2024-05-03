namespace Penumbra.GameData.Files.MaterialStructs;

// TODO: those values are not correct at all, just taken from legacy for now.
public unsafe struct ColorTable : IEnumerable<ColorTable.Row>
{
    /// <summary>
    /// The number after the parameter is the bit in the dye flags.
    /// <code>
    /// #       |    X (+0)    |    |    Y (+1)    |    |    Z (+2)   |    |   W (+3)    |
    /// --------------------------------------------------------------------------------------
    /// 0 (+ 0) |    Diffuse.R |  0 |    Diffuse.G |  0 |   Diffuse.B |  0 |             |  
    /// 1 (+ 4) |   Specular.R |  1 |   Specular.G |  1 |  Specular.B |  1 |         Unk |
    /// 2 (+ 8) |   Emissive.R |  2 |   Emissive.G |  2 |  Emissive.B |  2 |         Unk |  3
    /// 3 (+12) |          Unk |  6 |          Unk |  7 |         Unk |  8 |             |
    /// 4 (+16) |          Unk |  5 |              |    |         Unk |  4 |         Unk |  9
    /// 5 (+20) |              |    |          Unk | 11 |             |    |             |   
    /// 6 (+24) |   Shader Idx |    |   Tile Index |    |         Unk |    |         Unk | 10
    /// 7 (+28) | Tile Count.X |    | Tile Count.Y |    | Tile Skew.X |    | Tile Skew.Y |
    /// </code>
    /// </summary>
    public struct Row
    {
        public const int NumVec4 = 8;
        public const int Halves  = 4;
        public const int Size    = NumVec4 * Halves * 2;

        private fixed ushort _data[NumVec4 * Halves];

        public static readonly Row Default = new();

        public Vector3 Diffuse
        {
            readonly get => new(ToFloat(0), ToFloat(1), ToFloat(2));
            set
            {
                _data[0] = FromFloat(value.X);
                _data[1] = FromFloat(value.Y);
                _data[2] = FromFloat(value.Z);
            }
        }

        public Vector3 Specular
        {
            readonly get => new(ToFloat(4), ToFloat(5), ToFloat(6));
            set
            {
                _data[4] = FromFloat(value.X);
                _data[5] = FromFloat(value.Y);
                _data[6] = FromFloat(value.Z);
            }
        }

        public Vector3 Emissive
        {
            readonly get => new(ToFloat(8), ToFloat(9), ToFloat(10));
            set
            {
                _data[8]  = FromFloat(value.X);
                _data[9]  = FromFloat(value.Y);
                _data[10] = FromFloat(value.Z);
            }
        }

        public Vector2 MaterialRepeat
        {
            readonly get => new(ToFloat(28), ToFloat(29));
            set
            {
                _data[28] = FromFloat(value.X);
                _data[29] = FromFloat(value.Y);
            }
        }

        public Vector2 MaterialSkew
        {
            readonly get => new(ToFloat(30), ToFloat(31));
            set
            {
                _data[30] = FromFloat(value.X);
                _data[31] = FromFloat(value.Y);
            }
        }

        public float SpecularStrength
        {
            readonly get => ToFloat(3);
            set => _data[3] = FromFloat(value);
        }

        public float GlossStrength
        {
            readonly get => ToFloat(11);
            set => _data[11] = FromFloat(value);
        }

        public ushort TileSet
        {
            readonly get => (ushort)(ToFloat(25) * 64f);
            set => _data[25] = FromFloat((value + 0.5f) / 64f);
        }

        public readonly Span<Half> AsHalves()
        {
            fixed (ushort* ptr = _data)
            {
                return new Span<Half>(ptr, NumVec4 * Halves);
            }
        }

        private readonly float ToFloat(int idx)
            => (float)BitConverter.UInt16BitsToHalf(_data[idx]);

        private static ushort FromFloat(float x)
            => BitConverter.HalfToUInt16Bits((Half)x);

        public bool ApplyDyeTemplate(ColorDyeTable.Row dyeRow, StmFile.DyePack dyes1, StmFile.DyePack dyes2)
        {
            var ret = false;

            if (dyeRow.Diffuse && Diffuse != dyes1.Diffuse)
            {
                Diffuse = dyes1.Diffuse;
                ret     = true;
            }

            if (dyeRow.Specular && Specular != dyes1.Specular)
            {
                Specular = dyes1.Specular;
                ret      = true;
            }

            if (dyeRow.SpecularStrength && SpecularStrength != dyes1.SpecularPower)
            {
                SpecularStrength = dyes1.SpecularPower;
                ret              = true;
            }

            if (dyeRow.Emissive && Emissive != dyes1.Emissive)
            {
                Emissive = dyes1.Emissive;
                ret      = true;
            }

            if (dyeRow.Gloss && GlossStrength != dyes1.Gloss)
            {
                GlossStrength = dyes1.Gloss;
                ret           = true;
            }

            return ret;
        }
    }

    public const  int  NumUsedRows = 16;
    public const  int  NumRows     = 32;
    private fixed byte _rowData[NumRows * Row.Size];

    public ref Row this[int i]
    {
        get
        {
            fixed (byte* ptr = _rowData)
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

    public readonly ReadOnlySpan<byte> AsBytes()
    {
        fixed (byte* ptr = _rowData)
        {
            return new ReadOnlySpan<byte>(ptr, NumRows * Row.Size);
        }
    }

    public readonly Span<Half> AsHalves()
    {
        fixed (byte* ptr = _rowData)
        {
            return new Span<Half>((Half*)ptr, NumRows * 16);
        }
    }

    public void SetDefault()
    {
        for (var i = 0; i < NumRows; ++i)
            this[i] = Row.Default;
    }

    internal ColorTable(in LegacyColorTable oldTable)
    {
        for (var i = 0; i < LegacyColorTable.NumRows; ++i)
        {
            ref readonly var oldRow = ref oldTable[i];
            ref var          row    = ref this[i];
            row.Diffuse          = oldRow.Diffuse;
            row.Specular         = oldRow.Specular;
            row.Emissive         = oldRow.Emissive;
            row.MaterialRepeat   = oldRow.MaterialRepeat;
            row.MaterialSkew     = oldRow.MaterialSkew;
            row.SpecularStrength = oldRow.SpecularStrength;
            row.GlossStrength    = oldRow.GlossStrength;
            row.TileSet          = oldRow.TileSet;
        }
    }
}
