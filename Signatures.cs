using Dalamud.Plugin.Services;
using OtterGui.Log;

namespace Penumbra.GameData;

/// <summary> Signatures in use for Penumbra. </summary>
public static class Sigs
{
    // ObjectManager
    public const string UpdateObjectArrays = "40 57 48 83 EC ?? 48 89 5C 24 ?? 33 DB";

    // ResourceLoader.Debug
    public const string ResourceManager = "48 8B 0D ?? ?? ?? ?? 0F 5B F6";

    // ResourceLoader.Replacement
    public const string GetResourceSync  = "E8 ?? ?? ?? ?? 48 8B C8 8B C3 F0 0F C0 81";
    public const string GetResourceAsync = "E8 ?? ?? ?? 00 48 8B D8 EB ?? F0 FF 83 ?? ?? 00 00";

    public const string ReadFile =
        "48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 41 54 41 55 41 56 41 57 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 48 63 42";

    public const string ReadSqPack = "40 56 41 56 48 83 EC ?? 0F BE 02";

    // The actual name of this function is unknown but it's related to updating async resource load states.
    public const string UpdateResourceState = "E8 ?? ?? ?? ?? EB ?? 48 8D 4E ?? FF 15";

    // ResourceLoader.TexMdl
    public const string CheckFileState   = "E8 ?? ?? ?? ?? 48 85 C0 74 ?? 4C 8B C8 44 0F B6 C5";
    public const string LoadTexFileLocal = "48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 48 89 7C 24 ?? 41 56 48 83 EC ?? 49 8B E8 44 88 4C 24";

    public const string LoadMdlFileLocal =
        "48 89 5C 24 ?? 55 56 57 41 54 41 55 41 56 41 57 48 8D 6C 24 ?? 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 45 ?? 48 8B 72 ?? 4C 8B EA";

    public const string LoadMdlFileExtern       = "E8 ?? ?? ?? ?? EB 02 B0 F1";
    public const string LodConfig               = "48 8B 05 ?? ?? ?? ?? B3";
    public const string TexHandleOnLoad         = "40 53 55 41 54 41 55 41 56 41 57 48 81 EC ?? ?? ?? ?? 48 8B D9";
    public const string TexHandleUpdateCategory = "0F B7 41 ?? FF C8";
    public const string RsfServiceAddress       = "48 8B 05 ?? ?? ?? ?? 84 C9";
    public const string SoundOnLoad             = "40 56 57 41 54 48 81 EC ?? ?? ?? ?? 80 3A ?? 45 0F B6 E0 48 8B F2 48 8B F9 75 ?? 83 BA ?? ?? ?? ?? ?? 72 ?? 48 8B 01 FF 90 ?? ?? ?? ?? 3C";
    public const string LoadScdFileLocal        = "48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC 30 8B 79 ?? 48 8B DA 8B D7";

    // GameEventManager
    public const string ResourceHandleDestructor = "48 89 5C 24 ?? 57 48 83 EC ?? 48 8D 05 ?? ?? ?? ?? 48 8B D9 48 89 01 B8";

    public const string CharacterDestructor =
        "48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC ?? 48 8D 05 ?? ?? ?? ?? 48 8B D9 48 89 01 48 8D 05 ?? ?? ?? ?? 48 89 81 ?? ?? ?? ?? 48 81 C1";

    // Shapes.AttributeHooks
    public const string UpdateAttributes = "E8 ?? ?? ?? ?? 33 C0 89 86 ?? ?? ?? ?? 48 8B 8C 24";

    // PathResolver.AnimationState
    public const string LoadTimelineResources      = "E8 ?? ?? ?? ?? 83 7F ?? ?? 75 ?? 0F B6 87 ?? ?? ?? ?? A8";
    public const string CharacterBaseLoadAnimation = "E8 ?? ?? ?? ?? BA ?? ?? ?? ?? 48 8B CB 44 8B C2 E8 ?? ?? ?? ?? 33 C0";
    public const string LoadSomePap                = "48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 41 56 41 57 48 83 EC ?? 41 8B D9 89 51";
    public const string LoadSomeAction             = "E8 ?? ?? ?? ?? C6 83 ?? ?? ?? ?? ?? 8B 8E";
    public const string LoadCharacterVfx           = "E8 ?? ?? ?? ?? 48 85 FF 48 8D 97";
    public const string LoadAreaVfx                = "E8 ?? ?? ?? ?? 0F 28 74 24 ?? 48 89 43";
    public const string ScheduleClipUpdate         = "40 53 55 56 57 48 81 EC ?? ?? ?? ?? 48 8B F9";

