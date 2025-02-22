namespace Penumbra.GameData.Files.PhybStructs;

public struct PhybConnector
{
    public short ChainId1;
    public short ChainId2;
    public short NodeId1;
    public short NodeId2;
    public float CollisionRadius;
    public float Friction;
    public float Dampening;
    public float Repulsion;
    public uint  CollisionFlag;
    public uint  ContinuousCollisionFlag;
}
