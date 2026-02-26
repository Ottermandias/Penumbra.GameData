using Luna;
using Penumbra.GameData.Files.Utility;

namespace Penumbra.GameData.Files.PhybStructs;

public readonly struct PhybCollision
{
    public static PhybCollision Read(ReadOnlySpan<byte> data)
    {
        var ret = new PhybCollision();
        if (data.Length == 0)
            return ret;

        var counts = new SpanBinaryReader(data);
        var structs = new SpanBinaryReader(data[8..]);
        ReadList(ret.Capsules, ref counts, ref structs);
        ReadList(ret.Ellipsoids, ref counts, ref structs);
        ReadList(ret.NormalPlanes, ref counts, ref structs);
        ReadList(ret.ThreePointPlanes, ref counts, ref structs);
        ReadList(ret.Spheres, ref counts, ref structs);
        return ret;
    }

    public PhybCollision()
    { }

    public readonly List<PhybCapsule> Capsules = [];
    public readonly List<PhybEllipsoid> Ellipsoids = [];
    public readonly List<PhybNormalPlane> NormalPlanes = [];
    public readonly List<PhybThreePointPlane> ThreePointPlanes = [];
    public readonly List<PhybSphere> Spheres = [];

    public bool Valid
        => Capsules.Count is >= 0 and < byte.MaxValue
         && Ellipsoids.Count is >= 0 and < byte.MaxValue
         && NormalPlanes.Count is >= 0 and < byte.MaxValue
         && ThreePointPlanes.Count is >= 0 and < byte.MaxValue
         && Spheres.Count is >= 0 and < byte.MaxValue;

    public void Write(BinaryWriter w)
    {
        // Empty collisions are not written at all.
        if (Capsules.Count + Ellipsoids.Count + NormalPlanes.Count + ThreePointPlanes.Count + Spheres.Count == 0)
            return;

        w.Write((byte)Capsules.Count);
        w.Write((byte)Ellipsoids.Count);
        w.Write((byte)NormalPlanes.Count);
        w.Write((byte)ThreePointPlanes.Count);
        w.Write((byte)Spheres.Count);
        // Padding to 8.
        w.Write((byte)0xCC);
        w.Write((byte)0xCC);
        w.Write((byte)0xCC);
        foreach (var item in Capsules)
            w.Write(item);
        foreach (var item in Ellipsoids)
            w.Write(item);
        foreach (var item in NormalPlanes)
            w.Write(item);
        foreach (var item in ThreePointPlanes)
            w.Write(item);
        foreach (var item in Spheres)
            w.Write(item);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static void ReadList<T>(List<T> list, ref SpanBinaryReader counts, ref SpanBinaryReader structs)
        where T : unmanaged
    {
        var count = counts.ReadByte();
        list.Clear();
        list.EnsureCapacity(count);
        for (var i = 0; i < count; ++i)
            list.Add(structs.Read<T>());
    }
}
