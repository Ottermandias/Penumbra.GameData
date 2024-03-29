using Penumbra.GameData.Data;
using Penumbra.GameData.Files.Utility;
using DisassembledShader = Penumbra.GameData.Interop.DisassembledShader;

namespace Penumbra.GameData.Files;

public partial class ShpkFile : IWritable
{
    private const uint ShPkMagic = 0x6B506853u; // bytes of ShPk
    private const uint Dx9Magic  = 0x00395844u; // bytes of DX9\0
    private const uint Dx11Magic = 0x31315844u; // bytes of DX11

    public const uint MaterialParamsConstantId = 0x64D12851u; // g_MaterialParameter is a cbuffer filled from the ad hoc section of the mtrl
    public const uint TableSamplerId           = 0x2005679Fu; // g_SamplerTable is a texture filled from the mtrl's color set

    public const uint SelectorMultiplier        = 31;
    public const uint SelectorInverseMultiplier = 3186588639; // 31 * 3186588639 = 23 << 32 + 1, iow they're modular inverses

    public readonly bool Disassembled;

    public uint                   Version;
    public DxVersion              DirectXVersion;
    public Shader[]               VertexShaders;
    public Shader[]               PixelShaders;
    public uint                   MaterialParamsSize;
    public MaterialParam[]        MaterialParams;
    public Resource[]             Constants;
    public Resource[]             Samplers;
    public Resource[]             Uavs;
    public Key[]                  SystemKeys;
    public Key[]                  SceneKeys;
    public Key[]                  MaterialKeys;
    public Key[]                  SubViewKeys;
    public Node[]                 Nodes;
    public Dictionary<uint, uint> NodeSelectors;
    public byte[]                 AdditionalData;

    public  bool Valid { get; private set; }
    private bool _changed;

    public MaterialParam? GetMaterialParamById(uint id)
        => MaterialParams.FirstOrNull(m => m.Id == id);

    public Resource? GetConstantById(uint id)
        => Constants.FirstOrNull(c => c.Id == id);

    public Resource? GetConstantByName(string name)
        => Constants.FirstOrNull(c => c.Name == name);

    public Resource? GetSamplerById(uint id)
        => Samplers.FirstOrNull(s => s.Id == id);

    public Resource? GetSamplerByName(string name)
        => Samplers.FirstOrNull(s => s.Name == name);

    public Resource? GetUavById(uint id)
        => Uavs.FirstOrNull(u => u.Id == id);

    public Resource? GetUavByName(string name)
        => Uavs.FirstOrNull(u => u.Name == name);

    public Key? GetSystemKeyById(uint id)
        => SystemKeys.FirstOrNull(k => k.Id == id);

    public Key? GetSceneKeyById(uint id)
        => SceneKeys.FirstOrNull(k => k.Id == id);

    public Key? GetMaterialKeyById(uint id)
        => MaterialKeys.FirstOrNull(k => k.Id == id);

    public Node? GetNodeBySelector(uint selector)
    {
        if (NodeSelectors.TryGetValue(selector, out var index) && index < Nodes.Length)
            return Nodes[index];

        return null;
    }

    public ShpkFile(byte[] data, bool disassemble = false)
        : this((ReadOnlySpan<byte>)data, disassemble)
    { }

