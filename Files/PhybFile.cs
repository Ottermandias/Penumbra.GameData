using Luna;
using Penumbra.GameData.Files.PhybStructs;

namespace Penumbra.GameData.Files;

public class PhybFile
{
    public const uint MagicExtendedDataPost = 0x4B434150u;
    public const uint MagicExtendedDataPre  = 0x42485045u;

    public uint Version;
    public uint DataType;

    public PhybCollision       Collision;
    public List<PhybSimulator> Simulators = [];
    public PhybExtendedData?   Extended;

    public PhybFile(ReadOnlySpan<byte> data)
    {
        var r = new SpanBinaryReader(data);
        Version  = r.ReadUInt32();
        DataType = Version > 0 ? r.ReadUInt32() : 0u;
        var collisionOffset = r.ReadUInt32();
        var simOffset       = r.ReadUInt32();
        Collision = PhybCollision.Read(data[(int)collisionOffset..(int)simOffset]);

        // Check for extended data
        Extended = PhybExtendedData.Read(data, out var extendedOffset, out _);
        var simSize = extendedOffset - simOffset;
        if (simSize > 0)
        {
            var simulatorPart = r.SliceFrom((int)simOffset, (int)simSize);
            var numSimulators = simulatorPart.ReadUInt32();
            Simulators.EnsureCapacity((int)numSimulators);
            for (var i = 0; i < numSimulators; ++i)
                Simulators.Add(PhybSimulator.Read(ref simulatorPart));
        }
    }

    public void Write(BinaryWriter w)
    {
        if (Version == 0)
        {
            w.Write(0u);
            w.Write(0x0Cu);
            w.Write(0x0Cu);
            return;
        }

        w.Write(Version);
        w.Write(DataType);

        w.Write((uint)(w.BaseStream.Position + 8)); // CollisionOffset
        var simulatorOffsetPos = (int)w.BaseStream.Position;
        w.Write(0u); // SimulatorOffset placeholder

        Collision.Write(w);
        var simulatorOffset = (uint)w.BaseStream.Position;
        w.Write((uint)Simulators.Count);
        WriteSimulatorSection(w);

        Extended?.Write(w);
        w.Seek(simulatorOffsetPos, SeekOrigin.Begin);
        w.Write(simulatorOffset);
    }

    private void WriteSimulatorSection(BinaryWriter w)
    {
        var sectionPos = w.BaseStream.Position;
        var dataPos    = sectionPos + Simulators.Count * PhybSimulator.HeaderSize;
        var offsets    = new uint[Simulators.Count, 8];
        w.Seek((int)dataPos, SeekOrigin.Begin);

        WriteList(s => s.Collisions,          0);
        WriteList(s => s.CollisionConnectors, 1);

        var chainPos  = w.BaseStream.Position;
        var numChains = 0;
        foreach (var (idx, simulator) in Simulators.Index())
        {
            numChains       += simulator.Chains.Count;
            offsets[idx, 2] =  simulator.Chains.Count > 0 ? (uint)(w.BaseStream.Position - sectionPos) : 0u;
            foreach (var chain in simulator.Chains)
                chain.WriteHeader(w);
        }

        WriteList(s => s.Connectors,     3);
        WriteList(s => s.Attracts,       4);
        WriteList(s => s.Pins,           5);
        WriteList(s => s.Springs,        6);
        WriteList(s => s.PostAlignments, 7, 0xCCCCCCCCu);

        // Unsure if collisions need to go here.
        var chainOffsets = new (uint Collisions, uint Nodes)[numChains];
        foreach (var (idx, chain) in Simulators.SelectMany(s => s.Chains).Index())
        {
            chainOffsets[idx].Collisions = chain.Collisions.Count > 0 ? (uint)(w.BaseStream.Position - sectionPos) : 0u;
            foreach (var collision in chain.Collisions)
                w.Write(collision);
        }

        foreach (var (idx, chain) in Simulators.SelectMany(s => s.Chains).Index())
        {
            chainOffsets[idx].Nodes = chain.Nodes.Count > 0 ? (uint)(w.BaseStream.Position - sectionPos) : 0u;
            foreach (var node in chain.Nodes)
                w.Write(node);
        }

        var end = (int)w.BaseStream.Position;
        w.Seek((int)sectionPos, SeekOrigin.Begin);
        foreach (var (idx, simulator) in Simulators.Index())
        {
            w.Write((byte)simulator.Collisions.Count);
            w.Write((byte)simulator.CollisionConnectors.Count);
            w.Write((byte)simulator.Chains.Count);
            w.Write((byte)simulator.Connectors.Count);
            w.Write((byte)simulator.Attracts.Count);
            w.Write((byte)simulator.Pins.Count);
            w.Write((byte)simulator.Springs.Count);
            w.Write((byte)simulator.PostAlignments.Count);
            w.Write(simulator.Params);
            for (var i = 0; i < 8; ++i)
                w.Write(offsets[idx, i]);
        }

        w.Seek((int)chainPos, SeekOrigin.Begin);
        foreach (var (collisionOffset, nodeOffset) in chainOffsets)
        {
            w.Seek(40, SeekOrigin.Current);
            w.Write(collisionOffset);
            w.Write(nodeOffset);
        }

        w.Seek(end, SeekOrigin.Begin);
        return;


        void WriteList<T>(Func<PhybSimulator, List<T>> select, int slot, uint filler = 0u) where T : unmanaged
        {
            foreach (var (idx, simulator) in Simulators.Index())
            {
                var list = select(simulator);
                offsets[idx, slot] = list.Count > 0 ? (uint)(w.BaseStream.Position - sectionPos) : filler;
                foreach (var item in list)
                    w.Write(item);
            }
        }
    }
}
