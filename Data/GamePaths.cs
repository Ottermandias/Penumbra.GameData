using Penumbra.GameData.Enums;
using Penumbra.GameData.Structs;
using Penumbra.String;
using Penumbra.String.Classes;
using MtrlFile = Penumbra.GameData.Files.MtrlFile;

namespace Penumbra.GameData.Data;

/// <summary> Functions to create game paths for certain types of files. </summary>
public static class GamePaths
{
    public static class Imc
    {
        public static string Path(ObjectType type, PrimaryId primaryId)
            => Path(type, primaryId, 1);

        public static string Path(ObjectType type, PrimaryId primaryId, SecondaryId secondaryId)
            => type switch
            {
                ObjectType.Monster   => Monster(primaryId, secondaryId),
                ObjectType.Weapon    => Weapon(primaryId, secondaryId),
                ObjectType.DemiHuman => DemiHuman(primaryId, secondaryId),
                ObjectType.Equipment => Equipment(primaryId),
                ObjectType.Accessory => Accessory(primaryId),
                _                    => string.Empty,
            };

        public static string Monster(PrimaryId monsterId, SecondaryId bodyId)
            => $"chara/monster/m{monsterId.Id:D4}/obj/body/b{bodyId.Id:D4}/b{bodyId.Id:D4}.imc";

        public static string Weapon(PrimaryId weaponId, SecondaryId bodyId)
            => $"chara/weapon/w{weaponId.Id:D4}/obj/body/b{bodyId.Id:D4}/b{bodyId.Id:D4}.imc";

        public static string DemiHuman(PrimaryId demiId, SecondaryId equipId)
            => $"chara/demihuman/d{demiId.Id:D4}/obj/equipment/e{equipId.Id:D4}/e{equipId.Id:D4}.imc";

        public static string Equipment(PrimaryId equipId)
            => $"chara/equipment/e{equipId.Id:D4}/e{equipId.Id:D4}.imc";

        public static string Accessory(PrimaryId accessoryId)
            => $"chara/accessory/a{accessoryId.Id:D4}/a{accessoryId.Id:D4}.imc";
    }

    public static class Mdl
    {
        public static string Path(ObjectType type, PrimaryId primaryId, GenderRace secondaryId, EquipSlot slot = EquipSlot.Unknown)
            => type switch
            {
                ObjectType.Monster   => Monster(primaryId, (ushort)secondaryId),
                ObjectType.Weapon    => Weapon(primaryId, (ushort)secondaryId),
                ObjectType.DemiHuman => DemiHuman(primaryId, (ushort)secondaryId, slot),
                ObjectType.Equipment => Equipment(primaryId, secondaryId, slot),
                ObjectType.Accessory => Accessory(primaryId, secondaryId, slot),
                _                    => string.Empty,
            };

        public static string Path(ObjectType type, PrimaryId primaryId, SecondaryId secondaryId, EquipSlot slot = EquipSlot.Unknown)
            => type switch
            {
                ObjectType.Monster   => Monster(primaryId, secondaryId),
                ObjectType.Weapon    => Weapon(primaryId, secondaryId),
                ObjectType.DemiHuman => DemiHuman(primaryId, secondaryId, slot),
                ObjectType.Equipment => Equipment(primaryId, (GenderRace)secondaryId.Id, slot),
                ObjectType.Accessory => Accessory(primaryId, (GenderRace)secondaryId.Id, slot),
                _                    => string.Empty,
            };

        public static string Gear(PrimaryId id, GenderRace raceCode, EquipSlot slot)
            => slot.IsAccessory() ? Accessory(id, raceCode, slot) : Equipment(id, raceCode, slot);

        public static string Monster(PrimaryId monsterId, SecondaryId bodyId)
            => $"chara/monster/m{monsterId.Id:D4}/obj/body/b{bodyId.Id:D4}/model/m{monsterId.Id:D4}b{bodyId.Id:D4}.mdl";

        public static string Weapon(PrimaryId weaponId, SecondaryId bodyId)
            => $"chara/weapon/w{weaponId.Id:D4}/obj/body/b{bodyId.Id:D4}/model/w{weaponId.Id:D4}b{bodyId.Id:D4}.mdl";

