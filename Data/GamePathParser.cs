using Dalamud.Game;
using OtterGui.Log;
using OtterGui.Services;
using Penumbra.GameData.Enums;
using Penumbra.GameData.Structs;

namespace Penumbra.GameData.Data;

/// <summary> Wrapper class to parse paths to game files for what they affect. </summary>
public class GamePathParser(Logger log) : IService
{
    /// <summary> Obtain basic information about a file path. </summary>
    public GameObjectInfo GetFileInfo(string path)
    {
        path = path.ToLowerInvariant().Replace('\\', '/');

        var (fileType, objectType, match) = ParseGamePath(path);
        if (match is not { Success: true })
            return new GameObjectInfo
            {
                FileType   = fileType,
                ObjectType = objectType,
            };

        try
        {
            var groups = match.Groups;
            switch (objectType)
            {
                case ObjectType.Equipment:
                case ObjectType.Accessory:
                    return HandleEquipment(objectType, fileType, groups);
                case ObjectType.Weapon:    return HandleWeapon(fileType, groups);
                case ObjectType.Map:       return HandleMap(fileType, groups);
                case ObjectType.Monster:   return HandleMonster(fileType, groups);
                case ObjectType.DemiHuman: return HandleDemiHuman(fileType, groups);
                case ObjectType.Character: return HandleCustomization(fileType, groups);
                case ObjectType.Icon:      return HandleIcon(fileType, groups);
            }
        }
        catch (Exception e)
        {
            log.Error($"Could not parse {path}:\n{e}");
        }

        return new GameObjectInfo
        {
            FileType   = fileType,
            ObjectType = objectType,
        };
    }

    /// <summary> Get the key of a VFX symbol. </summary>
    /// <returns>The lower-case key or an empty string if no match is found.</returns>
    public string VfxToKey(string path)
    {
        var match = Parser.Character.Tmb().Match(path);
        if (match.Success)
            return match.Groups[Parser.Groups.ActionKey].Value.ToLowerInvariant();

        match = Parser.Character.Pap().Match(path);
        return match.Success ? match.Groups[Parser.Groups.ActionKey].Value.ToLowerInvariant() : string.Empty;
    }

    /// <summary> Obtain the ObjectType from a given path.</summary>
    public ObjectType PathToObjectType(string path)
    {
        if (path.Length == 0)
            return ObjectType.Unknown;

        var folderIdx = path.IndexOf('/');
        if (folderIdx < 0)
            return ObjectType.Unknown;

        var folder2Idx = path.IndexOf('/', folderIdx + 1);
        if (folder2Idx < 0)
            return ObjectType.Unknown;

        var folder1 = path.AsSpan(0,         folderIdx++);
        var folder2 = path.AsSpan(folderIdx, folder2Idx - folderIdx);

        return folder1 switch
        {
            CharacterFolder => folder2 switch
            {
                EquipmentFolder => ObjectType.Equipment,
                AccessoryFolder => ObjectType.Accessory,
                WeaponFolder    => ObjectType.Weapon,
                PlayerFolder    => ObjectType.Character,
                DemiHumanFolder => ObjectType.DemiHuman,
                MonsterFolder   => ObjectType.Monster,
                CommonFolder    => ObjectType.Character,
                _               => ObjectType.Unknown,
            },
            UiFolder => folder2 switch
            {
                IconFolder      => ObjectType.Icon,
                LoadingFolder   => ObjectType.LoadingScreen,
                MapFolder       => ObjectType.Map,
                InterfaceFolder => ObjectType.Interface,
                _               => ObjectType.Unknown,
            },
            CommonFolder => folder2 switch
            {
                FontFolder => ObjectType.Font,
                _          => ObjectType.Unknown,
            },
            HousingFolder => ObjectType.Housing,
            WorldFolder1 => folder2 switch
            {
                HousingFolder => ObjectType.Housing,
                _             => ObjectType.World,
            },
            WorldFolder2 => ObjectType.World,
            VfxFolder    => ObjectType.Vfx,
            _            => ObjectType.Unknown,
        };
    }

