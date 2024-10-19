using System.Collections.Frozen;
using Penumbra.GameData.Enums;

namespace Penumbra.GameData.Structs;

/// <summary> Equipment Parameter entries contain information on how a piece of equipment influences other parts of the character for a given primary ID. </summary>
[Flags]
public enum EqpEntry : ulong
{
    BodyEnabled              = 0x00_01ul,
    BodyHideWaist            = 0x00_02ul,
    BodyHideThighs           = 0x00_04ul,
    BodyHideGlovesS          = 0x00_08ul,
    _4                       = 0x00_10ul,
    BodyHideGlovesM          = 0x00_20ul,
    BodyHideGlovesL          = 0x00_40ul,
    BodyHideGorget           = 0x00_80ul,
    BodyShowLeg              = 0x01_00ul,
    BodyShowHand             = 0x02_00ul,
    BodyShowHead             = 0x04_00ul,
    BodyShowNecklace         = 0x08_00ul,
    BodyShowBracelet         = 0x10_00ul,
    BodyShowTail             = 0x20_00ul,
    BodyDisableBreastPhysics = 0x40_00ul,
    BodyUsesEvpTable         = 0x80_00ul,
    BodyMask                 = 0xFF_FFul,

    LegsEnabled      = 0x01ul << 16,
    LegsHideKneePads = 0x02ul << 16,
    LegsHideBootsS   = 0x04ul << 16,
    LegsHideBootsM   = 0x08ul << 16,
    _20              = 0x10ul << 16,
    LegsShowFoot     = 0x20ul << 16,
    LegsShowTail     = 0x40ul << 16,
    _23              = 0x80ul << 16,
    LegsMask         = 0xFFul << 16,

    HandsEnabled     = 0x01ul << 24,
    HandsHideElbow   = 0x02ul << 24,
    HandsHideForearm = 0x04ul << 24,
    _27              = 0x08ul << 24,
    HandShowBracelet = 0x10ul << 24,
    HandShowRingL    = 0x20ul << 24,
    HandShowRingR    = 0x40ul << 24,
    _31              = 0x80ul << 24,
    HandsMask        = 0xFFul << 24,

    FeetEnabled   = 0x01ul << 32,
    FeetHideKnee  = 0x02ul << 32,
    FeetHideCalf  = 0x04ul << 32,
    FeetHideAnkle = 0x08ul << 32,
    _36           = 0x10ul << 32,
    _37           = 0x20ul << 32,
    _38           = 0x40ul << 32,
    _39           = 0x80ul << 32,
    FeetMask      = 0xFFul << 32,

    HeadEnabled                    = 0x00_00_01ul << 40,
    HeadHideScalp                  = 0x00_00_02ul << 40,
    HeadHideHair                   = 0x00_00_04ul << 40,
    HeadShowHairOverride           = 0x00_00_08ul << 40,
    HeadHideNeck                   = 0x00_00_10ul << 40,
    HeadShowNecklace               = 0x00_00_20ul << 40,
    HeadShowEarringsHyurRoe        = 0x00_00_40ul << 40,
    HeadShowEarringsLalaElezen     = 0x00_00_80ul << 40,
    HeadShowEarringsMiqoHrothViera = 0x00_01_00ul << 40,
    HeadShowEarringsAura           = 0x00_02_00ul << 40,
    HeadShowEarHuman               = 0x00_04_00ul << 40,
    HeadShowEarMiqote              = 0x00_08_00ul << 40,
    HeadShowEarAuRa                = 0x00_10_00ul << 40,
    HeadShowEarViera               = 0x00_20_00ul << 40,
    _54                            = 0x00_40_00ul << 40,
    _55                            = 0x00_80_00ul << 40,
    HeadShowHrothgarHat            = 0x01_00_00ul << 40,
    HeadShowVieraHat               = 0x02_00_00ul << 40,
    HeadUsesEvpTable               = 0x04_00_00ul << 40,
    _59                            = 0x08_00_00ul << 40,
    _60                            = 0x10_00_00ul << 40,
    _61                            = 0x20_00_00ul << 40,
    _62                            = 0x40_00_00ul << 40,
    _63                            = 0x80_00_00ul << 40,
    HeadMask                       = 0xFF_FF_FFul << 40,
}

public static class Eqp
{
    /// <summary> The default EqpEntry for otherwise unspecified gear sets. </summary>
    /// <remarks> cf. Client::Graphics::Scene::CharacterUtility.GetSlotEqpFlags </remarks>
    public const EqpEntry DefaultEntry = (EqpEntry)0x3fe00070603f00;

