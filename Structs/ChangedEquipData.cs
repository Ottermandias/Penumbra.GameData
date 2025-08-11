using Penumbra.GameData.Enums;

namespace Penumbra.GameData.Structs;

/// <summary> The changed data contained in a draw object. </summary>
[StructLayout(LayoutKind.Explicit)]
public struct ChangedEquipData
{
    [FieldOffset(0)]
    public PrimaryId Model;

    [FieldOffset(2)]
    public Variant Variant;

    [FieldOffset(3)]
    public StainIds Stains;

    [FieldOffset(8)]
    public PrimaryId BonusModel;

    [FieldOffset(10)]
    public Variant BonusVariant;

    [FieldOffset(20)]
    public ushort VfxId;

    [FieldOffset(22)]
    public GenderRace GenderRace;

    [FieldOffset(24)]
    public nint Unknown;
}
