using Luna.Generators;

namespace Penumbra.GameData.Enums;

/// <summary> Types of game objects or identities. </summary>
[NamedEnum]
public enum ObjectType : byte
{
    Unknown,

    [Name("Visual Effect")]
    Vfx,

    [Name("Demi Human")]
    DemiHuman,

    [Name("Accessory")]
    Accessory,

    [Name("Doodad")]
    World,

    [Name("Housing Object")]
    Housing,

    [Name("Monster")]
    Monster,

    [Name("Icon")]
    Icon,

    [Name("Loading Screen")]
    LoadingScreen,

    [Name("Map")]
    Map,

    [Name("UI Element")]
    Interface,

    [Name("Equipment")]
    Equipment,

    [Name("Character")]
    Character,

    [Name("Weapon")]
    Weapon,

    [Name("Font")]
    Font,
}

public static partial class ObjectTypeExtensions
{
    /// <summary> A list of valid object types for IMC files. </summary>
    public static readonly IReadOnlyList<ObjectType> ValidImcTypes =
    [
        ObjectType.Equipment,
        ObjectType.Accessory,
        ObjectType.DemiHuman,
        ObjectType.Monster,
        ObjectType.Weapon,
    ];
}