    /// <summary> Get the number of bytes used per slot, and the offset of those bytes in the flag. </summary>
    public static (int, int) BytesAndOffset(EquipSlot slot)
        => slot switch
        {
            EquipSlot.Body  => (2, 0),
            EquipSlot.Legs  => (1, 2),
            EquipSlot.Hands => (1, 3),
            EquipSlot.Feet  => (1, 4),
            EquipSlot.Head  => (3, 5),
            _               => (0, 0),
        };

    /// <summary> Get the number of bytes used per slot, and the offset of those bytes in the flag. </summary>
    public static (int, EqpEntry) OffsetAndMask(EquipSlot slot)
        => slot switch
        {
            EquipSlot.Body  => (0 * 8, EqpEntry.BodyMask),
            EquipSlot.Legs  => (2 * 8, EqpEntry.LegsMask),
            EquipSlot.Hands => (3 * 8, EqpEntry.HandsMask),
            EquipSlot.Feet  => (4 * 8, EqpEntry.FeetMask),
            EquipSlot.Head  => (5 * 8, EqpEntry.HeadMask),
            _               => (0, 0),
        };

    /// <summary> Read an array of bytes and convert them to an EqpEntry given a slot. </summary>
    public static EqpEntry FromSlotAndBytes(EquipSlot slot, byte[] value)
    {
        EqpEntry ret = 0;
        var (bytes, offset) = BytesAndOffset(slot);
        if (bytes != value.Length)
            throw new ArgumentException();

        for (var i = 0; i < bytes; ++i)
            ret |= (EqpEntry)((ulong)value[i] << ((offset + i) * 8));

        return ret;
    }

    /// <summary> Get the mask for a given slot. </summary>
    public static EqpEntry Mask(EquipSlot slot)
        => OffsetAndMask(slot).Item2;

    /// <summary> Convert an entry flag to the slot it affects. </summary>
    public static EquipSlot ToEquipSlot(this EqpEntry entry)
    {
        return entry switch
        {
            EqpEntry.BodyEnabled              => EquipSlot.Body,
            EqpEntry.BodyHideWaist            => EquipSlot.Body,
            EqpEntry.BodyHideThighs           => EquipSlot.Body,
            EqpEntry.BodyHideGlovesS          => EquipSlot.Body,
            EqpEntry._4                       => EquipSlot.Body,
            EqpEntry.BodyHideGlovesM          => EquipSlot.Body,
            EqpEntry.BodyHideGlovesL          => EquipSlot.Body,
            EqpEntry.BodyHideGorget           => EquipSlot.Body,
            EqpEntry.BodyShowLeg              => EquipSlot.Body,
            EqpEntry.BodyShowHand             => EquipSlot.Body,
            EqpEntry.BodyShowHead             => EquipSlot.Body,
            EqpEntry.BodyShowNecklace         => EquipSlot.Body,
            EqpEntry.BodyShowBracelet         => EquipSlot.Body,
            EqpEntry.BodyShowTail             => EquipSlot.Body,
            EqpEntry.BodyDisableBreastPhysics => EquipSlot.Body,
            EqpEntry.BodyUsesEvpTable         => EquipSlot.Body,

            EqpEntry.LegsEnabled      => EquipSlot.Legs,
            EqpEntry.LegsHideKneePads => EquipSlot.Legs,
            EqpEntry.LegsHideBootsS   => EquipSlot.Legs,
            EqpEntry.LegsHideBootsM   => EquipSlot.Legs,
            EqpEntry._20              => EquipSlot.Legs,
            EqpEntry.LegsShowFoot     => EquipSlot.Legs,
            EqpEntry.LegsShowTail     => EquipSlot.Legs,
            EqpEntry._23              => EquipSlot.Legs,

            EqpEntry.HandsEnabled     => EquipSlot.Hands,
            EqpEntry.HandsHideElbow   => EquipSlot.Hands,
            EqpEntry.HandsHideForearm => EquipSlot.Hands,
            EqpEntry._27              => EquipSlot.Hands,
            EqpEntry.HandShowBracelet => EquipSlot.Hands,
            EqpEntry.HandShowRingL    => EquipSlot.Hands,
            EqpEntry.HandShowRingR    => EquipSlot.Hands,
            EqpEntry._31              => EquipSlot.Hands,

            EqpEntry.FeetEnabled   => EquipSlot.Feet,
            EqpEntry.FeetHideKnee  => EquipSlot.Feet,
            EqpEntry.FeetHideCalf  => EquipSlot.Feet,
            EqpEntry.FeetHideAnkle => EquipSlot.Feet,
            EqpEntry._36           => EquipSlot.Feet,
            EqpEntry._37           => EquipSlot.Feet,
            EqpEntry._38           => EquipSlot.Feet,
            EqpEntry._39           => EquipSlot.Feet,

            EqpEntry.HeadEnabled                    => EquipSlot.Head,
            EqpEntry.HeadHideScalp                  => EquipSlot.Head,
            EqpEntry.HeadHideHair                   => EquipSlot.Head,
            EqpEntry.HeadShowHairOverride           => EquipSlot.Head,
            EqpEntry.HeadHideNeck                   => EquipSlot.Head,
            EqpEntry.HeadShowNecklace               => EquipSlot.Head,
            EqpEntry.HeadShowEarringsHyurRoe        => EquipSlot.Head,
            EqpEntry.HeadShowEarringsLalaElezen     => EquipSlot.Head,
            EqpEntry.HeadShowEarringsMiqoHrothViera => EquipSlot.Head,
            EqpEntry.HeadShowEarringsAura           => EquipSlot.Head,
            EqpEntry.HeadShowEarHuman               => EquipSlot.Head,
            EqpEntry.HeadShowEarMiqote              => EquipSlot.Head,
            EqpEntry.HeadShowEarAuRa                => EquipSlot.Head,
            EqpEntry.HeadShowEarViera               => EquipSlot.Head,
            EqpEntry._54                            => EquipSlot.Head,
            EqpEntry._55                            => EquipSlot.Head,
            EqpEntry.HeadShowHrothgarHat            => EquipSlot.Head,
            EqpEntry.HeadShowVieraHat               => EquipSlot.Head,
            EqpEntry.HeadUsesEvpTable               => EquipSlot.Head,

            // currently unused
            EqpEntry._59 => EquipSlot.Unknown,
            EqpEntry._60 => EquipSlot.Unknown,
            EqpEntry._61 => EquipSlot.Unknown,
            EqpEntry._62 => EquipSlot.Unknown,
            EqpEntry._63 => EquipSlot.Unknown,

            _ => EquipSlot.Unknown,
        };
    }

