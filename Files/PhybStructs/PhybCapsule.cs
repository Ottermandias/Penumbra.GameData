namespace Penumbra.GameData.Files.PhybStructs;

public struct PhybCapsule
{
    public FixedString32 Name;
    public FixedString32 StartBone;
    public FixedString32 EndBone;
    public Vector3 StartOffset;
    public Vector3 EndOffset;
    public float Radius;
}
