using Dalamud.Plugin.Services;
using Luna;
using Penumbra.GameData.Structs;
using Penumbra.String.Classes;
using static Penumbra.GameData.Data.GamePaths;

namespace Penumbra.GameData.Files.Materials;

public abstract class FileMigrationContext(IDataManager gameData)
{
    public readonly record struct ResolvedPathData(string Mod, Guid Container, string File, string Swap);

    public readonly IDataManager           GameData = gameData;
    public abstract List<ResolvedPathData> ResolveGamePath(Utf8GamePath gamePath);
    public readonly List<MigrationAction>  Actions = [];

    public readonly Lazy<MtrlFile> GlassReferenceFile = new(() =>
    {
        var bytes = gameData.GetFile("chara/equipment/e5001/material/v0001/mt_c0101e5001_met_b.mtrl");
        return new MtrlFile(bytes!.Data);
    });

    public readonly Lazy<MtrlFile> HairReferenceFile = new(() =>
    {
        var bytes = gameData.GetFile("chara/human/c0801/obj/hair/h0115/material/v0001/mt_c0801h0115_hir_a.mtrl");
        return new MtrlFile(bytes!.Data);
    });
}

public abstract record MigrationAction;
public sealed record DeleteFileAction(string Filename) : MigrationAction;
public sealed record SetRedirection(string Mod, Guid Container, Utf8GamePath GamePath, string FilePath) : MigrationAction;
public sealed record SetFileSwap(string Mod, Guid Container, Utf8GamePath GamePath, string TargetPath) : MigrationAction;

public sealed record CreateIndexSampler(string NormalSampler, string IndexFilePath) : MigrationAction
{
    /// <summary> Reads and replaces the pixel data from normal sampler to index map. </summary>
    /// <exception cref="InvalidDataException"> The width and height have to be sized to powers of two, and the pixel array has to have correct size.</exception>
    public void CreateIndexData(byte[] pixelData, int width, int height)
    {
        const int rowBlend = 17;
        if (!BitOperations.IsPow2(width))
            throw new InvalidDataException("The normal texture data for creating an index file must have a width that is a power of two.");
        if (!BitOperations.IsPow2(height))
            throw new InvalidDataException("The normal texture data for creating an index file must have a height that is a power of two.");
        if (pixelData.Length != 4 * width * height)
            throw new InvalidDataException("The normal texture data for creating an index file has invalid size.");

        Parallel.For(0, pixelData.Length / 4, idx =>
        {
            var pixelIdx = 4 * idx;
            var normal   = pixelData[pixelIdx + 3];

            // Copied from TT
            var blendRem    = normal % (2 * rowBlend);
            var originalRow = normal / rowBlend;
            switch (blendRem)
            {
                // Goes to next row, clamped to the closer row.
                case > 25:
                    blendRem = 0;
                    ++originalRow;
                    break;
                // Stays in this row, clamped to the closer row.
                case > 17: blendRem = 17; break;
            }

            var newBlend = (byte)(255 - MathF.Round(blendRem / 17f * 255f));

            // Slight add here to push the color deeper into the row to ensure BC5 compression doesn't
            // cause any artifacting.
            var newRow = (byte)(originalRow / 2 * 17 + 4);

            pixelData[pixelIdx]     = newRow;
            pixelData[pixelIdx + 1] = newBlend;
            pixelData[pixelIdx + 2] = 0;
            pixelData[pixelIdx + 3] = 255;
        });
    }
}

public sealed record UpdateGearMaskSampler(string MaskPath, bool CharacterGlass) : MigrationAction
{
    public void ConvertGearMask(byte[] pixelData, int width, int height)
    {
        if (pixelData.Length != 4 * width * height)
            throw new InvalidDataException("The gear mask texture data to be updated has invalid size.");

        Parallel.For(0, pixelData.Length / 4, idx =>
        {
            var pixelIdx = 4 * idx;
            var green    = pixelData[pixelIdx + 1];

            (pixelData[pixelIdx], pixelData[pixelIdx + 2]) = (pixelData[pixelIdx + 2], pixelData[pixelIdx]);
            if (CharacterGlass)
                pixelData[pixelIdx + 1] = (byte)Math.Max(1, 255 - green);
        });
    }
}

public sealed record UpdateHairSamplers(string NormalPath, string MaskPath) : MigrationAction
{
    public void Convert(byte[] normal, byte[] mask, int width, int height)
    {
        if (normal.Length != 4 * width * height)
            throw new InvalidDataException("The hair normal texture data to be updated has invalid size.");
        if (normal.Length != mask.Length)
            throw new InvalidDataException("The hair mask texture data to be updated has invalid size.");

        Parallel.For(0, normal.Length / 4, idx =>
        {
            var pixelIdx = idx * 4;
            var oldRed   = mask[pixelIdx];

            normal[pixelIdx + 2] = mask[pixelIdx + 3]; // Map mask Alpha to normal Blue.
            mask[pixelIdx]       = mask[pixelIdx + 1]; // Map mask Green to mask Red.
            mask[pixelIdx + 2]   = 49;     // Map constant 49 to mask Blue.
            mask[pixelIdx + 3]   = oldRed; // Map mask Red to mask Alpha.
            // Scale inverted mask Red (roughness) to [10, 255] and map it to mask Green.
            mask[pixelIdx + 1] = (byte)Math.Clamp(Math.Round((255 - oldRed) * 245f / 255f + 10f), 0, 255);
        });
    }
}

