using ImSharp;
using Lumina.Excel.Sheets;
using Luna.Generators;

namespace Penumbra.GameData.Enums;

/// <summary> A full equipment type representing any type of equipment a character can wear. </summary>
[NamedEnum]
public enum FullEquipType : byte
{
    [Name(Omit: true)]
    Unknown,

    [Name("Head")]
    Head,

    [Name("Body")]
    Body,

    [Name("Hands")]
    Hands,

    [Name("Legs")]
    Legs,

    [Name("Feet")]
    Feet,

    [Name("Earrings")]
    Ears,

    [Name("Necklace")]
    Neck,

    [Name("Bracelets")]
    Wrists,

    [Name("Ring")]
    Finger,

    [Name("Fist Weapon")]
    Fists, // PGL, MNK

    [Name("Fist Weapon (Offhand)")]
    FistsOff,

    [Name("Sword")]
    Sword, // GLA, PLD Main

    [Name("Axe")]
    Axe, // MRD, WAR

    [Name("Bow")]
    Bow, // ARC, BRD

    [Name("Quiver")]
    BowOff,

    [Name("Lance")]
    Lance, // LNC, DRG,

    [Name("Greatstaff")]
    StaffBlm, // THM, BLM,

    [Name("Wand")]
    Wand, // CNJ, WHM Main

    [Name("Book")]
    Book, // ACN, SMN, SCH

    [Name("Dagger")]
    Daggers, // ROG, NIN

    [Name("Dagger (Offhand)")]
    DaggersOff,

    [Name("Broadsword")]
    Broadsword, // DRK,

    [Name("Gun")]
    Gun, // MCH,

    [Name("Aetherotransformer")]
    GunOff,

    [Name("Orrery")]
    Orrery, // AST,

    [Name("Card Holder")]
    OrreryOff,

    [Name("Katana")]
    Katana, // SAM

    [Name("Sheathe")]
    KatanaOff,

    [Name("Rapier")]
    Rapier, // RDM

    [Name("Focus")]
    RapierOff,

    [Name("Cane")]
    Cane, // BLU

    [Name("Gunblade")]
    Gunblade, // GNB,

    [Name("Glaive")]
    Glaives, // DNC,

    [Name("Glaive (Offhand)")]
    GlaivesOff,

    [Name("Scythe")]
    Scythe, // RPR,

    [Name("Nouliths")]
    Nouliths, // SGE

    [Name("Shield")]
    Shield, // GLA, PLD, THM, BLM, CNJ, WHM Off

    [Name("Saw")]
    Saw, // CRP

    [Name("Cross Pein Hammer")]
    CrossPeinHammer, // BSM

    [Name("Raising Hammer")]
    RaisingHammer, // ARM

    [Name("Lapidary Hammer")]
    LapidaryHammer, // GSM

    [Name("Round Knife")]
    Knife, // LTW

    [Name("Needle")]
    Needle, // WVR

    [Name("Alembic")]
    Alembic, // ALC

    [Name("Frypan")]
    Frypan, // CUL

    [Name("Pickaxe")]
    Pickaxe, // MIN

    [Name("Hatchet")]
    Hatchet, // BTN

    [Name("Fishing Rod")]
    FishingRod, // FSH

    [Name("Clawhammer")]
    ClawHammer, // CRP Off

    [Name("File")]
    File, // BSM Off

    [Name("Pliers")]
    Pliers, // ARM Off

    [Name("Grinding Wheel")]
    GrindingWheel, // GSM Off

    [Name("Awl")]
    Awl, // LTW Off

    [Name("Spinning Wheel")]
    SpinningWheel, // WVR Off

    [Name("Mortar")]
    Mortar, // ALC Off

    [Name("Culinary Knife")]
    CulinaryKnife, // CUL Off

    [Name("Sledgehammer")]
    Sledgehammer, // MIN Off

    [Name("Garden Scythe")]
    GardenScythe, // BTN Off

    [Name("Gig")]
    Gig, // FSH Off

    [Name("Brush")]
    Brush, // PCT

    [Name("Palette")]
    Palette, // PCT Off

    [Name("Twinfangs")]
    Twinfangs, // VPR

    [Name("Twinfangs (Offhand)")]
    TwinfangsOff, // VPR Off

