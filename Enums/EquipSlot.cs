using System.Collections.Frozen;
using Luna.Generators;

namespace Penumbra.GameData.Enums;

/// <summary> Equip Slot, mostly as defined by the games EquipSlotCategory. </summary>
[NamedEnum]
public enum EquipSlot : byte
{
    [Name("Unknown")]
    Unknown = 0,

    [Name("Primary Weapon")]
    MainHand = 1,

    [Name("Secondary Weapon")]
    OffHand = 2,

    [Name("Head")]
    Head = 3,

    [Name("Body")]
    Body = 4,

    [Name("Hands")]
    Hands = 5,

    [Name("Belt")]
    Belt = 6,

    [Name("Legs")]
    Legs = 7,

    [Name("Feet")]
    Feet = 8,

    [Name("Earrings")]
    Ears = 9,

    [Name("Necklace")]
    Neck = 10,

    [Name("Bracelets")]
    Wrists = 11,

    [Name("Right Ring")]
    RFinger = 12,

    [Name("Primary Weapon")]
    BothHand = 13,

    [Name("Left Ring")]
    LFinger = 14, // Not officially existing, means "weapon could be equipped in either hand" for the game.

    [Name("Head and Body")]
    HeadBody = 15,

    [Name("Costume")]
    BodyHandsLegsFeet = 16,

    [Name("Soul Crystal")]
    SoulCrystal = 17,

    [Name("Bottom")]
    LegsFeet = 18,

    [Name("Costume")]
    FullBody = 19,

    [Name("Top")]
    BodyHands = 20,

    [Name("Costume")]
    BodyLegsFeet = 21,

    [Name("Top")]
    ChestHands = 22,

    [Name("Costum")]
    ChestLegs = 23,

    [Name("Unknown")]
    Nothing = 24,

    [Name("Costume")]
    All = 25, // Not officially existing
}

[NamedEnum]
public enum HumanSlot : uint
{
    [Name("Head")]
    Head = 0,

    [Name("Body")]
    Body = 1,

    [Name("Hands")]
    Hands = 2,

    [Name("Legs")]
    Legs = 3,

    [Name("Feet")]
    Feet = 4,

    [Name("Earrings")]
    Ears = 5,

    [Name("Necklace")]
    Neck = 6,

    [Name("Bracelets")]
    Wrists = 7,

    [Name("Right Ring")]
    RFinger = 8,

    [Name("Left Ring")]
    LFinger = 9,

    [Name("Hair")]
    Hair = 10,

    [Name("Face")]
    Face = 11,

    [Name("Ears")]
    Ear = 12,

    [Name("Glasses")]
    Glasses = 16,

    [Name("Unknown Bonus")]
    UnkBonus = 17,

    [Name("Unknown")]
    Unknown = uint.MaxValue,
}

public static partial class EquipSlotExtensions
{
    /// <summary> Convert the integer to the EquipSlot it is used to represent in most model code. </summary>
    public static EquipSlot ToEquipSlot(this uint value)
        => value switch
        {
            0  => EquipSlot.Head,
            1  => EquipSlot.Body,
            2  => EquipSlot.Hands,
            3  => EquipSlot.Legs,
            4  => EquipSlot.Feet,
            5  => EquipSlot.Ears,
            6  => EquipSlot.Neck,
            7  => EquipSlot.Wrists,
            8  => EquipSlot.RFinger,
            9  => EquipSlot.LFinger,
            10 => EquipSlot.MainHand,
            11 => EquipSlot.OffHand,
            16 => EquipSlot.Head,
            _  => EquipSlot.Unknown,
        };

    public static EquipSlot ToEquipSlot(this HumanSlot slot)
        => slot switch
        {
            HumanSlot.Head    => EquipSlot.Head,
            HumanSlot.Body    => EquipSlot.Body,
            HumanSlot.Hands   => EquipSlot.Hands,
            HumanSlot.Legs    => EquipSlot.Legs,
            HumanSlot.Feet    => EquipSlot.Feet,
            HumanSlot.Ears    => EquipSlot.Ears,
            HumanSlot.Neck    => EquipSlot.Neck,
            HumanSlot.Wrists  => EquipSlot.Wrists,
            HumanSlot.RFinger => EquipSlot.RFinger,
            HumanSlot.LFinger => EquipSlot.LFinger,
            HumanSlot.Glasses => EquipSlot.Head,
            _                 => EquipSlot.Unknown,
        };

    public static uint ToIndex(this HumanSlot slot)
        => (uint)slot;

    public static bool ToSlotIndex(this HumanSlot slot, out int index)
    {
        (var ret, index) = slot switch
        {
            HumanSlot.Head     => (true, 0),
            HumanSlot.Body     => (true, 1),
            HumanSlot.Hands    => (true, 2),
            HumanSlot.Legs     => (true, 3),
            HumanSlot.Feet     => (true, 4),
            HumanSlot.Ears     => (true, 5),
            HumanSlot.Neck     => (true, 6),
            HumanSlot.Wrists   => (true, 7),
            HumanSlot.RFinger  => (true, 8),
            HumanSlot.LFinger  => (true, 9),
            HumanSlot.Glasses  => (true, 10),
            HumanSlot.UnkBonus => (true, 11),
            _                  => (false, -1),
        };
        return ret;
    }