    private const string CharacterFolder = "chara";
    private const string EquipmentFolder = "equipment";
    private const string PlayerFolder    = "human";
    private const string WeaponFolder    = "weapon";
    private const string AccessoryFolder = "accessory";
    private const string DemiHumanFolder = "demihuman";
    private const string MonsterFolder   = "monster";
    private const string CommonFolder    = "common";
    private const string UiFolder        = "ui";
    private const string IconFolder      = "icon";
    private const string LoadingFolder   = "loadingimage";
    private const string MapFolder       = "map";
    private const string InterfaceFolder = "uld";
    private const string FontFolder      = "font";
    private const string HousingFolder   = "hou";
    private const string VfxFolder       = "vfx";
    private const string WorldFolder1    = "bgcommon";
    private const string WorldFolder2    = "bg";

    private (FileType, ObjectType, Match?) ParseGamePath(string path)
    {
        var fileType   = Names.ExtensionToFileType.GetValueOrDefault(Path.GetExtension(path), FileType.Unknown);
        var objectType = PathToObjectType(path);

        var match = (fileType, objectType) switch
        {
            (FileType.Imc, ObjectType.Monster)      => Parser.Monster.Imc().Match(path),
            (FileType.Model, ObjectType.Monster)    => Parser.Monster.Mdl().Match(path),
            (FileType.Material, ObjectType.Monster) => Parser.Monster.Mtrl().Match(path),
            (FileType.Texture, ObjectType.Monster)  => Parser.Monster.Tex().Match(path),
            (FileType.Skeleton, ObjectType.Monster) => Parser.Monster.Skeleton().Match(path),
            (FileType.Physics, ObjectType.Monster)  => Parser.Monster.Skeleton().Match(path),

            (FileType.Imc, ObjectType.Weapon)      => Parser.Weapon.Imc().Match(path),
            (FileType.Model, ObjectType.Weapon)    => Parser.Weapon.Mdl().Match(path),
            (FileType.Material, ObjectType.Weapon) => Parser.Weapon.Mtrl().Match(path),
            (FileType.Texture, ObjectType.Weapon)  => Parser.Weapon.Tex().Match(path),
            (FileType.Skeleton, ObjectType.Weapon) => Parser.Weapon.Skeleton().Match(path),

            (FileType.Imc, ObjectType.DemiHuman)      => Parser.DemiHuman.Imc().Match(path),
            (FileType.Model, ObjectType.DemiHuman)    => Parser.DemiHuman.Mdl().Match(path),
            (FileType.Material, ObjectType.DemiHuman) => Parser.DemiHuman.Mtrl().Match(path),
            (FileType.Texture, ObjectType.DemiHuman)  => Parser.DemiHuman.Tex().Match(path),
            (FileType.Skeleton, ObjectType.DemiHuman) => Parser.DemiHuman.Skeleton().Match(path),
            (FileType.Physics, ObjectType.DemiHuman)  => Parser.DemiHuman.Skeleton().Match(path),

            (FileType.Imc, ObjectType.Equipment)      => Parser.Equipment.Imc().Match(path),
            (FileType.Model, ObjectType.Equipment)    => Parser.Equipment.Mdl().Match(path),
            (FileType.Material, ObjectType.Equipment) => Parser.Equipment.Mtrl().Match(path),
            (FileType.Texture, ObjectType.Equipment)  => Parser.Equipment.Tex().Match(path),
            (FileType.Vfx, ObjectType.Equipment)      => Parser.Equipment.Avfx().Match(path),

            (FileType.Imc, ObjectType.Accessory)      => Parser.Accessory.Imc().Match(path),
            (FileType.Model, ObjectType.Accessory)    => Parser.Accessory.Mdl().Match(path),
            (FileType.Material, ObjectType.Accessory) => Parser.Accessory.Mtrl().Match(path),
            (FileType.Texture, ObjectType.Accessory)  => Parser.Accessory.Tex().Match(path),
            (FileType.Vfx, ObjectType.Accessory)      => Parser.Accessory.Avfx().Match(path),

            (FileType.Model, ObjectType.Character)    => Parser.Character.Mdl().Match(path),
            (FileType.Material, ObjectType.Character) => Parser.Character.Mtrl().Match(path),
            (FileType.Texture, ObjectType.Character)  => TestCharacterTextures(path),
            (FileType.Skeleton, ObjectType.Character) => Parser.Character.Skeleton().Match(path),
            (FileType.Physics, ObjectType.Character)  => Parser.Character.Skeleton().Match(path),

            (FileType.Font, ObjectType.Font) => Parser.Font().Match(path),

            (FileType.Texture, ObjectType.Icon) => Parser.Icon().Match(path),
            (FileType.Texture, ObjectType.Map)  => Parser.Map().Match(path),
            _                                   => Match.Empty,
        };

        return (fileType, objectType, match.Success ? match : null);

        static Match TestCharacterTextures(string path)
        {
            ReadOnlySpan<Regex> regexes =
            [
                Parser.Character.Tex(),
                Parser.Character.Folder(),
                Parser.Character.Skin(),
                Parser.Character.Catchlight(),
                Parser.Character.Decal(),
            ];
            foreach (var regex in regexes)
            {
                var match = regex.Match(path);
                if (match.Success)
                    return match;
            }

            return Match.Empty;
        }
    }

