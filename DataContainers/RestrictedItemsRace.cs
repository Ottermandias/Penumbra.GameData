using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using Lumina.Excel.GeneratedSheets;
using OtterGui.Log;
using Penumbra.GameData.DataContainers.Bases;
using Penumbra.GameData.Enums;
using Penumbra.GameData.Structs;
using Race = Penumbra.GameData.Enums.Race;

namespace Penumbra.GameData.DataContainers;

public sealed class RestrictedItemsRace(DalamudPluginInterface pluginInterface, Logger log, IDataManager gameData)
    : DataSharer<IReadOnlySet<uint>>(pluginInterface, log, "RacialRestrictedItems", gameData.Language, 1, () => CreateItems(log, gameData))
{
    private static IReadOnlySet<uint> CreateItems(Logger log, IDataManager gameData)
    {
        var ret = RaceGenderGroup.Where(c => c is not 0 and not uint.MaxValue).ToHashSet();

        var items = gameData.GetExcelSheet<Item>()!;
        var categories = gameData.GetExcelSheet<EquipRaceCategory>(gameData.Language)!;
        foreach (var item in items.Where(i => i.EquipRestriction > 3))
        {
            if (ret.Contains((uint)item.ModelMain))
                continue;

            log.Information(
                $"{item.RowId:D5} {item.Name.ToDalamudString().TextValue} has unknown restriction group {categories.GetRow(item.EquipRestriction)!.RowId:D2}.");
        }

        return ret;
    }

    internal (bool Replaced, CharacterArmor Armor) Resolve(CharacterArmor armor, Race race, Gender gender)
    {
        var quad = armor.Set.Id | (uint)armor.Variant.Id << 16;
        if (!RaceGenderGroup.Contains(quad))
            return (false, armor);

        var idx = ((int)race - 1) * 2 + (gender is Gender.Female or Gender.FemaleNpc ? 1 : 0);
        var value = RaceGenderGroup[idx];
        return (value != quad, new CharacterArmor((ushort)value, (byte)(value >> 16), armor.Stain));
    }

    public int Count
        => Value.Count;

    public override long ComputeMemory()
        => Count * 16 + 32;

    public override int ComputeTotalCount()
        => Count;


    /// <summary>
    /// The racial starter sets are available for all 4 slots each,
    /// but have no associated accessories or hats.
    /// </summary>
    // @Formatter:off
    private static readonly uint[] RaceGenderGroup =
    [
        0x020054,
        0x020055,
        0x020056,
        0x020057,
        0x02005C,
        0x02005D,
        0x020058,
        0x020059,
        0x02005A,
        0x02005B,
        0x020101,
        0x020102,
        0x010255,
        uint.MaxValue, // TODO: Female Hrothgar
        0x0102E8,
        0x010245,
    ];
    // @Formatter:on
}
