namespace Penumbra.GameData.Files.MaterialStructs;

/// <summary> Legacy color row, and legacy interpretation of the new one. </summary>
public interface ILegacyColorRow : IColorRow
{
    Half SpecularMask { get; set; }
    Half Shininess    { get; set; }
}
