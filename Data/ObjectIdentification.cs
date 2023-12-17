using Penumbra.GameData.Enums;
using Penumbra.GameData.Structs;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using Penumbra.GameData.Actors;
using ObjectType = Penumbra.GameData.Enums.ObjectType;
using Penumbra.GameData.DataContainers;
using Penumbra.GameData.DataContainers.Bases;

namespace Penumbra.GameData.Data;

public sealed class ObjectIdentification(
    DictBNpcNames _bNpcNames,
    DictActions _actions,
    DictEmotes _emotes,
    DictModelChara _modelCharaToObjects,
    IdentificationListEquipment _equipmentIdentification,
    IdentificationListWeapons _weaponIdentification,
    IdentificationListModels _modelIdentification,
    GamePathParser _gamePathParser)
    : IAsyncService
{
    public Task Awaiter { get; } = Task.WhenAll(_bNpcNames.Awaiter, _actions.Awaiter, _emotes.Awaiter, _modelCharaToObjects.Awaiter,
        _equipmentIdentification.Awaiter, _weaponIdentification.Awaiter, _modelIdentification.Awaiter);

    public void Identify(IDictionary<string, object?> set, string path)
    {
        var extension = Path.GetExtension(path).ToLowerInvariant();
        if (extension is ".pap" or ".tmb" or ".scd" or ".avfx")
            if (IdentifyVfx(set, path))
                return;

        var info = _gamePathParser.GetFileInfo(path);
        IdentifyParsed(set, info);
    }

    public Dictionary<string, object?> Identify(string path)
    {
        Dictionary<string, object?> ret = [];
        Identify(ret, path);
        return ret;
    }

    public IEnumerable<EquipItem> Identify(SetId setId, WeaponType weaponType, Variant variant, EquipSlot slot)
        => slot switch
        {
            EquipSlot.MainHand => _weaponIdentification.Between(setId, weaponType, variant),
            EquipSlot.OffHand  => _weaponIdentification.Between(setId, weaponType, variant),
            _                  => _equipmentIdentification.Between(setId, slot, variant),
        };

    public IReadOnlyList<BNpcId> GetBNpcsFromName(BNpcNameId bNpcNameId)
    {
        var list = new List<BNpcId>(8);
        foreach (var (bNpcId, names) in _bNpcNames)
        {
            if (names.Contains(bNpcNameId.Id))
                list.Add(bNpcId);
        }

        return list;
    }

    public IReadOnlyList<(string Name, ObjectKind Kind, uint Id)> ModelCharaNames(ModelCharaId modelId)
        => modelId.Id >= _modelCharaToObjects.Count
            ? Array.Empty<(string Name, ObjectKind Kind, uint Id)>()
            : _modelCharaToObjects[modelId];

    public int NumModelChara
        => _modelCharaToObjects.Count;

    private void FindEquipment(IDictionary<string, object?> set, GameObjectInfo info)
    {
        var slot  = info.EquipSlot is EquipSlot.LFinger ? EquipSlot.RFinger : info.EquipSlot;
        var items = _equipmentIdentification.Between(info.PrimaryId, slot, info.Variant);
        foreach (var item in items)
            set[item.Name] = item;
    }

    private void FindWeapon(IDictionary<string, object?> set, GameObjectInfo info)
    {
        var items = _weaponIdentification.Between(info.PrimaryId, info.SecondaryId, info.Variant);
        foreach (var item in items)
            set[item.Name] = item;
    }

    private void FindModel(IDictionary<string, object?> set, GameObjectInfo info)
    {
        var type = info.ObjectType.ToModelType();
        if (type is 0 or CharacterBase.ModelType.Weapon)
            return;

        var models = _modelIdentification.Between(type, info.PrimaryId, (byte)info.SecondaryId, info.Variant);
        foreach (var model in models.Where(m => m.RowId != 0 && m.RowId < _modelCharaToObjects.Count))
        {
            var objectList = _modelCharaToObjects[model.RowId];
            foreach (var (name, kind, _) in objectList)
                set[$"{name} ({kind.ToName()})"] = model;
        }
    }

    private static void AddCounterString(IDictionary<string, object?> set, string data)
    {
        if (set.TryGetValue(data, out var obj) && obj is int counter)
            set[data] = counter + 1;
        else
            set[data] = 1;
    }

    private void IdentifyParsed(IDictionary<string, object?> set, GameObjectInfo info)
    {
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
            case ObjectType.LoadingScreen:
            case ObjectType.Map:
            case ObjectType.Interface:
            case ObjectType.Vfx:
            case ObjectType.World:
            case ObjectType.Housing:
            case ObjectType.Font:
                AddCounterString(set, info.ObjectType.ToString());
                break;
            case ObjectType.DemiHuman:
                FindModel(set, info);
                break;
            case ObjectType.Monster:
                FindModel(set, info);
                break;
            case ObjectType.Icon:
                set[$"Icon: {info.IconId}"] = null;
                break;
            case ObjectType.Accessory:
            case ObjectType.Equipment:
                FindEquipment(set, info);
                break;
            case ObjectType.Weapon:
                FindWeapon(set, info);
                break;
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
                                : $"Customization: {race} {gender} {info.BodySlot} ({info.CustomizationType}) {info.PrimaryId}";
                        set[customizationString] = null;
                        break;
                    }
                }

                break;
        }
    }

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
