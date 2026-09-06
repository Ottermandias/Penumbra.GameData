using Luna.Generators;
using Penumbra.GameData.Files.ShaderStructs;

namespace Penumbra.GameData.Files;

partial class SpmFile
{
    /// <remarks> The non-zero values of that enum are the CRC32 (as in <see cref="Name"/>) of their uppercased names. </remarks>
    [NamedEnum]
    public enum Table : uint
    {
        [Name("~")]
        Null = 0,

        Bg     = 0x2AF7F9B4,
        Chara  = 0xB9FDFB6C,
        Common = 0xA4D61674,
    }

    public static string DefaultSpmPath(Table table)
        => table switch
        {
            Table.Bg     => "common/graphics/bg_shader_param.spm",
            Table.Chara  => "common/graphics/chara_shader_param.spm",
            Table.Common => "common/graphics/common_shader_param.spm",
            _            => throw new ArgumentException($"Invalid SPM table 0x{(uint)table:X8}", nameof(table)),
        };

    /// <remarks>
    ///   The non-zero values of that enum are the CRC32 (as in <see cref="Name"/>) of their names.
    ///   (Except for "Id" which shall be uppercased.)
    /// </remarks>
    [NamedEnum]
    public enum Column : uint
    {
        [Name("~")]
        Null = 0,

        [Name("Lighting Type")]
        LightingType = 0xE8001A59,

        [Name("Subsurface Profile ID")]
        SubSurfaceProfileId = 0x8FB53404,

        [Name("Subsurface Width")]
        SubSurfaceWidth = 0xF30D1232,

        [Name("Backscatter Power")]
        BackScatterPower = 0x41338E94,

        [Name("Sheen Rate")]
        SheenRate = 0xB1FEBD21,

        [Name("Sheen Tint Rate")]
        SheenTintRate = 0xB7867D05,

        [Name("Sheen Aperture")]
        SheenAperture = 0x5C30C2FC,

        [Name("Use Subsurface Rate")]
        UseSubSurfaceRate = 0x947205D5,

        [Name("Hair Scatter Color Shift")]
        HairScatterColorShift = 0x671C995B,

        [Name("Hair Specular Shift")]
        HairSpecularShift = 0x6CD877F3,

        [Name("Fur Length")]
        FurLength = 0x85DC1E5C,

        [Name("Hair Roughness Offset Rate")]
        HairRoughnessOffsetRate = 0x8F6BA743,

        [Name("Subsurface Power")]
        SubSurfacePower = 0xD49D56BD,

        Reserve = 0x4D310CC0,

        [Name("Hair Specular Primary Shift")]
        HairSpecularPrimaryShift = 0xF33FF064,

        [Name("Hair Specular Secondary Shift")]
        HairSpecularSecondaryShift = 0xE0D24CB4,

        [Name("Hair Specular Backscatter Shift")]
        HairSpecularBackScatterShift = 0xA46A47BB,

        [Name("Hair Backscatter Roughness Offset Rate")]
        HairBackScatterRoughnessOffsetRate = 0x773AD7FB,

        [Name("Hair Secondary Roughness Offset Rate")]
        HairSecondaryRoughnessOffsetRate = 0xA8C99005,
    }

    public enum Type : uint
    {
        Float = 0,
        UInt  = 1,
        Named = 2,
    }

    /// <remarks> The non-zero values of that enum are the CRC32 (as in <see cref="Name"/>) of their uppercased names. </remarks>
    [NamedEnum]
    public enum NamedValue : uint
    {
        [Name("~")]
        Null = 0,

        Default = 0x417721BB,
        Legacy  = 0x56F16FCB,
        Hair    = 0x8B2653B1,
        Half    = 0xEC8B7389,
    }

    [StructLayout(LayoutKind.Explicit, Size = 4)]
    public struct Value : IEquatable<Value>
    {
        [FieldOffset(0)]
        public float Float;

        [FieldOffset(0)]
        public uint UInt;

        [FieldOffset(0)]
        public NamedValue Named;

        public override bool Equals([NotNullWhen(true)] object? obj)
            => obj is Value other && Equals(other);

        public override int GetHashCode()
            => HashCode.Combine(UInt);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Value other)
            => UInt == other.UInt;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Value left, Value right)
            => left.UInt == right.UInt;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Value left, Value right)
            => left.UInt != right.UInt;

        public string ToString(Type type, IFormatProvider? formatProvider)
            => type switch
            {
                Type.Float => Float.ToString(formatProvider),
                Type.UInt  => UInt.ToString(formatProvider),
                Type.Named => Named.ToName(),
                _          => throw new ArgumentException($"Invalid SPM value type {type}", nameof(type)),
            };
    }

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
    private record struct ColumnDefinition(Column Name, Type Type);

    [StructLayout(LayoutKind.Sequential)]
    private record struct RowDefinition(Table Table, uint Index);
}