public sealed class MaterialAnalyzer(MtrlFile material)
{
    public const           uint      PreDawntrailHairShaderConstant1 = 0x36080AD0;
    public const           uint      PreDawntrailHairShaderConstant2 = 0x992869AB;
    public static readonly HalfColor DefaultGlassSpecular            = new((Half)0.8100586, (Half)0.8100586, (Half)0.8100586);

    public readonly ShaderFamily Shader              = ShaderFamily.Identify(material.ShaderPackage.Name);
    public readonly bool         WasLegacyColorTable = material.Table is LegacyColorTable;

    /// <summary> Identify whether a material is assured to be a pre-Dawntrail or post-Dawntrail material, or can not be uniquely identified. </summary>
    /// <returns> True if the material is pre-Dawntrail, false if it is post-Dawntrail, null if we are not sure. </returns>
    public bool? IsPreDawntrail()
    {
        if (material.Table is LegacyColorTable || material.DyeTable is LegacyColorDyeTable)
            return true;

        if (Shader is not ShaderFamily.Hair)
            return material.Table is OpaqueColorTable or null && material.DyeTable is OpaqueColorDyeTable or null ? null : false;

        return material.ShaderPackage.Constants.Any(v => v.Id is PreDawntrailHairShaderConstant1, v => v.Id is PreDawntrailHairShaderConstant2);
    }

    public bool Migrate(FileMigrationContext context, bool force)
    {
        context.Actions.Clear();
        if (!(IsPreDawntrail() ?? force))
            return false;

        var ret = MigrateColorTable();
        ret |= MigrateShaderPackage(context);
        ret |= MigrateTexturePaths();
        ret |= MigrateSamplerCompatibility();
        ret |= MigrateNormalData(context);
        ret |= MigrateGearMask(context);

        context.Actions.RemoveDuplicates();

        return ret;
    }

    public bool MigrateColorTable()
    {
        var ret = false;
        if (material.Table is LegacyColorTable)
        {
            var newColorTable = new ColorTable(material.Table);
            // From TT.
            if (Shader is ShaderFamily.CharacterGlass)
                foreach (ref var row in newColorTable.AsRows())
                    row.SpecularColor = DefaultGlassSpecular;
            material.Table = newColorTable;
            ret            = true;
        }

        if (material.DyeTable is LegacyColorDyeTable)
        {
            var newDyeTable = new ColorDyeTable(material.DyeTable);
            // From TT.
            if (Shader is not ShaderFamily.Character and not ShaderFamily.CharacterLegacy)
                foreach (ref var row in newDyeTable.AsRows())
                    row.Template += 1000;
            material.DyeTable = newDyeTable;
            ret               = true;
        }

        material.UpdateFlags();

        return ret;
    }

    public bool MigrateShaderPackage(FileMigrationContext context)
        => Shader switch
        {
            ShaderFamily.Character      => MigrateCharacter(),
            ShaderFamily.CharacterGlass => MigrateCharacterGlass(context),
            ShaderFamily.Hair           => MigrateHair(context),
            _                           => false,
        };

    private bool MigrateCharacter()
    {
        material.ShaderPackage.Name = ShaderFamily.CharacterLegacy.ToName();
        return true;
    }

    private bool MigrateCharacterGlass(FileMigrationContext context)
    {
        var reference = context.GlassReferenceFile.Value;
        material.ShaderPackage.ShaderKeys =  [.. reference.ShaderPackage.ShaderKeys];
        material.ShaderPackage.Constants  =  [.. reference.ShaderPackage.Constants];
        material.AdditionalData           =  [.. reference.AdditionalData];
        material.ShaderPackage.Flags      &= ~(0x04u | 0x08u);
        return true;
    }

    private bool MigrateHair(FileMigrationContext context)
    {
        const uint alphaThresholdId = 0x29AC0223;

        if (material.FindSampler(ShpkFile.NormalSamplerId) < 0 || material.FindSampler(ShpkFile.MaskSamplerId) < 0)
            return false;

        var reference = context.HairReferenceFile.Value;

        var oldAlpha = material.ShaderPackage.Constants.FirstOrNull(c => c.Id is alphaThresholdId);
        material.ShaderPackage.Constants = [.. reference.ShaderPackage.Constants];
        material.AdditionalData          = [.. reference.AdditionalData];
        var alphaConstant = material.ShaderPackage.Constants.IndexOf(c => c.Id is alphaThresholdId);
        if (alphaConstant >= 0 && oldAlpha is { } value)
        {
            ref var alpha = ref material.ShaderPackage.Constants[alphaConstant];
            alpha.ByteOffset = value.ByteOffset;
            alpha.ByteSize   = value.ByteSize;
        }

        return true;
    }

