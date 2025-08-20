using Lumina.Data.Parsing;
using OtterGui.Extensions;
using Penumbra.GameData.Files.ModelStructs;

namespace Penumbra.GameData.Files;

public partial class MdlFile
{
    private static uint Write(BinaryWriter w, string s, long basePos)
    {
        var currentPos = w.BaseStream.Position;
        w.Write(Encoding.UTF8.GetBytes(s));
        w.Write((byte)0);
        return (uint)(currentPos - basePos);
    }

    private List<uint> WriteStrings(BinaryWriter w)
    {
        var startPos = (int)w.BaseStream.Position;
        var basePos  = startPos + 8;
        var count    = (ushort)(Attributes.Length + Bones.Length + Materials.Length + Shapes.Length);

        w.Write(count);
        w.Seek(basePos, SeekOrigin.Begin);
        var ret = Attributes.Concat(Bones)
            .Concat(Materials)
            .Concat(Shapes.Select(s => s.ShapeName))
            .Select(attribute => Write(w, attribute, basePos)).ToList();

        var padding = (w.BaseStream.Position & 0b11) > 0 ? (w.BaseStream.Position & ~0b11) + 4 : w.BaseStream.Position;
        for (var i = w.BaseStream.Position; i < padding; ++i)
            w.Write((byte)0);
        var size = (int)w.BaseStream.Position - basePos;
        w.Seek(startPos + 4, SeekOrigin.Begin);
        w.Write((uint)size);
        w.Seek(basePos + size, SeekOrigin.Begin);
        return ret;
    }

    private void WriteModelFileHeader(BinaryWriter w, uint totalSize)
    {
        w.Write(Version);
        w.Write(StackSize);
        w.Write(totalSize - StackSize - FileHeaderSize);
        w.Write((ushort)VertexDeclarations.Length);
        w.Write((ushort)Materials.Length);
        w.Write(0 < LodCount ? VertexOffset[0] + totalSize : 0u);
        w.Write(1 < LodCount ? VertexOffset[1] + totalSize : 0u);
        w.Write(2 < LodCount ? VertexOffset[2] + totalSize : 0u);
        w.Write(0 < LodCount ? IndexOffset[0] + totalSize : 0u);
        w.Write(1 < LodCount ? IndexOffset[1] + totalSize : 0u);
        w.Write(2 < LodCount ? IndexOffset[2] + totalSize : 0u);
        w.Write(VertexBufferSize[0]);
        w.Write(VertexBufferSize[1]);
        w.Write(VertexBufferSize[2]);
        w.Write(IndexBufferSize[0]);
        w.Write(IndexBufferSize[1]);
        w.Write(IndexBufferSize[2]);
        w.Write(LodCount);
        w.Write(EnableIndexBufferStreaming);
        w.Write(EnableEdgeGeometry);
        w.Write((byte)0); // Padding
    }

    private void WriteModelHeader(BinaryWriter w)
    {
        w.Write(Radius);
        w.Write((ushort)Meshes.Length);
        w.Write((ushort)Attributes.Length);
        w.Write((ushort)SubMeshes.Length);
        w.Write((ushort)Materials.Length);
        w.Write((ushort)Bones.Length);
        w.Write((ushort)BoneTables.Length);
        w.Write((ushort)Shapes.Length);
        w.Write((ushort)ShapeMeshes.Length);
        w.Write((ushort)ShapeValues.Length);
        w.Write(LodCount);
        w.Write((byte)Flags1);
        w.Write((ushort)ElementIds.Length);
        w.Write((byte)TerrainShadowMeshes.Length);
        w.Write((byte)Flags2);
        w.Write(ModelClipOutDistance);
        w.Write(ShadowClipOutDistance);
        w.Write(CullingGridCount);
        w.Write((ushort)TerrainShadowSubMeshes.Length);
        w.Write(Flags3);
        w.Write(BgChangeMaterialIndex);
        w.Write(BgCrestChangeMaterialIndex);
        w.Write((byte)NeckMorphs.Length);
        w.Write(BoneTableSum());
        w.Write(Unknown8);
        w.Write(Unknown9);
        w.Write((uint)0); // 6 byte padding
        w.Write((ushort)0);
        return;

        ushort BoneTableSum()
        {
            if (Version is V5)
                return 0;

            var count = BoneTables.Sum(t => (t.BoneCount & 1) == 1 ? t.BoneCount + 1 : t.BoneCount);
            return (ushort)count;
        }
    }

    private static void Write(BinaryWriter w, in MdlStructs.VertexElement vertex)
    {
        w.Write(vertex.Stream);
        w.Write(vertex.Offset);
        w.Write(vertex.Type);
        w.Write(vertex.Usage);
        w.Write(vertex.UsageIndex);
        w.Write((ushort)0); // 3 byte padding
        w.Write((byte)0);
    }

    private static void Write(BinaryWriter w, in MdlStructs.VertexDeclarationStruct vertexDecl)
    {
        foreach (var vertex in vertexDecl.VertexElements)
            Write(w, vertex);

        Write(w, new MdlStructs.VertexElement() { Stream = 255 });
        w.Seek((int)(NumVertices - 1 - vertexDecl.VertexElements.Length) * 8, SeekOrigin.Current);
    }

    private static void Write(BinaryWriter w, in MdlStructs.ElementIdStruct elementId)
    {
        w.Write(elementId.ElementId);
        w.Write(elementId.ParentBoneName);
        w.Write(elementId.Translate[0]);
        w.Write(elementId.Translate[1]);
        w.Write(elementId.Translate[2]);
        w.Write(elementId.Rotate[0]);
        w.Write(elementId.Rotate[1]);
        w.Write(elementId.Rotate[2]);
    }

