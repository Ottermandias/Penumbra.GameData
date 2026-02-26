using ImSharp;
using Luna.Generators;

namespace Penumbra.GameData.Enums;

/// <summary> All options that can be configured for humans via the customize array. </summary>
[NamedEnum]
public enum CustomizeIndex : byte
{
    [Name("Race")]
    Race,

    [Name("Gender")]
    Gender,

    [Name("Body Type")]
    BodyType,

    [Name("Height")]
    Height,

    [Name("Clan")]
    Clan,

    [Name("Head Style")]
    Face,

    [Name("Hair Style")]
    Hairstyle,

    [Name("Enable Highlights")]
    Highlights,

    [Name("Skin Color")]
    SkinColor,

    [Name("Right Eye")]
    EyeColorRight,

    [Name("Hair Color")]
    HairColor,

    [Name("Highlights Color")]
    HighlightsColor,

    [Name("Facial Feature 1")]
    FacialFeature1,

    [Name("Facial Feature 2")]
    FacialFeature2,

    [Name("Facial Feature 3")]
    FacialFeature3,

    [Name("Facial Feature 4")]
    FacialFeature4,

    [Name("Facial Feature 5")]
    FacialFeature5,

    [Name("Facial Feature 6")]
    FacialFeature6,

    [Name("Facial Feature 7")]
    FacialFeature7,

    [Name("Legacy Tattoo")]
    LegacyTattoo,

    [Name("Feature Color")]
    TattooColor,

    [Name("Eyebrow Style")]
    Eyebrows,

    [Name("Left Eye")]
    EyeColorLeft,

    [Name("Small Pupils")]
    EyeShape,

    [Name("Small Iris")]
    SmallIris,

    [Name("Nose Style")]
    Nose,

    [Name("Jaw Style")]
    Jaw,

    [Name("Mouth Style")]
    Mouth,

    [Name("Enable Lipstick")]
    Lipstick,

    [Name("Lip Color")]
    LipColor,

    [Name("Muscle Tone")]
    MuscleMass,

    [Name("Tail Shape")]
    TailShape,

    [Name("Bust Size")]
    BustSize,

    [Name("Face Paint")]
    FacePaint,

    [Name("Reverse Face Paint")]
    FacePaintReversed,

    [Name("Face Paint Color")]
    FacePaintColor,
}

public static class CustomizationExtensions
{
    /// <summary> The total number of options. </summary>
    public const int NumIndices = (int)CustomizeIndex.FacePaintColor + 1;

    /// <summary> A list of all options that are not race or body type. </summary>
    public static readonly CustomizeIndex[] All = CustomizeIndex.Values
        .Where(v => v is not CustomizeIndex.Race and not CustomizeIndex.BodyType).ToArray();

    /// <summary> A set of all options that are not race, gender, clan or body type. </summary>
    public static readonly CustomizeIndex[] AllBasic = All
        .Where(v => v is not CustomizeIndex.Gender and not CustomizeIndex.Clan).ToArray();

    /// <summary> A set of all options that are not race, gender, clan, face, or body type. </summary>
    public static readonly CustomizeIndex[] AllBasicWithoutFace = AllBasic
        .Where(v => v is not CustomizeIndex.Face).ToArray();

    /// <summary> Get the index of the customization option in the customize array, and a mask for its value. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static (int ByteIdx, byte Mask) ToByteAndMask(this CustomizeIndex index)
        => index switch
        {
            CustomizeIndex.Race              => (0, 0xFF),
            CustomizeIndex.Gender            => (1, 0xFF),
            CustomizeIndex.BodyType          => (2, 0xFF),
            CustomizeIndex.Height            => (3, 0xFF),
            CustomizeIndex.Clan              => (4, 0xFF),
            CustomizeIndex.Face              => (5, 0xFF),
            CustomizeIndex.Hairstyle         => (6, 0xFF),
            CustomizeIndex.Highlights        => (7, 0x80),
            CustomizeIndex.SkinColor         => (8, 0xFF),
            CustomizeIndex.EyeColorRight     => (9, 0xFF),
            CustomizeIndex.HairColor         => (10, 0xFF),
            CustomizeIndex.HighlightsColor   => (11, 0xFF),
            CustomizeIndex.FacialFeature1    => (12, 0x01),
            CustomizeIndex.FacialFeature2    => (12, 0x02),
            CustomizeIndex.FacialFeature3    => (12, 0x04),
            CustomizeIndex.FacialFeature4    => (12, 0x08),
            CustomizeIndex.FacialFeature5    => (12, 0x10),
            CustomizeIndex.FacialFeature6    => (12, 0x20),
            CustomizeIndex.FacialFeature7    => (12, 0x40),
            CustomizeIndex.LegacyTattoo      => (12, 0x80),
            CustomizeIndex.TattooColor       => (13, 0xFF),
            CustomizeIndex.Eyebrows          => (14, 0xFF),
            CustomizeIndex.EyeColorLeft      => (15, 0xFF),
            CustomizeIndex.EyeShape          => (16, 0x7F),
            CustomizeIndex.SmallIris         => (16, 0x80),
            CustomizeIndex.Nose              => (17, 0xFF),
            CustomizeIndex.Jaw               => (18, 0xFF),
            CustomizeIndex.Mouth             => (19, 0x7F),
            CustomizeIndex.Lipstick          => (19, 0x80),
            CustomizeIndex.LipColor          => (20, 0xFF),
            CustomizeIndex.MuscleMass        => (21, 0xFF),
            CustomizeIndex.TailShape         => (22, 0xFF),
            CustomizeIndex.BustSize          => (23, 0xFF),
            CustomizeIndex.FacePaint         => (24, 0x7F),
            CustomizeIndex.FacePaintReversed => (24, 0x80),
            CustomizeIndex.FacePaintColor    => (25, 0xFF),
            _                                => (0, 0x00),
        };
}
