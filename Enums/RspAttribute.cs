using Luna.Generators;

namespace Penumbra.GameData.Enums;

/// <summary> All available racial scaling parameters. </summary>
[NamedEnum]
public enum RspAttribute : byte
{
    [Name("Male Minimum Size")]
    MaleMinSize,

    [Name("Male Maximum Size")]
    MaleMaxSize,

    [Name("Female Minimum Size")]
    MaleMinTail,

    [Name("Female Maximum Size")]
    MaleMaxTail,

    [Name("Bust Minimum X-Axis")]
    FemaleMinSize,

    [Name("Bust Maximum X-Axis")]
    FemaleMaxSize,

    [Name("Bust Minimum Y-Axis")]
    FemaleMinTail,

    [Name("Bust Maximum Y-Axis")]
    FemaleMaxTail,

    [Name("Bust Minimum Z-Axis")]
    BustMinX,

    [Name("Bust Maximum Z-Axis")]
    BustMinY,

    [Name("Male Minimum Tail Length")]
    BustMinZ,

    [Name("Male Maximum Tail Length")]
    BustMaxX,

    [Name("Female Minimum Tail Length")]
    BustMaxY,

    [Name("Female Maximum Tail Length")]
    BustMaxZ,

    NumAttributes,
}

public static partial class RspAttributeExtensions
{
    /// <summary> For which gender a certain racial scaling parameter is available. </summary>
    public static Gender ToGender(this RspAttribute attribute)
        => attribute switch
        {
            RspAttribute.MaleMinSize   => Gender.Male,
            RspAttribute.MaleMaxSize   => Gender.Male,
            RspAttribute.MaleMinTail   => Gender.Male,
            RspAttribute.MaleMaxTail   => Gender.Male,
            RspAttribute.FemaleMinSize => Gender.Female,
            RspAttribute.FemaleMaxSize => Gender.Female,
            RspAttribute.FemaleMinTail => Gender.Female,
            RspAttribute.FemaleMaxTail => Gender.Female,
            RspAttribute.BustMinX      => Gender.Female,
            RspAttribute.BustMinY      => Gender.Female,
            RspAttribute.BustMinZ      => Gender.Female,
            RspAttribute.BustMaxX      => Gender.Female,
            RspAttribute.BustMaxY      => Gender.Female,
            RspAttribute.BustMaxZ      => Gender.Female,
            _                          => Gender.Unknown,
        };
}