    private static unsafe void Write<T>(BinaryWriter w, in T data) where T : unmanaged
    {
        fixed (T* ptr = &data)
        {
            var bytePtr = (byte*)ptr;
            var size    = sizeof(T);
            var span    = new ReadOnlySpan<byte>(bytePtr, size);
            w.Write(span);
        }
    }


    private static void Write(BinaryWriter w, BoneTableStruct bone)
    {
        foreach (var index in bone.BoneIndex)
            w.Write(index);

        w.Write(bone.BoneCount);
        w.Write((ushort)0); // 3 bytes padding
        w.Write((byte)0);
    }

    private void Write(BinaryWriter w, int shapeIdx, IReadOnlyList<uint> offsets)
    {
        var shape  = Shapes[shapeIdx];
        var offset = offsets[Attributes.Length + Bones.Length + Materials.Length + shapeIdx];
        w.Write(offset);
        w.Write(shape.ShapeMeshStartIndex[0]);
        w.Write(shape.ShapeMeshStartIndex[1]);
        w.Write(shape.ShapeMeshStartIndex[2]);
        w.Write(shape.ShapeMeshCount[0]);
        w.Write(shape.ShapeMeshCount[1]);
        w.Write(shape.ShapeMeshCount[2]);
    }

    private static void Write(BinaryWriter w, MdlStructs.BoundingBoxStruct box)
    {
        w.Write(box.Min[0]);
        w.Write(box.Min[1]);
        w.Write(box.Min[2]);
        w.Write(box.Min[3]);
        w.Write(box.Max[0]);
        w.Write(box.Max[1]);
        w.Write(box.Max[2]);
        w.Write(box.Max[3]);
    }

    public byte[] Write()
    {
        using var stream = new MemoryStream();
        using (var w = new BinaryWriter(stream))
        {
            // Skip and write this later when we actually know it.
            w.Seek((int)FileHeaderSize, SeekOrigin.Begin);

            foreach (var vertexDecl in VertexDeclarations)
                Write(w, vertexDecl);

            var offsets = WriteStrings(w);
            WriteModelHeader(w);

            foreach (var elementId in ElementIds)
                Write(w, elementId);

            // LoDs contain absolute offsets, and will need to be written after the rest of the size is known.
            var lodPosition = (int)w.BaseStream.Position;
            unsafe
            {
                w.Seek(sizeof(MdlStructs.LodStruct) * 3, SeekOrigin.Current);
            }

            if (Flags2.HasFlag(MdlStructs.ModelFlags2.ExtraLodEnabled))
                foreach (var extraLod in ExtraLods)
                    Write(w, extraLod);

            foreach (var mesh in Meshes)
                Write(w, mesh);

            for (var i = 0; i < Attributes.Length; ++i)
                w.Write(offsets[i]);

            foreach (var terrainShadowMesh in TerrainShadowMeshes)
                Write(w, terrainShadowMesh);

            foreach (var subMesh in SubMeshes)
                Write(w, subMesh);

            foreach (var terrainShadowSubMesh in TerrainShadowSubMeshes)
                Write(w, terrainShadowSubMesh);

            for (var i = 0; i < Materials.Length; ++i)
                w.Write(offsets[Attributes.Length + Bones.Length + i]);

            for (var i = 0; i < Bones.Length; ++i)
                w.Write(offsets[Attributes.Length + i]);

            if (Version is V5)
                BoneTableStruct.WriteV5(w, BoneTables);
            else
                BoneTableStruct.WriteV6(w, BoneTables);

            for (var i = 0; i < Shapes.Length; ++i)
                Write(w, i, offsets);

            foreach (var shapeMesh in ShapeMeshes)
                Write(w, shapeMesh);

            foreach (var shapeValue in ShapeValues)
                Write(w, shapeValue);

            w.Write(SubMeshBoneMap.Length * 2);
            foreach (var bone in SubMeshBoneMap)
                w.Write(bone);

            var pos     = w.BaseStream.Position + 1;
            var padding = (byte)(pos & 0b111);
            if (padding > 0)
                padding = (byte)(8 - padding);
            w.Write(padding);
            for (var i = 0; i < padding; ++i)
                w.Write((byte)(0xDEADBEEFF00DCAFEu >> (8 * (7 - i))));

            Write(w, BoundingBoxes);
            Write(w, ModelBoundingBoxes);
            Write(w, WaterBoundingBoxes);
            Write(w, VerticalFogBoundingBoxes);
            foreach (var box in BoneBoundingBoxes)
                Write(w, box);

            var totalSize = (uint)w.BaseStream.Position;
            w.Write(RemainingData);

            // Write header data.
            w.Seek(0, SeekOrigin.Begin);
            WriteModelFileHeader(w, totalSize);

            // LoD data also includes absolute offsets, move back to their position and write out now we know how large the metadata is.
            w.Seek(lodPosition, SeekOrigin.Begin);
            foreach (var (lod, index) in Lods.WithIndex())
            {
                Write(w, lod with
                {
                    VertexDataOffset = index < LodCount ? lod.VertexDataOffset + totalSize : 0,
                    IndexDataOffset = index < LodCount ? lod.IndexDataOffset + totalSize : 0,
                });
            }
        }

        return stream.ToArray();
    }
}
