using Luna;
using Penumbra.GameData.Files.Utility;

namespace Penumbra.GameData.Files;

public partial class ShpkFile
{
    public byte[] Write()
    {
        // Writing version 0x0E01 is not fully implemented yet as it might require recomputing NodeAliasClusters.
        if (Version > 0x0D01)
            throw new NotImplementedException($"Write support for version 0x{Version:X8} of the ShPk file format is not implemented yet.");

        if (SubViewKeys.Length != 2)
            throw new InvalidDataException();

        if (Version < 0x0D01 && (HullShaders.Length > 0 || DomainShaders.Length > 0 || GeometryShaders.Length > 0))
            throw new InvalidDataException();

        if (Version < 0x0E01 && NodeAliasClusters.Length > 0)
            throw new InvalidDataException();

        using var stream  = new MemoryStream();
        using var blobs   = new MemoryStream();
        using var strings = new StringPool();
        var       aliases = new Dictionary<uint, uint>(NodeSelectors);

        foreach (var node in Nodes)
            aliases.Remove(node.Selector);

        using (var w = new BinaryWriter(stream))
        {
            w.Write(ShPkMagic);
            w.Write(Version);
            w.Write(DirectXVersion switch
            {
                DxVersion.DirectX9  => Dx9Magic,
                DxVersion.DirectX11 => Dx11Magic,
                _                   => throw new NotImplementedException(),
            });
            var offsetsPosition = stream.Position;
            w.Write(0u); // Placeholder for file size
            w.Write(0u); // Placeholder for blobs offset
            w.Write(0u); // Placeholder for strings offset
            w.Write((uint)VertexShaders.Length);
            w.Write((uint)PixelShaders.Length);
            w.Write(MaterialParamsSize);
            w.Write((ushort)MaterialParams.Length);
            w.Write((ushort)(IsLegacy || MaterialParamsDefaults == null ? 0 : 1));
            w.Write((uint)Constants.Length);
            w.Write((ushort)Samplers.Length);
            w.Write((ushort)(IsLegacy ? 0 : Textures.Length));
            w.Write((uint)Uavs.Length);
            w.Write((uint)SystemKeys.Length);
            w.Write((uint)SceneKeys.Length);
            w.Write((uint)MaterialKeys.Length);
            w.Write((uint)Nodes.Length);
            w.Write((uint)aliases.Count);

            if (Version >= 0x0D01)
            {
                w.Write((uint)HullShaders.Length);
                w.Write((uint)DomainShaders.Length);
                w.Write((uint)GeometryShaders.Length);
            }

            if (Version >= 0x0E01)
                w.Write((uint)NodeAliasClusters.Length);

            WriteShaderArray(w, VertexShaders,   blobs, strings, Version, IsLegacy);
            WriteShaderArray(w, PixelShaders,    blobs, strings, Version, IsLegacy);
            WriteShaderArray(w, HullShaders,     blobs, strings, Version, IsLegacy);
            WriteShaderArray(w, DomainShaders,   blobs, strings, Version, IsLegacy);
            WriteShaderArray(w, GeometryShaders, blobs, strings, Version, IsLegacy);

            foreach (var materialParam in MaterialParams)
            {
                w.Write(materialParam.Id);
                w.Write(materialParam.ByteOffset);
                w.Write(materialParam.ByteSize);
            }

            if (!IsLegacy && MaterialParamsDefaults != null)
            {
                w.Write(MaterialParamsDefaults, 0, Math.Min((int)MaterialParamsSize, MaterialParamsDefaults.Length));
                if (MaterialParamsDefaults.Length < MaterialParamsSize)
                    w.Write(new byte[MaterialParamsSize - MaterialParamsDefaults.Length]);
            }

            WriteResourceArray(w, Constants, strings);
            WriteResourceArray(w, Samplers,  strings);
            if (!IsLegacy)
                WriteResourceArray(w, Textures, strings);
            WriteResourceArray(w, Uavs,      strings);

            foreach (var key in SystemKeys)
            {
                w.Write(key.Id);
                w.Write(key.DefaultValue);
            }

            foreach (var key in SceneKeys)
            {
                w.Write(key.Id);
                w.Write(key.DefaultValue);
            }

            foreach (var key in MaterialKeys)
            {
                w.Write(key.Id);
                w.Write(key.DefaultValue);
            }

            foreach (var key in SubViewKeys)
                w.Write(key.DefaultValue);

            foreach (var node in Nodes)
            {
                if (node.PassIndices.Length != 16
                 || node.SystemKeys.Length != SystemKeys.Length
                 || node.SceneKeys.Length != SceneKeys.Length
                 || node.MaterialKeys.Length != MaterialKeys.Length
                 || node.SubViewKeys.Length != SubViewKeys.Length)
                    throw new InvalidDataException();

                w.Write(node.Selector);
                w.Write(node.Passes.Length);
                w.Write(node.PassIndices);
                if (Version >= 0x0D01)
                {
                    // TODO Update when we know more about this.
                    foreach (var key in node.Unk131Keys)
                        w.Write(key);
                }
                foreach (var key in node.SystemKeys)
                    w.Write(key);
                foreach (var key in node.SceneKeys)
                    w.Write(key);
                foreach (var key in node.MaterialKeys)
                    w.Write(key);
                foreach (var key in node.SubViewKeys)
                    w.Write(key);
                foreach (var pass in node.Passes)
                {
                    w.Write(pass.Id);
                    w.Write(pass.VertexShader);
                    w.Write(pass.PixelShader);
                    if (Version >= 0x0D01)
                    {
                        w.Write(pass.HullShader);
                        w.Write(pass.DomainShader);
                        w.Write(pass.GeometryShader);
                    }
                }
            }

            foreach (var alias in aliases)
            {
                w.Write(alias.Key);
                w.Write(alias.Value);
            }

            foreach (var cluster in NodeAliasClusters)
            {
                // See the comment next to those fields' declaration.
                w.Write(cluster.SubViewValue2);
                w.Write(cluster.SubViewValue1);
                w.Write((uint)cluster.SubClusters.Length);
                w.Write(cluster.Unk141E);
                foreach (var subCluster in cluster.SubClusters)
                    w.Write(in subCluster);
            }

            w.Write(AdditionalData);

            var blobsOffset = (int)stream.Position;
            blobs.WriteTo(stream);

            var stringsOffset = (int)stream.Position;
            strings.Data.WriteTo(stream);

            var fileSize = (int)stream.Position;

            stream.Seek(offsetsPosition, SeekOrigin.Begin);
            w.Write(fileSize);
            w.Write(blobsOffset);
            w.Write(stringsOffset);
        }

        return stream.ToArray();
    }