    public ShpkFile(ReadOnlySpan<byte> data, bool disassemble = false)
    {
        Disassembled = disassemble;

        var r = new SpanBinaryReader(data);

        if (r.ReadUInt32() != ShPkMagic)
            throw new InvalidDataException();

        Version = r.ReadUInt32();
        DirectXVersion = r.ReadUInt32() switch
        {
            Dx9Magic  => DxVersion.DirectX9,
            Dx11Magic => DxVersion.DirectX11,
            _         => throw new InvalidDataException(),
        };
        if (r.ReadUInt32() != data.Length)
            throw new InvalidDataException();

        var blobsOffset       = r.ReadUInt32();
        var stringsOffset     = r.ReadUInt32();
        var vertexShaderCount = r.ReadUInt32();
        var pixelShaderCount  = r.ReadUInt32();
        MaterialParamsSize = r.ReadUInt32();
        var materialParamCount = r.ReadUInt32();
        var constantCount      = r.ReadUInt32();
        var samplerCount       = r.ReadUInt32();
        var uavCount           = r.ReadUInt32();
        var systemKeyCount     = r.ReadUInt32();
        var sceneKeyCount      = r.ReadUInt32();
        var materialKeyCount   = r.ReadUInt32();
        var nodeCount          = r.ReadUInt32();
        var nodeAliasCount     = r.ReadUInt32();

        var blobs   = data[(int)blobsOffset..(int)stringsOffset];
        var strings = new SpanBinaryReader(data[(int)stringsOffset..]);

        VertexShaders = ReadShaderArray(ref r, (int)vertexShaderCount, DisassembledShader.ShaderStage.Vertex, DirectXVersion, disassemble,
            blobs,                             ref strings);
        PixelShaders = ReadShaderArray(ref r, (int)pixelShaderCount, DisassembledShader.ShaderStage.Pixel, DirectXVersion, disassemble, blobs,
            ref strings);

        MaterialParams = r.Read<MaterialParam>((int)materialParamCount).ToArray();

        Constants = ReadResourceArray(ref r, (int)constantCount, ref strings);
        Samplers  = ReadResourceArray(ref r, (int)samplerCount,  ref strings);
        Uavs      = ReadResourceArray(ref r, (int)uavCount,      ref strings);

        SystemKeys   = ReadKeyArray(ref r, (int)systemKeyCount);
        SceneKeys    = ReadKeyArray(ref r, (int)sceneKeyCount);
        MaterialKeys = ReadKeyArray(ref r, (int)materialKeyCount);

        var subViewKey1Default = r.ReadUInt32();
        var subViewKey2Default = r.ReadUInt32();

        SubViewKeys = new Key[]
        {
            new()
            {
                Id           = 1,
                DefaultValue = subViewKey1Default,
                Values       = Array.Empty<uint>(),
            },
            new()
            {
                Id           = 2,
                DefaultValue = subViewKey2Default,
                Values       = Array.Empty<uint>(),
            },
        };

        Nodes = ReadNodeArray(ref r, (int)nodeCount, SystemKeys.Length, SceneKeys.Length, MaterialKeys.Length, SubViewKeys.Length);

        NodeSelectors = new Dictionary<uint, uint>(Nodes.Length + (int)nodeAliasCount);
        for (var i = 0; i < Nodes.Length; ++i)
            NodeSelectors.TryAdd(Nodes[i].Selector, (uint)i);
        foreach (var alias in r.Read<NodeAlias>((int)nodeAliasCount))
            NodeSelectors.TryAdd(alias.Selector, alias.Node);

        AdditionalData = r.Read<byte>((int)(blobsOffset - r.Position)).ToArray(); // This should be empty, but just in case.

        if (disassemble)
            UpdateUsed();

        UpdateKeyValues();

        Valid    = true;
        _changed = false;
    }