        public static string DemiHuman(PrimaryId demiId, SecondaryId equipId, EquipSlot slot)
            => $"chara/demihuman/d{demiId.Id:D4}/obj/equipment/e{equipId.Id:D4}/model/d{demiId.Id:D4}e{equipId.Id:D4}_{slot.ToSuffix()}.mdl";

        public static string Equipment(PrimaryId equipId, GenderRace raceCode, EquipSlot slot)
            => $"chara/equipment/e{equipId.Id:D4}/model/c{raceCode.ToRaceCode()}e{equipId.Id:D4}_{slot.ToSuffix()}.mdl";

        public static string Accessory(PrimaryId accessoryId, GenderRace raceCode, EquipSlot slot)
            => $"chara/accessory/a{accessoryId.Id:D4}/model/c{raceCode.ToRaceCode()}a{accessoryId.Id:D4}_{slot.ToSuffix()}.mdl";

        public static string Customization(GenderRace raceCode, BodySlot slot, PrimaryId slotId, CustomizationType type)
            => $"chara/human/c{raceCode.ToRaceCode()}/obj/{slot.ToSuffix()}/{slot.ToAbbreviation()}{slotId.Id:D4}/model/c{raceCode.ToRaceCode()}{slot.ToAbbreviation()}{slotId.Id:D4}_{type.ToSuffix()}.mdl";
    }


    public static class Mtrl
    {
        public static string Path(ObjectType type, PrimaryId primaryId, SecondaryId secondaryId, Variant variant, string suffix,
            EquipSlot slot = EquipSlot.Unknown)
            => type switch
            {
                ObjectType.Monster   => Monster(primaryId, secondaryId, variant, suffix),
                ObjectType.Weapon    => Weapon(primaryId, secondaryId, variant, suffix),
                ObjectType.DemiHuman => DemiHuman(primaryId, secondaryId, slot, variant, suffix),
                ObjectType.Equipment => Equipment(primaryId, (GenderRace)secondaryId.Id, slot, variant, suffix),
                ObjectType.Accessory => Accessory(primaryId, (GenderRace)secondaryId.Id, slot, variant, suffix),
                _                    => string.Empty,
            };

        public static string Gear(PrimaryId id, GenderRace raceCode, EquipSlot slot, Variant variant, string suffix)
            => slot.IsAccessory() ? Accessory(id, raceCode, slot, variant, suffix) : Equipment(id, raceCode, slot, variant, suffix);

        public static string GearFolder(EquipSlot slot, PrimaryId id, Variant variant)
            => slot.IsAccessory() ? AccessoryFolder(id, variant) : EquipmentFolder(id, variant);

        public static string Monster(PrimaryId monsterId, SecondaryId bodyId, Variant variant, string suffix)
            => $"chara/monster/m{monsterId.Id:D4}/obj/body/b{bodyId.Id:D4}/material/v{variant.Id:D4}/mt_m{monsterId.Id:D4}b{bodyId.Id:D4}_{suffix}.mtrl";

        public static string Weapon(PrimaryId weaponId, SecondaryId bodyId, Variant variant, string suffix)
            => $"chara/weapon/w{weaponId.Id:D4}/obj/body/b{bodyId.Id:D4}/material/v{variant.Id:D4}/mt_w{weaponId.Id:D4}b{bodyId.Id:D4}_{suffix}.mtrl";

        public static string DemiHuman(PrimaryId demiId, SecondaryId equipId, EquipSlot slot, Variant variant, string suffix)
            => $"chara/demihuman/d{demiId.Id:D4}/obj/equipment/e{equipId.Id:D4}/material/v{variant.Id:D4}/mt_d{demiId.Id:D4}e{equipId.Id:D4}_{slot.ToSuffix()}_{suffix}.mtrl";

        public static string Equipment(PrimaryId equipId, GenderRace raceCode, EquipSlot slot, Variant variant, string suffix)
            => $"{EquipmentFolder(equipId, variant)}/mt_c{raceCode.ToRaceCode()}e{equipId.Id:D4}_{slot.ToSuffix()}_{suffix}.mtrl";

        public static string EquipmentFolder(PrimaryId equipId, Variant variant)
            => $"chara/equipment/e{equipId.Id:D4}/material/v{variant.Id:D4}";