    /// <summary> Get human-readable names for the known flags. </summary>
    public static string ToLocalName(this EqpEntry entry)
        => entry switch
        {
            EqpEntry.BodyEnabled              => "Enabled",
            EqpEntry.BodyHideWaist            => "Hide Waist",
            EqpEntry.BodyHideThighs           => "Hide Thigh Pads",
            EqpEntry.BodyHideGlovesS          => "Hide Small Gloves",
            EqpEntry._4                       => "Unknown 4",
            EqpEntry.BodyHideGlovesM          => "Hide Medium Gloves",
            EqpEntry.BodyHideGlovesL          => "Hide Large Gloves",
            EqpEntry.BodyHideGorget           => "Hide Gorget",
            EqpEntry.BodyShowLeg              => "Show Legs",
            EqpEntry.BodyShowHand             => "Show Hands",
            EqpEntry.BodyShowHead             => "Show Head",
            EqpEntry.BodyShowNecklace         => "Show Necklace",
            EqpEntry.BodyShowBracelet         => "Show Bracelet",
            EqpEntry.BodyShowTail             => "Show Tail",
            EqpEntry.BodyDisableBreastPhysics => "Disable Breast Physics",
            EqpEntry.BodyUsesEvpTable         => "Uses EVP Table",

            EqpEntry.LegsEnabled      => "Enabled",
            EqpEntry.LegsHideKneePads => "Hide Knee Pads",
            EqpEntry.LegsHideBootsS   => "Hide Small Boots",
            EqpEntry.LegsHideBootsM   => "Hide Medium Boots",
            EqpEntry._20              => "Unknown 20",
            EqpEntry.LegsShowFoot     => "Show Foot",
            EqpEntry.LegsShowTail     => "Show Tail",
            EqpEntry._23              => "Unknown 23",

            EqpEntry.HandsEnabled     => "Enabled",
            EqpEntry.HandsHideElbow   => "Hide Elbow",
            EqpEntry.HandsHideForearm => "Hide Forearm",
            EqpEntry._27              => "Unknown 27",
            EqpEntry.HandShowBracelet => "Show Bracelet",
            EqpEntry.HandShowRingL    => "Show Left Ring",
            EqpEntry.HandShowRingR    => "Show Right Ring",
            EqpEntry._31              => "Unknown 31",

            EqpEntry.FeetEnabled   => "Enabled",
            EqpEntry.FeetHideKnee  => "Hide Knees",
            EqpEntry.FeetHideCalf  => "Hide Calves",
            EqpEntry.FeetHideAnkle => "Hide Ankles",
            EqpEntry._36           => "Unknown 36",
            EqpEntry._37           => "Unknown 37",
            EqpEntry._38           => "Unknown 38",
            EqpEntry._39           => "Unknown 39",

            EqpEntry.HeadEnabled                    => "Enabled",
            EqpEntry.HeadHideScalp                  => "Hide Scalp",
            EqpEntry.HeadHideHair                   => "Hide Hair",
            EqpEntry.HeadShowHairOverride           => "Show Hair Override",
            EqpEntry.HeadHideNeck                   => "Hide Neck",
            EqpEntry.HeadShowNecklace               => "Show Necklace",
            EqpEntry.HeadShowEarringsHyurRoe        => "Show Earrings (Hyur, Roegadyn)",
            EqpEntry.HeadShowEarringsLalaElezen     => "Show Earrings (Elezen, Lalafell)",
            EqpEntry.HeadShowEarringsMiqoHrothViera => "Show Earrings (Miqo'te, Hrothgar, Viera)",
            EqpEntry.HeadShowEarringsAura           => "Show Earrings (Au Ra)",
            EqpEntry.HeadShowEarHuman               => "Show Ears (Human)",
            EqpEntry.HeadShowEarMiqote              => "Show Ears (Miqo'te)",
            EqpEntry.HeadShowEarAuRa                => "Show Ears (Au Ra)",
            EqpEntry.HeadShowEarViera               => "Show Ears (Viera)",
            EqpEntry._54                            => "Unknown 54",
            EqpEntry._55                            => "Unknown 55",
            EqpEntry.HeadShowHrothgarHat            => "Show on Hrothgar",
            EqpEntry.HeadShowVieraHat               => "Show on Viera",
            EqpEntry.HeadUsesEvpTable               => "Uses EVP Table",

            EqpEntry._59 => "Unknown 59",
            EqpEntry._60 => "Unknown 60",
            EqpEntry._61 => "Unknown 61",
            EqpEntry._62 => "Unknown 62",
            EqpEntry._63 => "Unknown 63",

            _ => throw new InvalidEnumArgumentException(),
        };

