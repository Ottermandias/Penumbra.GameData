namespace Penumbra.GameData.Enums;

/// <summary> Bonus Equip Slots as flags, currently two. </summary>
[Flags]
public enum BonusEquipFlag : byte
{
    Unknown = 0x00,
    Glasses = 0x01,
    UnkSlot = 0x02,
}

public static class BonusSlotExtensions
{
    public const BonusEquipFlag All = BonusEquipFlag.Glasses;

    public static readonly IReadOnlyList<BonusEquipFlag> AllFlags = [BonusEquipFlag.Glasses];

    public static uint ToIndex(this BonusEquipFlag flag)
        => flag switch
        {
            BonusEquipFlag.Glasses => 0,
            BonusEquipFlag.UnkSlot => 1,
            _                      => uint.MaxValue,
        };

    public static uint ToSlot(this BonusEquipFlag flag)
        => flag switch
        {
            BonusEquipFlag.Glasses => 10,
            BonusEquipFlag.UnkSlot => 11,
            _                      => uint.MaxValue,
        };

    public static uint ToModelIndex(this BonusEquipFlag flag)
        => flag switch
        {
            BonusEquipFlag.Glasses => 16,
            BonusEquipFlag.UnkSlot => 17,
            _                      => uint.MaxValue,
        };

    public static string ToSuffix(this BonusEquipFlag value)
        => value switch
        {
            BonusEquipFlag.Glasses => "met",
            BonusEquipFlag.UnkSlot => "unk",
            _                      => "unk",
        };

    public static string ToName(this BonusEquipFlag value)
        => value switch
        {
            BonusEquipFlag.Glasses => "Glasses",
            BonusEquipFlag.UnkSlot => "Bonus Slot 2",
            _                      => "Unknown",
        };

    public static BonusEquipFlag ToBonusSlot(this uint slot)
        => slot switch
        {
            0 => BonusEquipFlag.Glasses,
            1 => BonusEquipFlag.UnkSlot,
            _ => BonusEquipFlag.Unknown,
        };
}
