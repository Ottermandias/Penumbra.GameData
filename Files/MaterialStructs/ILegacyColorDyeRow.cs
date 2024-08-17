namespace Penumbra.GameData.Files.MaterialStructs;

/// <summary> Legacy color dye row, and legacy interpretation of the new one. </summary>
public interface ILegacyColorDyeRow : IColorDyeRow
{
    public bool Shininess    { get; set; }
    public bool SpecularMask { get; set; }
}
