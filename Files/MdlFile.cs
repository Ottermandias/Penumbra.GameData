using Lumina.Data;
using Lumina.Data.Parsing;
using Lumina.Extensions;

// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Penumbra.GameData.Files;

public partial class MdlFile : IWritable
{
    public const uint NumVertices    = 17;
    public const uint FileHeaderSize = 0x44;

    // Refers to string, thus not Lumina struct.
    public struct Shape
    {
        public string   ShapeName = string.Empty;
        public ushort[] ShapeMeshStartIndex;
        public ushort[] ShapeMeshCount;

        public Shape(MdlStructs.ShapeStruct data, uint[] offsets, string[] strings)
        {
            var idx = offsets.AsSpan().IndexOf(data.StringOffset);
            ShapeName           = idx >= 0 ? strings[idx] : string.Empty;
            ShapeMeshStartIndex = data.ShapeMeshStartIndex;
            ShapeMeshCount      = data.ShapeMeshCount;
        }
    }

    // Raw data to write back.
    public uint   Version = 16777221;
    public float  Radius;
    public float  ModelClipOutDistance;
    public float  ShadowClipOutDistance;
    public byte   BgChangeMaterialIndex;
    public byte   BgCrestChangeMaterialIndex;
    public ushort Unknown4;
    public byte   Unknown5;
    public byte   Unknown6;
    public ushort Unknown7;
    public ushort Unknown8;
    public ushort Unknown9;

    // Offsets are stored relative to RuntimeSize instead of file start.
    public uint[] VertexOffset = [0, 0, 0];
    public uint[] IndexOffset  = [0, 0, 0];

    public uint[] VertexBufferSize = [0, 0, 0];
    public uint[] IndexBufferSize  = [0, 0, 0];
    public byte   LodCount;
    public bool   EnableIndexBufferStreaming;
    public bool   EnableEdgeGeometry;

    public MdlStructs.ModelFlags1 Flags1;
    public MdlStructs.ModelFlags2 Flags2;

    public MdlStructs.BoundingBoxStruct BoundingBoxes            = EmptyBoundingBox;
    public MdlStructs.BoundingBoxStruct ModelBoundingBoxes       = EmptyBoundingBox;
    public MdlStructs.BoundingBoxStruct WaterBoundingBoxes       = EmptyBoundingBox;
    public MdlStructs.BoundingBoxStruct VerticalFogBoundingBoxes = EmptyBoundingBox;

    public MdlStructs.VertexDeclarationStruct[]    VertexDeclarations     = [];
    public MdlStructs.ElementIdStruct[]            ElementIds             = [];
    public MeshStruct[]                            Meshes                 = [];
    public MdlStructs.BoneTableStruct[]            BoneTables             = [];
    public MdlStructs.BoundingBoxStruct[]          BoneBoundingBoxes      = [];
    public MdlStructs.SubmeshStruct[]              SubMeshes              = [];
    public MdlStructs.ShapeMeshStruct[]            ShapeMeshes            = [];
    public MdlStructs.ShapeValueStruct[]           ShapeValues            = [];
    public MdlStructs.TerrainShadowMeshStruct[]    TerrainShadowMeshes    = [];
    public MdlStructs.TerrainShadowSubmeshStruct[] TerrainShadowSubMeshes = [];
    public MdlStructs.LodStruct[]                  Lods                   = [];
    public MdlStructs.ExtraLodStruct[]             ExtraLods              = [];
    public ushort[]                                SubMeshBoneMap         = [];

    // Strings are written in order
    public string[] Attributes = [];
    public string[] Bones      = [];
    public string[] Materials  = [];
    public Shape[]  Shapes     = [];

    // Raw, unparsed data.
    public byte[] RemainingData = [];

    public bool Valid { get; }

    public MdlFile()
    { }

