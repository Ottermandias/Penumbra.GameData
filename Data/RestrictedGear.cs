using Penumbra.GameData.DataContainers;
using Penumbra.GameData.DataContainers.Bases;
using Penumbra.GameData.Enums;
using Penumbra.GameData.Structs;

namespace Penumbra.GameData.Data;

/// <summary>
/// Handle gender- or race-locked gear in the draw model itself.
/// Racial gear gets swapped to the correct current race and gender (it is one set each).
/// Gender-locked gear gets swapped to the equivalent set if it exists (most of them do), 
/// with some items getting send to emperor's new clothes and a few funny entries.
/// </summary>
public sealed class RestrictedGear(RestrictedItemsRace _raceSet, RestrictedItemsMale _maleSet, RestrictedItemsFemale _femaleSet)
    : IAsyncService
{
    /// <summary>
    /// Resolve a model given by its model id, variant and slot for your current race and gender.
    /// </summary>
    /// <param name="armor">The equipment piece.</param>
    /// <param name="slot">The equipment slot.</param>
    /// <param name="race">The intended race.</param>
    /// <param name="gender">The intended gender.</param>
    /// <returns>True and the changed-to piece of gear or false and the same piece of gear.</returns>
    public (bool Replaced, CharacterArmor Armor) ResolveRestricted(CharacterArmor armor, EquipSlot slot, Race race, Gender gender)
    {
        // Check racial gear, this does not need slots.
        (var replaced, armor) = _raceSet.Resolve(armor, race, gender);
        if (replaced)
            return (replaced, armor);

        // Some items lead to the exact same model- and variant id just gender specified, 
        // so check for actual difference in the Replaced bool.
        (replaced, armor) = _maleSet.Resolve(armor, slot, race, gender);
        if (replaced)
            return (replaced, armor);

        return _femaleSet.Resolve(armor, slot, race, gender);
    }

    public Task Awaiter { get; } = Task.WhenAll(_raceSet.Awaiter, _maleSet.Awaiter, _femaleSet.Awaiter);
}
