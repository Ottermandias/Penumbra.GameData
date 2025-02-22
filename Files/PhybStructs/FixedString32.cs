using Dalamud.Memory;

namespace Penumbra.GameData.Files.PhybStructs;

[InlineArray(32)]
public struct FixedString32
{
    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "InlineArray")]
    private byte _element0;

    public override unsafe string ToString()
    {
        fixed (byte* ptr = &_element0)
        {
            return MemoryHelper.ReadStringNullTerminated((nint)ptr);
        }
    }
}
