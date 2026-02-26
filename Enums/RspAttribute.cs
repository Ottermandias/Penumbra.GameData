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

    [Name("Male Minimum Tail Length")]
    MaleMinTail,

    [Name("Male Maximum Tail Length")]
    MaleMaxTail,

    [Name("Female Minimum Size")]
    FemaleMinSize,

    [Name("Female Maximum Size")]
    FemaleMaxSize,

    [Name("Female Minimum Tail Length")]
    FemaleMinTail,

    [Name("Female Maximum Tail Length")]
    FemaleMaxTail,

    [Name("Bust Minimum X-Axis")]
    BustMinX,

    [Name("Bust Minimum Y-Axis")]
    BustMinY,

    [Name("Bust Minimum Z-Axis")]
    BustMinZ,

    [Name("Bust Maximum X-Axis")]
    BustMaxX,

    [Name("Bust Maximum Y-Axis")]
    BustMaxY,

    [Name("Bust Maximum Z-Axis")]
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
