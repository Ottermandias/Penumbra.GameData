using System;
using System.Linq;
using Lumina.Data.Parsing;
using Penumbra.GameData.Files.Utility;
using Penumbra.GameData.Structs;

namespace Penumbra.GameData.Files;

public partial class MtrlFile : IWritable, ICloneable
{
    public readonly uint Version;

    public bool Valid
        => CheckTextures();

    public Texture[]         Textures;
    public UvSet[]           UvSets;
    public ColorSet[]        ColorSets;
    public ColorDyeSet[]     ColorDyeSets;
    public ShaderPackageData ShaderPackage;
    public byte[]            AdditionalData;

    public bool ApplyDyeTemplate(StmFile stm, int colorSetIdx, int rowIdx, StainId stainId)
    {
        if (colorSetIdx < 0 || colorSetIdx >= ColorDyeSets.Length || rowIdx is < 0 or >= ColorSet.RowArray.NumRows)
            return false;

        var dyeSet = ColorDyeSets[colorSetIdx].Rows[rowIdx];
        if (!stm.TryGetValue(dyeSet.Template, stainId, out var dyes))
            return false;

        return ColorSets[colorSetIdx].Rows[rowIdx].ApplyDyeTemplate(dyeSet, dyes);
    }

    public Span<float> GetConstantValues(Constant constant)
    {
        if ((constant.ByteOffset & 0x3) != 0
         || (constant.ByteSize & 0x3) != 0
         || (constant.ByteOffset + constant.ByteSize) >> 2 > ShaderPackage.ShaderValues.Length)
            return null;

        return ShaderPackage.ShaderValues.AsSpan().Slice(constant.ByteOffset >> 2, constant.ByteSize >> 2);
    }

    public (Sampler? MtrlSampler, ShpkFile.Resource? ShpkSampler)[] GetSamplersByTexture(ShpkFile? shpk)
    {
        var samplers = Array.ConvertAll<Texture, (Sampler?, ShpkFile.Resource?)>(Textures, _ => (null, null));
        foreach (var sampler in ShaderPackage.Samplers)
            samplers[sampler.TextureIndex] = (sampler, shpk?.GetSamplerById(sampler.SamplerId));

        return samplers;
    }

    public MtrlFile(byte[] data)
        : this((ReadOnlySpan<byte>)data)
    {
    }

    public MtrlFile(ReadOnlySpan<byte> data)
    {
        var r = new SpanBinaryReader(data);

        Version = r.ReadUInt32();
        r.ReadUInt16(); // file size
        var dataSetSize             = r.ReadUInt16();
        var stringTableSize         = r.ReadUInt16();
        var shaderPackageNameOffset = r.ReadUInt16();
        var textureCount            = r.ReadByte();
        var uvSetCount              = r.ReadByte();
        var colorSetCount           = r.ReadByte();
        var additionalDataSize      = r.ReadByte();

        Textures  = ReadTextureOffsets(r, textureCount, out var textureOffsets);
        UvSets    = ReadUvSetOffsets(r, uvSetCount, out var uvOffsets);
        ColorSets = ReadColorSetOffsets(r, colorSetCount, out var colorOffsets);

        var strings = r.ReadStringPool(stringTableSize);
        for (var i = 0; i < textureCount; ++i)
            Textures[i].Path = strings.GetNullTerminatedString(textureOffsets[i]);

        for (var i = 0; i < uvSetCount; ++i)
            UvSets[i].Name = strings.GetNullTerminatedString(uvOffsets[i]);

        for (var i = 0; i < colorSetCount; ++i)
            ColorSets[i].Name = strings.GetNullTerminatedString(colorOffsets[i]);

        ShaderPackage.Name = strings.GetNullTerminatedString(shaderPackageNameOffset);

        AdditionalData = r.ReadBytes(additionalDataSize).ToArray();
        var colorSetFlags = AdditionalData.Length > 0 ? AdditionalData[0] : (byte)0;

        ColorDyeSets = Array.Empty<ColorDyeSet>();
        if ((colorSetFlags & 0x08) != 0 && ColorSets.Length * ColorSet.RowArray.NumRows * ColorSet.Row.Size < dataSetSize)
            FindOrAddColorDyeSet();

        {
            var dataSet = r.ReadSlice(dataSetSize);
            for (var i = 0; i < ColorSets.Length; ++i)
            {
                if ((colorSetFlags & 0x04) != 0 && dataSet.Remaining >= ColorSet.RowArray.NumRows * ColorSet.Row.Size)
                {
                    ColorSets[i].Rows = dataSet.ReadStructure<ColorSet.RowArray>();
                    ColorSets[i].HasRows = true;
                }
                else
                {
                    ColorSets[i].HasRows = false;
                }
            }

            for (var i = 0; i < ColorDyeSets.Length; ++i)
                ColorDyeSets[i].Rows = dataSet.ReadStructure<ColorDyeSet.RowArray>();
        }

        var shaderValueListSize = r.ReadUInt16();
        var shaderKeyCount      = r.ReadUInt16();
        var constantCount       = r.ReadUInt16();
        var samplerCount        = r.ReadUInt16();
        ShaderPackage.Flags = r.ReadUInt32();

        ShaderPackage.ShaderKeys   = r.ReadStructures<ShaderKey>(shaderKeyCount).ToArray();
        ShaderPackage.Constants    = r.ReadStructures<Constant>(constantCount).ToArray();
        ShaderPackage.Samplers     = r.ReadStructures<Sampler>(samplerCount).ToArray();
        ShaderPackage.ShaderValues = r.ReadStructures<float>(shaderValueListSize / 4).ToArray();
    }

