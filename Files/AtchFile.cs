using OtterGui;
using Penumbra.GameData.Files.Utility;
using Penumbra.String;

namespace Penumbra.GameData.Files;

public class AtchFile : IWritable
{
    public record struct AtchState(ByteString Bone, float Scale, Vector3 Offset, Vector3 Rotation)
    {
        public static AtchState Read(ref SpanBinaryReader reader)
        {
            var stringOffset = reader.ReadInt32();
            var boneName     = reader.ReadByteString(stringOffset);
            var scale        = reader.ReadSingle();
            var offsetX      = reader.ReadSingle();
            var offsetY      = reader.ReadSingle();
            var offsetZ      = reader.ReadSingle();
            var rotationX    = reader.ReadSingle();
            var rotationY    = reader.ReadSingle();
            var rotationZ    = reader.ReadSingle();
            return new AtchState
            {
                Bone     = ByteString.FromSpanUnsafe(boneName, true, null, true).Clone(),
                Scale    = scale,
                Offset   = new Vector3(offsetX,   offsetY,   offsetZ),
                Rotation = new Vector3(rotationX, rotationY, rotationZ),
            };
        }
    }

    public record AtchEntry
    {
        public          ByteString      Name      = ByteString.Empty;
        public readonly List<AtchState> States    = [];
        public          bool            Accessory = false;

        public void ReadStates(ref SpanBinaryReader reader, ushort numStates)
        {
            States.Clear();
            States.EnsureCapacity(numStates);
            for (var i = 0; i < numStates; ++i)
                States.Add(AtchState.Read(ref reader));
        }
    }

    public readonly List<AtchEntry> Entries;

    public AtchFile(ReadOnlySpan<byte> data)
    {
        var r          = new SpanBinaryReader(data);
        var numEntries = r.ReadUInt16();
        var numStates  = r.ReadUInt16();
        Entries = new List<AtchEntry>(numEntries);
        for (var i = 0; i < numEntries; ++i)
        {
            var name = r.ReadByteString(r.Position);
            r.Skip(name.Length + 1);
            Entries.Add(new AtchEntry { Name = ByteString.FromSpanUnsafe(name, true, null, true).Revert() });
        }

        var currentUlong = 0ul;
        for (var i = 0; i < numEntries; ++i)
        {
            var bitIdx = i & 0x3F;
            if (bitIdx == 0)
                currentUlong = r.ReadUInt64();

            Entries[i].Accessory = ((currentUlong >> bitIdx) & 1) == 1;
        }

        var numBytes  = BitOperations.RoundUpToPowerOf2(numEntries) >> 3;
        var readBytes = (numEntries + 63) >> 3;
        var padding   = (int)(numBytes - readBytes);
        r.Skip(padding);

        foreach (var entry in Entries)
            entry.ReadStates(ref r, numStates);
    }


    public bool Valid
        => Entries.Count > 0 && Entries.All(e => e.Name.Length > 0 && e.States.Count == Entries[0].States.Count);

    public byte[] Write()
    {
        using var ms = new MemoryStream();
        using var w  = new BinaryWriter(ms);
        w.Write((ushort)Entries.Count);
        if (Entries.Count == 0)
        {
            w.Write((ushort)0);
            return ms.ToArray();
        }

        var firstEntry = Entries[0];
        w.Write((ushort)firstEntry.States.Count);
        foreach (var entry in Entries)
        {
            // Write reversed.
            for (var i = entry.Name.Length - 1; i >= 0; --i)
                w.Write(entry.Name[i]);
            w.Write((byte)0);
        }

        switch (ms.Position & 3)
        {
            case 3:
                w.Write((byte)0);
                break;
            case 2:
                w.Write((ushort)0);
                break;
            case 1:
                w.Write((ushort)0);
                w.Write((byte)0);
                break;
        }

        var currentUlong = 0ul;
        foreach (var (entry, i) in Entries.WithIndex())
        {
            var bitIdx = i & 0x3F;
            if (entry.Accessory)
                currentUlong |= 1ul << bitIdx;
            if (bitIdx == 0x3F)
            {
                w.Write(currentUlong);
                currentUlong = 0;
            }
        }

        if ((Entries.Count & 0x3F) != 0x3F)
            w.Write(currentUlong);

        var numBytes     = BitOperations.RoundUpToPowerOf2((uint)Entries.Count) >> 3;
        var writtenBytes = (uint)(Entries.Count + 63) >> 3;
        var paddingLongs = (numBytes - writtenBytes) >> 3;
        for (var i = 0; i < paddingLongs; ++i)
            w.Write((ulong)0);

        var stringStart = ms.Position + 32 * Entries.Count * firstEntry.States.Count;
        var pool        = new StringPool();
        foreach (var entry in Entries)
        {
            if (entry.States.Count != firstEntry.States.Count)
                throw new Exception(
                    $".atch file is invalid: different number of states in entry {entry.Name} compared to first entry {firstEntry.Name}.");

            foreach (var state in entry.States)
            {
                var offset = pool.FindOrAddString(state.Bone);
                w.Write((uint)(offset + stringStart));
                w.Write(state.Scale);
                w.Write(state.Offset.X);
                w.Write(state.Offset.Y);
                w.Write(state.Offset.Z);
                w.Write(state.Rotation.X);
                w.Write(state.Rotation.Y);
                w.Write(state.Rotation.Z);
            }
        }

        pool.WriteTo(ms);
        return ms.ToArray();
    }
}
