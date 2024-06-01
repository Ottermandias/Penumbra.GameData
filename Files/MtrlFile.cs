using Lumina.Data.Parsing;
using Penumbra.GameData.Files.MaterialStructs;
using Penumbra.GameData.Files.Utility;
using Penumbra.GameData.Structs;

namespace Penumbra.GameData.Files;

public partial class MtrlFile : IWritable, ICloneable
{
    public readonly uint Version;

    public bool Valid
        => CheckTextures();

    public Texture[]         Textures;
    public AttributeSet[]    UvSets;
    public AttributeSet[]    ColorSets;
    public ColorTable        Table;
    public ColorDyeTable     DyeTable;
    public ShaderPackageData ShaderPackage;
    public byte[]            AdditionalData;

    private byte TableFlags
    {
        get => AdditionalData.Length > 0 ? AdditionalData[0] : (byte)0;
        set
        {
            if (AdditionalData.Length == 0)
            {
                if (value == 0)
                    return;

                AdditionalData = new byte[4];
            }

            AdditionalData[0] = value;
        }
    }

    private bool IsDawnTrail
    {
        get => AdditionalData.Length > 1 && AdditionalData[1] == 0x05 && (AdditionalData[0] & 0x30) == 0x30;
        set
        {
            if (AdditionalData.Length == 0)
            {
                if (value == false)
                    return;

                AdditionalData = new byte[4];
            }

            AdditionalData[1] = (byte)(value ? 0x05 : 0);
            AdditionalData[0] = (byte)(value ? AdditionalData[0] | 0x30 : AdditionalData[0] & ~0x30);
        }
    }

    public bool HasTable
    {
        get => (TableFlags & 0x4) != 0;
        set => TableFlags = (byte)(value ? TableFlags | 0x4 : TableFlags & ~0x4);
    }

    public bool HasDyeTable
    {
        get => (TableFlags & 0x8) != 0;
        set => TableFlags = (byte)(value ? TableFlags | 0x8 : TableFlags & ~0x8);
    }

    public bool ApplyDyeTemplate(StmFile stm, int rowIdx, StainId stainId1, StainId stainId2)
    {
        if (!HasDyeTable || rowIdx is < 0 or >= LegacyColorTable.NumRows)
            return false;

        var dyeSet = DyeTable[rowIdx];
        var ret = false;
        if (stainId1 != 0 && stm.TryGetValue(dyeSet.Template, stainId1, out var dyes1))
            ret |= Table[rowIdx].ApplyDyeTemplate1(dyeSet, dyes1);
        if (stainId2 != 0 && stm.TryGetValue(dyeSet.Template, stainId2, out var dyes2))
            ret |= Table[rowIdx].ApplyDyeTemplate2(dyeSet, dyes2);

        return ret;
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
    { }

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

        Textures  = ReadTextureOffsets(ref r, textureCount, out var textureOffsets);
        UvSets    = ReadAttributeSetOffsets(ref r, uvSetCount,    out var uvOffsets);
        ColorSets = ReadAttributeSetOffsets(ref r, colorSetCount, out var colorOffsets);

        var strings = r.SliceFromHere(stringTableSize);
        for (var i = 0; i < textureCount; ++i)
            Textures[i].Path = strings.ReadString(textureOffsets[i]);

        for (var i = 0; i < uvSetCount; ++i)
            UvSets[i].Name = strings.ReadString(uvOffsets[i]);

        for (var i = 0; i < colorSetCount; ++i)
            ColorSets[i].Name = strings.ReadString(colorOffsets[i]);

        ShaderPackage.Name = strings.ReadString(shaderPackageNameOffset);

        AdditionalData = r.Read<byte>(additionalDataSize).ToArray();

        if (IsDawnTrail)
        {
            var dataSet = r.SliceFromHere(dataSetSize);
            if (HasTable && dataSet.Remaining >= ColorTable.NumRows * ColorTable.Row.Size)
                Table = dataSet.Read<ColorTable>();
            else
                Table.SetDefault();
            if (HasDyeTable && dataSet.Remaining >= ColorDyeTable.NumRows * ColorDyeTable.Row.Size)
                DyeTable = dataSet.Read<ColorDyeTable>();
        }
        else
        {
            var dataSet = r.SliceFromHere(dataSetSize);
            if (HasTable && dataSet.Remaining >= LegacyColorTable.NumRows * LegacyColorTable.Row.Size)
            {
                var table = dataSet.Read<LegacyColorTable>();
                Table = new ColorTable(table);
            }
            else
            {
                Table.SetDefault();
            }

            if (HasDyeTable && dataSet.Remaining >= LegacyColorDyeTable.NumRows * LegacyColorDyeTable.Row.Size)
            {
                var dyeTable = dataSet.Read<LegacyColorDyeTable>();
                DyeTable = new ColorDyeTable(dyeTable);
            }
        }

        var shaderValueListSize = r.ReadUInt16();
        var shaderKeyCount      = r.ReadUInt16();
        var constantCount       = r.ReadUInt16();
        var samplerCount        = r.ReadUInt16();
        ShaderPackage.Flags = r.ReadUInt32();

        ShaderPackage.ShaderKeys   = r.Read<ShaderKey>(shaderKeyCount).ToArray();
        ShaderPackage.Constants    = r.Read<Constant>(constantCount).ToArray();
        ShaderPackage.Samplers     = r.Read<Sampler>(samplerCount).ToArray();
        ShaderPackage.ShaderValues = r.Read<float>(shaderValueListSize / 4).ToArray();
    }

    private MtrlFile(MtrlFile original)
    {
        Version = original.Version;

        Textures       = (Texture[])original.Textures.Clone();
        UvSets         = (AttributeSet[])original.UvSets.Clone();
        ColorSets      = (AttributeSet[])original.ColorSets.Clone();
        Table          = original.Table;
        DyeTable       = original.DyeTable;
        ShaderPackage  = original.ShaderPackage.Clone();
        AdditionalData = (byte[])original.AdditionalData.Clone();
    }

    public MtrlFile Clone()
        => new(this);

    object ICloneable.Clone()
        => new MtrlFile(this);

    private static Texture[] ReadTextureOffsets(ref SpanBinaryReader r, int count, out ushort[] offsets)
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

    private static AttributeSet[] ReadAttributeSetOffsets(ref SpanBinaryReader r, int count, out ushort[] offsets)
    {
        var ret = new AttributeSet[count];
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

    public struct AttributeSet
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