    public void UpdateResources()
    {
        var constants = new Dictionary<uint, Resource>();
        var samplers  = new Dictionary<uint, Resource>();
        var uavs      = new Dictionary<uint, Resource>();

        static void CollectResources(Dictionary<uint, Resource> resources, Resource[] shaderResources, Func<uint, Resource?> getExistingById,
            DisassembledShader.ResourceType type)
        {
            foreach (var resource in shaderResources)
            {
                if (resources.TryGetValue(resource.Id, out var carry) && type != DisassembledShader.ResourceType.ConstantBuffer)
                    continue;

                var existing = getExistingById(resource.Id);
                resources[resource.Id] = new Resource
                {
                    Id = resource.Id,
                    Name = resource.Name,
                    Slot = existing?.Slot ?? (type == DisassembledShader.ResourceType.ConstantBuffer ? (ushort)65535 : (ushort)2),
                    Size = type == DisassembledShader.ResourceType.ConstantBuffer ? Math.Max(carry.Size, resource.Size) : existing?.Size ?? 0,
                    Used = null,
                    UsedDynamically = null,
                };
            }
        }

        foreach (var shader in VertexShaders)
        {
            CollectResources(constants, shader.Constants, GetConstantById, DisassembledShader.ResourceType.ConstantBuffer);
            CollectResources(samplers,  shader.Samplers,  GetSamplerById,  DisassembledShader.ResourceType.Sampler);
            CollectResources(uavs,      shader.Uavs,      GetUavById,      DisassembledShader.ResourceType.Uav);
        }

        foreach (var shader in PixelShaders)
        {
            CollectResources(constants, shader.Constants, GetConstantById, DisassembledShader.ResourceType.ConstantBuffer);
            CollectResources(samplers,  shader.Samplers,  GetSamplerById,  DisassembledShader.ResourceType.Sampler);
            CollectResources(uavs,      shader.Uavs,      GetUavById,      DisassembledShader.ResourceType.Uav);
        }

        Constants = constants.Values.ToArray();
        Samplers  = samplers.Values.ToArray();
        Uavs      = uavs.Values.ToArray();
        UpdateUsed();

        // Ceil required size to a multiple of 16 bytes.
        // Offsets can be skipped, MaterialParamsConstantId's size is the count.
        MaterialParamsSize = (GetConstantById(MaterialParamsConstantId)?.Size ?? 0u) << 4;
        foreach (var param in MaterialParams)
            MaterialParamsSize = Math.Max(MaterialParamsSize, (uint)param.ByteOffset + param.ByteSize);
        MaterialParamsSize = (MaterialParamsSize + 0xFu) & ~0xFu;
    }

    private void UpdateUsed()
    {
        var cUsage = new Dictionary<uint, (DisassembledShader.VectorComponents[], DisassembledShader.VectorComponents)>();
        var sUsage = new Dictionary<uint, (DisassembledShader.VectorComponents[], DisassembledShader.VectorComponents)>();
        var uUsage = new Dictionary<uint, (DisassembledShader.VectorComponents[], DisassembledShader.VectorComponents)>();

        static void CollectUsed(Dictionary<uint, (DisassembledShader.VectorComponents[], DisassembledShader.VectorComponents)> usage,
            Resource[] resources)
        {
            foreach (var resource in resources)
            {
                if (resource.Used == null)
                    continue;

                usage.TryGetValue(resource.Id, out var carry);
                carry.Item1 ??= Array.Empty<DisassembledShader.VectorComponents>();
                var combined = new DisassembledShader.VectorComponents[Math.Max(carry.Item1.Length, resource.Used.Length)];
                for (var i = 0; i < combined.Length; ++i)
                    combined[i] = (i < carry.Item1.Length ? carry.Item1[i] : 0) | (i < resource.Used.Length ? resource.Used[i] : 0);
                usage[resource.Id] = (combined, carry.Item2 | (resource.UsedDynamically ?? 0));
            }
        }

        static void CopyUsed(Resource[] resources,
            Dictionary<uint, (DisassembledShader.VectorComponents[], DisassembledShader.VectorComponents)> used)
        {
            for (var i = 0; i < resources.Length; ++i)
            {
                if (used.TryGetValue(resources[i].Id, out var usage))
                {
                    resources[i].Used            = usage.Item1;
                    resources[i].UsedDynamically = usage.Item2;
                }
                else
                {
                    resources[i].Used            = null;
                    resources[i].UsedDynamically = null;
                }
            }
        }

        foreach (var shader in VertexShaders)
        {
            CollectUsed(cUsage, shader.Constants);
            CollectUsed(sUsage, shader.Samplers);
            CollectUsed(uUsage, shader.Uavs);
        }

        foreach (var shader in PixelShaders)
        {
            CollectUsed(cUsage, shader.Constants);
            CollectUsed(sUsage, shader.Samplers);
            CollectUsed(uUsage, shader.Uavs);
        }

        CopyUsed(Constants, cUsage);
        CopyUsed(Samplers,  sUsage);
        CopyUsed(Uavs,      uUsage);
    }

