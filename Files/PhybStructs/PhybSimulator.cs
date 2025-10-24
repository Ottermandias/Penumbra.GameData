using Luna;

namespace Penumbra.GameData.Files.PhybStructs;

public struct PhybSimulator
{
    public const int HeaderSize = 8 + PhybSimulatorParams.Size + 8 * 4; 

    public PhybSimulatorParams Params;

    public readonly List<PhybCollisionData> Collisions          = [];
    public readonly List<PhybCollisionData> CollisionConnectors = [];
    public readonly List<PhybChain>         Chains              = [];
    public readonly List<PhybConnector>     Connectors          = [];
    public readonly List<PhybAttract>       Attracts            = [];
    public readonly List<PhybPin>           Pins                = [];
    public readonly List<PhybSpring>        Springs             = [];
    public readonly List<PhybPostAlignment> PostAlignments      = [];

    public PhybSimulator()
    { }

    public static PhybSimulator Read(ref SpanBinaryReader simulatorPart)
    {
        var ret                 = new PhybSimulator();
        var collisions          = GetCount(ref simulatorPart, ret.Collisions);
        var collisionConnectors = GetCount(ref simulatorPart, ret.CollisionConnectors);
        var chains              = GetCount(ref simulatorPart, ret.Chains);
        var connectors          = GetCount(ref simulatorPart, ret.Connectors);
        var attracts            = GetCount(ref simulatorPart, ret.Attracts);
        var pins                = GetCount(ref simulatorPart, ret.Pins);
        var springs             = GetCount(ref simulatorPart, ret.Springs);
        var postAlignments      = GetCount(ref simulatorPart, ret.PostAlignments);

        ret.Params = simulatorPart.Read<PhybSimulatorParams>();

        var collisionsSpan = GetSpan(ref simulatorPart);
        for (var i = 0; i < collisions; ++i)
            ret.Collisions.Add(collisionsSpan.Read<PhybCollisionData>());

        var collisionConnectorsSpan = GetSpan(ref simulatorPart);
        for (var i = 0; i < collisionConnectors; ++i)
            ret.CollisionConnectors.Add(collisionConnectorsSpan.Read<PhybCollisionData>());

        var chainsSpan         = GetSpan(ref simulatorPart);
        for (var i = 0; i < chains; ++i)
            ret.Chains.Add(PhybChain.Read(ref chainsSpan, simulatorPart));

        var connectorsSpan     = GetSpan(ref simulatorPart);
        for (var i = 0; i < connectors; ++i)
            ret.Connectors.Add(connectorsSpan.Read<PhybConnector>());

        var attractsSpan       = GetSpan(ref simulatorPart);
        for (var i = 0; i < attracts; ++i)
            ret.Attracts.Add(attractsSpan.Read<PhybAttract>());

        var pinsSpan           = GetSpan(ref simulatorPart);
        for (var i = 0; i < pins; ++i)
            ret.Pins.Add(pinsSpan.Read<PhybPin>());

        var springsSpan        = GetSpan(ref simulatorPart);
        for (var i = 0; i < springs; ++i)
            ret.Springs.Add(springsSpan.Read<PhybSpring>());

        var postAlignmentsSpan = GetSpan(ref simulatorPart);
        for (var i = 0; i < postAlignments; ++i)
            ret.PostAlignments.Add(postAlignmentsSpan.Read<PhybPostAlignment>());

        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static int GetCount<T>(ref SpanBinaryReader simulatorPart, List<T> list)
    {
        var ret = simulatorPart.ReadByte();
        list.EnsureCapacity(ret);
        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static SpanBinaryReader GetSpan(ref SpanBinaryReader simulatorPart)
    {
        var offset = simulatorPart.ReadUInt32();
        if (offset == 0xCCCCCCCCu)
            offset = 4u;
        else
            offset += 4u;
        return simulatorPart.SliceFrom((int)offset, simulatorPart.Length - (int)offset);
    }
}
