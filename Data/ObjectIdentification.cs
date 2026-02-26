using Penumbra.GameData.Enums;
using Penumbra.GameData.Structs;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using Luna;
using Penumbra.GameData.Actors;
using ObjectType = Penumbra.GameData.Enums.ObjectType;
using Penumbra.GameData.DataContainers;

namespace Penumbra.GameData.Data;

/// <summary> Identify items or game objects from paths or IDs. </summary>
public sealed class ObjectIdentification(
    DictBNpcNames bNpcNames,
    DictAction actions,
    DictEmote emotes,
    DictModelChara modelCharaToObjects,
    IdentificationListEquipment equipmentIdentification,
    IdentificationListWeapons weaponIdentification,
    IdentificationListModels modelIdentification,
    GamePathParser gamePathParser)
    : IAsyncService
{
    /// <summary> Finished when all data tasks are finished. </summary>
    public Task Awaiter { get; } = Task.WhenAll(bNpcNames.Awaiter, actions.Awaiter, emotes.Awaiter, modelCharaToObjects.Awaiter,
        equipmentIdentification.Awaiter, weaponIdentification.Awaiter, modelIdentification.Awaiter);

    /// <inheritdoc/>
    public bool Finished
        => Awaiter.IsCompletedSuccessfully;

    /// <summary> Identify all affected game identities using <paramref name="path"/> and add those items to <paramref name="set"/>, </summary>
    /// <param name="set"> The set to add identities to. </param>
    /// <param name="path"> The path to parse and identify. </param>
    public void Identify(IDictionary<string, IIdentifiedObjectData> set, string path)
    {
        var extension = Path.GetExtension(path).ToLowerInvariant();
        if (extension is ".pap" or ".tmb")
            if (IdentifyVfx(set, path))
                return;

        var info = gamePathParser.GetFileInfo(path);
        IdentifyParsed(set, info);
    }

    /// <summary> Identify all affected game identities using <paramref name="path"/> and return them. </summary>
    /// <param name="path"> The path to parse and identify. </param>
    /// <returns> A dictionary of affected game identities. </returns>
    public Dictionary<string, IIdentifiedObjectData> Identify(string path)
    {
        Dictionary<string, IIdentifiedObjectData> ret = [];
        Identify(ret, path);
        return ret;
    }

    /// <summary> Identify all equipment items using the specified values. </summary>
    /// <param name="primaryId"> The primary ID of the model. </param>
    /// <param name="secondaryId"> The secondary ID of the model, if any. </param>
    /// <param name="variant"> The variant of the material. </param>
    /// <param name="slot"> The slot the item is used in. </param>
    /// <returns> An enumeration of all affected items. </returns>
    public IEnumerable<EquipItem> Identify(PrimaryId primaryId, SecondaryId secondaryId, Variant variant, EquipSlot slot)
        => slot switch
        {
            EquipSlot.MainHand or EquipSlot.OffHand => weaponIdentification.Between(primaryId, secondaryId, variant),
            _                                       => equipmentIdentification.Between(primaryId, slot, variant),
        };

    /// <summary> Identify all bonus items using the specified values. </summary>
    /// <param name="primaryId"> The primary ID of the model. </param>
    /// <param name="variant"> The variant of the material. </param>
    /// <param name="slot"> The bonus slot the item is used in. </param>
    /// <returns> An enumeration of all affected items. </returns>
    public IEnumerable<EquipItem> Identify(PrimaryId primaryId, Variant variant, BonusItemFlag slot)
    {
        if (primaryId.Id == 0 && variant.Id == 0)
            return [EquipItem.BonusItemNothing(slot)];

        return equipmentIdentification.Between(primaryId, slot.ToEquipSlot(), variant);
    }

    /// <summary> Find and add all equipment pieces affected by <paramref name="info"/>. </summary>
    private void FindEquipment(IDictionary<string, IIdentifiedObjectData> set, GameObjectInfo info)
    {
        var slot  = info.EquipSlot is EquipSlot.LFinger ? EquipSlot.RFinger : info.EquipSlot;
        var items = equipmentIdentification.Between(info.PrimaryId, slot, info.Variant);
        foreach (var item in items)
            set.UpdateCountOrSet(item.Name, () => new IdentifiedItem(item));
    }

    /// <summary> Find and add all weapons affected by <paramref name="info"/>. </summary>
    private void FindWeapon(IDictionary<string, IIdentifiedObjectData> set, GameObjectInfo info)
    {
        var items = weaponIdentification.Between(info.PrimaryId, info.SecondaryId, info.Variant);
        foreach (var item in items)
            set.UpdateCountOrSet(item.Name, () => new IdentifiedItem(item));
    }

    /// <summary> Find and add all models affected by <paramref name="info"/>. </summary>
    private void FindModel(IDictionary<string, IIdentifiedObjectData> set, GameObjectInfo info)
    {
        var type = info.ObjectType.ToModelType();
        if (type is 0 or CharacterBase.ModelType.Weapon)
            return;

        var models = modelIdentification.Between(type, info.PrimaryId, (byte)info.SecondaryId.Id, info.Variant);
        foreach (var model in models.Where(m => m.RowId != 0 && m.RowId < modelCharaToObjects.Count))
        {
            var objectList = modelCharaToObjects[model.RowId];
            foreach (var (name, kind, _) in objectList)
                set.UpdateCountOrSet($"{name} ({kind.ToName()})", () => new IdentifiedModel(model));
        }
    }

    /// <summary> Identify and add a game object info. </summary>
    private void IdentifyParsed(IDictionary<string, IIdentifiedObjectData> set, GameObjectInfo info)
    {
        // Some file types are only counted.
        switch (info.FileType)
        {
            case FileType.Sound:
                set.UpdateCountOrSet(FileType.Sound.ToString(), () => new IdentifiedCounter());
                return;
            case FileType.Animation:
            case FileType.Pap:
                set.UpdateCountOrSet(FileType.Animation.ToString(), () => new IdentifiedCounter());
                return;
            case FileType.Shader:
                set.UpdateCountOrSet(FileType.Shader.ToString(), () => new IdentifiedCounter());
                return;
        }

        switch (info.ObjectType)
        {
            // Some objects are only counted;
            case ObjectType.LoadingScreen:
            case ObjectType.Map:
            case ObjectType.Interface:
            case ObjectType.Vfx:
            case ObjectType.World:
            case ObjectType.Housing:
            case ObjectType.Font:
                set.UpdateCountOrSet(info.ObjectType.ToString(), () => new IdentifiedCounter());
                break;
            // We can differentiate icons by ID.
            case ObjectType.Icon: set.UpdateCountOrSet($"Icon: {info.IconId}", () => new IdentifiedName()); break;
            // Demihumans and monsters affect models
            case ObjectType.DemiHuman:
            case ObjectType.Monster:
                FindModel(set, info);
                break;
            // Accessory and Equipment are equipment pieces.
            case ObjectType.Accessory:
            case ObjectType.Equipment:
                FindEquipment(set, info);
                break;
            // Weapons are handled separately from other equipment.
            case ObjectType.Weapon: FindWeapon(set, info); break;
            // Characters can have different affected options.
            case ObjectType.Character:
                var (gender, race) = info.GenderRace.Split();
                var raceString   = race != ModelRace.Unknown ? race.ToName() + " " : "";
                var genderString = gender != Gender.Unknown ? gender.ToName() + " " : "Player ";
                switch (info.CustomizationType)
                {
                    case CustomizationType.Skin:
                        set.UpdateCountOrSet($"Customization: {raceString}{genderString}Skin Textures", () => new IdentifiedName());
                        break;
                    case CustomizationType.DecalFace:
                        set.UpdateCountOrSet($"Customization: Face Decal {info.PrimaryId}",
                            () => IdentifiedCustomization.FacePaint((CustomizeValue)info.PrimaryId.Id));
                        break;
                    case CustomizationType.Iris when race == ModelRace.Unknown:
                        set.UpdateCountOrSet("Customization: All Eyes (Catchlight)", () => new IdentifiedName());
                        break;
                    case CustomizationType.DecalEquip:
                        set.UpdateCountOrSet($"Equipment Decal {info.PrimaryId}", () => new IdentifiedName());
                        break;
                    default:
                    {
                        var customizationString = race is ModelRace.Unknown
                         || info.BodySlot is BodySlot.Unknown
                         || info.CustomizationType is CustomizationType.Unknown
                                ? "Customization: Unknown"
                                : $"Customization: {race.ToName()} {gender.ToName()} {CompareBodyCustomization(info)} {info.PrimaryId}";
                        set.UpdateCountOrSet(customizationString, () => info.BodySlot switch
                        {
                            BodySlot.Hair => IdentifiedCustomization.Hair(race, gender, (CustomizeValue)info.PrimaryId.Id),
                            BodySlot.Tail => IdentifiedCustomization.Tail(race, gender, (CustomizeValue)info.PrimaryId.Id),
                            BodySlot.Ear  => IdentifiedCustomization.Ears(race, gender, (CustomizeValue)info.PrimaryId.Id),
                            BodySlot.Face => IdentifiedCustomization.Face(race, gender, (CustomizeValue)info.PrimaryId.Id),
                            _             => (IIdentifiedObjectData)new IdentifiedName(),
                        });
                        break;
                    }
                }

                break;
        }
    }

    private string CompareBodyCustomization(GameObjectInfo info)
    {
        return (info.BodySlot, info.CustomizationType) switch
        {
            (BodySlot.Hair, CustomizationType.Hair) or
                (BodySlot.Face, CustomizationType.Face) or
                (BodySlot.Tail, CustomizationType.Tail) or
                (BodySlot.Body, CustomizationType.Body) or
                (BodySlot.Ear, CustomizationType.Ear) => info.BodySlot.ToString(),
            _ => $"{info.BodySlot} ({info.CustomizationType})",
        };
    }

    /// <summary> Identify and parse VFX identities. </summary>
    private bool IdentifyVfx(IDictionary<string, IIdentifiedObjectData> set, string path)
    {
        var key      = GamePathParser.VfxToKey(path);
        var fileName = Path.GetFileName(path);
        var ret      = false;

        if (key.Length > 0 && actions.TryGetValue(key, out var foundActions) && foundActions.Count > 0)
        {
            foreach (var action in foundActions)
                set.UpdateCountOrSet($"Action: {action.Name.ExtractTextExtended()}", () => new IdentifiedAction(action));
            ret = true;
        }

        if (fileName.Length > 0 && emotes.TryGetValue(fileName, out var foundEmotes) && foundEmotes.Count > 0)
        {
            foreach (var emote in foundEmotes)
                set.UpdateCountOrSet($"Emote: {emote.Name.ExtractTextExtended()}", () => new IdentifiedEmote(emote));
            ret = true;
        }

        return ret;
    }
}
