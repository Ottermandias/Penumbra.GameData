using ImSharp;
using Luna.Generators;

namespace Penumbra.GameData.Files.Materials;

[NamedEnum]
public enum ShaderFamily
{
    [Name("")]
    Unknown,

    [Name("character.shpk")]
    Character,

    [Name("characterlegacy.shpk")]
    CharacterLegacy,

    [Name("hair.shpk")]
    Hair,

    [Name("skin.shpk")]
    Skin,

    [Name("iris.shpk")]
    Iris,

    [Name("characterglass.shpk")]
    CharacterGlass,

    [Name("characterstockings.shpk")]
    CharacterStockings,

    [Name("charactertransparency.shpk")]
    CharacterTransparency,

    [Name("charactertattoo.shpk")]
    CharacterTattoo,

    [Name("characterocclusion.shpk")]
    CharacterOcclusion,

    [Name("hairmask.shpk")]
    HairMask,
}

public static partial class ShaderFamilyExtensions
{
    extension(ShaderFamily family)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Matches(ReadOnlySpan<byte> text)
            => family.ToNameU8().Equals(text);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Matches(ReadOnlySpan<char> text, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
            => family.ToName().Equals(text, comparison);

        public static ShaderFamily Identify(ReadOnlySpan<byte> text)
        {
            foreach (var value in ShaderFamily.Values)
            {
                if (value.Matches(text))
                    return value;
            }

            return ShaderFamily.Unknown;
        }

        public static ShaderFamily Identify(ReadOnlySpan<char> text, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            foreach (var value in ShaderFamily.Values)
            {
                if (value.Matches(text, comparison))
                    return value;
            }

            return ShaderFamily.Unknown;
        }
    }
}
