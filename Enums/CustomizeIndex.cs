using ImSharp;

namespace Penumbra.GameData.Enums;

/// <summary> All options that can be configured for humans via the customize array. </summary>
public enum CustomizeIndex : byte
{
    Race,
    Gender,
    BodyType,
    Height,
    Clan,
    Face,
    Hairstyle,
    Highlights,
    SkinColor,
    EyeColorRight,
    HairColor,
    HighlightsColor,
    FacialFeature1,
    FacialFeature2,
    FacialFeature3,
    FacialFeature4,
    FacialFeature5,
    FacialFeature6,
    FacialFeature7,
    LegacyTattoo,
    TattooColor,
    Eyebrows,
    EyeColorLeft,
    EyeShape,
    SmallIris,
    Nose,
    Jaw,
    Mouth,
    Lipstick,
    LipColor,
    MuscleMass,
    TailShape,
    BustSize,
    FacePaint,
    FacePaintReversed,
    FacePaintColor,
}

public static class CustomizationExtensions
{
    /// <summary> The total number of options. </summary>
    public const int NumIndices = (int)CustomizeIndex.FacePaintColor + 1;

    /// <summary> A list of all options that are not race or body type. </summary>
    public static readonly CustomizeIndex[] All = Enum.GetValues<CustomizeIndex>()
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


    /// <summary> Get the human-readable name for a customization option. </summary>
    public static ReadOnlySpan<byte> ToDefaultName(this CustomizeIndex customizeIndex)
        => customizeIndex switch
        {
            CustomizeIndex.Race              => "Race"u8,
            CustomizeIndex.Gender            => "Gender"u8,
            CustomizeIndex.BodyType          => "Body Type"u8,
            CustomizeIndex.Height            => "Height"u8,
            CustomizeIndex.Clan              => "Clan"u8,
            CustomizeIndex.Face              => "Head Style"u8,
            CustomizeIndex.Hairstyle         => "Hair Style"u8,
            CustomizeIndex.Highlights        => "Enable Highlights"u8,
            CustomizeIndex.SkinColor         => "Skin Color"u8,
            CustomizeIndex.EyeColorRight     => "Right Eye"u8,
            CustomizeIndex.HairColor         => "Hair Color"u8,
            CustomizeIndex.HighlightsColor   => "Highlights Color"u8,
            CustomizeIndex.TattooColor       => "Feature Color"u8,
            CustomizeIndex.Eyebrows          => "Eyebrow Style"u8,
            CustomizeIndex.EyeColorLeft      => "Left Eye"u8,
            CustomizeIndex.EyeShape          => "Small Pupils"u8,
            CustomizeIndex.Nose              => "Nose Style"u8,
            CustomizeIndex.Jaw               => "Jaw Style"u8,
            CustomizeIndex.Mouth             => "Mouth Style"u8,
            CustomizeIndex.MuscleMass        => "Muscle Tone"u8,
            CustomizeIndex.TailShape         => "Tail Shape"u8,
            CustomizeIndex.BustSize          => "Bust Size"u8,
            CustomizeIndex.FacePaint         => "Face Paint"u8,
            CustomizeIndex.FacePaintColor    => "Face Paint Color"u8,
            CustomizeIndex.LipColor          => "Lip Color"u8,
            CustomizeIndex.FacialFeature1    => "Facial Feature 1"u8,
            CustomizeIndex.FacialFeature2    => "Facial Feature 2"u8,
            CustomizeIndex.FacialFeature3    => "Facial Feature 3"u8,
            CustomizeIndex.FacialFeature4    => "Facial Feature 4"u8,
            CustomizeIndex.FacialFeature5    => "Facial Feature 5"u8,
            CustomizeIndex.FacialFeature6    => "Facial Feature 6"u8,
            CustomizeIndex.FacialFeature7    => "Facial Feature 7"u8,
            CustomizeIndex.LegacyTattoo      => "Legacy Tattoo"u8,
            CustomizeIndex.SmallIris         => "Small Iris"u8,
            CustomizeIndex.Lipstick          => "Enable Lipstick"u8,
            CustomizeIndex.FacePaintReversed => "Reverse Face Paint"u8,
            _                                => StringU8.Empty,
        };
}
