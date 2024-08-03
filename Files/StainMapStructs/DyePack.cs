using Penumbra.GameData.Structs;

namespace Penumbra.GameData.Files.StainMapStructs;

/// <summary>
/// All dye-able color set information for a row - GUD (Dawntrail) format.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public record struct DyePack : IDyePack<DyePack>
{
    public const string DefaultStmPath = "chara/base_material/stainingtemplate_gud.stm";

    static string IDyePack<DyePack>.DefaultStmPath => DefaultStmPath;

    public   HalfColor DiffuseColor;
    public   HalfColor SpecularColor;
    public   HalfColor EmissiveColor;
    public   Half      Scalar3;
    public   Half      Metalness;
    public   Half      Roughness;
    public   Half      SheenRate;
    public   Half      SheenTintRate;
    public   Half      SheenAperture;
    public   Half      Anisotropy;
    internal Half      RawSphereMapIndex;
    public   Half      SphereMapMask;

    public ushort SphereMapIndex
    {
        readonly get => (ushort)RawSphereMapIndex;
        set => RawSphereMapIndex = (Half)value;
    }

    readonly HalfColor IDyePack<DyePack>.DiffuseColor  => DiffuseColor;
    readonly HalfColor IDyePack<DyePack>.SpecularColor => SpecularColor;
    readonly HalfColor IDyePack<DyePack>.EmissiveColor => EmissiveColor;

    public static int ColorCount  => 3;
    public static int ScalarCount => 9;
}