    [Name("Handaxe")]
    Handaxe, // BST TODO

    [Name("Glasses")]
    Glasses,

    [Name("Primary Weapon")]
    UnknownMainhand,

    [Name("Secondary Weapon")]
    UnknownOffhand,

    [Name("Staff")]
    StaffWhm, // CNJ, WHM

    [Name("Scepter")]
    Scepter, // THM, BLM, Main
}

public static partial class FullEquipTypeExtensions
{
    /// <summary> Obtain the FullEquipType of an item. </summary>
    internal static FullEquipType ToEquipType(this Item item)
    {
        var slot   = (EquipSlot)item.EquipSlotCategory.RowId;
        var weapon = (WeaponCategory)item.ItemUICategory.RowId;
        return slot.ToEquipType(weapon);
    }

    /// <summary> Return whether a FullEquipType is not fully known. </summary>
    public static bool IsUnknown(this FullEquipType type)
        => type is FullEquipType.Unknown or FullEquipType.UnknownMainhand or FullEquipType.UnknownOffhand;

    /// <summary> Return whether a FullEquipType is a primary weapon type. </summary>
    public static bool IsWeapon(this FullEquipType type)
        => type switch
        {
            FullEquipType.Fists           => true,
            FullEquipType.Sword           => true,
            FullEquipType.Axe             => true,
            FullEquipType.Bow             => true,
            FullEquipType.Lance           => true,
            FullEquipType.StaffBlm        => true,
            FullEquipType.StaffWhm        => true,
            FullEquipType.Wand            => true,
            FullEquipType.Book            => true,
            FullEquipType.Daggers         => true,
            FullEquipType.Broadsword      => true,
            FullEquipType.Gun             => true,
            FullEquipType.Orrery          => true,
            FullEquipType.Katana          => true,
            FullEquipType.Rapier          => true,
            FullEquipType.Cane            => true,
            FullEquipType.Scepter         => true,
            FullEquipType.Gunblade        => true,
            FullEquipType.Glaives         => true,
            FullEquipType.Scythe          => true,
            FullEquipType.Nouliths        => true,
            FullEquipType.Shield          => true,
            FullEquipType.Brush           => true,
            FullEquipType.Twinfangs       => true,
            FullEquipType.Handaxe         => true,
            FullEquipType.UnknownMainhand => true,
            _                             => false,
        };

    /// <summary> Return whether two weapon types are compatible. </summary>
    public static bool IsCompatible(this FullEquipType type, FullEquipType other)
        => type switch
        {
            FullEquipType.Scepter or FullEquipType.Wand or FullEquipType.StaffWhm or FullEquipType.StaffBlm => other is FullEquipType.Scepter
                or FullEquipType.Wand or FullEquipType.StaffBlm or FullEquipType.StaffWhm,
            _ => type == other,
        };

    /// <summary> Return whether an offhand weapon type is compatible with the current mainhand state. </summary>
    /// <param name="type"> The new offhand item's type. </param>
    /// <param name="gameMainhand"> The game's mainhand type. </param>
    /// <param name="actualMainhand"> The current actual mainhand type. </param>
    /// <param name="gameOffhand"> The game's offhand type. </param>
    /// <returns> True whether the new item can be applied to the offhand. </returns>
    public static bool IsOffhandCompatible(this FullEquipType type, FullEquipType gameMainhand, FullEquipType actualMainhand,
        FullEquipType gameOffhand)
    {
        // If the mainhand type is the default one, just check against the default offhand.
        if (gameMainhand == actualMainhand)
            return type.IsCompatible(gameOffhand);

        // Otherwise check against the expected offhand.
        var validOffhand = actualMainhand.ValidOffhand();
        return type.IsCompatible(validOffhand);
    }

    /// <summary> Get all compatible types for a weapon type, excluding itself. </summary>
    public static IReadOnlyList<FullEquipType> CompatibleTypes(this FullEquipType type)
        => type switch
        {
            FullEquipType.Scepter  => [FullEquipType.Wand, FullEquipType.StaffWhm, FullEquipType.StaffBlm],
            FullEquipType.Wand     => [FullEquipType.Scepter, FullEquipType.StaffWhm, FullEquipType.StaffBlm],
            FullEquipType.StaffWhm => [FullEquipType.Scepter, FullEquipType.Wand, FullEquipType.StaffBlm],
            FullEquipType.StaffBlm => [FullEquipType.Scepter, FullEquipType.Wand, FullEquipType.StaffWhm],
            _                      => [],
        };