    public void UpdateKeyValues()
    {
        static HashSet<uint>[] InitializeValueSet(Key[] keys)
            => Array.ConvertAll(keys, key => new HashSet<uint>()
            {
                key.DefaultValue,
            });

        static void CollectValues(HashSet<uint>[] valueSets, uint[] values)
        {
            for (var i = 0; i < valueSets.Length; ++i)
                valueSets[i].Add(values[i]);
        }

        static void CopyValues(Key[] keys, HashSet<uint>[] valueSets)
        {
            for (var i = 0; i < keys.Length; ++i)
            {
                keys[i].Values = valueSets[i].ToArray();
                Array.Sort(keys[i].Values);
            }
        }

        var systemKeyValues   = InitializeValueSet(SystemKeys);
        var sceneKeyValues    = InitializeValueSet(SceneKeys);
        var materialKeyValues = InitializeValueSet(MaterialKeys);
        var subViewKeyValues  = InitializeValueSet(SubViewKeys);
        foreach (var node in Nodes)
        {
            CollectValues(systemKeyValues,   node.SystemKeys);
            CollectValues(sceneKeyValues,    node.SceneKeys);
            CollectValues(materialKeyValues, node.MaterialKeys);
            CollectValues(subViewKeyValues,  node.SubViewKeys);
        }

        CopyValues(SystemKeys,   systemKeyValues);
        CopyValues(SceneKeys,    sceneKeyValues);
        CopyValues(MaterialKeys, materialKeyValues);
        CopyValues(SubViewKeys,  subViewKeyValues);
    }

    public static uint BuildSelector(Span<uint> systemKeys, Span<uint> sceneKeys, Span<uint> materialKeys, Span<uint> subViewKeys)
        => BuildSelector(BuildSelector(systemKeys), BuildSelector(sceneKeys), BuildSelector(materialKeys), BuildSelector(subViewKeys));

    [SkipLocalsInit]
    public static uint BuildSelector(uint systemKeySelector, uint sceneKeySelector, uint materialKeySelector, uint subViewKeySelector)
    {
        Span<uint> parts = stackalloc uint[4];
        parts[0] = systemKeySelector;
        parts[1] = sceneKeySelector;
        parts[2] = materialKeySelector;
        parts[3] = subViewKeySelector;
        return BuildSelector(parts);
    }

    public static uint BuildSelector(Span<uint> keys)
    {
        unchecked
        {
            var selector   = 0u;
            var multiplier = 1u;
            foreach (var key in keys)
            {
                selector   += key * multiplier;
                multiplier *= SelectorMultiplier;
            }

            return selector;
        }
    }

    public static uint BuildSelector(IEnumerable<uint> keys)
    {
        unchecked
        {
            var selector   = 0u;
            var multiplier = 1u;
            foreach (var key in keys)
            {
                selector   += key * multiplier;
                multiplier *= SelectorMultiplier;
            }

            return selector;
        }
    }

    public static IEnumerable<uint> AllSelectors(Memory<Key> keys)
    {
        if (keys.Length == 0)
        {
            yield return 0;

            yield break;
        }
        else if (keys.Length == 1)
        {
            foreach (var value in keys.Span[0].Values)
                yield return value;
        }
        else
        {
            var values = keys.Span[0].Values;
            foreach (var selector in AllSelectors(keys[1..]))
            {
                var multiplied = unchecked(selector * SelectorMultiplier);
                foreach (var value in values)
                    yield return unchecked(value + multiplied);
            }
        }
    }

    public void SetInvalid()
        => Valid = false;

    public void SetChanged()
        => _changed = true;

    public bool IsChanged()
    {
        var changed = _changed;
        _changed = false;
        return changed;
    }

    private static void ClearUsed(Resource[] resources)
    {
        for (var i = 0; i < resources.Length; ++i)
        {
            resources[i].Used            = null;
            resources[i].UsedDynamically = null;
        }
    }

    private static Resource[] ReadResourceArray(ref SpanBinaryReader r, int count, ref SpanBinaryReader strings)
    {
        var ret = new Resource[count];
        for (var i = 0; i < count; ++i)
        {
            var id        = r.ReadUInt32();
            var strOffset = r.ReadUInt32();
            var strSize   = r.ReadUInt32();
            ret[i] = new Resource
            {
                Id   = id,
                Name = strings.ReadString((int)strOffset, (int)strSize),
                Slot = r.ReadUInt16(),
                Size = r.ReadUInt16(),
            };
        }

        return ret;
    }

