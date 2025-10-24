using Luna;

namespace Penumbra.GameData.Files.PhybStructs;

public struct PhybExtendedData
{
    public PhybExtendedData()
    { }

    public  byte[] Table   = [];

    public static PhybExtendedData? Read(ReadOnlySpan<byte> data, out uint extendedOffset, out uint extendedLength)
    {
        if (data.Length < PhybExtendedPostamble.Size)
        {
            extendedOffset = (uint)data.Length;
            extendedLength = 0;
            return null;
        }

        var ret = new PhybExtendedData();
        var postAmble = new SpanBinaryReader(data).SliceFrom(data.Length - PhybExtendedPostamble.Size, PhybExtendedPostamble.Size)
            .Read<PhybExtendedPostamble>();
        if (postAmble.MagicNumber != PhybFile.MagicExtendedDataPost)
        {
            extendedOffset = (uint)data.Length;
            extendedLength = 0;
            return null;
        }


        var extended = new SpanBinaryReader(data);
        extended.Skip(data.Length - (int)postAmble.TotalSize);
        if (extended.ReadUInt32() == PhybFile.MagicExtendedDataPre)
            extended.Skip(-4); // from VFXEdit which is questionable here.

        extendedOffset = (uint)extended.Position;
        var preAmble = extended.Read<PhybExtendedPreamble>();
        extendedLength = (uint)preAmble.OffsetPost;
        ret.Table      = data.Slice(extended.Position, (int)extendedLength).ToArray();
        return ret;
    }

    public void Write(BinaryWriter w)
    {
        var padding = (int) (w.BaseStream.Position & 7);
        if (padding != 0)
        {
            padding = 8 - padding;
            ReadOnlySpan<byte> span = [0, 0, 0, 0, 0, 0, 0];
            w.Write(span[..padding]);
        }

        var pos = w.BaseStream.Position;
        w.Write(PhybFile.MagicExtendedDataPre);
        w.Write((ushort)1);
        w.Write((ushort)0);
        w.Write((ulong)Table.Length);
        w.Write(Table);
        w.Write(PhybFile.MagicExtendedDataPost);
        w.Write((ushort)1);
        w.Write((ushort)1);
        w.Write(pos - w.BaseStream.Position + 0x8);
        w.Write(w.BaseStream.Position - pos + 0x8 + padding);
    }

    public struct PhybExtendedPostamble
    {
        public const int Size = 0x18;

        public uint   MagicNumber;
        public ushort Version;
        public ushort Count;
        public ulong  OffsetStart;
        public ulong  TotalSize;
    }

    public struct PhybExtendedPreamble
    {
        public uint   MagicNumber;
        public ushort Version;
        public ushort Count;
        public ulong  OffsetPost;
    }
}
