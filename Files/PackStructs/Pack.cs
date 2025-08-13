using Penumbra.GameData.Files.PackStructs;

/// <summary> A pack with arbitrary data. </summary>
public ref struct Pack
{
    /// <summary> The footer for this pack. </summary>
    public PackHeader Header;

    /// <summary> The data of this pack. </summary>
    public ReadOnlySpan<byte> Data;

    /// <summary> Write a simple pack with one actual pack structure. </summary>
    /// <param name="writer"> The writer to write to. </param>
    /// <param name="type"> The type to write for the single pack structure. </param>
    /// <param name="version"> The version to write into the footers. </param>
    /// <param name="data"> The actual data to write. </param>
    public static unsafe void Write(BinaryWriter writer, uint type, ushort version, ReadOnlySpan<byte> data)
    {
        writer.Write(type);
        writer.Write(version);
        writer.Write((ushort)0);
        writer.Write((long)data.Length);
        writer.Write(data);
        writer.Write(PackFooter.PackType);
        writer.Write(version);
        writer.Write((ushort)1);
        long offset = sizeof(PackHeader) + data.Length;
        writer.Write(-offset);
        writer.Write((ulong)(offset + sizeof(PackHeader) + sizeof(ulong)));
    }
}
