using Dalamud.Plugin.Services;
using Luna;
using Penumbra.GameData.Files.StainMapStructs;
using Penumbra.GameData.Structs;

namespace Penumbra.GameData.Files;

public readonly record struct StmKeyType(uint Value)
{
    public static implicit operator StmKeyType(int v)
        => new((uint)v);

    public static implicit operator StmKeyType(uint v)
        => new(v);

    public static implicit operator StmKeyType(ushort v)
        => new(v);

    public int Int
        => (int)Value;

    public ushort UShort
        => (ushort)Value;

    public override string ToString()
        => Value.ToString();
}

public partial class StmFile<TDyePack> where TDyePack : unmanaged, IDyePack
{
    public const string LegacyPath = LegacyDyePack.DefaultStmPath;
    public const string GudPath    = DyePack.DefaultStmPath;

    /// <summary>
    /// All currently available dyeing templates with their IDs.
    /// </summary>
    public readonly IReadOnlyDictionary<StmKeyType, StainingTemplateEntry> Entries;

    /// <summary>
    /// Access a specific dye pack.
    /// </summary>
    /// <param name="template">The ID of the accessed template.</param>
    /// <param name="idx">The ID of the Stain.</param>
    /// <returns>The corresponding color set information or a defaulted DyePack of 0-entries.</returns>
    public TDyePack this[StmKeyType template, int idx]
        => Entries.TryGetValue(template, out var entry) ? entry[idx] : default;

    /// <inheritdoc cref="this[StmKeyType, StainId]"/>
    public TDyePack this[StmKeyType template, StainId idx]
        => this[template, (int)idx.Id];

    /// <summary>
    /// Try to access a specific dye pack.
    /// </summary>
    /// <param name="template">The ID of the accessed template.</param>
    /// <param name="idx">The ID of the Stain.</param>
    /// <param name="dyes">On success, the corresponding color set information, otherwise a defaulted DyePack.</param>
    /// <returns>True on success, false otherwise.</returns>
    public bool TryGetValue(StmKeyType template, StainId idx, out TDyePack dyes)
    {
        if (idx.Id is > 0 and <= StainingTemplateEntry.NumElements && Entries.TryGetValue(template, out var entry))
        {
            dyes = entry[idx];
            return true;
        }

        dyes = default;
        return false;
    }

    /// <inheritdoc cref="TryGetValue(StmKeyType, StainId, out TDyePack)"/>
    /// <returns>On success, the corresponding color set information, otherwise null.</returns>
    public TDyePack? GetValueOrNull(StmKeyType template, StainId idx)
        => TryGetValue(template, idx, out var dyes) ? dyes : null;

    /// <summary>
    /// Create an STM file from the given data array.
    /// </summary>
    public StmFile(byte[] data)
        : this(data.AsSpan())
    { }

    public unsafe StmFile(ReadOnlySpan<byte> data)
    {
        var br    = new SpanBinaryReader(data);
        var magic = br.ReadUInt16();
        if (magic != 0x534D)
            throw new InvalidDataException($"Invalid STM magic number 0x{magic:X4}");

        var version    = br.ReadUInt16();
        var numEntries = br.ReadUInt16();
        var numColors  = br.ReadByte();
        var numScalars = br.ReadByte();

        switch (version)
        {
            case 0x0101:
                if (numColors != 0 || numScalars != 0)
                    throw new InvalidDataException($"Unexpected column counts in STM v1.1 file: {numColors} colors, {numScalars} scalars");

                numColors  = 3;
                numScalars = 2;
                break;
            case 0x200: break;
            case 0x201: break;
            default:    throw new InvalidDataException($"Unrecognized STM version v{version >> 2}.{version & 0xFF}");
        }

        if (numColors != TDyePack.ColorCount || numScalars != TDyePack.ScalarCount)
            throw new InvalidDataException(
                $"Dye pack type {typeof(TDyePack)} expects STM file to have {TDyePack.ColorCount} colors and {TDyePack.ScalarCount} scalars per row, got file with {numColors} colors and {numScalars} scalars");

        if (numColors * 6 + numScalars * 2 != sizeof(TDyePack))
            throw new InvalidOperationException(
                $"Dye pack type {typeof(TDyePack)} has a size of {sizeof(TDyePack)} bytes, but expects {numColors} colors and {numScalars} scalars, that is {numColors * 6 + numScalars * 2} bytes");

        var keys    = br.Read<StmKeyType>(numEntries);
        var offsets = br.Read<StmKeyType>(numEntries);

        var lengths = new int[numEntries]; // in bytes
        for (var i = 1; i < numEntries; ++i)
            lengths[i - 1] = (offsets[i].Int - offsets[i - 1].Int) << 1;
        lengths[numEntries - 1] = br.Remaining - (offsets[numEntries - 1].Int << 1);

        var entries = new Dictionary<StmKeyType, StainingTemplateEntry>(numEntries);
        Entries = entries;

        br.SliceFromHere(offsets[0].Int);
        for (var i = 0; i < numEntries; ++i)
            entries.Add(keys[i], new StainingTemplateEntry(br.SliceFromHere(lengths[i])));
    }

    /// <summary>
    /// Try to read and parse the default STM file given by Lumina.
    /// </summary>
    public StmFile(IDataManager gameData)
        : this(gameData.GetFile(TDyePack.DefaultStmPath)?.Data ?? [])
    { }
}
