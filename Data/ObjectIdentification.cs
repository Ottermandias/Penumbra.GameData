using Dalamud;
using Lumina.Excel.GeneratedSheets;
using Penumbra.GameData.Enums;
using Penumbra.GameData.Structs;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using Penumbra.GameData.Actors;
using Penumbra.GameData.Files;
using Action = Lumina.Excel.GeneratedSheets.Action;
using ObjectType = Penumbra.GameData.Enums.ObjectType;

namespace Penumbra.GameData.Data;

internal sealed class ObjectIdentification : DataSharer, IObjectIdentifier
{
    public const int IdentificationVersion = 6;

    public           IGamePathParser                                                       GamePathParser { get; }
    public readonly  IReadOnlyList<IReadOnlyList<uint>>                                    BnpcNames;
    public readonly  IReadOnlyList<IReadOnlyList<(string Name, ObjectKind Kind, uint Id)>> ModelCharaToObjects;
    public readonly  IReadOnlyDictionary<string, IReadOnlyList<Action>>                    Actions;
    public readonly  IReadOnlyDictionary<string, IReadOnlyList<Emote>>                     Emotes;
    private readonly ActorManager.ActorManagerData                                         _actorData;

    IReadOnlyDictionary<string, IReadOnlyList<Action>> IObjectIdentifier.Actions
        => Actions;

    IReadOnlyDictionary<string, IReadOnlyList<Emote>> IObjectIdentifier.Emotes
        => Emotes;

    private readonly EquipmentIdentificationList _equipment;
    private readonly WeaponIdentificationList    _weapons;
    private readonly ModelIdentificationList     _modelIdentifierToModelChara;

    public ObjectIdentification(DalamudPluginInterface pluginInterface, IDataManager dataManager, ItemData itemData, ClientLanguage language,
        IPluginLog log)
        : base(pluginInterface, language, IdentificationVersion, log)
    {
        GamePathParser = new GamePathParser(log);
        _actorData     = new ActorManager.ActorManagerData(pluginInterface, dataManager, language, log);
        _equipment     = new EquipmentIdentificationList(pluginInterface, language, itemData, log);
        _weapons       = new WeaponIdentificationList(pluginInterface, language, itemData, log);
        Actions        = TryCatchData("Actions", () => CreateActionList(dataManager));
        Emotes         = TryCatchData("Emotes",  () => CreateEmoteList(dataManager));

        _modelIdentifierToModelChara = new ModelIdentificationList(pluginInterface, language, dataManager, log);
        BnpcNames                    = TryCatchData("BNpcNames",    NpcNames.CreateNames);
        ModelCharaToObjects          = TryCatchData("ModelObjects", () => CreateModelObjects(_actorData, dataManager, language));
    }

    public string Name(ObjectKind kind, NpcId id)
        => _actorData.ToName(kind, id);

    public void Identify(IDictionary<string, object?> set, string path)
    {
        var extension = Path.GetExtension(path)?.ToLowerInvariant() ?? string.Empty;
        if (extension is ".pap" or ".tmb" or ".scd" or ".avfx")
            if (IdentifyVfx(set, path))
                return;

        var info = GamePathParser.GetFileInfo(path);
        IdentifyParsed(set, info);
    }

    public Dictionary<string, object?> Identify(string path)
    {
        Dictionary<string, object?> ret = new();
        Identify(ret, path);
        return ret;
    }

    public IEnumerable<EquipItem> Identify(SetId setId, WeaponType weaponType, Variant variant, EquipSlot slot)
        => slot switch
        {
            EquipSlot.MainHand => _weapons.Between(setId, weaponType, variant),
            EquipSlot.OffHand  => _weapons.Between(setId, weaponType, variant),
            _                  => _equipment.Between(setId, slot, variant),
        };

    public IReadOnlyList<BnpcNameId> GetBnpcNames(BnpcId bNpcId)
        => bNpcId.Id >= BnpcNames.Count
            ? Array.Empty<BnpcNameId>()
            : new TransformList<uint, BnpcNameId>(BnpcNames[(int)bNpcId.Id], i => (BnpcNameId)i);

    public IReadOnlyList<(string Name, ObjectKind Kind, uint Id)> ModelCharaNames(ModelCharaId modelId)
        => modelId.Id >= ModelCharaToObjects.Count
            ? Array.Empty<(string Name, ObjectKind Kind, uint Id)>()
            : ModelCharaToObjects[(int)modelId.Id];

    public int NumModelChara
        => ModelCharaToObjects.Count;