        public static string Accessory(PrimaryId accessoryId, GenderRace raceCode, EquipSlot slot, Variant variant, string suffix)
            => $"{AccessoryFolder(accessoryId, variant)}/mt_c{raceCode.ToRaceCode()}a{accessoryId.Id:D4}_{slot.ToSuffix()}_{suffix}.mtrl";

        public static string AccessoryFolder(PrimaryId accessoryId, Variant variant)
            => $"chara/accessory/a{accessoryId.Id:D4}/material/v{variant.Id:D4}";

        public static string Customization(GenderRace raceCode, BodySlot slot, PrimaryId slotId, string fileName,
            out GenderRace actualGr, out PrimaryId actualSlotId)
            => Customization(raceCode, slot, slotId, fileName, out actualGr, out actualSlotId, Variant.None);

        public static string Customization(GenderRace raceCode, BodySlot slot, PrimaryId slotId, string fileName,
            out GenderRace actualGr, out PrimaryId actualSlotId, Variant variant)
        {
            switch (slot)
            {
                case BodySlot.Hair:
                    actualSlotId = slotId;
                    return Hair(raceCode, slotId, fileName, out actualGr);
                case BodySlot.Tail:
                    actualGr = raceCode;
                    return Tail(raceCode, slotId, fileName, variant, out actualSlotId);
                default:
                    actualSlotId = slotId;
                    actualGr     = raceCode;
                    return $"{CustomizationFolder(raceCode, slot, slotId, variant)}{fileName}";
            }
        }

        public static string Hair(GenderRace raceCode, PrimaryId slotId, string fileName, out GenderRace actualGr)
        {
            actualGr = MaterialHandling.GetGameGenderRace(raceCode, slotId);
            var folder = CustomizationFolder(actualGr, BodySlot.Hair, slotId, 1);
            return actualGr == raceCode
                ? $"{folder}{fileName}"
                : $"{folder}/mt_c{actualGr.ToRaceCode()}{fileName[9..]}";
        }

        public static string Tail(GenderRace raceCode, PrimaryId slotId, string fileName, Variant variant, out PrimaryId actualSlotId)
        {
            switch (raceCode)
            {
                case GenderRace.HrothgarMale:
                case GenderRace.HrothgarFemale:
                case GenderRace.HrothgarMaleNpc:
                case GenderRace.HrothgarFemaleNpc:
                    var folder = CustomizationFolder(raceCode, BodySlot.Tail, 1, variant == Variant.None ? 1 : variant);
                    actualSlotId = 1;
                    return $"{folder}{fileName}";
                default:
                    actualSlotId = slotId;
                    return $"{CustomizationFolder(raceCode, BodySlot.Tail, slotId, variant)}{fileName}";
            }
        }

        public static string CustomizationFolder(GenderRace raceCode, BodySlot slot, PrimaryId slotId, Variant variant)
            => variant == Variant.None
                ? CustomizationFolder(raceCode, slot, slotId)
                : $"chara/human/c{raceCode.ToRaceCode()}/obj/{slot.ToSuffix()}/{slot.ToAbbreviation()}{slotId.Id:D4}/material/v{variant.Id:D4}";

        public static string CustomizationFolder(GenderRace raceCode, BodySlot slot, PrimaryId slotId)
            => $"chara/human/c{raceCode.ToRaceCode()}/obj/{slot.ToSuffix()}/{slot.ToAbbreviation()}{slotId.Id:D4}/material";
    }

    public static class Tex
    {
        public static string Path(ObjectType type, PrimaryId primaryId, SecondaryId secondaryId, Variant variant, char suffix1,
            EquipSlot slot = EquipSlot.Unknown, char suffix2 = '\0')
            => type switch
            {
                ObjectType.Monster   => Monster(primaryId, secondaryId, variant, suffix1, suffix2),
                ObjectType.Weapon    => Weapon(primaryId, secondaryId, variant, suffix1, suffix2),
                ObjectType.DemiHuman => DemiHuman(primaryId, secondaryId, slot, variant, suffix1, suffix2),
                ObjectType.Equipment => Equipment(primaryId, (GenderRace)secondaryId.Id, slot, variant, suffix1, suffix2),
                ObjectType.Accessory => Accessory(primaryId, (GenderRace)secondaryId.Id, slot, variant, suffix1, suffix2),
                _                    => string.Empty,
            };

