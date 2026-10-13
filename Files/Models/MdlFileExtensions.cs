using System.Buffers.Binary;
using static Lumina.Data.Parsing.MdlStructs;

namespace Penumbra.GameData.Files.Models;

public static class MdlFileExtensions
{
    extension(BoundingBoxStruct)
    {
        public static BoundingBoxStruct Create(in Vector3 min, in Vector3 max)
            => new()
            {
                Min = [min.X, min.Y, min.Z, 1f],
                Max = [max.X, max.Y, max.Z, 1f],
            };

        public static BoundingBoxStruct Empty
            => new()
            {
                Min = [0f, 0f, 0f, 0f],
                Max = [0f, 0f, 0f, 0f],
            };
    }

    extension(MdlFile.VertexType vertexType)
    {
        public int Size
            => vertexType switch
            {
                MdlFile.VertexType.Single1 => 4,
                MdlFile.VertexType.Single2 => 8,
                MdlFile.VertexType.Single3 => 12,
                MdlFile.VertexType.Single4 => 16,
                MdlFile.VertexType.UByte4  => 4,
                MdlFile.VertexType.Short2  => 4,
                MdlFile.VertexType.Short4  => 8,
                MdlFile.VertexType.NByte4  => 4,
                MdlFile.VertexType.NShort2 => 4,
                MdlFile.VertexType.NShort4 => 8,
                MdlFile.VertexType.Half2   => 4,
                MdlFile.VertexType.Half4   => 8,
                MdlFile.VertexType.UShort2 => 4,
                MdlFile.VertexType.UShort4 => 8,
                _                          => throw new NotSupportedException($"Unsupported vertex type: {vertexType}"),
            };

        /// <summary> Read the vertex attribute from the span and return a set of 4 floats. </summary>
        /// <remarks>
        ///   Integer attributes are returned as their raw numerical values.<br/>
        ///   Missing components are returned as <see cref="float.NaN"/>.<br/>
        ///   Normalized values are read as unsigned data, i.e. in [0, 255] and [0, 65535] respectively, without applying normalization, since normalization depends on usage.
        /// </remarks>
        public Vector4 Read(ReadOnlySpan<byte> data)
        {
            switch (vertexType)
            {
                // @formatter:off
                case MdlFile.VertexType.Single1: data.CheckSize(4);  return new Vector4(data.ReadFloat(0),  float.NaN,          float.NaN,          float.NaN);
                case MdlFile.VertexType.Single2: data.CheckSize(8);  return new Vector4(data.ReadFloat(0),  data.ReadFloat(4),  float.NaN,          float.NaN);
                case MdlFile.VertexType.Single3: data.CheckSize(12); return new Vector4(data.ReadFloat(0),  data.ReadFloat(4),  data.ReadFloat(8),  float.NaN);
                case MdlFile.VertexType.Single4: data.CheckSize(16); return new Vector4(data.ReadFloat(0),  data.ReadFloat(4),  data.ReadFloat(8),  data.ReadFloat(12));
                case MdlFile.VertexType.UByte4:  data.CheckSize(4);  return new Vector4(data[0],            data[1],            data[2],            data[3]);
                case MdlFile.VertexType.Short2:  data.CheckSize(4);  return new Vector4(data.ReadShort(0),  data.ReadShort(2),  float.NaN,          float.NaN);
                case MdlFile.VertexType.Short4:  data.CheckSize(8);  return new Vector4(data.ReadShort(0),  data.ReadShort(2),  data.ReadShort(4),  data.ReadShort(6));
                case MdlFile.VertexType.NByte4:  data.CheckSize(4);  return new Vector4(data[0],            data[1],            data[2],            data[3]);
                case MdlFile.VertexType.NShort2: data.CheckSize(4);  return new Vector4(data.ReadUShort(0), data.ReadUShort(2),  float.NaN,          float.NaN);
                case MdlFile.VertexType.NShort4: data.CheckSize(8);  return new Vector4(data.ReadUShort(0), data.ReadUShort(2), data.ReadUShort(4), data.ReadUShort(6));
                case MdlFile.VertexType.Half2:   data.CheckSize(4);  return new Vector4(data.ReadHalf(0),   data.ReadHalf(2),   float.NaN,          float.NaN);
                case MdlFile.VertexType.Half4:   data.CheckSize(8);  return new Vector4(data.ReadHalf(0),   data.ReadHalf(2),   data.ReadHalf(4),   data.ReadHalf(6));
                case MdlFile.VertexType.UShort2: data.CheckSize(4);  return new Vector4(data.ReadUShort(0), data.ReadUShort(2), float.NaN,          float.NaN);
                case MdlFile.VertexType.UShort4: data.CheckSize(8);  return new Vector4(data.ReadUShort(0), data.ReadUShort(2), data.ReadUShort(4), data.ReadUShort(6));
                // @formatter:on
                default: throw new ArgumentOutOfRangeException(nameof(vertexType), vertexType, null);
            }
        }
    }

    extension(ReadOnlySpan<byte> data)
    {
        private void CheckSize(int size)
        {
            if (data.Length < size)
                throw new InvalidDataException($"Provided data was size {data.Length} when {size} was required.");
        }

        private float ReadFloat(int offset)
            => BinaryPrimitives.ReadSingleLittleEndian(data[offset..]);

        private float ReadHalf(int offset)
            => (float)BinaryPrimitives.ReadHalfLittleEndian(data[offset..]);

        private ushort ReadUShort(int offset)
            => BinaryPrimitives.ReadUInt16LittleEndian(data[offset..]);

        private short ReadShort(int offset)
            => BinaryPrimitives.ReadInt16LittleEndian(data[offset..]);
    }
}