    private static void WriteResourceArray(BinaryWriter w, Resource[] array, StringPool strings)
    {
        foreach (var buf in array)
        {
            var (strOffset, strSize) = strings.FindOrAddString(buf.Name);
            w.Write(buf.Id);
            w.Write(strOffset);
            w.Write((ushort)strSize);
            w.Write(buf.IsTexture);
            w.Write(buf.Slot);
            w.Write(buf.Size);
        }
    }

    private static void WriteShaderArray(BinaryWriter w, Shader[] array, MemoryStream blobs, StringPool strings, uint version, bool isLegacy)
    {
        foreach (var shader in array)
        {
            if (shader.IsLegacy != isLegacy)
                throw new InvalidDataException();

            var blobOffset = (int)blobs.Position;
            blobs.Write(shader.AdditionalHeader);
            blobs.Write(shader.Blob);
            var blobSize = (int)blobs.Position - blobOffset;

            w.Write(blobOffset);
            w.Write(blobSize);
            w.Write((ushort)shader.Constants.Length);
            w.Write((ushort)shader.Samplers.Length);
            w.Write((ushort)shader.Uavs.Length);
            w.Write((ushort)(isLegacy ? 0 : shader.Textures.Length));

            if (version >= 0x0D01)
            {
                // TODO Update when we know more about this.
                w.Write(shader.Unk131);
            }

            WriteResourceArray(w, shader.Constants, strings);
            WriteResourceArray(w, shader.Samplers,  strings);
            WriteResourceArray(w, shader.Uavs,      strings);
            if (!isLegacy)
                WriteResourceArray(w, shader.Textures, strings);
        }
    }
}
