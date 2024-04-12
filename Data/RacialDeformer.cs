using Penumbra.GameData.Files;
using Penumbra.GameData.Files.Utility;

namespace Penumbra.GameData.Data;

/// <remarks> This type is, notably, part of the PBD file format. </remarks>
public sealed partial class RacialDeformer : ICloneable, IWritable
{
    public readonly Dictionary<string, TransformMatrix> DeformMatrices = [];

    public bool IsEmpty
        => DeformMatrices.Count == 0;

    public RacialDeformer()
    {
    }

    public RacialDeformer(ReadOnlySpan<byte> data)
    {
        var reader = new SpanBinaryReader(data);

        var bones = new string[reader.ReadInt32()];

        for (var i = 0; i < bones.Length; i++)
            bones[i] = reader.ReadString(reader.ReadUInt16());
        if ((bones.Length & 1) != 0)
            reader.ReadUInt16();

        foreach (var bone in bones)
            DeformMatrices.Add(bone, reader.Read<TransformMatrix>());
    }

    public RacialDeformer Clone()
    {
        var clone = new RacialDeformer();

        foreach (var (bone, matrix) in DeformMatrices)
            clone.DeformMatrices.Add(bone, matrix);

        return clone;
    }

    object ICloneable.Clone()
        => Clone();

    public void IntersectWith(RacialDeformer deformer)
    {
        foreach (var bone in DeformMatrices.Keys.Where(key => !deformer.DeformMatrices.ContainsKey(key)).ToList())
            DeformMatrices.Remove(bone);
    }

    public void AddDefaults(RacialDeformer deformer)
    {
        foreach (var (bone, matrix) in deformer.DeformMatrices)
            DeformMatrices.TryAdd(bone, matrix);
    }

    public void Append(RacialDeformer deformer, bool addMissing = true)
    {
        foreach (var (bone, matrix) in deformer.DeformMatrices)
        {
            if (DeformMatrices.TryGetValue(bone, out var thisMatrix))
                DeformMatrices[bone] = thisMatrix.Append(in matrix);
            else if (addMissing)
                DeformMatrices.Add(bone, matrix);
        }
    }

    public void Prepend(RacialDeformer deformer, bool addMissing = true)
    {
        foreach (var (bone, matrix) in deformer.DeformMatrices)
        {
            if (DeformMatrices.TryGetValue(bone, out var thisMatrix))
                DeformMatrices[bone] = thisMatrix.Prepend(in matrix);
            else if (addMissing)
                DeformMatrices.Add(bone, matrix);
        }
    }

    public void Invert()
    {
        foreach (var (bone, matrix) in DeformMatrices.ToArray())
            DeformMatrices[bone] = matrix.Invert();
    }
}