    /// <summary> Return whether a FullEquipType is a primary or secondary tool. </summary>
    public static bool IsTool(this FullEquipType type)
        => type switch
        {
            FullEquipType.Saw             => true,
            FullEquipType.CrossPeinHammer => true,
            FullEquipType.RaisingHammer   => true,
            FullEquipType.LapidaryHammer  => true,
            FullEquipType.Knife           => true,
            FullEquipType.Needle          => true,
            FullEquipType.Alembic         => true,
            FullEquipType.Frypan          => true,
            FullEquipType.Pickaxe         => true,
            FullEquipType.Hatchet         => true,
            FullEquipType.FishingRod      => true,
            FullEquipType.ClawHammer      => true,
            FullEquipType.File            => true,
            FullEquipType.Pliers          => true,
            FullEquipType.GrindingWheel   => true,
            FullEquipType.Awl             => true,
            FullEquipType.SpinningWheel   => true,
            FullEquipType.Mortar          => true,
            FullEquipType.CulinaryKnife   => true,
            FullEquipType.Sledgehammer    => true,
            FullEquipType.GardenScythe    => true,
            FullEquipType.Gig             => true,
            _                             => false,
        };

    /// <summary> Return whether a FullEquipType is a piece of primary equipment. </summary>
    public static bool IsEquipment(this FullEquipType type)
        => type switch
        {
            FullEquipType.Head  => true,
            FullEquipType.Body  => true,
            FullEquipType.Hands => true,
            FullEquipType.Legs  => true,
            FullEquipType.Feet  => true,
            _                   => false,
        };

    /// <summary> Return whether a FullEquipType is a piece of secondary equipment. </summary>
    public static bool IsAccessory(this FullEquipType type)
        => type switch
        {
            FullEquipType.Ears   => true,
            FullEquipType.Neck   => true,
            FullEquipType.Wrists => true,
            FullEquipType.Finger => true,
            _                    => false,
        };

    /// <summary> Return whether a FullEquipType is a bonus slot. </summary>
    public static bool IsBonus(this FullEquipType type)
        => type switch
        {
            FullEquipType.Glasses => true,
            _                     => false,
        };

    public static BonusItemFlag ToBonus(this FullEquipType type)
        => type switch
        {
            FullEquipType.Glasses => BonusItemFlag.Glasses,
            _                     => BonusItemFlag.Unknown,
        };