    protected override void DisposeInternal()
    {
        _actorData.Dispose();
        _weapons.Dispose(PluginInterface, Language);
        _equipment.Dispose(PluginInterface, Language);
        DisposeTag("Actions");
        DisposeTag("Emotes");
        DisposeTag("Models");

        _modelIdentifierToModelChara.Dispose(PluginInterface, Language);
        DisposeTag("BNpcNames");
        DisposeTag("ModelObjects");
    }

    private IReadOnlyDictionary<string, IReadOnlyList<Action>> CreateActionList(IDataManager gameData)
    {
        var sheet   = gameData.GetExcelSheet<Action>(Language)!;
        var storage = new ConcurrentDictionary<string, ConcurrentBag<Action>>();

        void AddAction(string? key, Action action)
        {
            if (key.IsNullOrEmpty())
                return;

            key = key.ToLowerInvariant();
            if (storage.TryGetValue(key, out var actions))
                actions.Add(action);
            else
                storage[key] = new ConcurrentBag<Action> { action };
        }

        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount,
        };

        Parallel.ForEach(sheet.Where(a => !a.Name.RawData.IsEmpty), options, action =>
        {
            var startKey = action.AnimationStart?.Value?.Name?.Value?.Key.ToDalamudString().ToString();
            var endKey   = action.AnimationEnd?.Value?.Key.ToDalamudString().ToString();
            var hitKey   = action.ActionTimelineHit?.Value?.Key.ToDalamudString().ToString();
            AddAction(startKey, action);
            AddAction(endKey,   action);
            AddAction(hitKey,   action);
        });

