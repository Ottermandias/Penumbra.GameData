using Luna;

namespace Penumbra.GameData.Files.PhybStructs;

public struct PhybChain
{
    public float     Dampening;
    public float     MaxSpeed;
    public float     Friction;
    public float     CollisionDampening;
    public float     RepulsionStrength;
    public Vector3   LastBoneOffset;
    public ChainType Type;

    public readonly List<PhybCollisionData> Collisions = [];
    public readonly List<PhybNode>          Nodes      = [];

    public PhybChain()
    { }

    public static PhybChain Read(ref SpanBinaryReader r, SpanBinaryReader simulatorPart)
    {
        var ret           = new PhybChain();
        var numCollisions = r.ReadUInt16();
        var numNodes      = r.ReadUInt16();

        ret.Dampening          = r.ReadSingle();
        ret.MaxSpeed           = r.ReadSingle();
        ret.Friction           = r.ReadSingle();
        ret.CollisionDampening = r.ReadSingle();
        ret.RepulsionStrength  = r.ReadSingle();
        ret.LastBoneOffset     = r.Read<Vector3>();
        ret.Type               = r.Read<ChainType>();
        var collisionOffset = (int)(r.ReadUInt32() + 4);
        var nodeOffset      = (int)(r.ReadUInt32() + 4);

        var slice = simulatorPart.SliceFrom(collisionOffset, simulatorPart.Length - collisionOffset);
        ret.Collisions.EnsureCapacity(numCollisions);
        for (var i = 0; i < numCollisions; ++i)
            ret.Collisions.Add(slice.Read<PhybCollisionData>());

        slice = simulatorPart.SliceFrom(nodeOffset, simulatorPart.Length - nodeOffset);
        ret.Nodes.EnsureCapacity(numNodes);
        for (var i = 0; i < numNodes; ++i)
            ret.Nodes.Add(slice.Read<PhybNode>());
        return ret;
    }

    public void WriteHeader(BinaryWriter w)
    {
        w.Write((ushort)Collisions.Count);
        w.Write((ushort)Nodes.Count);
        w.Write(Dampening);
        w.Write(MaxSpeed);
        w.Write(Friction);
        w.Write(CollisionDampening);
        w.Write(RepulsionStrength);
        w.Write(LastBoneOffset);
        w.Write(Type);
        w.Write(0ul);
    }

    public enum ChainType : uint
    {
        Sphere  = 0,
        Capsule = 1,
    }
}