    private static GameObjectInfo HandleEquipment(ObjectType type, FileType fileType, GroupCollection groups)
    {
        var setId = ushort.Parse(groups[Parser.Groups.PrimaryId].Value);
        if (fileType is FileType.Imc)
            return GameObjectInfo.Equipment(fileType, setId);

        if (fileType is FileType.Vfx)
        {
            var variant = byte.Parse(groups[Parser.Groups.Variant].Value);
            return GameObjectInfo.GearEffect(type, setId, variant);
        }

        var gr   = Names.GenderRaceFromCode(groups[Parser.Groups.RaceCode].Value);
        var slot = Names.SuffixToEquipSlot[groups[Parser.Groups.Slot].Value];
        if (fileType is FileType.Model)
            return GameObjectInfo.Equipment(fileType, setId, gr, slot);

        var variant2 = byte.Parse(groups[Parser.Groups.Variant].Value);
        return GameObjectInfo.Equipment(fileType, setId, gr, slot, variant2);
    }

    private static GameObjectInfo HandleWeapon(FileType fileType, GroupCollection groups)
    {
        var weaponId = ushort.Parse(groups[Parser.Groups.PrimaryId].Value);
        var setId    = ushort.Parse(groups[Parser.Groups.SecondaryId].Value);
        if (fileType is FileType.Imc or FileType.Model or FileType.Skeleton or FileType.Physics)
            return GameObjectInfo.Weapon(fileType, setId, weaponId);

        var variant = byte.Parse(groups[Parser.Groups.Variant].Value);
        return GameObjectInfo.Weapon(fileType, setId, weaponId, variant);
    }

    private static GameObjectInfo HandleMonster(FileType fileType, GroupCollection groups)
    {
        var monsterId = ushort.Parse(groups[Parser.Groups.PrimaryId].Value);
        var bodyId    = ushort.Parse(groups[Parser.Groups.SecondaryId].Value);
        if (fileType is FileType.Imc or FileType.Model or FileType.Skeleton or FileType.Physics)
            return GameObjectInfo.Monster(fileType, monsterId, bodyId);

        var variant = byte.Parse(groups[Parser.Groups.Variant].Value);
        return GameObjectInfo.Monster(fileType, monsterId, bodyId, variant);
    }