        public static string Gear(PrimaryId id, GenderRace raceCode, EquipSlot slot, Variant variant, char suffix1, char suffix2 = '\0')
            => slot.IsAccessory()
                ? Accessory(id, raceCode, slot, variant, suffix1, suffix2)
                : Equipment(id, raceCode, slot, variant, suffix1, suffix2);

        public static string Monster(PrimaryId monsterId, SecondaryId bodyId, Variant variant, char suffix1, char suffix2 = '\0')
            => $"chara/monster/m{monsterId.Id:D4}/obj/body/b{bodyId.Id:D4}/texture/v{variant.Id:D2}_m{monsterId.Id:D4}b{bodyId.Id:D4}{(suffix2 != '\0' ? $"_{suffix2}" : string.Empty)}_{suffix1}.tex";

        public static string Weapon(PrimaryId weaponId, SecondaryId bodyId, Variant variant, char suffix1, char suffix2 = '\0')
            => $"chara/weapon/w{weaponId.Id:D4}/obj/body/b{bodyId.Id:D4}/texture/v{variant.Id:D2}_w{weaponId.Id:D4}b{bodyId.Id:D4}{(suffix2 != '\0' ? $"_{suffix2}" : string.Empty)}_{suffix1}.tex";

        public static string DemiHuman(PrimaryId demiId, SecondaryId equipId, EquipSlot slot, Variant variant, char suffix1,
            char suffix2 = '\0')
            => $"chara/demihuman/d{demiId.Id:D4}/obj/equipment/e{equipId.Id:D4}/texture/v{variant.Id:D2}_d{demiId.Id:D4}e{equipId.Id:D4}_{slot.ToSuffix()}{(suffix2 != '\0' ? $"_{suffix2}" : string.Empty)}_{suffix1}.tex";

        public static string Equipment(PrimaryId equipId, GenderRace raceCode, EquipSlot slot, Variant variant, char suffix1,
            char suffix2 = '\0')
            => $"chara/equipment/e{equipId.Id:D4}/texture/v{variant.Id:D2}_c{raceCode.ToRaceCode()}e{equipId.Id:D4}_{slot.ToSuffix()}{(suffix2 != '\0' ? $"_{suffix2}" : string.Empty)}_{suffix1}.tex";

        public static string Accessory(PrimaryId accessoryId, GenderRace raceCode, EquipSlot slot, Variant variant, char suffix1,
            char suffix2 = '\0')
            => $"chara/accessory/a{accessoryId.Id:D4}/texture/v{variant.Id:D2}_c{raceCode.ToRaceCode()}a{accessoryId.Id:D4}_{slot.ToSuffix()}{(suffix2 != '\0' ? $"_{suffix2}" : string.Empty)}_{suffix1}.tex";

        public static string Customization(GenderRace raceCode, BodySlot slot, PrimaryId slotId, char suffix1, bool minus = false,
            CustomizationType type = CustomizationType.Unknown, char suffix2 = '\0')
            => Customization(raceCode, slot, slotId, suffix1, Variant.None, minus, type, suffix2);

        public static string Customization(GenderRace raceCode, BodySlot slot, PrimaryId slotId, char suffix1, Variant variant,
            bool minus = false,
            CustomizationType type = CustomizationType.Unknown, char suffix2 = '\0')
            => $"chara/human/c{raceCode.ToRaceCode()}/obj/{slot.ToSuffix()}/{slot.ToAbbreviation()}{slotId.Id:D4}/texture/"
              + (minus ? "--" : string.Empty)
              + (variant != Variant.None ? $"v{variant.Id:D2}_" : string.Empty)
              + $"c{raceCode.ToRaceCode()}{slot.ToAbbreviation()}{slotId.Id:D4}{(type != CustomizationType.Unknown ? $"_{type.ToSuffix()}" : string.Empty)}{(suffix2 != '\0' ? $"_{suffix2}" : string.Empty)}_{suffix1}.tex";