    /// <summary> Return the actual equipment slot a FullEquipType will be equipped to. </summary>
    public static EquipSlot ToSlot(this FullEquipType type)
        => type switch
        {
            FullEquipType.Head            => EquipSlot.Head,
            FullEquipType.Body            => EquipSlot.Body,
            FullEquipType.Hands           => EquipSlot.Hands,
            FullEquipType.Legs            => EquipSlot.Legs,
            FullEquipType.Feet            => EquipSlot.Feet,
            FullEquipType.Ears            => EquipSlot.Ears,
            FullEquipType.Neck            => EquipSlot.Neck,
            FullEquipType.Wrists          => EquipSlot.Wrists,
            FullEquipType.Finger          => EquipSlot.RFinger,
            FullEquipType.Fists           => EquipSlot.MainHand,
            FullEquipType.FistsOff        => EquipSlot.OffHand,
            FullEquipType.Sword           => EquipSlot.MainHand,
            FullEquipType.Axe             => EquipSlot.MainHand,
            FullEquipType.Bow             => EquipSlot.MainHand,
            FullEquipType.BowOff          => EquipSlot.OffHand,
            FullEquipType.Lance           => EquipSlot.MainHand,
            FullEquipType.StaffBlm        => EquipSlot.MainHand,
            FullEquipType.StaffWhm        => EquipSlot.MainHand,
            FullEquipType.Wand            => EquipSlot.MainHand,
            FullEquipType.Book            => EquipSlot.MainHand,
            FullEquipType.Daggers         => EquipSlot.MainHand,
            FullEquipType.DaggersOff      => EquipSlot.OffHand,
            FullEquipType.Broadsword      => EquipSlot.MainHand,
            FullEquipType.Gun             => EquipSlot.MainHand,
            FullEquipType.GunOff          => EquipSlot.OffHand,
            FullEquipType.Orrery          => EquipSlot.MainHand,
            FullEquipType.OrreryOff       => EquipSlot.OffHand,
            FullEquipType.Katana          => EquipSlot.MainHand,
            FullEquipType.KatanaOff       => EquipSlot.OffHand,
            FullEquipType.Rapier          => EquipSlot.MainHand,
            FullEquipType.RapierOff       => EquipSlot.OffHand,
            FullEquipType.Cane            => EquipSlot.MainHand,
            FullEquipType.Scepter         => EquipSlot.MainHand,
            FullEquipType.Gunblade        => EquipSlot.MainHand,
            FullEquipType.Glaives         => EquipSlot.MainHand,
            FullEquipType.GlaivesOff      => EquipSlot.OffHand,
            FullEquipType.Scythe          => EquipSlot.MainHand,
            FullEquipType.Nouliths        => EquipSlot.MainHand,
            FullEquipType.Shield          => EquipSlot.OffHand,
            FullEquipType.Saw             => EquipSlot.MainHand,
            FullEquipType.CrossPeinHammer => EquipSlot.MainHand,
            FullEquipType.RaisingHammer   => EquipSlot.MainHand,
            FullEquipType.LapidaryHammer  => EquipSlot.MainHand,
            FullEquipType.Knife           => EquipSlot.MainHand,
            FullEquipType.Needle          => EquipSlot.MainHand,
            FullEquipType.Alembic         => EquipSlot.MainHand,
            FullEquipType.Frypan          => EquipSlot.MainHand,
            FullEquipType.Pickaxe         => EquipSlot.MainHand,
            FullEquipType.Hatchet         => EquipSlot.MainHand,
            FullEquipType.FishingRod      => EquipSlot.MainHand,
            FullEquipType.ClawHammer      => EquipSlot.OffHand,
            FullEquipType.File            => EquipSlot.OffHand,
            FullEquipType.Pliers          => EquipSlot.OffHand,
            FullEquipType.GrindingWheel   => EquipSlot.OffHand,
            FullEquipType.Awl             => EquipSlot.OffHand,
            FullEquipType.SpinningWheel   => EquipSlot.OffHand,
            FullEquipType.Mortar          => EquipSlot.OffHand,
            FullEquipType.CulinaryKnife   => EquipSlot.OffHand,
            FullEquipType.Sledgehammer    => EquipSlot.OffHand,
            FullEquipType.GardenScythe    => EquipSlot.OffHand,
            FullEquipType.Gig             => EquipSlot.OffHand,
            FullEquipType.Twinfangs       => EquipSlot.MainHand,
            FullEquipType.TwinfangsOff    => EquipSlot.OffHand,
            FullEquipType.Brush           => EquipSlot.MainHand,
            FullEquipType.Palette         => EquipSlot.OffHand,
            FullEquipType.Handaxe         => EquipSlot.MainHand,
            FullEquipType.Glasses         => BonusItemFlag.Glasses.ToEquipSlot(),
            FullEquipType.UnknownMainhand => EquipSlot.MainHand,
            FullEquipType.UnknownOffhand  => EquipSlot.OffHand,
            _                             => EquipSlot.Unknown,
        };

    /// <summary> Convert an EquipSlot and a weapon category to a FullEquipType. </summary>
    /// <param name="slot"> The slot to convert. </param>
    /// <param name="category"> The weapon category to use if the slot is mainhand or offhand. </param>
    /// <param name="mainhand"> Whether to use the mainhand or offhand type of weapon. </param>
    public static FullEquipType ToEquipType(this EquipSlot slot, WeaponCategory category = WeaponCategory.Unknown, bool mainhand = true)
        => slot switch
        {
            EquipSlot.Head              => FullEquipType.Head,
            EquipSlot.Body              => FullEquipType.Body,
            EquipSlot.Hands             => FullEquipType.Hands,
            EquipSlot.Legs              => FullEquipType.Legs,
            EquipSlot.Feet              => FullEquipType.Feet,
            EquipSlot.Ears              => FullEquipType.Ears,
            EquipSlot.Neck              => FullEquipType.Neck,
            EquipSlot.Wrists            => FullEquipType.Wrists,
            EquipSlot.RFinger           => FullEquipType.Finger,
            EquipSlot.LFinger           => FullEquipType.Finger,
            EquipSlot.HeadBody          => FullEquipType.Body,
            EquipSlot.BodyHandsLegsFeet => FullEquipType.Body,
            EquipSlot.LegsFeet          => FullEquipType.Legs,
            EquipSlot.FullBody          => FullEquipType.Body,
            EquipSlot.BodyHands         => FullEquipType.Body,
            EquipSlot.BodyLegsFeet      => FullEquipType.Body,
            EquipSlot.ChestHands        => FullEquipType.Body,
            EquipSlot.ChestLegs         => FullEquipType.Body,
            EquipSlot.MainHand          => category.ToEquipType(mainhand),
            EquipSlot.OffHand           => category.ToEquipType(mainhand),
            EquipSlot.BothHand          => category.ToEquipType(mainhand),
            _                           => FullEquipType.Unknown,
        };

