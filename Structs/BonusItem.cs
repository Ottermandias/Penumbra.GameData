global using PseudoBonusItem = System.ValueTuple<string, uint, ushort, ushort, byte, byte>;
using Penumbra.GameData.Enums;

namespace Penumbra.GameData.Structs;

public readonly record struct BonusItem(string Name, IconId Icon, BonusItemId Id, PrimaryId ModelId, Variant Variant, BonusItemFlag Slot)
{
    public static BonusItem Empty(BonusItemFlag slot)
        => new("Nothing", 0, 0, 0, 0, slot);

    public static explicit operator BonusItem(PseudoBonusItem item)
        => new(item.Item1, item.Item2, item.Item3, item.Item4, item.Item5, (BonusItemFlag)item.Item6);

    public static implicit operator PseudoBonusItem(BonusItem item)
        => (item.Name, item.Icon.Id, item.Id.Id, item.ModelId.Id, item.Variant.Id, (byte)item.Slot);

    public CharacterArmor ToArmor()
        => new(ModelId, Variant, StainIds.None);
}