        return storage.ToDictionary(kvp => kvp.Key, kvp => (IReadOnlyList<Action>)kvp.Value.Distinct().ToArray());
    }

    private IReadOnlyDictionary<string, IReadOnlyList<Emote>> CreateEmoteList(IDataManager gameData)
    {
        var sheet   = gameData.GetExcelSheet<Emote>(Language)!;
        var storage = new ConcurrentDictionary<string, ConcurrentBag<Emote>>();

        void AddEmote(string? key, Emote emote)
        {
            if (key.IsNullOrEmpty())
                return;

            key = key.ToLowerInvariant();
            if (storage.TryGetValue(key, out var emotes))
                emotes.Add(emote);
            else
                storage[key] = new ConcurrentBag<Emote> { emote };
        }

        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount,
        };
        var seenTmbs = new ConcurrentDictionary<string, TmbFile>();

        void ProcessEmote(Emote emote)
        {
            var emoteTmbs = new HashSet<string>(8);
            var tmbs      = new Queue<string>(8);

            foreach (var timeline in emote.ActionTimeline.Where(t => t.Row != 0 && t.Value != null).Select(t => t.Value!))
            {
                var key = timeline.Key.ToDalamudString().TextValue;
                tmbs.Enqueue(GamePaths.Vfx.ActionTmb(key));
                AddEmote(Path.GetFileName(key) + ".pap", emote);
            }

            while (tmbs.TryDequeue(out var tmbPath))
            {
                if (!emoteTmbs.Add(tmbPath))
                    continue;

                AddEmote(Path.GetFileName(tmbPath), emote);

                try
                {
                    var file = gameData.GetFile(tmbPath);
                    if (file != null)
                    {
                        if (!seenTmbs.TryGetValue(tmbPath, out var tmb))
                        {
                            tmb = new TmbFile(file.DataSpan);
                            seenTmbs.TryAdd(tmbPath, tmb);
                        }

                        foreach (var subfile in tmb.Paths)
                        {
                            AddEmote(Path.GetFileName(subfile), emote);
                            if (Path.GetExtension(subfile) == ".tmb")
                                tmbs.Enqueue($"chara/action/{subfile}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Warning($"Unknown Error while creating data:\n{ex}");
                }
            }
        }

        Parallel.ForEach(sheet.Where(n => n.Name.RawData.Length > 0), options, ProcessEmote);

        var sit = sheet.GetRow(50)!;
        AddEmote("s_pose01_loop.pap", sit);
        AddEmote("s_pose02_loop.pap", sit);
        AddEmote("s_pose03_loop.pap", sit);
        AddEmote("s_pose04_loop.pap", sit);
        AddEmote("s_pose05_loop.pap", sit);

        var sitOnGround = sheet.GetRow(52)!;
        AddEmote("j_pose01_loop.pap", sitOnGround);
        AddEmote("j_pose02_loop.pap", sitOnGround);
        AddEmote("j_pose03_loop.pap", sitOnGround);
        AddEmote("j_pose04_loop.pap", sitOnGround);

        var doze = sheet.GetRow(13)!;
        AddEmote("l_pose01_loop.pap", doze);
        AddEmote("l_pose02_loop.pap", doze);
        AddEmote("l_pose03_loop.pap", doze);

        return storage.ToDictionary(kvp => kvp.Key, kvp => (IReadOnlyList<Emote>)kvp.Value.Distinct().ToArray());
    }

    private void FindEquipment(IDictionary<string, object?> set, GameObjectInfo info)
    {
        var items = _equipment.Between(info.PrimaryId, info.EquipSlot, info.Variant);
        foreach (var item in items)
            set[item.Name] = item;
    }

    private void FindWeapon(IDictionary<string, object?> set, GameObjectInfo info)
    {
        var items = _weapons.Between(info.PrimaryId, info.SecondaryId, info.Variant);
        foreach (var item in items)
            set[item.Name] = item;
    }

    private void FindModel(IDictionary<string, object?> set, GameObjectInfo info)
    {
        var type = info.ObjectType.ToModelType();
        if (type is 0 or CharacterBase.ModelType.Weapon)
            return;

        var models = _modelIdentifierToModelChara.Between(type, info.PrimaryId, (byte)info.SecondaryId, info.Variant);
        foreach (var model in models.Where(m => m.RowId != 0 && m.RowId < ModelCharaToObjects.Count))
        {
            var objectList = ModelCharaToObjects[(int)model.RowId];
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
        var key      = GamePathParser.VfxToKey(path);
        var fileName = Path.GetFileName(path);
        var ret      = false;

        if (key.Length > 0 && Actions.TryGetValue(key, out var actions) && actions.Count > 0)
        {
            foreach (var action in actions)
                set[$"Action: {action.Name.ToDalamudString()}"] = action;
            ret = true;
        }

        if (fileName.Length > 0 && Emotes.TryGetValue(fileName, out var emotes) && emotes.Count > 0)
        {
            foreach (var emote in emotes)
                set[$"Emote: {emote.Name.ToDalamudString()}"] = emote;
            ret = true;
        }

        return ret;
    }

    private IReadOnlyList<IReadOnlyList<(string Name, ObjectKind Kind, uint Id)>> CreateModelObjects(ActorManager.ActorManagerData actors,
        IDataManager gameData, ClientLanguage language)
    {
        var modelSheet = gameData.GetExcelSheet<ModelChara>(language)!;
        var ret        = new List<ConcurrentBag<(string Name, ObjectKind Kind, uint Id)>>((int)modelSheet.RowCount);

        for (var i = -1; i < modelSheet.Last().RowId; ++i)
            ret.Add(new ConcurrentBag<(string Name, ObjectKind Kind, uint Id)>());

        void AddChara(int modelChara, ObjectKind kind, uint dataId, uint displayId)
        {
            if (modelChara >= ret.Count)
                return;

            if (actors.TryGetName(kind, dataId, out var name))
                ret[modelChara].Add((name, kind, displayId));
        }

        var oTask = Task.Run(() =>
        {
            foreach (var ornament in gameData.GetExcelSheet<Ornament>(language)!)
                AddChara(ornament.Model, ObjectKind.Ornament, ornament.RowId, ornament.RowId);
        });

        var mTask = Task.Run(() =>
        {
            foreach (var mount in gameData.GetExcelSheet<Mount>(language)!)
                AddChara((int)mount.ModelChara.Row, ObjectKind.MountType, mount.RowId, mount.RowId);
        });

        var cTask = Task.Run(() =>
        {
            foreach (var companion in gameData.GetExcelSheet<Companion>(language)!)
                AddChara((int)companion.Model.Row, ObjectKind.Companion, companion.RowId, companion.RowId);
        });

        var eTask = Task.Run(() =>
        {
            foreach (var eNpc in gameData.GetExcelSheet<ENpcBase>(language)!)
                AddChara((int)eNpc.ModelChara.Row, ObjectKind.EventNpc, eNpc.RowId, eNpc.RowId);
        });

        var options = new ParallelOptions()
        {
            MaxDegreeOfParallelism = Math.Max(1, 1),
            //MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount / 2),
        };

        Parallel.ForEach(gameData.GetExcelSheet<BNpcBase>(language)!.Where(b => b.RowId < BnpcNames.Count), options, bNpc =>
        {
            foreach (var name in BnpcNames[(int)bNpc.RowId])
                AddChara((int)bNpc.ModelChara.Row, ObjectKind.BattleNpc, name, bNpc.RowId);
        });

        Task.WaitAll(oTask, mTask, cTask, eTask);

        return ret.Select(s => !s.IsEmpty
            ? s.ToArray()
            : Array.Empty<(string Name, ObjectKind Kind, uint Id)>()).ToArray();
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
