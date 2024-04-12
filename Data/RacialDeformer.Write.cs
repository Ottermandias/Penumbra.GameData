using Penumbra.GameData.Files.Utility;

namespace Penumbra.GameData.Data;

public partial class RacialDeformer
{
    public bool Valid
        => true;

    public byte[] Write()
    {
        var matrices = DeformMatrices.ToArray();

        using var stream = new MemoryStream();
        var names = new StringPool();

        var namesOffset = 4 + matrices.Length * 50 + ((matrices.Length & 1) != 0 ? 2 : 0);

        using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
        {
            writer.Write(matrices.Length);

            foreach (var (bone, _) in matrices)
                writer.Write((ushort)names.FindOrAddString(bone).Offset);
            if ((matrices.Length & 1) != 0)
                writer.Write((ushort)0);

            foreach (var (_, matrix) in matrices)
                writer.Write(matrix);
        }

        names.WriteTo(stream);

        if ((stream.Length & 3) != 0)
            stream.Write(new byte[4 - (stream.Length & 3)]);

        return stream.ToArray();
    }
}