    public const string UnkMountAnimation =
        "48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 48 89 7C 24 ?? 41 54 41 56 41 57 48 83 EC ?? 45 8B F9";

    public const string UnkParasolAnimation = "E8 ?? ?? ?? ?? E9 ?? ?? ?? ?? 48 8B CB E8 ?? ?? ?? ?? 84 C0 0F 84 ?? ?? ?? ?? 41 83 BE";
    public const string Dismount = "E8 ?? ?? ?? ?? 48 8B 4F ?? F6 81";
    public const string GetCachedScheduleResource = "40 53 48 83 EC ?? 44 8B 4A";
    public const string LoadActionTmb = "E8 ?? ?? ?? ?? 48 89 43 ?? 0F B6 45";
    public const string SchedulerResourceManagementInstance = "48 89 3D ?? ?? ?? ?? 8B C7";

    public const string
        ApricotListenerSoundPlay =
            "41 54 41 55 41 56 41 57 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 4D 8B F9"; // Inlined in 48 89 83 ?? ?? ?? ?? 48 8B 46 ?? 48 85 C0 

    public const string ApricotListenerSoundPlayCaller = "4C 8B DC 56 48 81 EC ?? ?? ?? ?? F3 0F 10 89";
    public const string FootStepSound                  = "E9 ?? ?? ?? ?? 49 8B CE E8 ?? ?? ?? ?? 8B F8";

    // PathResolver.DrawObjectState
    public const string EnableDraw                 = "E8 ?? ?? ?? ?? 48 8B 8B ?? ?? ?? ?? 48 85 C9 74 33 45 33 C0";
    public const string SetupPlayerNpc             = "E8 ?? ?? ?? ?? 48 C7 40 ?? ?? ?? ?? ?? 48 8B 4B";
    public const string ConstructCutsceneCharacter = "48 89 5C 24 ?? 56 48 83 EC ?? 48 8B F1 45 33 C0";

    // PathResolver.Meta
    public const string UpdateModel       = "E8 ?? ?? ?? ?? 45 84 FF 4C 8B 7C 24 ?? 74 ?? 80 A7";
    public const string SetupVisor        = "E8 ?? ?? ?? ?? 48 8B 8B ?? ?? ?? ?? 0F 57 FF";
    public const string SetupVieraEars    = "E8 ?? ?? ?? ?? 48 8D 8F ?? ?? ?? ?? 4C 8D 4C 24";
    public const string RspSetupCharacter = "88 54 24 ?? 48 89 4C 24 ?? 53 55 56 57 41 54 41 55 41 57";
    public const string UpdateDrawData    = "40 53 57 41 56 48 83 EC ?? 48 8B F9 41 0F B6 D8";

    public const string GetEqdpAccessoryEntry = "E8 ?? ?? ?? ?? 41 BF ?? ?? ?? ?? 83 FB";
    public const string GetEqdpEquipEntry     = "E8 ?? ?? ?? ?? 85 DB 75 ?? F6 45";
    public const string GetEqpEntry           = "E8 ?? ?? ?? ?? 48 8B 87 ?? ?? ?? ?? 41 BC";
    public const string FindEstEntry          = "48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC ?? 41 0F B7 C0";
    public const string GetGmpEntry           = "E8 ?? ?? ?? ?? 48 8B 44 24 ?? 0F 57 C9";
    public const string GetRspBust            = "E8 ?? ?? ?? ?? 8B 48 ?? F2 0F 10 38";
    public const string GetRspHeight          = "E8 ?? ?? ?? ?? 66 0F 6E CD 48 8D 44 24";
    public const string GetRspTail            = "E8 ?? ?? ?? ?? F3 0F 11 46 ?? 83 FD";
    public const string AtchCaller1           = "E8 ?? ?? ?? ?? 8B D3 49 8B CC E8 ?? ?? ?? ?? F3 0F 10 15";
    public const string AtchCaller2           = "E8 ?? ?? ?? ?? 48 8B 8E ?? ?? ?? ?? 84 C0 74 ?? 8B D5";