    private static Shader[] ReadShaderArray(ref SpanBinaryReader r, int count, DisassembledShader.ShaderStage stage, DxVersion directX,
        bool disassemble, ReadOnlySpan<byte> blobs, ref SpanBinaryReader strings)
    {
        var extraHeaderSize = stage switch
        {
            DisassembledShader.ShaderStage.Vertex => directX switch
            {
                DxVersion.DirectX9  => 4,
                DxVersion.DirectX11 => 8,
                _                   => throw new NotImplementedException(),
            },
            _ => 0,
        };

        var ret = new Shader[count];
        for (var i = 0; i < count; ++i)
        {
            var blobOffset    = r.ReadUInt32();
            var blobSize      = r.ReadUInt32();
            var constantCount = r.ReadUInt16();
            var samplerCount  = r.ReadUInt16();
            var uavCount      = r.ReadUInt16();
            if (r.ReadUInt16() != 0)
                throw new NotImplementedException();

            var rawBlob = blobs.Slice((int)blobOffset, (int)blobSize);

            ret[i] = new Shader
            {
                Stage            = disassemble ? stage : DisassembledShader.ShaderStage.Unspecified,
                DirectXVersion   = directX,
                Constants        = ReadResourceArray(ref r, constantCount, ref strings),
                Samplers         = ReadResourceArray(ref r, samplerCount,  ref strings),
                Uavs             = ReadResourceArray(ref r, uavCount,      ref strings),
                AdditionalHeader = rawBlob[..extraHeaderSize].ToArray(),
                Blob             = rawBlob[extraHeaderSize..].ToArray(),
            };
        }

        return ret;
    }

    private static Key[] ReadKeyArray(ref SpanBinaryReader r, int count)
    {
        var ret = new Key[count];
        for (var i = 0; i < count; ++i)
        {
            ret[i] = new Key
            {
                Id           = r.ReadUInt32(),
                DefaultValue = r.ReadUInt32(),
                Values       = Array.Empty<uint>(),
            };
        }

        return ret;
    }

    private static Node[] ReadNodeArray(ref SpanBinaryReader r, int count, int systemKeyCount, int sceneKeyCount, int materialKeyCount,
        int subViewKeyCount)
    {
        var ret = new Node[count];
        for (var i = 0; i < count; ++i)
        {
            var selector  = r.ReadUInt32();
            var passCount = r.ReadUInt32();
            ret[i] = new Node
            {
                Selector     = selector,
                PassIndices  = r.Read<byte>(16).ToArray(),
                SystemKeys   = r.Read<uint>(systemKeyCount).ToArray(),
                SceneKeys    = r.Read<uint>(sceneKeyCount).ToArray(),
                MaterialKeys = r.Read<uint>(materialKeyCount).ToArray(),
                SubViewKeys  = r.Read<uint>(subViewKeyCount).ToArray(),
                Passes       = r.Read<Pass>((int)passCount).ToArray(),
            };
        }

        return ret;
    }

    public enum DxVersion : uint
    {
        DirectX9  = 9,
        DirectX11 = 11,
    }

    public struct Resource
    {
        public uint                                   Id;
        public string                                 Name;
        public ushort                                 Slot;
        public ushort                                 Size;
        public DisassembledShader.VectorComponents[]? Used;
        public DisassembledShader.VectorComponents?   UsedDynamically;
    }

    public struct MaterialParam
    {
        public uint   Id;
        public ushort ByteOffset;
        public ushort ByteSize;
    }

    public struct Pass
    {
        public uint Id;
        public uint VertexShader;
        public uint PixelShader;
    }

    public struct Key
    {
        public uint   Id;
        public uint   DefaultValue;
        public uint[] Values;
    }

    public struct Node
    {
        public uint   Selector;
        public byte[] PassIndices;
        public uint[] SystemKeys;
        public uint[] SceneKeys;
        public uint[] MaterialKeys;
        public uint[] SubViewKeys;
        public Pass[] Passes;
    }

    public struct NodeAlias // aka Item
    {
        public uint Selector;
        public uint Node;
    }
}
