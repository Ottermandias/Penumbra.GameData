using Lumina.Excel.GeneratedSheets;
using Penumbra.Api.Enums;
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
            _                         => (ChangedItemType.Customization, 0),
        };
    }
}