    // PathResolver.PathState
    public const string HumanVTable = "48 8D 05 ?? ?? ?? ?? 48 89 03 89 8B";

    public const string WeaponVTable =
        "48 8D 05 ?? ?? ?? ?? 48 89 8B ?? ?? ?? ?? 48 89 03 48 8B C3 89 8B";

    public const string DemiHumanVTable = "48 8D 05 ?? ?? ?? ?? 48 89 03 48 8B C3 89 8B ?? ?? ?? ?? 66 89 8B";

    public const string MonsterVTable =
        "48 8D 05 ?? ?? ?? ?? 48 89 03 33 C0 48 89 83 ?? ?? ?? ?? 48 89 83 ?? ?? ?? ?? 48 89 83 ?? ?? ?? ?? 66 89 83 ?? ?? ?? ?? 48 8B C3";

    // PathResolver.Subfiles
    public const string LoadMtrl = "4C 8B DC 57 41 54 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 48 8B F9";

    public const string LoadMtrlTex = "E8 ?? ?? ?? ?? 84 C0 B9 ?? ?? ?? ?? BA ?? ?? ?? ?? 0F 44 CA 0F B6 C1 48 83 C4 ?? C3 0F B6 C2";

    public const string LoadMtrlShpk =
        "48 89 5C 24 ?? 57 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 44 0F B7 89";

    public const string ApricotResourceLoad = "48 89 74 24 ?? 57 48 83 EC ?? 41 0F B6 F0 48 8B F9 40 80 FE";

    // CharacterUtility
    public const string CharacterUtility       = "48 8B 05 ?? ?? ?? ?? 83 B9";
    public const string LoadCharacterResources = "E8 ?? ?? ?? ?? 48 8D 8F ?? ?? ?? ?? E8 ?? ?? ?? ?? 33 D2 45 33 C0";

    // MetaFileManager
    public const string GetFileSpace = "E8 ?? ?? ?? ?? 41 B9 ?? ?? ?? ?? 4C 8B C0 33 D2";

    // ResidentResourceManager
    public const string ResidentResourceManager = "0F 44 FE 48 8B 0D ?? ?? ?? ?? 48 85 C9 74 05";

    public const string LoadPlayerResources =
        "E8 ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? BA ?? ?? ?? ?? 41 B8 ?? ?? ?? ?? 48 8B 48 30 48 8B 01 FF 50 10 48 85 C0 74 0A";

    public const string UnloadPlayerResources =
        "41 55 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 4C 8B E9 48 83 C1";

    // ModelResourceHandleUtility
    public const string GetMaterialFileNameBySlot = "48 8B 81 ?? ?? ?? ?? 8B D2 44 0F B7 04 ?? 48 C7 C0";

    // ModelRenderer
    public const string RenderManager = "48 03 2D ?? ?? ?? ?? 0F";

    // ShaderReplacementFixer
    public const string ModelRendererOnRenderMaterial = "E8 ?? ?? ?? ?? 44 0F B7 28";
    public const string ModelRendererUnkFunc          = "E8 ?? ?? ?? ?? 41 0F B6 46 ?? C0 E8";

    // RenderTargetHdrEnabler
    public const string RenderTargetManagerInitialize = "40 55 53 56 57 41 54 41 55 41 56 41 57 48 8B EC 48 83 EC ?? 48 8B 1D";
    public const string DeviceCreateTexture2D         = "E8 ?? ?? ?? ?? 48 89 07 48 8D 7F 20";

    // Job service
    public const string ChangeJob = "48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 41 56 41 57 48 83 EC ?? 44 0F B6 FA";

    // Flag Slot for Update service
    public const string FlagSlotForUpdate      = "48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC ?? 8B DA 49 8B F0 48 8B F9";
    public const string FlagBonusSlotForUpdate = "48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC ?? 49 8B F0 8B DA 48 8B F9 83 FA ?? 72";

    // Partial/composable materials
    public const string CreateMaterial = "E8 ?? ?? ?? ?? 48 89 87 ?? ?? ?? ?? 48 85 C0 75 ?? 48 8B 8F ?? ?? ?? ?? 48 85 C9 74 ?? 48 8B 01";