    private static GameObjectInfo HandleDemiHuman(FileType fileType, GroupCollection groups)
    {
        var demiHumanId = ushort.Parse(groups[Parser.Groups.PrimaryId].Value);
        var equipId     = ushort.Parse(groups[Parser.Groups.SecondaryId].Value);
        if (fileType is FileType.Imc or FileType.Skeleton or FileType.Physics)
            return GameObjectInfo.DemiHuman(fileType, demiHumanId, equipId);

        var slot = Names.SuffixToEquipSlot[groups[Parser.Groups.Slot].Value];
        if (fileType is FileType.Model)
            return GameObjectInfo.DemiHuman(fileType, demiHumanId, equipId, slot);

        var variant = byte.Parse(groups[Parser.Groups.Variant].Value);
        return GameObjectInfo.DemiHuman(fileType, demiHumanId, equipId, slot, variant);
    }

    private static GameObjectInfo HandleCustomization(FileType fileType, GroupCollection groups)
    {
        if (groups[Parser.Groups.Catchlight].Success)
            return GameObjectInfo.Customization(fileType, CustomizationType.Iris);

        if (groups[Parser.Groups.Skin].Success)
            return GameObjectInfo.Customization(fileType, CustomizationType.Skin);

        var id = ushort.Parse(groups[Parser.Groups.PrimaryId].Value);
        if (groups.TryGetValue(Parser.Groups.Decal, out var decal) && decal.Success)
        {
            var tmpType = decal.Value switch
            {
                "face"  => CustomizationType.DecalFace,
                "equip" => CustomizationType.DecalEquip,
                _       => CustomizationType.Unknown,
            };
            return GameObjectInfo.Customization(fileType, tmpType, id);
        }

        var gr       = Names.GenderRaceFromCode(groups[Parser.Groups.RaceCode].Value);
        var bodySlot = Names.StringToBodySlot[groups[Parser.Groups.BodyType].Value];
        if (fileType is FileType.Skeleton)
            return GameObjectInfo.Customization(fileType, CustomizationType.Skeleton, id, gr, bodySlot);
        if (fileType is FileType.Physics)
            return GameObjectInfo.Customization(fileType, CustomizationType.Physics, id, gr, bodySlot);

        var type = groups.TryGetValue(Parser.Groups.BodyTypeSlot, out var s) && s.Success
            ? Names.SuffixToCustomizationType[s.Value]
            : CustomizationType.Skin;
        if (fileType is FileType.Material)
        {
            var variant = groups.TryGetValue(Parser.Groups.Variant, out var v) && v.Success ? byte.Parse(v.Value) : (byte)0;
            return GameObjectInfo.Customization(fileType, type, id, gr, bodySlot, variant);
        }

        return GameObjectInfo.Customization(fileType, type, id, gr, bodySlot);
    }

    private static GameObjectInfo HandleIcon(FileType fileType, GroupCollection groups)
    {
        var hq = groups[Parser.Groups.Quality].Success;
        var hr = groups[Parser.Groups.Resolution].Success;
        var id = uint.Parse(groups[Parser.Groups.PrimaryId].Value);
        if (!groups.TryGetValue(Parser.Groups.Language, out var lang) || !lang.Success)
            return GameObjectInfo.Icon(fileType, id, hq, hr);

        var language = lang.Value switch
        {
            "en" => ClientLanguage.English,
            "ja" => ClientLanguage.Japanese,
            "de" => ClientLanguage.German,
            "fr" => ClientLanguage.French,
            _    => ClientLanguage.English,
        };
        return GameObjectInfo.Icon(fileType, id, hq, hr, language);
    }

    private static GameObjectInfo HandleMap(FileType fileType, GroupCollection groups)
    {
        var map     = Encoding.ASCII.GetBytes(groups[Parser.Groups.PrimaryId].Value);
        var variant = byte.Parse(groups[Parser.Groups.Variant].Value);
        if (groups.TryGetValue(Parser.Groups.Suffix, out var suf) && suf.Success)
        {
            var suffix = Encoding.ASCII.GetBytes(suf.Value)[0];
            return GameObjectInfo.Map(fileType, map[0], map[1], map[2], map[3], variant, suffix);
        }

        return GameObjectInfo.Map(fileType, map[0], map[1], map[2], map[3], variant);
    }
}