    public bool MigrateTexturePaths()
    {
        var ret = false;
        // Remove DX11 flags and update paths if necessary.
        foreach (ref var tex in material.Textures.AsSpan())
        {
            if (!tex.DX11)
                continue;

            ret = true;
            if (Tex.HandleDx11Path(tex, out var newPath))
                tex.Path = newPath;

            tex.DX11 = false;
        }

        return ret;
    }

    public bool MigrateSamplerCompatibility()
    {
        var specular = material.FindSampler(ShpkFile.SpecularSamplerId);
        var diffuse  = material.FindSampler(ShpkFile.DiffuseSamplerId);
        if (specular < 0 || diffuse < 0)
            return false;

        material.ShaderPackage.Samplers[specular].SamplerId             = ShpkFile.MaskSamplerId;
        material.GetOrAddShaderKey(0xC8BD1DEF, 0x198D11CD, out _).Value = 0x198D11CD;
        material.GetOrAddShaderKey(0xB616DC5A, 0x600EF9DF, out _).Value = 0x600EF9DF;
        return true;
    }

    public bool MigrateNormalData(FileMigrationContext context)
    {
        // Skip materials that already have an index sampler.
        if (material.FindSampler(ShpkFile.IndexSamplerId) >= 0 || !WasLegacyColorTable)
            return false;

        var normal = material.FindSampler(ShpkFile.NormalSamplerId);
        if (normal < 0)
            return false;

        ref var sampler = ref material.ShaderPackage.Samplers[normal];
        if (sampler.TextureIndex >= material.Textures.Length)
            return false;

        ref var texture = ref material.Textures[sampler.TextureIndex];
        if (!Utf8GamePath.FromString(texture.Path,               out var normalGamePath)
         || !Utf8GamePath.FromString(GetIndexName(texture.Path), out var indexGamePath))
            throw new Exception("Could not create Game Path.");

        var resolvedPaths = context.ResolveGamePath(normalGamePath);
        var visitedPaths  = new Dictionary<string, string>(resolvedPaths.Count);
        foreach (var pathData in resolvedPaths)
        {
            if (pathData.Swap.Length > 0)
                // Best effort, replace the existing file swap with what may or may not be the new index texture to the prior default material.
                context.Actions.Add(new SetFileSwap(pathData.Mod, pathData.Container, indexGamePath, GetIndexName(pathData.Swap)));

            if (pathData.File.Length > 0)
            {
                // If we have already visited this particular path, we only need to add redirections.
                // Otherwise, we need to create the index map and remove the normal map.
                if (!visitedPaths.TryGetValue(pathData.File, out var indexName))
                {
                    indexName = GetIndexName(pathData.File);
                    visitedPaths.Add(pathData.File, indexName);
                    context.Actions.Add(new CreateIndexSampler(pathData.File, indexName));
                }

                context.Actions.Add(new SetRedirection(pathData.Mod, pathData.Container, indexGamePath, indexName));
            }
        }

        // TT sets default flags and only copies UV tiling, we copy everything.
        material.GetOrAddSampler(ShpkFile.IndexSamplerId, GetIndexName(texture.Path)).Flags = sampler.Flags;
        return true;
    }

    public bool MigrateGearMask(FileMigrationContext context)
    {
        if (!WasLegacyColorTable)
            return false;
        if (Shader is not (ShaderFamily.Character or ShaderFamily.CharacterLegacy or ShaderFamily.CharacterGlass))
            return false;

        // Use mask-as-specular compatibility mode.
        if (material.GetShaderKey(0xC8BD1DEF)?.Value is 0xA02F4828 or 0x198D11CD)
            return false;

        var samplerIdx = material.FindSampler(ShpkFile.MaskSamplerId);
        if (samplerIdx < 0)
            return false;

        ref readonly var sampler = ref material.ShaderPackage.Samplers[samplerIdx];
        if (sampler.TextureIndex >= material.Textures.Length)
            return false;

        ref readonly var texture = ref material.Textures[sampler.TextureIndex];
        if (!Utf8GamePath.FromString(texture.Path, out var maskPath))
            return false;

        var resolved = context.ResolveGamePath(maskPath);
        var visited  = new HashSet<string>(resolved.Count);
        foreach (var file in resolved.Where(file => file.File.Length > 0 && visited.Add(file.File)))
            context.Actions.Add(new UpdateGearMaskSampler(file.File, Shader is ShaderFamily.CharacterGlass));

        return visited.Count > 0;
    }

    public static string GetIndexName(string normalPath)
    {
        // TODO 20261018: maybe make the filename generation smarter.
        var span = normalPath.AsSpan();
        if (span.EndsWith(".tex"))
        {
            span = span[..^4];
            if (span.EndsWith("_n"))
                return $"{span[..^2]}_id.tex";
            if (span.EndsWith("_norm"))
                return $"{span[..^5]}_id.tex";

            return $"{span}_id.tex";
        }

        return normalPath + "_id.tex";
    }
}
