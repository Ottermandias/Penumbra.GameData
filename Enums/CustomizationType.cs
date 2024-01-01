namespace Penumbra.GameData.Enums;

/// <summary> Human Customizations. </summary>
public enum CustomizationType : byte
{
    Unknown,
    Body,
    Tail,
    Face,
    Iris,
    Accessory,
    Hair,
    Ear,
    DecalFace,
    DecalEquip,
    Skin,
    Etc,
}

public static class CustomizationTypeEnumExtension
{
    /// <summary> Convert a customization type to the suffix used by the game. </summary>
    public static string ToSuffix(this CustomizationType value)
        => value switch
        {
            CustomizationType.Body      => "top",
            CustomizationType.Face      => "fac",
            CustomizationType.Iris      => "iri",
            CustomizationType.Accessory => "acc",
            CustomizationType.Hair      => "hir",
            CustomizationType.Tail      => "til",
            CustomizationType.Ear       => "zer",
            CustomizationType.Etc       => "etc",
            _                           => throw new InvalidEnumArgumentException(),
        };
}

public static partial class Names
{
    /// <summary> A dictionary converting path suffices into CustomizationType. </summary>
    // TODO: FrozenDictionary
    public static readonly Dictionary<string, CustomizationType> SuffixToCustomizationType = new()
    {
        { CustomizationType.Body.ToSuffix(), CustomizationType.Body },
        { CustomizationType.Face.ToSuffix(), CustomizationType.Face },
        { CustomizationType.Iris.ToSuffix(), CustomizationType.Iris },
        { CustomizationType.Accessory.ToSuffix(), CustomizationType.Accessory },
        { CustomizationType.Hair.ToSuffix(), CustomizationType.Hair },
        { CustomizationType.Tail.ToSuffix(), CustomizationType.Tail },
        { CustomizationType.Ear.ToSuffix(), CustomizationType.Ear },
        { CustomizationType.Etc.ToSuffix(), CustomizationType.Etc },
    };
}