        public const string LegacyDecal   = "chara/common/texture/decal_equip/_stigma.tex";
        public const string Transparent   = "chara/common/texture/transparent.tex";
        public const string Dummy         = "common/graphics/texture/dummy.tex";
        public const string TileOrbArray  = "chara/common/texture/tile_orb_array.tex";
        public const string TileNormArray = "chara/common/texture/tile_norm_array.tex";
        public const string SphereDArray  = "chara/common/texture/sphere_d_array.tex";

        public const int TileOrbArrayCrc32  = unchecked((int)0x946D97B8u);
        public const int TileNormArrayCrc32 = unchecked((int)0xCD07EB85u);
        public const int SphereDArrayCrc32  = unchecked((int)0xF97FB20Bu);

        public static readonly Utf8GamePath LegacyDecalUtf8 =
            Utf8GamePath.FromString(LegacyDecal, out var p) ? p : Utf8GamePath.Empty;

        public static readonly Utf8GamePath TransparentUtf8 =
            Utf8GamePath.FromString(Transparent, out var p) ? p : Utf8GamePath.Empty;

        public static readonly Utf8GamePath TileOrbArrayUtf8 =
            Utf8GamePath.FromString(TileOrbArray, out var p) ? p : Utf8GamePath.Empty;

        public static readonly Utf8GamePath TileNormArrayUtf8 =
            Utf8GamePath.FromString(TileNormArray, out var p) ? p : Utf8GamePath.Empty;

        public static readonly Utf8GamePath SphereDArrayUtf8 =
            Utf8GamePath.FromString(SphereDArray, out var p) ? p : Utf8GamePath.Empty;

        public static string EquipDecal(byte decalId)
            => $"chara/common/texture/decal_equip/-decal_{decalId:D3}.tex";

        public static string FaceDecal(byte decalId)
            => $"chara/common/texture/decal_face/_decal_{decalId}.tex";

        /// <summary> DX11 specific textures get '--' prepended to the filename. Even if the filename already starts with '--'. </summary>
        /// <param name="texture"> The texture struct to check. </param>
        /// <param name="ret"> The correctly modified path. </param>
        /// <returns> Whether the return path was manipulated. </returns>
        public static bool HandleDx11Path(in MtrlFile.Texture texture, out string ret)
        {
            if (!texture.DX11)
            {
                ret = texture.Path;
                return false;
            }

            var fileName       = System.IO.Path.GetFileName(texture.Path.AsSpan());
            var lastSlashIndex = texture.Path.LastIndexOf('/');
            ret = lastSlashIndex >= 0
                ? $"{texture.Path.AsSpan()[..lastSlashIndex]}/--{fileName}"
                : $"--{fileName}";
            return true;
        }
    }

    public static class Sklb
    {
        public static string Path(ObjectType type, PrimaryId id)
            => type switch
            {
                ObjectType.Monster   => Monster(id),
                ObjectType.Weapon    => Weapon(id),
                ObjectType.DemiHuman => DemiHuman(id),
                _                    => string.Empty,
            };

        public static string Monster(PrimaryId monsterId)
            => $"chara/monster/m{monsterId.Id:D4}/skeleton/base/b0001/skl_m{monsterId.Id:D4}b0001.sklb";

        public static string Weapon(PrimaryId weaponId)
            => $"chara/weapon/w{weaponId.Id:D4}/skeleton/base/b0001/skl_w{weaponId.Id:D4}b0001.sklb";

        public static string DemiHuman(PrimaryId demiHumanId)
            => $"chara/demihuman/d{demiHumanId.Id:D4}/skeleton/base/b0001/skl_d{demiHumanId.Id:D4}b0001.sklb";

        public static string Customization(GenderRace raceCode, string slot, PrimaryId slotId)
            => $"chara/human/c{raceCode.ToRaceCode()}/skeleton/{slot}/{slot[0]}{slotId.Id:D4}/skl_c{raceCode.ToRaceCode()}{slot[0]}{slotId.Id:D4}.sklb";

        public const string MaterialAnimationSkeleton = "chara/common/animation/skl_material.sklb";

        public static readonly Utf8GamePath MaterialAnimationSkeletonUtf8 =
            Utf8GamePath.FromString(MaterialAnimationSkeleton, out var p) ? p : Utf8GamePath.Empty;
    }