    public static FullEquipType ToEquipType(this BonusItemFlag bonusSlot)
        => bonusSlot switch
        {
            BonusItemFlag.Glasses => FullEquipType.Glasses,
            BonusItemFlag.UnkSlot => FullEquipType.Unknown,
            _                     => FullEquipType.Unknown,
        };

    /// <summary> Convert a weapon category to a FullEquipType. </summary>
    /// <param name="category"> The category to convert. </param>
    /// <param name="mainhand"> Whether to use the mainhand or offhand type of weapon. </param>
    public static FullEquipType ToEquipType(this WeaponCategory category, bool mainhand = true)
        => category switch
        {
            WeaponCategory.Pugilist when mainhand    => FullEquipType.Fists,
            WeaponCategory.Pugilist                  => FullEquipType.FistsOff,
            WeaponCategory.Gladiator                 => FullEquipType.Sword,
            WeaponCategory.Marauder                  => FullEquipType.Axe,
            WeaponCategory.Archer when mainhand      => FullEquipType.Bow,
            WeaponCategory.Archer                    => FullEquipType.BowOff,
            WeaponCategory.Lancer                    => FullEquipType.Lance,
            WeaponCategory.Thaumaturge1              => FullEquipType.Scepter,
            WeaponCategory.Thaumaturge2              => FullEquipType.StaffBlm,
            WeaponCategory.Conjurer1                 => FullEquipType.Wand,
            WeaponCategory.Conjurer2                 => FullEquipType.StaffWhm,
            WeaponCategory.Arcanist                  => FullEquipType.Book,
            WeaponCategory.Shield                    => FullEquipType.Shield,
            WeaponCategory.CarpenterMain             => FullEquipType.Saw,
            WeaponCategory.CarpenterOff              => FullEquipType.ClawHammer,
            WeaponCategory.BlacksmithMain            => FullEquipType.CrossPeinHammer,
            WeaponCategory.BlacksmithOff             => FullEquipType.File,
            WeaponCategory.ArmorerMain               => FullEquipType.RaisingHammer,
            WeaponCategory.ArmorerOff                => FullEquipType.Pliers,
            WeaponCategory.GoldsmithMain             => FullEquipType.LapidaryHammer,
            WeaponCategory.GoldsmithOff              => FullEquipType.GrindingWheel,
            WeaponCategory.LeatherworkerMain         => FullEquipType.Knife,
            WeaponCategory.LeatherworkerOff          => FullEquipType.Awl,
            WeaponCategory.WeaverMain                => FullEquipType.Needle,
            WeaponCategory.WeaverOff                 => FullEquipType.SpinningWheel,
            WeaponCategory.AlchemistMain             => FullEquipType.Alembic,
            WeaponCategory.AlchemistOff              => FullEquipType.Mortar,
            WeaponCategory.CulinarianMain            => FullEquipType.Frypan,
            WeaponCategory.CulinarianOff             => FullEquipType.CulinaryKnife,
            WeaponCategory.MinerMain                 => FullEquipType.Pickaxe,
            WeaponCategory.MinerOff                  => FullEquipType.Sledgehammer,
            WeaponCategory.BotanistMain              => FullEquipType.Hatchet,
            WeaponCategory.BotanistOff               => FullEquipType.GardenScythe,
            WeaponCategory.FisherMain                => FullEquipType.FishingRod,
            WeaponCategory.FisherOff                 => FullEquipType.Gig,
            WeaponCategory.Rogue when mainhand       => FullEquipType.Daggers,
            WeaponCategory.Rogue                     => FullEquipType.DaggersOff,
            WeaponCategory.DarkKnight                => FullEquipType.Broadsword,
            WeaponCategory.Machinist when mainhand   => FullEquipType.Gun,
            WeaponCategory.Machinist                 => FullEquipType.GunOff,
            WeaponCategory.Astrologian when mainhand => FullEquipType.Orrery,
            WeaponCategory.Astrologian               => FullEquipType.OrreryOff,
            WeaponCategory.Samurai when mainhand     => FullEquipType.Katana,
            WeaponCategory.Samurai                   => FullEquipType.KatanaOff,
            WeaponCategory.RedMage when mainhand     => FullEquipType.Rapier,
            WeaponCategory.RedMage                   => FullEquipType.RapierOff,
            WeaponCategory.Scholar                   => FullEquipType.Book,
            WeaponCategory.BlueMage                  => FullEquipType.Cane,
            WeaponCategory.Gunbreaker                => FullEquipType.Gunblade,
            WeaponCategory.Dancer when mainhand      => FullEquipType.Glaives,
            WeaponCategory.Dancer                    => FullEquipType.GlaivesOff,
            WeaponCategory.Reaper                    => FullEquipType.Scythe,
            WeaponCategory.Sage                      => FullEquipType.Nouliths,
            WeaponCategory.Viper when mainhand       => FullEquipType.Twinfangs,
            WeaponCategory.Viper                     => FullEquipType.TwinfangsOff,
            WeaponCategory.Pictomancer when mainhand => FullEquipType.Brush,
            WeaponCategory.Pictomancer               => FullEquipType.Palette,
            WeaponCategory.Beastmaster               => FullEquipType.Handaxe,
            _ when mainhand                          => FullEquipType.UnknownMainhand,
            _                                        => FullEquipType.UnknownOffhand,
        };

