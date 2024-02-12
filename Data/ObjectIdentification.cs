using Penumbra.GameData.Enums;
using Penumbra.GameData.Structs;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using OtterGui.Services;
using Penumbra.GameData.Actors;
using ObjectType = Penumbra.GameData.Enums.ObjectType;
using Penumbra.GameData.DataContainers;

namespace Penumbra.GameData.Data;

/// <summary> Identify items or game objects from paths or IDs. </summary>
public sealed class ObjectIdentification(
    DictBNpcNames _bNpcNames,
    DictAction _actions,
    DictEmote _emotes,
    DictModelChara _modelCharaToObjects,
    IdentificationListEquipment _equipmentIdentification,
    IdentificationListWeapons _weaponIdentification,
    IdentificationListModels _modelIdentification,
    GamePathParser _gamePathParser)
    : IAsyncService
{
    /// <summary> Finished when all data tasks are finished. </summary>
    public Task Awaiter { get; } = Task.WhenAll(_bNpcNames.Awaiter, _actions.Awaiter, _emotes.Awaiter, _modelCharaToObjects.Awaiter,
        _equipmentIdentification.Awaiter, _weaponIdentification.Awaiter, _modelIdentification.Awaiter);

    /// <inheritdoc/>
    public bool Finished
        => Awaiter.IsCompletedSuccessfully;

    /// <summary> Identify all affected game identities using <paramref name="path"/> and add those items to <paramref name="set"/>, </summary>
    /// <param name="set"> The set to add identities to. </param>
    /// <param name="path"> The path to parse and identify. </param>
    public void Identify(IDictionary<string, object?> set, string path)
    {
        var extension = Path.GetExtension(path).ToLowerInvariant();
        if (extension is ".pap" or ".tmb" or ".scd" or ".avfx")
            if (IdentifyVfx(set, path))
                return;

        var info = _gamePathParser.GetFileInfo(path);
        IdentifyParsed(set, info);
    }

    /// <summary> Identify all affected game identities using <paramref name="path"/> and return them. </summary>
    /// <param name="path"> The path to parse and identify. </param>
    /// <returns> A dictionary of affected game identities. </returns>
    public Dictionary<string, object?> Identify(string path)
    {
        Dictionary<string, object?> ret = [];
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
            EquipSlot.MainHand or EquipSlot.OffHand => _weaponIdentification.Between(primaryId, secondaryId, variant),
            _                                       => _equipmentIdentification.Between(primaryId, slot, variant),
        };

    /// <summary> Find and add all equipment pieces affected by <paramref name="info"/>. </summary>
    private void FindEquipment(IDictionary<string, object?> set, GameObjectInfo info)
    {
        var slot  = info.EquipSlot is EquipSlot.LFinger ? EquipSlot.RFinger : info.EquipSlot;
        var items = _equipmentIdentification.Between(info.PrimaryId, slot, info.Variant);
        foreach (var item in items)
            set[item.Name] = item;
    }

    /// <summary> Find and add all weapons affected by <paramref name="info"/>. </summary>
    private void FindWeapon(IDictionary<string, object?> set, GameObjectInfo info)
    {
        var items = _weaponIdentification.Between(info.PrimaryId, info.SecondaryId, info.Variant);
        foreach (var item in items)
            set[item.Name] = item;
    }

    /// <summary> Find and add all models affected by <paramref name="info"/>. </summary>
    private void FindModel(IDictionary<string, object?> set, GameObjectInfo info)
    {
        var type = info.ObjectType.ToModelType();
        if (type is 0 or CharacterBase.ModelType.Weapon)
            return;

        var models = _modelIdentification.Between(type, info.PrimaryId, (byte)info.SecondaryId.Id, info.Variant);
        foreach (var model in models.Where(m => m.RowId != 0 && m.RowId < _modelCharaToObjects.Count))
        {
            var objectList = _modelCharaToObjects[model.RowId];
            foreach (var (name, kind, _) in objectList)
                set[$"{name} ({kind.ToName()})"] = model;
        }
    }

    /// <summary> Identities that only count their appearances store a counter value, increment or set that. </summary>
    private static void AddCounterString(IDictionary<string, object?> set, string data)
    {
        if (set.TryGetValue(data, out var obj) && obj is int counter)
            set[data] = counter + 1;
        else
            set[data] = 1;
    }

    /// <summary> Identify and add a game object info. </summary>
    private void IdentifyParsed(IDictionary<string, object?> set, GameObjectInfo info)
    {
        // Some file types are only counted.
        switch (info.FileType)
        {
            case FileType.Sound:
                AddCounterString(set, FileType.Sound.ToString());
                return;
            case FileType.Animation:
            case FileType.Pap:
                AddCounterString(set, FileType.Animation.ToString());
                return;
            case FileType.Shader:
                AddCounterString(set, FileType.Shader.ToString());
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
                AddCounterString(set, info.ObjectType.ToString());
                break;
            // We can differentiate icons by ID.
            case ObjectType.Icon:
                set[$"Icon: {info.IconId}"] = null;
                break;
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
            case ObjectType.Weapon:
                FindWeapon(set, info);
                break;
            // Characters can have different affected options.
            case ObjectType.Character:
                var (gender, race) = info.GenderRace.Split();
                var raceString   = race != ModelRace.Unknown ? race.ToName() + " " : "";
                var genderString = gender != Gender.Unknown ? gender.ToName() + " " : "Player ";
                switch (info.CustomizationType)
                {
                    case CustomizationType.Skin:
                        set[$"Customization: {raceString}{genderString}Skin Textures"] = null;
                        break;
                    case CustomizationType.DecalFace:
                        set[$"Customization: Face Decal {info.PrimaryId}"] = null;
                        break;
                    case CustomizationType.Iris when race == ModelRace.Unknown:
                        set[$"Customization: All Eyes (Catchlight)"] = null;
                        break;
                    case CustomizationType.DecalEquip:
                        set[$"Equipment Decal {info.PrimaryId}"] = null;
                        break;
                    default:
                    {
                        var customizationString = race == ModelRace.Unknown
                         || info.BodySlot == BodySlot.Unknown
                         || info.CustomizationType == CustomizationType.Unknown
                                ? "Customization: Unknown"
                                : $"Customization: {race.ToName()} {gender.ToName()} {info.BodySlot} ({info.CustomizationType}) {info.PrimaryId}";
                        set[customizationString] = info.CustomizationType is CustomizationType.Hair
                            ? (race, gender, CustomizeIndex.Hairstyle, (CustomizeValue)info.PrimaryId.Id)
                            : null;
                        break;
                    }
                }

                break;
        }
    }

    /// <summary> Identify and parse VFX identities. </summary>
    private bool IdentifyVfx(IDictionary<string, object?> set, string path)
    {
        var key      = _gamePathParser.VfxToKey(path);
        var fileName = Path.GetFileName(path);
        var ret      = false;

        if (key.Length > 0 && _actions.TryGetValue(key, out var actions) && actions.Count > 0)
        {
            foreach (var action in actions)
                set[$"Action: {action.Name.ToDalamudString()}"] = action;
            ret = true;
        }

        if (fileName.Length > 0 && _emotes.TryGetValue(fileName, out var emotes) && emotes.Count > 0)
        {
            foreach (var emote in emotes)
                set[$"Emote: {emote.Name.ToDalamudString()}"] = emote;
            ret = true;
        }

        return ret;
    }

    /// <summary> Currently unused. </summary>
    public static unsafe ulong KeyFromCharacterBase(CharacterBase* drawObject)
    {
        var type = (*(delegate* unmanaged<CharacterBase*, uint>**)drawObject)[Offsets.DrawObjectGetModelTypeVfunc](drawObject);
        var unk  = (ulong)*((byte*)drawObject + Offsets.DrawObjectModelUnk1) << 8;
        return type switch
        {
            1 => type | unk,
            2 => type | unk | ((ulong)*(ushort*)((byte*)drawObject + Offsets.DrawObjectModelUnk3) << 16),
            3 => type
              | unk
              | ((ulong)*(ushort*)((byte*)drawObject + Offsets.DrawObjectModelUnk2) << 16)
              | ((ulong)**(ushort**)((byte*)drawObject + Offsets.DrawObjectModelUnk4) << 32)
              | ((ulong)**(ushort**)((byte*)drawObject + Offsets.DrawObjectModelUnk3) << 40),
            _ => 0u,
        };
    }
}