    public MdlFile(byte[] data)
    {
        using var stream = new MemoryStream(data);
        using var r      = new LuminaBinaryReader(stream);

        var header = LoadModelFileHeader(r);
        LodCount         = header.LodCount;
        VertexBufferSize = header.VertexBufferSize;
        IndexBufferSize  = header.IndexBufferSize;
        VertexOffset     = header.VertexOffset;
        IndexOffset      = header.IndexOffset;

        var dataOffset = FileHeaderSize + header.RuntimeSize + header.StackSize;
        for (var i = 0; i < LodCount; ++i)
        {
            VertexOffset[i] -= dataOffset;
            IndexOffset[i]  -= dataOffset;
        }

        VertexDeclarations = new MdlStructs.VertexDeclarationStruct[header.VertexDeclarationCount];
        for (var i = 0; i < header.VertexDeclarationCount; ++i)
            VertexDeclarations[i] = MdlStructs.VertexDeclarationStruct.Read(r);

        var (offsets, strings) = LoadStrings(r);

        var modelHeader = LoadModelHeader(r);
        ElementIds = new MdlStructs.ElementIdStruct[modelHeader.ElementIdCount];
        for (var i = 0; i < modelHeader.ElementIdCount; i++)
            ElementIds[i] = MdlStructs.ElementIdStruct.Read(r);

        Lods = new MdlStructs.LodStruct[3];
        for (var i = 0; i < 3; i++)
        {
            var lod = r.ReadStructure<MdlStructs.LodStruct>();
            if (i < LodCount)
            {
                lod.VertexDataOffset -= dataOffset;
                lod.IndexDataOffset  -= dataOffset;
            }

            Lods[i] = lod;
        }

        ExtraLods = modelHeader.ExtraLodEnabled
            ? r.ReadStructuresAsArray<MdlStructs.ExtraLodStruct>(3)
            : [];

        Meshes = new MeshStruct[modelHeader.MeshCount];
        for (var i = 0; i < modelHeader.MeshCount; i++)
            Meshes[i] = MeshStruct.Read(r);

        Attributes = new string[modelHeader.AttributeCount];
        for (var i = 0; i < modelHeader.AttributeCount; ++i)
        {
            var offset    = r.ReadUInt32();
            var stringIdx = offsets.AsSpan().IndexOf(offset);
            Attributes[i] = stringIdx >= 0 ? strings[stringIdx] : string.Empty;
        }

        TerrainShadowMeshes    = r.ReadStructuresAsArray<MdlStructs.TerrainShadowMeshStruct>(modelHeader.TerrainShadowMeshCount);
        SubMeshes              = r.ReadStructuresAsArray<MdlStructs.SubmeshStruct>(modelHeader.SubmeshCount);
        TerrainShadowSubMeshes = r.ReadStructuresAsArray<MdlStructs.TerrainShadowSubmeshStruct>(modelHeader.TerrainShadowSubmeshCount);

        Materials = new string[modelHeader.MaterialCount];
        for (var i = 0; i < modelHeader.MaterialCount; ++i)
        {
            var offset    = r.ReadUInt32();
            var stringIdx = offsets.AsSpan().IndexOf(offset);
            Materials[i] = stringIdx >= 0 ? strings[stringIdx] : string.Empty;
        }

        Bones = new string[modelHeader.BoneCount];
        for (var i = 0; i < modelHeader.BoneCount; ++i)
        {
            var offset    = r.ReadUInt32();
            var stringIdx = offsets.AsSpan().IndexOf(offset);
            Bones[i] = stringIdx >= 0 ? strings[stringIdx] : string.Empty;
        }

        BoneTables = new MdlStructs.BoneTableStruct[modelHeader.BoneTableCount];
        for (var i = 0; i < modelHeader.BoneTableCount; i++)
            BoneTables[i] = MdlStructs.BoneTableStruct.Read(r);

        Shapes = new Shape[modelHeader.ShapeCount];
        for (var i = 0; i < modelHeader.ShapeCount; i++)
            Shapes[i] = new Shape(MdlStructs.ShapeStruct.Read(r), offsets, strings);

        ShapeMeshes = r.ReadStructuresAsArray<MdlStructs.ShapeMeshStruct>(modelHeader.ShapeMeshCount);
        ShapeValues = r.ReadStructuresAsArray<MdlStructs.ShapeValueStruct>(modelHeader.ShapeValueCount);

        var submeshBoneMapSize = r.ReadUInt32();
        SubMeshBoneMap = r.ReadStructures<ushort>((int)submeshBoneMapSize / 2).ToArray();

        var paddingAmount = r.ReadByte();
        r.Seek(r.BaseStream.Position + paddingAmount);

        // Dunno what this first one is for?
        BoundingBoxes            = MdlStructs.BoundingBoxStruct.Read(r);
        ModelBoundingBoxes       = MdlStructs.BoundingBoxStruct.Read(r);
        WaterBoundingBoxes       = MdlStructs.BoundingBoxStruct.Read(r);
        VerticalFogBoundingBoxes = MdlStructs.BoundingBoxStruct.Read(r);
        BoneBoundingBoxes        = new MdlStructs.BoundingBoxStruct[modelHeader.BoneCount];
        for (var i = 0; i < modelHeader.BoneCount; i++)
            BoneBoundingBoxes[i] = MdlStructs.BoundingBoxStruct.Read(r);

        var runtimePadding = header.RuntimeSize + FileHeaderSize + header.StackSize - r.BaseStream.Position;
        if (runtimePadding > 0)
            r.ReadBytes((int)runtimePadding);
        RemainingData = r.ReadBytes((int)(r.BaseStream.Length - r.BaseStream.Position));
        Valid         = true;
    }

