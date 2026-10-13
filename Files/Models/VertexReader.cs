namespace Penumbra.GameData.Files.Models;

public struct VertexReader
{
    private readonly MdlFile    _mdl;
    private readonly MeshStruct _mesh;
    private readonly int        _meshIndex;
    private readonly int        _baseOffset;


    private int                 _currentUsageIndex = int.MinValue;
    private int                 _currentOffset;
    private byte                _currentStride;
    private byte                _currentSize;
    private MdlFile.VertexUsage _currentUsage;
    private MdlFile.VertexType  _currentType;

    public int Count
        => _mesh.VertexCount;

    public VertexReader(MdlFile mdl, int lod, int meshIndex)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)lod, mdl.LodCount, nameof(lod));
        _mdl        = mdl;
        _meshIndex  = meshIndex;
        _mesh       = mdl.Meshes[meshIndex];
        _baseOffset = (int)_mdl.VertexOffset[lod];
    }

    public Vector3 GetPosition(int vertexIndex)
    {
        var data = GetAttribute(vertexIndex, MdlFile.VertexUsage.Position, 0, out var type);
        var ret  = type.Read(data);
        return new Vector3(ret.X, ret.Y, ret.Z);
    }

    public Vector4 GetBlendIndices(int vertexIndex)
    {
        var data = GetAttribute(vertexIndex, MdlFile.VertexUsage.BlendIndices, 0, out var type);
        return type.Read(data);
    }

    public Vector4 GetBlendWeights(int vertexIndex)
    {
        var data = GetAttribute(vertexIndex, MdlFile.VertexUsage.BlendWeights, 0, out var type);
        return type.Read(data);
    }

    public void PrepareUsage(MdlFile.VertexUsage usage, int usageIndex)
    {
        if (usage == _currentUsage && usageIndex == _currentUsageIndex)
            return;

        foreach (var element in _mdl.VertexDeclarations[_meshIndex].VertexElements)
        {
            if ((MdlFile.VertexUsage)element.Usage != usage || element.UsageIndex != usageIndex)
                continue;

            var stream = element.Stream;
            if (stream >= _mesh.VertexStreamCount)
                throw new InvalidDataException("Invalid vertex stream.");

            _currentType       = (MdlFile.VertexType)element.Type;
            _currentStride     = _mesh.VertexBufferStride(stream);
            _currentOffset     = checked(_baseOffset + (int)_mesh.VertexBufferOffset(stream) + element.Offset);
            _currentUsage      = usage;
            _currentUsageIndex = usageIndex;
            _currentSize       = (byte)_currentType.Size;
            return;
        }

        throw new InvalidDataException($"Vertex attribute {usage}[{usageIndex}] not found.");
    }

    public ReadOnlySpan<byte> GetAttribute(int vertexIndex, MdlFile.VertexUsage usage, int usageIndex, out MdlFile.VertexType type)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)vertexIndex, (uint)Count, nameof(vertexIndex));
        PrepareUsage(usage, usageIndex);
        type = _currentType;
        return _mdl.RemainingData.AsSpan(checked(_currentOffset + vertexIndex * _currentStride), _currentSize);
    }
}
