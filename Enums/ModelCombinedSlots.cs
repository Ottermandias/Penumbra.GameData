using Luna.Generators;

namespace Penumbra.GameData.Enums;

using static ModelCombinedSlots;

/// <summary>
/// Combined enum that can represent a slot of a model (in one-hot encoding)
/// or a combination of them, including weapons and ornaments.
/// </summary>
[Flags]
[NamedEnum(Class: nameof(ModelCombinedSlotsExtensions))]
public enum ModelCombinedSlots : ulong
{
    // Bit/byte allocation diagram:
    // +----+----------+----------+
    // |    | 76543210 | Overall  |
    // +----+----------+----------+
    // |  0 | WNEFLHBH | Human    |
    // |  8 | BCCTFHLR | Human    |
    // | 16 | ......BF | Human    |
    // | 24 | ........ | -        |
    // | 32 | .......M | Mainhand |
    // | 40 | .......O | Offhand  |
    // | 48 | ........ | -        |
    // | 56 | ........ | -        |
    // +----+----------+----------+

    #region Human

    [Name("Head")]
    Head = 1UL << 0,

    [Name("Body")]
    Body = 1UL << 1,

    [Name("Hands")]
    Hands = 1UL << 2,

    [Name("Legs")]
    Legs = 1UL << 3,

    [Name("Feet")]
    Feet = 1UL << 4,

    [Name("Earrings")]
    Ears = 1UL << 5,

    [Name("Necklace")]
    Neck = 1UL << 6,

    [Name("Bracelets")]
    Wrists = 1UL << 7,

    [Name("Right Ring")]
    RightFinger = 1UL << 8,

    [Name("Left Ring")]
    LeftFinger = 1UL << 9,

    [Name("Hair")]
    Hair = 1UL << 10,

    [Name("Face")]
    Face = 1UL << 11,

    [Name("Tail / Ears")]
    RacialFeature = 1UL << 12,

    [Name("Connector 1")]
    Connector1 = 1UL << 13,

    [Name("Connector 2")]
    Connector2 = 1UL << 14,

    [Name("Body 3")]
    Body3 = 1UL << 15,

    [Name("Facewear")]
    Facewear = 1UL << 16,

    [Name("Unused Bonus Slot")]
    Bonus2 = 1UL << 17,

    #endregion

    #region Weapons

    [Name("Primary Weapon")]
    Mainhand = 1UL << 32,

    [Name("Secondary Weapon")]
    Offhand = 1UL << 40,

    #endregion
}

/// <summary> Extension methods for, and related to, <see cref="ModelCombinedSlots"/>. </summary>
public static partial class ModelCombinedSlotsExtensions
{
    public const ModelCombinedSlots AllEquipment       = Head | Body | Hands | Legs | Feet;
    public const ModelCombinedSlots AllAccessories     = Ears | Neck | Wrists | RightFinger | LeftFinger;
    public const ModelCombinedSlots AllEquipmentPieces = AllEquipment | AllAccessories;

    public const ModelCombinedSlots AllCustomization = Hair | Face | RacialFeature | Connector1 | Connector2 | Body3;
    public const ModelCombinedSlots AllBonus         = Facewear;

    public const ModelCombinedSlots AllHuman   = AllEquipment | AllAccessories | AllCustomization | AllBonus;
    public const ModelCombinedSlots AllWeapons = Mainhand | Offhand;
    public const ModelCombinedSlots All        = AllHuman | AllWeapons;

    public const ModelCombinedSlots BonusItemFlagMask        = Facewear | Bonus2;
    public const ModelCombinedSlots CrestFlagMask            = AllEquipmentPieces | AllWeapons;
    public const ModelCombinedSlots EquipFlagMask            = AllEquipmentPieces | AllWeapons;
    public const ModelCombinedSlots StainEquipFlagMask       = AllEquipmentPieces | AllWeapons;
    public const ModelCombinedSlots CombinedItemSlotFlagMask = EquipFlagMask | BonusItemFlagMask;
    public const ModelCombinedSlots EquipSlotMask            = AllEquipmentPieces | AllWeapons;
    public const ModelCombinedSlots FullEquipTypeMask        = AllEquipmentPieces | Facewear | AllWeapons;
    public const ModelCombinedSlots HumanSlotMask            = AllEquipmentPieces | Hair | Face | RacialFeature | BonusItemFlagMask;

    /// <summary> Extension methods for <see cref="ModelCombinedSlots"/>. </summary>
    /// <param name="slots"> The slots. </param>
    extension(ModelCombinedSlots slots)
    {
        #region Accessors

        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => BitOperations.PopCount((ulong)slots);
        }