    private MdlStructs.ModelFileHeader LoadModelFileHeader(LuminaBinaryReader r)
    {
        var header = MdlStructs.ModelFileHeader.Read(r);
        Version                    = header.Version;
        EnableIndexBufferStreaming = header.EnableIndexBufferStreaming;
        EnableEdgeGeometry         = header.EnableEdgeGeometry;
        return header;
    }

    private MdlStructs.ModelHeader LoadModelHeader(BinaryReader r)
    {
        var modelHeader = r.ReadStructure<MdlStructs.ModelHeader>();
        Radius = modelHeader.Radius;
        Flags1 = (MdlStructs.ModelFlags1)(modelHeader.GetType()
                .GetField("Flags1", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?.GetValue(modelHeader)
         ?? 0);
        Flags2 = (MdlStructs.ModelFlags2)(modelHeader.GetType()
                .GetField("Flags2", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?.GetValue(modelHeader)
         ?? 0);
        ModelClipOutDistance  = modelHeader.ModelClipOutDistance;
        ShadowClipOutDistance = modelHeader.ShadowClipOutDistance;
        Unknown4              = modelHeader.Unknown4;
        Unknown5 = (byte)(modelHeader.GetType()
                .GetField("Unknown5", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?.GetValue(modelHeader)
         ?? 0);
        Unknown6                   = modelHeader.Unknown6;
        Unknown7                   = modelHeader.Unknown7;
        Unknown8                   = modelHeader.Unknown8;
        Unknown9                   = modelHeader.Unknown9;
        BgChangeMaterialIndex      = modelHeader.BGChangeMaterialIndex;
        BgCrestChangeMaterialIndex = modelHeader.BGCrestChangeMaterialIndex;

        return modelHeader;
    }

    private static (uint[], string[]) LoadStrings(BinaryReader r)
    {
        var stringCount = r.ReadUInt16();
        r.ReadUInt16();
        var stringSize = (int)r.ReadUInt32();
        var stringData = r.ReadBytes(stringSize);
        var start      = 0;
        var strings    = new string[stringCount];
        var offsets    = new uint[stringCount];
        for (var i = 0; i < stringCount; ++i)
        {
            var span = stringData.AsSpan(start);
            var idx  = span.IndexOf((byte)'\0');
            strings[i] = Encoding.UTF8.GetString(span[..idx]);
            offsets[i] = (uint)start;
            start      = start + idx + 1;
        }

        return (offsets, strings);
    }

    public unsafe uint StackSize
        => (uint)(VertexDeclarations.Length * NumVertices * sizeof(MdlStructs.VertexElement));

    public enum VertexType
    {
        Single1 = 0,
        Single2 = 1,
        Single3 = 2,
        Single4 = 3,

        // Unk4  = 4,
        UByte4  = 5,
        Short2  = 6,
        Short4  = 7,
        NByte4  = 8,
        NShort2 = 9,
        NShort4 = 10,

        // Unk11 = 11,
        // Unk12 = 12
        Half2 = 13,
        Half4 = 14,
        // Unk15 = 15
    }

    public enum VertexUsage
    {
        Position     = 0,
        BlendWeights = 1,
        BlendIndices = 2,
        Normal       = 3,
        UV           = 4,
        Tangent2     = 5,
        Tangent1     = 6,
        Color        = 7,
    }

    public static MdlStructs.BoundingBoxStruct EmptyBoundingBox = new()
    {
        Min = [0f, 0f, 0f, 0f],
        Max = [0f, 0f, 0f, 0f],
    };

    public struct MeshStruct
    {
        public  ushort VertexCount;
        private ushort Padding;
        public  uint   IndexCount;
        public  ushort MaterialIndex;
        public  ushort SubMeshIndex;
        public  ushort SubMeshCount;
        public  ushort BoneTableIndex;
        public  uint   StartIndex;
        public  uint[] VertexBufferOffset;
        public  byte[] VertexBufferStride;
        private byte   _vertexStreamCountByte;

        public byte VertexStreamCount
        {
            get => (byte)(_vertexStreamCountByte & 3);
            set => _vertexStreamCountByte = (byte)((value & 3) | VertexStreamCountRemainder);
        }

        public byte VertexStreamCountRemainder
            => (byte)(_vertexStreamCountByte & ~3);

        public static MeshStruct Read(LuminaBinaryReader br)
            => new()
            {
                VertexCount            = br.ReadUInt16(),
                Padding                = br.ReadUInt16(),
                IndexCount             = br.ReadUInt32(),
                MaterialIndex          = br.ReadUInt16(),
                SubMeshIndex           = br.ReadUInt16(),
                SubMeshCount           = br.ReadUInt16(),
                BoneTableIndex         = br.ReadUInt16(),
                StartIndex             = br.ReadUInt32(),
                VertexBufferOffset     = br.ReadUInt32Array(3),
                VertexBufferStride     = br.ReadBytes(3),
                _vertexStreamCountByte = br.ReadByte(),
            };
    }
}
