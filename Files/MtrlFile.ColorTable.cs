using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace Penumbra.GameData.Files;

public partial class MtrlFile
{
    public unsafe struct ColorTable : IEnumerable<ColorTable.Row>
    {
        public struct Row
        {
            public const int Size = 32;

            private fixed ushort _data[16];

            public static readonly Row Default = new()
            {
                Diffuse          = Vector3.One,
                Specular         = Vector3.One,
                SpecularStrength = 1.0f,
                Emissive         = Vector3.Zero,
                GlossStrength    = 20.0f,
                TileSet          = 0,
                MaterialRepeat   = new Vector2(16.0f),
                MaterialSkew     = Vector2.Zero,
            };

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
                readonly get => new(ToFloat(12), ToFloat(15));
                set
                {
                    _data[12] = FromFloat(value.X);
                    _data[15] = FromFloat(value.Y);
                }
            }

            public Vector2 MaterialSkew
            {
                readonly get => new(ToFloat(13), ToFloat(14));
                set
                {
                    _data[13] = FromFloat(value.X);
                    _data[14] = FromFloat(value.Y);
                }
            }

            public float SpecularStrength
            {
                readonly get => ToFloat(3);
                set => _data[3] = FromFloat(value);
            }

            public float GlossStrength
            {
                readonly get => ToFloat(7);
                set => _data[7] = FromFloat(value);
            }

            public ushort TileSet
            {
                readonly get => (ushort)(ToFloat(11) * 64f);
                set => _data[11] = FromFloat((value + 0.5f) / 64f);
            }

            public readonly Span<Half> AsHalves()
            {
                fixed (ushort* ptr = _data)
                {
                    return new Span<Half>(ptr, 16);
                }
            }

            public bool ApplyDyeTemplate(ColorDyeTable.Row dyeRow, StmFile.DyePack dyes)
            {
                var ret = false;

                if (dyeRow.Diffuse && Diffuse != dyes.Diffuse)
                {
                    Diffuse = dyes.Diffuse;
                    ret     = true;
                }

                if (dyeRow.Specular && Specular != dyes.Specular)
                {
                    Specular = dyes.Specular;
                    ret      = true;
                }

                if (dyeRow.SpecularStrength && SpecularStrength != dyes.SpecularPower)
                {
                    SpecularStrength = dyes.SpecularPower;
                    ret              = true;
                }

                if (dyeRow.Emissive && Emissive != dyes.Emissive)
                {
                    Emissive = dyes.Emissive;
                    ret      = true;
                }

                if (dyeRow.Gloss && GlossStrength != dyes.Gloss)
                {
                    GlossStrength = dyes.Gloss;
                    ret           = true;
                }

                return ret;
            }

            private readonly float ToFloat(int idx)
                => (float)BitConverter.UInt16BitsToHalf(_data[idx]);

            private static ushort FromFloat(float x)
                => BitConverter.HalfToUInt16Bits((Half)x);

        }

        public const  int  NumRows = 16;
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
    }
}