        public ModelCombinedSlots First
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => unchecked((ModelCombinedSlots)((long)slots & -(long)slots));
        }

        public ModelCombinedSlots ExceptFirst
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => unchecked((ModelCombinedSlots)((long)slots & ((long)slots - 1L)));
        }

        #endregion

        #region Outbound Conversions

        public BonusItemFlag ToBonusItemFlag()
            => unchecked((BonusItemFlag)((ulong)(slots & BonusItemFlagMask) >> 16));

        public CrestFlag ToCrestFlag()
        {
            var equipment = (ulong)(slots & AllEquipmentPieces) << 1;
            var mainhand  = (ulong)(slots & Mainhand) >> 21;
            var offhand   = (ulong)(slots & Offhand) >> 40;
            return unchecked((CrestFlag)(equipment | mainhand | offhand));
        }

        public EquipFlag ToEquipFlag()
        {
            var equipment = (ulong)(slots & AllEquipmentPieces);
            var mainhand  = (ulong)(slots & Mainhand) >> 22;
            var offhand   = (ulong)(slots & Offhand) >> 29;
            return unchecked((EquipFlag)(equipment | mainhand | offhand));
        }

        public EquipFlag ToStainEquipFlag()
        {
            var equipment = (ulong)(slots & AllEquipmentPieces) << 12;
            var mainhand  = (ulong)(slots & Mainhand) >> 10;
            var offhand   = (ulong)(slots & Offhand) >> 17;
            return unchecked((EquipFlag)(equipment | mainhand | offhand));
        }

        public CombinedItemSlotFlag ToCombinedItemSlotFlag()
        {
            var equipment = (ulong)(slots & AllEquipmentPieces);
            var bonus     = (ulong)(slots & BonusItemFlagMask) >> 4;
            var mainhand  = (ulong)(slots & Mainhand) >> 22;
            var offhand   = (ulong)(slots & Offhand) >> 29;
            return unchecked((CombinedItemSlotFlag)(equipment | bonus | mainhand | offhand));
        }

        /// <returns>
        /// The <see cref="EquipSlot"/> that exactly matches the given <see cref="ModelCombinedSlots"/>, if any.
        /// Otherwise, <see cref="EquipSlot.Unknown"/>.
        /// </returns>
        public EquipSlot ToEquipSlot()
            => slots switch
            {
                0 => EquipSlot.Nothing,

                Head        => EquipSlot.Head,
                Body        => EquipSlot.Body,
                Hands       => EquipSlot.Hands,
                Legs        => EquipSlot.Legs,
                Feet        => EquipSlot.Feet,
                Ears        => EquipSlot.Ears,
                Neck        => EquipSlot.Neck,
                Wrists      => EquipSlot.Wrists,
                RightFinger => EquipSlot.RFinger,
                LeftFinger  => EquipSlot.LFinger,
                Mainhand    => EquipSlot.MainHand,
                Offhand     => EquipSlot.OffHand,

                Head | Body        => EquipSlot.HeadBody,
                Body | Hands       => EquipSlot.ChestHands,
                Body | Legs        => EquipSlot.ChestLegs,
                Legs | Feet        => EquipSlot.LegsFeet,
                Mainhand | Offhand => EquipSlot.BothHand,

                Body | Hands | Legs => EquipSlot.BodyHands,
                Body | Legs | Feet  => EquipSlot.BodyLegsFeet,

                Body | Hands | Legs | Feet => EquipSlot.BodyHandsLegsFeet,

                AllEquipment => EquipSlot.FullBody,

                AllEquipmentPieces => EquipSlot.All,

                _ => EquipSlot.Unknown,
            };

        /// <returns>
        /// The <see cref="FullEquipType"/> that corresponds to the least significant applicable bit
        /// of the given <see cref="ModelCombinedSlots"/>.
        /// </returns>
        public FullEquipType ToFullEquipType()
            => BitOperations.TrailingZeroCount((ulong)(slots & FullEquipTypeMask)) switch
            {
                0      => FullEquipType.Head,
                1      => FullEquipType.Body,
                2      => FullEquipType.Hands,
                3      => FullEquipType.Legs,
                4      => FullEquipType.Feet,
                5      => FullEquipType.Ears,
                6      => FullEquipType.Neck,
                7      => FullEquipType.Wrists,
                8 or 9 => FullEquipType.Finger,
                16     => FullEquipType.Glasses,
                32     => FullEquipType.UnknownMainhand,
                48     => FullEquipType.UnknownOffhand,
                _      => FullEquipType.Unknown,
            };

        /// <returns>
        /// The <see cref="HumanSlot"/> that corresponds to the least significant applicable bit
        /// of the given <see cref="ModelCombinedSlots"/>.
        /// </returns>
        public HumanSlot ToHumanSlot()
        {
            var slot = (HumanSlot)BitOperations.TrailingZeroCount((ulong)(slots & HumanSlotMask));
            return slot is <= HumanSlot.Ear or HumanSlot.Glasses or HumanSlot.UnkBonus
                ? slot
                : HumanSlot.Unknown;
        }

        #endregion
    }

    #region Inbound Conversions

    public static ModelCombinedSlots ToModelCombinedSlots(this BonusItemFlag flag)
        => (ModelCombinedSlots)((ulong)flag << 16) & BonusItemFlagMask;

    public static ModelCombinedSlots ToModelCombinedSlots(this CrestFlag flag)
    {
        var equipment = (ModelCombinedSlots)((ulong)flag >> 1) & AllEquipmentPieces;
        var mainhand  = (ModelCombinedSlots)((ulong)flag << 21) & Mainhand;
        var offhand   = (ModelCombinedSlots)((ulong)flag << 40) & Offhand;
        return equipment | mainhand | offhand;
    }

    extension(EquipFlag flag)
    {
        public ModelCombinedSlots ToModelCombinedSlots()
        {
            var equipment = (ModelCombinedSlots)flag & AllEquipmentPieces;
            var mainhand  = (ModelCombinedSlots)((ulong)flag << 22) & Mainhand;
            var offhand   = (ModelCombinedSlots)((ulong)flag << 29) & Offhand;
            return equipment | mainhand | offhand;
        }

        public ModelCombinedSlots StainToModelCombinedSlots()
        {
            var equipment = (ModelCombinedSlots)((ulong)flag >> 12) & AllEquipmentPieces;
            var mainhand  = (ModelCombinedSlots)((ulong)flag << 10) & Mainhand;
            var offhand   = (ModelCombinedSlots)((ulong)flag << 17) & Offhand;
            return equipment | mainhand | offhand;
        }
    }

    public static ModelCombinedSlots ToModelCombinedSlots(this CombinedItemSlotFlag flag)
    {
        var equipment = (ModelCombinedSlots)flag & AllEquipmentPieces;
        var bonus     = (ModelCombinedSlots)((ulong)flag << 4) & BonusItemFlagMask;
        var mainhand  = (ModelCombinedSlots)((ulong)flag << 22) & Mainhand;
        var offhand   = (ModelCombinedSlots)((ulong)flag << 29) & Offhand;
        return equipment | bonus | mainhand | offhand;
    }

    public static ModelCombinedSlots ToModelCombinedSlots(this EquipSlot slot)
        => slot switch
        {
            EquipSlot.Unknown           => 0,
            EquipSlot.MainHand          => Mainhand,
            EquipSlot.OffHand           => Offhand,
            EquipSlot.Head              => Head,
            EquipSlot.Body              => Body,
            EquipSlot.Hands             => Hands,
            EquipSlot.Belt              => 0,
            EquipSlot.Legs              => Legs,
            EquipSlot.Feet              => Feet,
            EquipSlot.Ears              => Ears,
            EquipSlot.Neck              => Neck,
            EquipSlot.Wrists            => Wrists,
            EquipSlot.RFinger           => RightFinger,
            EquipSlot.BothHand          => Mainhand | Offhand,
            EquipSlot.LFinger           => LeftFinger,
            EquipSlot.HeadBody          => Head | Body,
            EquipSlot.BodyHandsLegsFeet => Body | Hands | Legs | Feet,
            EquipSlot.SoulCrystal       => 0,
            EquipSlot.LegsFeet          => Legs | Feet,
            EquipSlot.FullBody          => AllEquipment,
            EquipSlot.BodyHands         => Body | Hands | Legs,
            EquipSlot.BodyLegsFeet      => Body | Legs | Feet,
            EquipSlot.ChestHands        => Body | Hands,
            EquipSlot.ChestLegs         => Body | Legs,
            EquipSlot.Nothing           => 0,
            EquipSlot.All               => AllEquipmentPieces,
            _                           => 0,
        };

    public static ModelCombinedSlots ToModelCombinedSlots(this FullEquipType type)
        => type is FullEquipType.Glasses
            ? Facewear
            : type.ToSlot().ToModelCombinedSlots();

    public static ModelCombinedSlots ToModelCombinedSlots(this HumanSlot slot)
        => slot is <= HumanSlot.Ear or HumanSlot.Glasses or HumanSlot.UnkBonus
            ? unchecked((ModelCombinedSlots)(1UL << (int)slot))
            : 0;

    #endregion
}