    public static class Skp
    {
        public static string Path(ObjectType type, PrimaryId id)
            => type switch
            {
                ObjectType.Monster   => Monster(id),
                ObjectType.Weapon    => Weapon(id),
                ObjectType.DemiHuman => DemiHuman(id),
                _                    => string.Empty,
            };

        public static string Monster(PrimaryId monsterId)
            => $"chara/monster/m{monsterId.Id:D4}/skeleton/base/b0001/skl_m{monsterId.Id:D4}b0001.skp";

        public static string Weapon(PrimaryId weaponId)
            => $"chara/weapon/w{weaponId.Id:D4}/skeleton/base/b0001/skl_w{weaponId.Id:D4}b0001.skp";

        public static string DemiHuman(PrimaryId demiHumanId)
            => $"chara/demihuman/d{demiHumanId.Id:D4}/skeleton/base/b0001/skl_d{demiHumanId.Id:D4}b0001.skp";

        public static string Customization(GenderRace raceCode, string slot, PrimaryId slotId)
            => $"chara/human/c{raceCode.ToRaceCode()}/skeleton/{slot}/{slot[0]}{slotId.Id:D4}/skl_c{raceCode.ToRaceCode()}{slot[0]}{slotId.Id:D4}.skp";
    }

    public static class Eid
    {
        public static string Path(ObjectType type, PrimaryId id)
            => type switch
            {
                ObjectType.Monster   => Monster(id),
                ObjectType.Weapon    => Weapon(id),
                ObjectType.DemiHuman => DemiHuman(id),
                _                    => string.Empty,
            };

        public static string Monster(PrimaryId monsterId)
            => $"chara/monster/m{monsterId.Id:D4}/skeleton/base/b0001/eid_m{monsterId.Id:D4}b0001.eid";

        public static string Weapon(PrimaryId weaponId)
            => $"chara/weapon/w{weaponId.Id:D4}/skeleton/base/b0001/eid_w{weaponId.Id:D4}b0001.eid";

        public static string DemiHuman(PrimaryId demiHumanId)
            => $"chara/demihuman/d{demiHumanId.Id:D4}/skeleton/base/b0001/eid_d{demiHumanId.Id:D4}b0001.eid";
    }

    public static class Phyb
    {
        public static string Customization(GenderRace raceCode, string slot, PrimaryId slotId)
            => $"chara/human/c{raceCode.ToRaceCode()}/skeleton/{slot}/{slot[0]}{slotId.Id:D4}/phy_c{raceCode.ToRaceCode()}{slot[0]}{slotId.Id:D4}.phyb";
    }

    public static class Kdb
    {
        public static string Customization(GenderRace raceCode, string slot, PrimaryId slotId)
            => $"chara/human/c{raceCode.ToRaceCode()}/skeleton/{slot}/{slot[0]}{slotId.Id:D4}/kdi_c{raceCode.ToRaceCode()}{slot[0]}{slotId.Id:D4}.kdb";
    }

    public static class Pbd
    {
        public const string Path = "chara/xls/boneDeformer/human.pbd";

        public static readonly Utf8GamePath PathUtf8 =
            Utf8GamePath.FromString(Path, out var p) ? p : Utf8GamePath.Empty;
    }

    public static class Avfx
    {
        public static string Path(EquipSlot slot, PrimaryId id, byte effectId)
            => slot.IsAccessory() ? Accessory(id, effectId) : Equipment(id, effectId);

        public static string Equipment(PrimaryId equipId, byte effectId)
            => $"chara/equipment/e{equipId.Id:D4}/vfx/eff/ve{effectId:D4}.avfx";

        public static string Accessory(PrimaryId equipId, byte effectId)
            => $"chara/accessory/a{equipId.Id:D4}/vfx/eff/ve{effectId:D4}.avfx";
    }

    public static class Tmb
    {
        public static string Action(string key)
            => $"chara/action/{key}.tmb";
    }

    public static string Attach(GenderRace gr)
        => $"chara/xls/attachOffset/c{gr.ToRaceCode()}.atch";

    public static string Shader(string name)
        => $"shader/sm5/shpk/{name}";
}