    private MtrlFile(MtrlFile original)
    {
        Version = original.Version;

        Textures       = (Texture[])original.Textures.Clone();
        UvSets         = (UvSet[])original.UvSets.Clone();
        ColorSets      = (ColorSet[])original.ColorSets.Clone();
        ColorDyeSets   = (ColorDyeSet[])original.ColorDyeSets.Clone();
        ShaderPackage  = original.ShaderPackage.Clone();
        AdditionalData = (byte[])original.AdditionalData.Clone();
    }

    public MtrlFile Clone()
        => new(this);

    object ICloneable.Clone()
        => new MtrlFile(this);

    private static Texture[] ReadTextureOffsets(SpanBinaryReader r, int count, out ushort[] offsets)
    {
        var ret = new Texture[count];
        offsets = new ushort[count];
        for (var i = 0; i < count; ++i)
        {
            offsets[i]   = r.ReadUInt16();
            ret[i].Flags = r.ReadUInt16();
        }

        return ret;
    }

    private static UvSet[] ReadUvSetOffsets(SpanBinaryReader r, int count, out ushort[] offsets)
    {
        var ret = new UvSet[count];
        offsets = new ushort[count];
        for (var i = 0; i < count; ++i)
        {
            offsets[i]   = r.ReadUInt16();
            ret[i].Index = r.ReadUInt16();
        }

        return ret;
    }

    private static ColorSet[] ReadColorSetOffsets(SpanBinaryReader r, int count, out ushort[] offsets)
    {
        var ret = new ColorSet[count];
        offsets = new ushort[count];
        for (var i = 0; i < count; ++i)
        {
            offsets[i]   = r.ReadUInt16();
            ret[i].Index = r.ReadUInt16();
        }

        return ret;
    }

    private bool CheckTextures()
        => Textures.All(texture => texture.Path.Contains('/'));

    public struct UvSet
    {
        public string Name;
        public ushort Index;
    }

    public struct Texture
    {
        public const ushort DX11Flag = 0x8000;

        public string Path;
        public ushort Flags;

        public bool DX11
        {
            get => (Flags & DX11Flag) != 0;
            set => Flags = value ? (ushort)(Flags | DX11Flag) : (ushort)(Flags & ~DX11Flag);
        }
    }

    public struct Constant
    {
        public uint   Id;
        public ushort ByteOffset;
        public ushort ByteSize;
    }

    public struct ShaderPackageData : ICloneable
    {
        public string      Name;
        public ShaderKey[] ShaderKeys;
        public Constant[]  Constants;
        public Sampler[]   Samplers;
        public float[]     ShaderValues;
        public uint        Flags;

        public ShaderPackageData Clone()
            => new()
            {
                Name         = Name,
                ShaderKeys   = (ShaderKey[])ShaderKeys.Clone(),
                Constants    = (Constant[])Constants.Clone(),
                Samplers     = (Sampler[])Samplers.Clone(),
                ShaderValues = (float[])ShaderValues.Clone(),
                Flags        = Flags,
            };

        object ICloneable.Clone()
            => Clone();
    }
}
