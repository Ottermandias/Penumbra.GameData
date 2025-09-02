using ImSharp;
using Penumbra.GameData.Data;
using Penumbra.GameData.DataContainers;
using Penumbra.GameData.Enums;
using Penumbra.GameData.Structs;

namespace Penumbra.GameData.Gui.Debug;

/// <summary> Draw a resolver for restricted gear. </summary>
public class RestrictedGearDrawer(RestrictedGear restrictedGear, RestrictedItemsMale male, RestrictedItemsFemale female) : IGameDataDrawer
{
    /// <inheritdoc/>
    public ReadOnlySpan<byte> Label
        => "Restricted Gear Data"u8;

    /// <inheritdoc/>
    public bool Disabled
        => !restrictedGear.Finished;

    private CharacterArmor _resolveArmor;

    /// <inheritdoc/>
    public void Draw()
    {
        DrawResolver();
        DrawRacialData();
        DrawDictData("Male-Restricted Sets"u8,   male.Value);
        DrawDictData("Female-Restricted Sets"u8, female.Value);
    }

    /// <summary> Draw input and result for resolving restricted models. </summary>
    private void DrawResolver()
    {
        ImEx.TextFrameAligned("Resolve Model"u8);
        ModelInput.DrawArmorInput(ref _resolveArmor);
        foreach (var race in Enum.GetValues<Race>().Skip(1))
        {
            ReadOnlySpan<Gender> genders = [Gender.Male, Gender.Female];
            foreach (var gender in genders)
            {
                foreach (var slot in EquipSlotExtensions.EqdpSlots)
                {
                    var (replaced, model) = restrictedGear.ResolveRestricted(_resolveArmor, slot, race, gender);
                    if (replaced)
                        Im.Text($"{race.ToName()} - {gender} - {slot.ToName()} resolves to {model}.");
                }
            }
        }
    }

    private static void DrawRacialData()
    {
        using var tree = Im.Tree.Node("Race-Restricted Sets"u8);
        if (!tree)
            return;

        using var table = Im.Table.Begin("race"u8, 4, TableFlags.RowBackground | TableFlags.SizingFixedFit);
        if (!table)
            return;

        foreach (var (idx, value) in RestrictedItemsRace.RaceGenderGroup.Index())
        {
            var armor  = new CharacterArmor((PrimaryId)(value & 0xFFFF), (Variant)(value >> 16), StainIds.None);
            var gender = idx % 2 == 0 ? Gender.Male : Gender.Female;
            var race   = (Race)(idx / 2 + 1);
            table.DrawColumn(race.ToNameU8());
            table.DrawColumn(gender.ToNameU8());
            table.DrawColumn($"{armor.Set.Id}");
            table.DrawColumn($"{armor.Variant.Id}");
        }
    }

    private static void DrawDictData(ReadOnlySpan<byte> name, IReadOnlyDictionary<uint, uint> dict)
    {
        using var tree = Im.Tree.Node(name);
        if (!tree)
            return;

        using var table = Im.Table.Begin("table"u8, 5, TableFlags.RowBackground | TableFlags.SizingFixedFit);
        if (!table)
            return;

        foreach (var (value, replacement) in dict)
        {
            var armor  = new CharacterArmor((PrimaryId)(value & 0xFFFF),       (Variant)(value >> 16),       StainIds.None);
            var armor2 = new CharacterArmor((PrimaryId)(replacement & 0xFFFF), (Variant)(replacement >> 16), StainIds.None);
            var slot   = (EquipSlot)(value >> 24);
            table.DrawColumn(slot.ToNameU8());
            table.DrawColumn($"{armor.Set.Id}");
            table.DrawColumn($"{armor.Variant.Id}");
            table.DrawColumn($"{armor2.Set.Id}");
            table.DrawColumn($"{armor2.Variant.Id}");
        }
    }
}
