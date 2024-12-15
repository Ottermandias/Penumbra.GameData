namespace Penumbra.GameData.Files.MaterialStructs;

/// <summary> Common properties between all known color dye row types. </summary>
public interface IColorDyeRow
{
    public ushort Template { get; set; }
    public byte   Channel  { get; }

    public bool DiffuseColor  { get; set; }
    public bool SpecularColor { get; set; }
    public bool EmissiveColor { get; set; }
}
