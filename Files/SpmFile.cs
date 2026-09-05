using System.Collections.Frozen;
using System.Collections.Immutable;
using Dalamud.Plugin.Services;

namespace Penumbra.GameData.Files;

using ColumnTuple = (SpmFile.Column Name, SpmFile.Type Type, SpmFile.Value? ConstantValue);
using RowTuple = (SpmFile.Table Table, uint Index, bool IsBlank, ImmutableArray<SpmFile.Value> Values);

public partial class SpmFile
{
    public readonly uint Version;

    public readonly ImmutableArray<ColumnTuple> Columns;
    public readonly ImmutableArray<RowTuple>    Rows;

    public readonly FrozenDictionary<Column, int>                    ColumnDictionary;
    public readonly FrozenDictionary<(Table Table, uint Index), int> RowDictionary;

    public SpmFile(byte[] data)
        : this((ReadOnlySpan<byte>)data)
    { }

    public unsafe SpmFile(ReadOnlySpan<byte> data)
    {
        if (data.Length < sizeof(Header))
            throw new InvalidDataException($"Cannot extract SPM header ({sizeof(Header)} bytes): file too short ({data.Length} bytes)");

        ref readonly var header = ref MemoryMarshal.Cast<byte, Header>(data)[0];

        Version = header.Version;
        if (Version is not 0x01000000u)
            throw new InvalidDataException($"Cannot read SPM file of version 0x{Version:X8}");

        var columnSpan =
            MemoryMarshal.Cast<byte, ColumnDefinition>(data.Slice(header.ColumnsOffset << 2, header.ColumnCount * sizeof(ColumnDefinition)));
        var rowSpan = MemoryMarshal.Cast<byte, RowDefinition>(data.Slice(header.RowsOffset << 2, header.RowCount * sizeof(RowDefinition)));
        var valueSpan =
            MemoryMarshal.Cast<byte, Value>(data.Slice(header.ValuesOffset << 2, header.ColumnCount * header.RowCount * sizeof(Value)));

        var rows = new List<RowTuple>();
        for (var i = 0; i < header.RowCount; ++i)
        {
            ref readonly var definition = ref rowSpan[i];

            var values  = valueSpan.Slice(header.ColumnCount * i, header.ColumnCount).ToImmutableArray();
            var isBlank = MemoryMarshal.AsBytes(values.AsSpan()).IndexOfAnyExcept((byte)0) < 0;

            rows.Add((definition.Table, definition.Index, isBlank, values));
        }

        Rows          = [.. rows];
        RowDictionary = rows.Index().ToFrozenDictionary(entry => (entry.Item.Table, entry.Item.Index), entry => entry.Index);

        var columns = new List<ColumnTuple>();
        for (var i = 0; i < header.ColumnCount; ++i)
        {
            ref readonly var definition = ref columnSpan[i];

            var firstFound = false;
            var first      = default(Value);
            var isConstant = true;
            foreach (var row in Rows)
            {
                if (row.IsBlank)
                    continue;

                if (!firstFound)
                {
                    first      = row.Values[i];
                    firstFound = true;
                    continue;
                }

                if (row.Values[i] != first)
                {
                    isConstant = false;
                    break;
                }
            }

            columns.Add((definition.Name, definition.Type, isConstant ? first : null));
        }

        Columns          = [.. columns];
        ColumnDictionary = columns.Index().ToFrozenDictionary(entry => entry.Item.Name, entry => entry.Index);
    }

    /// <summary>
    /// Try to read and parse the default SPM file given by Lumina for the given table.
    /// </summary>
    public SpmFile(IDataManager gameData, Table table)
        : this(gameData.GetFile(DefaultSpmPath(table))?.Data ?? [])
    { }

    [StructLayout(LayoutKind.Sequential)]
    private struct Header
    {
        public uint   Version;
        public byte   ColumnCount;
        public byte   RowCount;
        public ushort ColumnsOffset;
        public ushort RowsOffset;
        public ushort ValuesOffset;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct ColumnDefinition
    {
        public Column Name;
        public Type   Type;
    }

    [StructLayout(LayoutKind.Sequential)]
    private record struct RowDefinition(Table Table, uint Index);
}
