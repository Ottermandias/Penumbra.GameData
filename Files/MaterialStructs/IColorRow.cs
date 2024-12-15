using Penumbra.GameData.Structs;

namespace Penumbra.GameData.Files.MaterialStructs;

/// <summary> Common properties between all known color row types. </summary>
public interface IColorRow
{
    HalfColor DiffuseColor  { get; set; }
    HalfColor SpecularColor { get; set; }
    HalfColor EmissiveColor { get; set; }

    ushort        TileIndex     { get; set; }
    Half          TileAlpha     { get; }
    HalfMatrix2x2 TileTransform { get; set; }
}
