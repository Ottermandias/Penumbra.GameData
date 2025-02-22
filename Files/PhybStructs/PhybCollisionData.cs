namespace Penumbra.GameData.Files.PhybStructs;

public struct PhybCollisionData
{
    public FixedString32 Name;
    public CollisionType Type;

    public enum CollisionType : uint
    {
        Both    = 0,
        Outside = 1,
        Inside  = 2,
    }
}
