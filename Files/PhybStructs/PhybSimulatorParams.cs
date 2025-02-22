namespace Penumbra.GameData.Files.PhybStructs;

public struct PhybSimulatorParams
{
    public const int Size = 32;

    public Vector3        Gravity;
    public Vector3        Wind;
    public short          ConstraintLoop;
    public short          CollisionLoop;
    public SimulatorFlags Flags;
    public byte           Group;
    public ushort         Padding;

    [Flags]
    public enum SimulatorFlags : byte
    {
        Simulating           = 0x01,
        CollisionsHandled    = 0x02,
        ContinuousCollisions = 0x04,
        UsingGroundPlane     = 0x08,
        FixedLength          = 0x10,
    }
}
