namespace Penumbra.GameData.Files.PhybStructs;

public struct PhybNode
{
    public FixedString32 BoneName;
    public float         Radius;
    public float         AttractByAnimation;
    public float         WindScale;
    public float         GravityScale;
    public float         ConeMaxAngle;
    public Vector3       ConeAxisOffset;
    public Vector3       ConstraintPlaneNormal;
    public uint          CollisionFlag;
    public uint          ContinuousCollisionFlag;
}