    /// <summary> Get all flags for a given slot. </summary>
    private static IReadOnlyList<EqpEntry> GetEntriesForSlot(EquipSlot slot)
    {
        return ((EqpEntry[])Enum.GetValues(typeof(EqpEntry)))
            .Where(e => e.ToEquipSlot() == slot)
            .ToArray();
    }

    /// <summary> A list of flags affecting the body. </summary>
    public static readonly IReadOnlyList<EqpEntry> EqpAttributesBody = GetEntriesForSlot(EquipSlot.Body);

    /// <summary> A list of flags affecting the legs. </summary>
    public static readonly IReadOnlyList<EqpEntry> EqpAttributesLegs = GetEntriesForSlot(EquipSlot.Legs);

    /// <summary> A list of flags affecting the hands. </summary>
    public static readonly IReadOnlyList<EqpEntry> EqpAttributesHands = GetEntriesForSlot(EquipSlot.Hands);

    /// <summary> A list of flags affecting the feet. </summary>
    public static readonly IReadOnlyList<EqpEntry> EqpAttributesFeet = GetEntriesForSlot(EquipSlot.Feet);

    /// <summary> A list of flags affecting the head. </summary>
    public static readonly IReadOnlyList<EqpEntry> EqpAttributesHead = GetEntriesForSlot(EquipSlot.Head);

    /// <summary> A dictionary mapping slots to a list of their affected flags. </summary>
    public static readonly IReadOnlyDictionary<EquipSlot, IReadOnlyList<EqpEntry>> EqpAttributes =
        FrozenDictionary.ToFrozenDictionary<EquipSlot, IReadOnlyList<EqpEntry>>(
        [
            new KeyValuePair<EquipSlot, IReadOnlyList<EqpEntry>>(EquipSlot.Body,  EqpAttributesBody),
            new KeyValuePair<EquipSlot, IReadOnlyList<EqpEntry>>(EquipSlot.Legs,  EqpAttributesLegs),
            new KeyValuePair<EquipSlot, IReadOnlyList<EqpEntry>>(EquipSlot.Hands, EqpAttributesHands),
            new KeyValuePair<EquipSlot, IReadOnlyList<EqpEntry>>(EquipSlot.Feet,  EqpAttributesFeet),
            new KeyValuePair<EquipSlot, IReadOnlyList<EqpEntry>>(EquipSlot.Head,  EqpAttributesHead),
        ]);
}