    public static object? ToSpecificEnum(this HumanSlot slot)
        => slot switch
        {
            HumanSlot.Head     => EquipSlot.Head,
            HumanSlot.Body     => EquipSlot.Body,
            HumanSlot.Hands    => EquipSlot.Hands,
            HumanSlot.Legs     => EquipSlot.Legs,
            HumanSlot.Feet     => EquipSlot.Feet,
            HumanSlot.Ears     => EquipSlot.Ears,
            HumanSlot.Neck     => EquipSlot.Neck,
            HumanSlot.Wrists   => EquipSlot.Wrists,
            HumanSlot.RFinger  => EquipSlot.RFinger,
            HumanSlot.LFinger  => EquipSlot.LFinger,
            HumanSlot.Glasses  => BonusItemFlag.Glasses,
            HumanSlot.UnkBonus => BonusItemFlag.UnkSlot,
            HumanSlot.Hair     => BodySlot.Hair,
            HumanSlot.Face     => BodySlot.Face,
            HumanSlot.Ear      => BodySlot.Ear,
            _                  => null,
        };

    public static HumanSlot ToHumanSlot(this EquipSlot slot)
        => slot switch
        {
            EquipSlot.Head              => HumanSlot.Head,
            EquipSlot.Body              => HumanSlot.Body,
            EquipSlot.Hands             => HumanSlot.Hands,
            EquipSlot.Legs              => HumanSlot.Legs,
            EquipSlot.Feet              => HumanSlot.Feet,
            EquipSlot.Ears              => HumanSlot.Ears,
            EquipSlot.Neck              => HumanSlot.Neck,
            EquipSlot.Wrists            => HumanSlot.Wrists,
            EquipSlot.RFinger           => HumanSlot.RFinger,
            EquipSlot.LFinger           => HumanSlot.LFinger,
            EquipSlot.HeadBody          => HumanSlot.Body,
            EquipSlot.BodyHandsLegsFeet => HumanSlot.Body,
            EquipSlot.LegsFeet          => HumanSlot.Legs,
            EquipSlot.FullBody          => HumanSlot.Body,
            EquipSlot.BodyHands         => HumanSlot.Body,
            EquipSlot.BodyLegsFeet      => HumanSlot.Body,
            EquipSlot.ChestHands        => HumanSlot.Body,
            EquipSlot.ChestLegs         => HumanSlot.Body,
            _                           => HumanSlot.Unknown,
        };

    public static uint ToIndex(this EquipSlot slot)
        => slot switch
        {
            EquipSlot.Head     => 0,
            EquipSlot.Body     => 1,
            EquipSlot.Hands    => 2,
            EquipSlot.Legs     => 3,
            EquipSlot.Feet     => 4,
            EquipSlot.Ears     => 5,
            EquipSlot.Neck     => 6,
            EquipSlot.Wrists   => 7,
            EquipSlot.RFinger  => 8,
            EquipSlot.LFinger  => 9,
            EquipSlot.MainHand => 10,
            EquipSlot.OffHand  => 11,
            _                  => uint.MaxValue,
        };

    /// <summary> Get the suffix used for a specific EquipSlot in file names. </summary>
    public static string ToSuffix(this EquipSlot value)
        => value switch
        {
            EquipSlot.Head    => "met",
            EquipSlot.Hands   => "glv",
            EquipSlot.Legs    => "dwn",
            EquipSlot.Feet    => "sho",
            EquipSlot.Body    => "top",
            EquipSlot.Ears    => "ear",
            EquipSlot.Neck    => "nek",
            EquipSlot.RFinger => "rir",
            EquipSlot.LFinger => "ril",
            EquipSlot.Wrists  => "wrs",
            _                 => "unk",
        };

    /// <summary> Convert the EquipSlotCategory value to the actual inventory slot it is put in. </summary>
    public static EquipSlot ToSlot(this EquipSlot value)
        => value switch
        {
            EquipSlot.MainHand          => EquipSlot.MainHand,
            EquipSlot.OffHand           => EquipSlot.OffHand,
            EquipSlot.Head              => EquipSlot.Head,
            EquipSlot.Body              => EquipSlot.Body,
            EquipSlot.Hands             => EquipSlot.Hands,
            EquipSlot.Belt              => EquipSlot.Belt,
            EquipSlot.Legs              => EquipSlot.Legs,
            EquipSlot.Feet              => EquipSlot.Feet,
            EquipSlot.Ears              => EquipSlot.Ears,
            EquipSlot.Neck              => EquipSlot.Neck,
            EquipSlot.Wrists            => EquipSlot.Wrists,
            EquipSlot.RFinger           => EquipSlot.RFinger,
            EquipSlot.BothHand          => EquipSlot.MainHand,
            EquipSlot.LFinger           => EquipSlot.RFinger,
            EquipSlot.HeadBody          => EquipSlot.Body,
            EquipSlot.BodyHandsLegsFeet => EquipSlot.Body,
            EquipSlot.SoulCrystal       => EquipSlot.SoulCrystal,
            EquipSlot.LegsFeet          => EquipSlot.Legs,
            EquipSlot.FullBody          => EquipSlot.Body,
            EquipSlot.BodyHands         => EquipSlot.Body,
            EquipSlot.BodyLegsFeet      => EquipSlot.Body,
            EquipSlot.ChestHands        => EquipSlot.Body,
            EquipSlot.ChestLegs         => EquipSlot.Body,
            _                           => EquipSlot.Unknown,
        };