    // Composable array textures
    public const string TextureResourceHandleVTable =
        "48 8D 05 ?? ?? ?? ?? 48 8B D9 48 89 01 48 8B 89 ?? ?? ?? ?? 48 85 C9 74 ?? E8 ?? ?? ?? ?? 48 8B 8B ?? ?? ?? ?? 48 85 C9 74 ?? E8 ?? ?? ?? ?? 48 8B CB";

    // PBD Postprocessor
    public const string LoadEpbdData = "40 53 57 48 83 EC ?? 48 8B 01 48 8B F9 FF 90";

    public const string UpdateOrnament = "E8 ?? ?? ?? ?? F3 0F 10 4E ?? 0F 57 D2";

    public const string PrepareColorSet = "E8 ?? ?? ?? ?? 49 89 04 ?? 49 83 C5";
    public const string UpdateColorSets = "89 54 24 ?? 55 56 41 56 48 81 EC";

    public const string CalculateHeight = "E8 ?? ?? ?? FF 48 8B 0D ?? ?? ?? ?? 0F 28 F0";

    public const string SetFreeCompanyCrestBitfield = "E8 ?? ?? ?? ?? 48 8B 55 ?? 48 8B CE E8";

    // Inlined in this, but still called in 4 others.
    public const string CrestChangeCaller =
        "48 89 5C 24 ?? 55 56 57 41 54 41 55 41 56 41 57 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 44 0F B6 B9";

    public const string SetUnlockLinkValue   = "48 83 EC ?? 8B C2 83 E0";
    public const string RequestGlamourPlates = "E8 ?? ?? ?? ?? BF ?? ?? ?? ?? 48 8D 44 24 ?? 8B CF 66 66 0F 1F 84 ?? 00 00 00 00 89 28";

    // Pap redirection
    public const string LoadAlwaysResidentMotionPacks =
        "E8 ?? ?? ?? ?? 48 8B D0 48 8D 8E ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 8B 4C 24 ?? 48 8B BC 24 ?? ?? ?? ?? 48 85 C9";

    public const string LoadWeaponDependentResidentMotionPacks =
        "E8 ?? ?? ?? ?? 48 8B D0 48 8D 8E ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 8B 4C 24 ?? 48 8B BC 24 ?? ?? ?? ?? 48 8B 5C 24";

    public const string LoadInitialResidentMotionPacks = "E8 ?? ?? ?? ?? 48 8B 5D ?? 48 8B 7D ?? 48 3B DF 74 ?? 66 66 0F 1F 84 ?? 00 00 00 00 48 8B 96";
    public const string LoadMotionPacks                = "E8 ?? ?? ?? 00 48 8B 44 24 ?? 49 89 04 24";
    public const string LoadMotionPacks2               = "48 89 5C 24 ?? 55 56 57 41 54 41 55 41 56 41 57 48 8D AC 24 ?? ?? ?? ?? 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 85 ?? ?? ?? ?? 4C 8B 9D";
    public const string LoadMigratoryMotionPack        = "E9 ?? ?? ?? ?? 8B 84 24 ?? ?? ?? ?? 48 8D 8C 24";

    [Conditional("DEBUG")]
    public static void TestSignatures(ISigScanner scanner, Logger log)
    {
        var fields = typeof(Sigs).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(f => f is { IsLiteral: true, IsInitOnly: false } && f.FieldType == typeof(string));
        var missingSignatures = false;
        foreach (var field in fields)
        {
            if (field.GetRawConstantValue() is not string value)
            {
                log.Error($"[Signature] {field.Name}: Could not get value for field.");
                missingSignatures = true;
                continue;
            }
        
            try
            {
                var results = scanner.ScanAllText(value);
                switch (results.Length)
                {
                    case 0:
                        log.Error($"[Signature] {field.Name} ({value}): Not found.");
                        missingSignatures = true;
                        break;
                    case 1: 
                        log.Information($"[Signature] {field.Name} ({value}): Match found.");
                        continue;
                    default:
                        log.Error($"[Signature] {field.Name} ({value}): Multiple matches.");
                        missingSignatures = true;
                        break;
                }
            }
            catch (Exception ex)
            {
                log.Error($"[Signature] {field.Name} ({value}): Unknown Error - {ex.Message}");
                missingSignatures = true;
            }
        }

        if (missingSignatures)
            throw new Exception("Missing Signatures!");
    }
}
