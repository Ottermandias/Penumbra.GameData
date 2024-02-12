using Lumina.Excel.GeneratedSheets;
using Penumbra.Api.Enums;
using Penumbra.GameData.Structs;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace Penumbra.GameData.Enums;

public static class ChangedItemExtensions
{
    /// <summary> Convert an object to a changed item type and its ID. </summary>
    public static (ChangedItemType, uint) ChangedItemToTypeAndId(object? item)
    {
        return item switch
        {
            null                      => (ChangedItemType.None, 0),
            (Item i, FullEquipType t) => (t.IsOffhandType() ? ChangedItemType.ItemOffhand : ChangedItemType.Item, i.RowId),
            Action a                  => (ChangedItemType.Action, a.RowId),
            (ModelRace r, Gender g, CustomizeIndex i, CustomizeValue v) => (ChangedItemType.Customization,
                (uint)r | ((uint)g << 8) | ((uint)i << 16) | ((uint)v.Value << 24)),
            _ => (ChangedItemType.Unknown, 0),
        };
    }

    public static (ModelRace Race, Gender Gender, CustomizeIndex Index, CustomizeValue Value) Split(uint id)
        => ((ModelRace)id, (Gender)(id >> 8), (CustomizeIndex)(id >> 16), (CustomizeValue)(id >> 24));
}