    /// <summary> Obtain the correct offhand FullEquipType for a Mainhand FullEquipType, excluding tools. </summary>
    public static FullEquipType ValidOffhand(this FullEquipType type)
        => type switch
        {
            FullEquipType.Fists     => FullEquipType.FistsOff,
            FullEquipType.Sword     => FullEquipType.Shield,
            FullEquipType.Wand      => FullEquipType.Shield,
            FullEquipType.Scepter   => FullEquipType.Shield,
            FullEquipType.Handaxe   => FullEquipType.Shield,
            FullEquipType.Daggers   => FullEquipType.DaggersOff,
            FullEquipType.Gun       => FullEquipType.GunOff,
            FullEquipType.Orrery    => FullEquipType.OrreryOff,
            FullEquipType.Rapier    => FullEquipType.RapierOff,
            FullEquipType.Glaives   => FullEquipType.GlaivesOff,
            FullEquipType.Bow       => FullEquipType.BowOff,
            FullEquipType.Katana    => FullEquipType.KatanaOff,
            FullEquipType.Twinfangs => FullEquipType.TwinfangsOff,
            FullEquipType.Brush     => FullEquipType.Palette,
            _                       => FullEquipType.Unknown,
        };

    /// <summary> Obtain the correct offhand FullEquipType for a Mainhand FullEquipType, including tools. </summary>
    public static FullEquipType Offhand(this FullEquipType type)
        => type switch
        {
            FullEquipType.Fists           => FullEquipType.FistsOff,
            FullEquipType.Sword           => FullEquipType.Shield,
            FullEquipType.Wand            => FullEquipType.Shield,
            FullEquipType.Scepter         => FullEquipType.Shield,
            FullEquipType.Handaxe         => FullEquipType.Shield,
            FullEquipType.Daggers         => FullEquipType.DaggersOff,
            FullEquipType.Gun             => FullEquipType.GunOff,
            FullEquipType.Orrery          => FullEquipType.OrreryOff,
            FullEquipType.Rapier          => FullEquipType.RapierOff,
            FullEquipType.Glaives         => FullEquipType.GlaivesOff,
            FullEquipType.Bow             => FullEquipType.BowOff,
            FullEquipType.Katana          => FullEquipType.KatanaOff,
            FullEquipType.Saw             => FullEquipType.ClawHammer,
            FullEquipType.CrossPeinHammer => FullEquipType.File,
            FullEquipType.RaisingHammer   => FullEquipType.Pliers,
            FullEquipType.LapidaryHammer  => FullEquipType.GrindingWheel,
            FullEquipType.Knife           => FullEquipType.Awl,
            FullEquipType.Needle          => FullEquipType.SpinningWheel,
            FullEquipType.Alembic         => FullEquipType.Mortar,
            FullEquipType.Frypan          => FullEquipType.CulinaryKnife,
            FullEquipType.Pickaxe         => FullEquipType.Sledgehammer,
            FullEquipType.Hatchet         => FullEquipType.GardenScythe,
            FullEquipType.FishingRod      => FullEquipType.Gig,
            FullEquipType.Twinfangs       => FullEquipType.TwinfangsOff,
            FullEquipType.Brush           => FullEquipType.Palette,
            _                             => FullEquipType.Unknown,
        };