    /// <summary> Returns true for the 5 primary equipment slots. </summary>
    public static bool IsEquipment(this EquipSlot value)
    {
        return value switch
        {
            EquipSlot.Head  => true,
            EquipSlot.Hands => true,
            EquipSlot.Legs  => true,
            EquipSlot.Feet  => true,
            EquipSlot.Body  => true,
            _               => false,
        };
    }

    /// <summary> Returns true for the 5 secondary equipment slots, of which LFinger does not really exist. </summary>
    public static bool IsAccessory(this EquipSlot value)
    {
        return value switch
        {
            EquipSlot.Ears    => true,
            EquipSlot.Neck    => true,
            EquipSlot.RFinger => true,
            EquipSlot.LFinger => true,
            EquipSlot.Wrists  => true,
            _                 => false,
        };
    }

    /// <summary> Returns true for anything worn in an actual equipment slot. </summary>
    public static bool IsEquipmentPiece(this EquipSlot value)
    {
        return value switch
        {
            // Accessories
            EquipSlot.RFinger => true,
            EquipSlot.Wrists  => true,
            EquipSlot.Ears    => true,
            EquipSlot.Neck    => true,
            // Equipment
            EquipSlot.Head              => true,
            EquipSlot.Body              => true,
            EquipSlot.Hands             => true,
            EquipSlot.Legs              => true,
            EquipSlot.Feet              => true,
            EquipSlot.BodyHands         => true,
            EquipSlot.BodyHandsLegsFeet => true,
            EquipSlot.BodyLegsFeet      => true,
            EquipSlot.FullBody          => true,
            EquipSlot.HeadBody          => true,
            EquipSlot.LegsFeet          => true,
            EquipSlot.ChestHands        => true,
            EquipSlot.ChestLegs         => true,
            _                           => false,
        };
    }

    /// <summary> A list of all primary equipment pieces. </summary>
    public static readonly IReadOnlyList<EquipSlot> EquipmentSlots = Enum.GetValues<EquipSlot>().Where(e => e.IsEquipment()).ToArray();

    /// <summary> A list of all secondary equipment pieces. </summary>
    public static readonly IReadOnlyList<EquipSlot> AccessorySlots = Enum.GetValues<EquipSlot>().Where(e => e.IsAccessory()).ToArray();

    /// <summary> A list of all primary and secondary equipment pieces. </summary>
    public static readonly IReadOnlyList<EquipSlot> EqdpSlots = EquipmentSlots.Concat(AccessorySlots).ToArray();

    /// <summary> A list of both weapon slots. </summary>
    public static readonly IReadOnlyList<EquipSlot> WeaponSlots =
    [
        EquipSlot.MainHand,
        EquipSlot.OffHand,
    ];

    /// <summary> A list of all equipment slots. </summary>
    public static readonly EquipSlot[] FullSlots = [.. WeaponSlots, .. EqdpSlots];
}

public static partial class Names
{
    /// <summary> A dictionary converting path suffices into EquipSlot. </summary>
    public static readonly IReadOnlyDictionary<string, EquipSlot> SuffixToEquipSlot = FrozenDictionary.ToFrozenDictionary(
    [
        new KeyValuePair<string, EquipSlot>(EquipSlot.Head.ToSuffix(),    EquipSlot.Head),
        new KeyValuePair<string, EquipSlot>(EquipSlot.Hands.ToSuffix(),   EquipSlot.Hands),
        new KeyValuePair<string, EquipSlot>(EquipSlot.Legs.ToSuffix(),    EquipSlot.Legs),
        new KeyValuePair<string, EquipSlot>(EquipSlot.Feet.ToSuffix(),    EquipSlot.Feet),
        new KeyValuePair<string, EquipSlot>(EquipSlot.Body.ToSuffix(),    EquipSlot.Body),
        new KeyValuePair<string, EquipSlot>(EquipSlot.Ears.ToSuffix(),    EquipSlot.Ears),
        new KeyValuePair<string, EquipSlot>(EquipSlot.Neck.ToSuffix(),    EquipSlot.Neck),
        new KeyValuePair<string, EquipSlot>(EquipSlot.RFinger.ToSuffix(), EquipSlot.RFinger),
        new KeyValuePair<string, EquipSlot>(EquipSlot.LFinger.ToSuffix(), EquipSlot.LFinger),
        new KeyValuePair<string, EquipSlot>(EquipSlot.Wrists.ToSuffix(),  EquipSlot.Wrists),
    ]);
}