    /// <summary> Whether an Offhand FullEquipType allows putting Nothing into the Offhand slot. </summary>
    public static bool AllowsNothing(this FullEquipType type)
        => type switch
        {
            FullEquipType.Head      => true,
            FullEquipType.Body      => true,
            FullEquipType.Hands     => true,
            FullEquipType.Legs      => true,
            FullEquipType.Feet      => true,
            FullEquipType.Ears      => true,
            FullEquipType.Neck      => true,
            FullEquipType.Wrists    => true,
            FullEquipType.Finger    => true,
            FullEquipType.BowOff    => true,
            FullEquipType.GunOff    => true,
            FullEquipType.OrreryOff => true,
            FullEquipType.KatanaOff => true,
            FullEquipType.RapierOff => true,
            FullEquipType.Shield    => true,
            FullEquipType.Palette   => true,
            FullEquipType.Glasses   => true,
            _                       => false,
        };

    /// <summary> The human-readable suffix for inferred offhand types. </summary>
    internal static string OffhandTypeSuffix(this FullEquipType type)
        => type switch
        {
            FullEquipType.FistsOff     => " (Offhand)",
            FullEquipType.DaggersOff   => " (Offhand)",
            FullEquipType.GunOff       => " (Aetherotransformer)",
            FullEquipType.OrreryOff    => " (Card Holder)",
            FullEquipType.RapierOff    => " (Focus)",
            FullEquipType.GlaivesOff   => " (Offhand)",
            FullEquipType.BowOff       => " (Quiver)",
            FullEquipType.KatanaOff    => " (Sheathe)",
            FullEquipType.TwinfangsOff => " (Offhand)",
            FullEquipType.Palette      => " (Palette)",
            _                          => string.Empty,
        };

    /// <summary> Whether a FullEquipType is an inferred offhand type. </summary>
    public static bool IsOffhandType(this FullEquipType type)
        => type.OffhandTypeSuffix().Length > 0;

    /// <summary> A list of all weapon types. </summary>
    public static readonly IReadOnlyList<FullEquipType> WeaponTypes
        = FullEquipType.Values.Where(v => v.IsWeapon()).Except([FullEquipType.UnknownMainhand])
            .ToArray();

    /// <summary> A list of all tool types, including offhands. </summary>
    public static readonly IReadOnlyList<FullEquipType> ToolTypes
        = FullEquipType.Values.Where(v => v.IsTool()).ToArray();

    /// <summary> A list of all equipment types. </summary>
    public static readonly IReadOnlyList<FullEquipType> EquipmentTypes
        = FullEquipType.Values.Where(v => v.IsEquipment()).ToArray();

    /// <summary> A list of all accessory types. </summary>
    public static readonly IReadOnlyList<FullEquipType> AccessoryTypes
        = FullEquipType.Values.Where(v => v.IsAccessory()).ToArray();

    /// <summary> A list of all inferred offhand types. </summary>
    public static readonly IReadOnlyList<FullEquipType> OffhandTypes
        = FullEquipType.Values.Where(IsOffhandType).ToArray();

    /// <summary> A list of all inferred offhand types. </summary>
    public static readonly IReadOnlyList<FullEquipType> BonusTypes
        = FullEquipType.Values.Where(IsBonus).ToArray();
}
