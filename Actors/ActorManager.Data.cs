using System.Collections.Frozen;
using Dalamud;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.GeneratedSheets;
using Lumina.Text;
using Penumbra.GameData.Data;
using Penumbra.GameData.Structs;
using Penumbra.String;
using Character = FFXIVClientStructs.FFXIV.Client.Game.Character.Character;
using ObjectKind = Dalamud.Game.ClientState.Objects.Enums.ObjectKind;
using Task = System.Threading.Tasks.Task;

namespace Penumbra.GameData.Actors;

public sealed partial class ActorManager : IDisposable
{
    public sealed class ActorManagerData : DataSharer
    {
        /// <summary> Worlds available for players. </summary>
        public IReadOnlyDictionary<ushort, string> Worlds { get; }

        /// <summary> Valid Mount names in title case by mount id. </summary>
        public IReadOnlyDictionary<uint, string> Mounts { get; }

        /// <summary> Valid Companion names in title case by companion id. </summary>
        public IReadOnlyDictionary<uint, string> Companions { get; }

        /// <summary> Valid ornament names by id. </summary>
        public IReadOnlyDictionary<uint, string> Ornaments { get; }

        /// <summary> Valid BNPC names in title case by BNPC Name id. </summary>
        public IReadOnlyDictionary<uint, string> BNpcs { get; }

        /// <summary> Valid ENPC names in title case by ENPC id. </summary>
        public IReadOnlyDictionary<uint, string> ENpcs { get; }

        public ActorManagerData(DalamudPluginInterface pluginInterface, IDataManager gameData, ClientLanguage language, IPluginLog log)
            : base(pluginInterface, language, 5, log)
        {
            log.Debug("[ActorData] Creating ActorManagerData started.");

            var worldTask      = TryCatchData("Worlds",     () => CreateWorldData(gameData));
            var mountsTask     = TryCatchData("Mounts",     () => CreateMountData(gameData));
            var companionsTask = TryCatchData("Companions", () => CreateCompanionData(gameData));
            var ornamentsTask  = TryCatchData("Ornaments",  () => CreateOrnamentData(gameData));
            var bNpcsTask      = TryCatchData("BNpcs",      () => CreateBNpcData(gameData));
            var eNpcsTask      = TryCatchData("ENpcs",      () => CreateENpcData(gameData));

            Worlds     = worldTask.Result;
            Mounts     = mountsTask.Result;
            Companions = companionsTask.Result;
            Ornaments  = ornamentsTask.Result;
            BNpcs      = bNpcsTask.Result;
            ENpcs      = eNpcsTask.Result;
            log.Debug("[ActorData] Creating ActorManagerData finished.");
        }

        /// <summary>
        /// Return the world name including the Any World option.
        /// </summary>
        public string ToWorldName(WorldId worldId)
            => worldId == WorldId.AnyWorld ? "Any World" : Worlds.TryGetValue(worldId.Id, out var name) ? name : "Invalid";

        /// <summary>
        /// Return the world id corresponding to the given name.
        /// </summary>
        /// <returns>ushort.MaxValue if the name is empty, 0 if it is not a valid world, or the worlds id.</returns>
        public WorldId ToWorldId(string worldName)
            => worldName.Length != 0
                ? (WorldId)Worlds.FirstOrDefault(kvp => string.Equals(kvp.Value, worldName, StringComparison.OrdinalIgnoreCase), default).Key
                : WorldId.AnyWorld;

        /// <summary>
        /// Convert a given ID for a certain ObjectKind to a name.
        /// </summary>
        /// <returns>Invalid or a valid name.</returns>
        public string ToName(ObjectKind kind, NpcId dataId)
            => TryGetName(kind, dataId, out var ret) ? ret : "Invalid";


        /// <summary>
        /// Convert a given ID for a certain ObjectKind to a name.
        /// </summary>
        public bool TryGetName(ObjectKind kind, NpcId dataId, [NotNullWhen(true)] out string? name)
        {
            name = null;
            return kind switch
            {
                ObjectKind.MountType => Mounts.TryGetValue(dataId.Id, out name),
                ObjectKind.Companion => Companions.TryGetValue(dataId.Id, out name),
                ObjectKind.Ornament  => Ornaments.TryGetValue(dataId.Id, out name),
                ObjectKind.BattleNpc => BNpcs.TryGetValue(dataId.Id, out name),
                ObjectKind.EventNpc  => ENpcs.TryGetValue(dataId.Id, out name),
                _                    => false,
            };
        }

        protected override void DisposeInternal()
        {
            DisposeTag("Worlds");
            DisposeTag("Mounts");
            DisposeTag("Companions");
            DisposeTag("Ornaments");
            DisposeTag("BNpcs");
            DisposeTag("ENpcs");
        }

        private Task<IReadOnlyDictionary<ushort, string>> CreateWorldData(IDataManager gameData)
            => Task.Run(() =>
            {
                var dict = new Dictionary<ushort, string>();
                Log.Debug("[ActorData] Collecting World data...");
                foreach (var w in gameData.GetExcelSheet<World>(Language)!.Where(w => w.IsPublic && !w.Name.RawData.IsEmpty))
                    dict.TryAdd((ushort)w.RowId, string.Intern(w.Name.ToDalamudString().TextValue));
                Log.Debug($"[ActorData] Collected {dict.Count} Worlds.");
                return (IReadOnlyDictionary<ushort, string>)dict.ToFrozenDictionary();
            });

        private Task<IReadOnlyDictionary<uint, string>> CreateMountData(IDataManager gameData)
            => Task.Run(() =>
            {
                var dict = new Dictionary<uint, string>();
                Log.Debug("[ActorData] Collecting Mount data...");
                dict.TryAdd(119, "Falcon (Porter)");
                dict.TryAdd(295, "Hippo Cart (Quest)");
                dict.TryAdd(296, "Hippo Cart (Quest)");
                dict.TryAdd(298, "Miw Miisv (Quest)");
                dict.TryAdd(309, "Moon-hopper (Quest)");
                foreach (var m in gameData.GetExcelSheet<Mount>(Language)!)
                {
                    if (m.Singular.RawData.Length > 0 && m.Order >= 0)
                    {
                        dict.TryAdd(m.RowId, ToTitleCaseExtended(m.Singular, m.Article));
                    }
                    else if (m.Unknown18.RawData.Length > 0)
                    {
                        var whistle = m.Unknown18.ToDalamudString().ToString();
                        whistle = whistle.Replace("SE_Bt_Etc_", string.Empty)
                            .Replace("Mount_",  string.Empty)
                            .Replace("_call",   string.Empty)
                            .Replace("Whistle", string.Empty);
                        dict.TryAdd(m.RowId, $"? {whistle} #{m.RowId}");
                    }
                }

                Log.Debug($"[ActorData] Collected {dict.Count} Mounts.");
                return (IReadOnlyDictionary<uint, string>)dict.ToFrozenDictionary();
            });

        private Task<IReadOnlyDictionary<uint, string>> CreateCompanionData(IDataManager gameData)
            => Task.Run(() =>
            {
                var dict = new Dictionary<uint, string>();
                Log.Debug("[ActorData] Collecting Companion data...");
                foreach (var c in gameData.GetExcelSheet<Companion>(Language)!.Where(c
                             => c.Singular.RawData.Length > 0 && c.Order < ushort.MaxValue))
                    dict.TryAdd(c.RowId, ToTitleCaseExtended(c.Singular, c.Article));
                Log.Debug($"[ActorData] Collected {dict.Count} Companions.");
                return (IReadOnlyDictionary<uint, string>)dict.ToFrozenDictionary();
            });

        private Task<IReadOnlyDictionary<uint, string>> CreateOrnamentData(IDataManager gameData)
            => Task.Run(() =>
            {
                var dict = new Dictionary<uint, string>();
                Log.Debug("[ActorData] Collecting Ornament data...");
                foreach (var o in gameData.GetExcelSheet<Ornament>(Language)!.Where(o => o.Singular.RawData.Length > 0))
                    dict.TryAdd(o.RowId, ToTitleCaseExtended(o.Singular, o.Article));
                Log.Debug($"[ActorData] Collected {dict.Count} Ornaments.");
                return (IReadOnlyDictionary<uint, string>)dict.ToFrozenDictionary();
            });

        private Task<IReadOnlyDictionary<uint, string>> CreateBNpcData(IDataManager gameData)
            => Task.Run(() =>
            {
                var dict = new Dictionary<uint, string>();
                Log.Debug("[ActorData] Collecting BNPC data...");
                foreach (var n in gameData.GetExcelSheet<BNpcName>(Language)!.Where(n => n.Singular.RawData.Length > 0))
                    dict.TryAdd(n.RowId, ToTitleCaseExtended(n.Singular, n.Article));
                Log.Debug($"[ActorData] Collected {dict.Count} BNPCs.");
                return (IReadOnlyDictionary<uint, string>)dict.ToFrozenDictionary();
            });

        private Task<IReadOnlyDictionary<uint, string>> CreateENpcData(IDataManager gameData)
            => Task.Run(() =>
            {
                var dict = new Dictionary<uint, string>();
                Log.Debug("[ActorData] Collecting ENPC data...");
                foreach (var n in gameData.GetExcelSheet<ENpcResident>(Language)!.Where(e => e.Singular.RawData.Length > 0))
                    dict.TryAdd(n.RowId, ToTitleCaseExtended(n.Singular, n.Article));
                Log.Debug($"[ActorData] Collected {dict.Count} ENPCs.");
                return (IReadOnlyDictionary<uint, string>)dict.ToFrozenDictionary();
            });

        private static string ToTitleCaseExtended(SeString s, sbyte article)
        {
            if (article == 1)
                return string.Intern(s.ToDalamudString().ToString());

            var sb        = new StringBuilder(s.ToDalamudString().ToString());
            var lastSpace = true;
            for (var i = 0; i < sb.Length; ++i)
            {
                if (sb[i] == ' ')
                {
                    lastSpace = true;
                }
                else if (lastSpace)
                {
                    lastSpace = false;
                    sb[i]     = char.ToUpperInvariant(sb[i]);
                }
            }

            return string.Intern(sb.ToString());
        }
    }

    public readonly ActorManagerData Data;

    public ActorManager(DalamudPluginInterface pluginInterface, IObjectTable objects, IClientState state, IFramework framework,
        IGameInteropProvider interop, IDataManager gameData, IGameGui gameGui, Func<ushort, short> toParentIdx, IPluginLog log)
        : this(pluginInterface, objects, state, framework, interop, gameData, gameGui, gameData.Language, toParentIdx, log)
    { }

    public ActorManager(DalamudPluginInterface pluginInterface, IObjectTable objects, IClientState state, IFramework framework,
        IGameInteropProvider interop, IDataManager gameData, IGameGui gameGui, ClientLanguage language, Func<ushort, short> toParentIdx,
        IPluginLog log)
    {
        _framework   = framework;
        _objects     = objects;
        _gameGui     = gameGui;
        _clientState = state;
        _toParentIdx = toParentIdx;
        Data         = new ActorManagerData(pluginInterface, gameData, language, log);

        ActorIdentifier.Manager = this;

        interop.InitializeFromAttributes(this);
    }

    public unsafe ActorIdentifier GetCurrentPlayer()
    {
        var address = (Character*)_objects.GetObjectAddress(0);
        return address == null
            ? ActorIdentifier.Invalid
            : CreateIndividualUnchecked(IdentifierType.Player, new ByteString(address->GameObject.Name), address->HomeWorld,
                ObjectKind.None,                               uint.MaxValue);
    }

    public ActorIdentifier GetInspectPlayer()
    {
        var addon = _gameGui.GetAddonByName("CharacterInspect");
        return addon == IntPtr.Zero 
            ? ActorIdentifier.Invalid 
            : CreatePlayer(InspectName, InspectWorldId);
    }

    public unsafe bool ResolvePartyBannerPlayer(ScreenActor type, out ActorIdentifier id)
    {
        id = ActorIdentifier.Invalid;
        var module = Framework.Instance()->GetUiModule()->GetAgentModule();
        if (module == null)
            return false;

        var agent = (AgentBannerInterface*)module->GetAgentByInternalId(AgentId.BannerParty);
        if (agent == null || !agent->AgentInterface.IsAgentActive())
            agent = (AgentBannerInterface*)module->GetAgentByInternalId(AgentId.BannerMIP);
        if (agent == null || !agent->AgentInterface.IsAgentActive())
            return false;

        var idx = (ushort)type - (ushort)ScreenActor.CharacterScreen;
        if (agent->Data == null)
            return true;

        ref var character = ref agent->Data->CharacterArraySpan[idx];

        var name = new ByteString(character.Name1.StringPtr);
        id = CreatePlayer(name, (WorldId)character.WorldId);
        return true;
    }

    private unsafe bool SearchPlayerCustomize(Character* character, ObjectIndex idx, out ActorIdentifier id)
    {
        var other = (Character*)_objects.GetObjectAddress(idx.Index);
        if (other == null
         || !CustomizeData.ScreenActorEquals((CustomizeData*)character->DrawData.CustomizeData.Data,
                (CustomizeData*)other->DrawData.CustomizeData.Data))
        {
            id = ActorIdentifier.Invalid;
            return false;
        }

        id = FromObject(&other->GameObject, out _, false, true, false);
        return true;
    }

    private unsafe ActorIdentifier SearchPlayersCustomize(Character* gameObject, ObjectIndex idx1, ObjectIndex idx2, ObjectIndex idx3)
        => SearchPlayerCustomize(gameObject,  idx1, out var ret)
         || SearchPlayerCustomize(gameObject, idx2, out ret)
         || SearchPlayerCustomize(gameObject, idx3, out ret)
                ? ret
                : ActorIdentifier.Invalid;

    private unsafe ActorIdentifier SearchPlayersCustomize(Character* gameObject)
    {
        for (var i = 0; i < ObjectIndex.CutsceneStart.Index; i += 2)
        {
            var obj = (GameObject*)_objects.GetObjectAddress(i);
            if (obj != null
             && obj->ObjectKind is (byte)ObjectKind.Player
             && Compare(gameObject, (Character*)obj))
                return FromObject(obj, out _, false, true, false);
        }

        return ActorIdentifier.Invalid;

        static bool Compare(Character* a, Character* b)
        {
            var data1  = (CustomizeData*)a->DrawData.CustomizeData.Data;
            var data2  = (CustomizeData*)b->DrawData.CustomizeData.Data;
            var equals = CustomizeData.ScreenActorEquals(data1, data2);
            return equals;
        }
    }

    public unsafe bool ResolveMahjongPlayer(ScreenActor type, out ActorIdentifier id)
    {
        id = ActorIdentifier.Invalid;
        if (_clientState.TerritoryType != 831 && _gameGui.GetAddonByName("EmjIntro") == IntPtr.Zero)
            return false;

        var obj = (Character*)_objects.GetObjectAddress((int)type);
        if (obj == null)
            return false;

        id = type switch
        {
            ScreenActor.CharacterScreen => GetCurrentPlayer(),
            ScreenActor.ExamineScreen   => SearchPlayersCustomize(obj, 2, 4, 6),
            ScreenActor.FittingRoom     => SearchPlayersCustomize(obj, 4, 2, 6),
            ScreenActor.DyePreview      => SearchPlayersCustomize(obj, 6, 2, 4),
            _                           => ActorIdentifier.Invalid,
        };
        return true;
    }

    public unsafe bool ResolvePvPBannerPlayer(ScreenActor type, out ActorIdentifier id)
    {
        id = ActorIdentifier.Invalid;
        if (!_clientState.IsPvPExcludingDen)
            return false;

        var addon = (AtkUnitBase*)_gameGui.GetAddonByName("PvPMap");
        if (addon == null || addon->IsVisible)
            return false;

        var obj = (Character*)_objects.GetObjectAddress((int)type);
        if (obj == null)
            return false;

        id = type switch
        {
            ScreenActor.CharacterScreen => SearchPlayersCustomize(obj),
            ScreenActor.ExamineScreen   => SearchPlayersCustomize(obj),
            ScreenActor.FittingRoom     => SearchPlayersCustomize(obj),
            ScreenActor.DyePreview      => SearchPlayersCustomize(obj),
            ScreenActor.Portrait        => SearchPlayersCustomize(obj),
            _                           => ActorIdentifier.Invalid,
        };
        return true;
    }

    public unsafe ActorIdentifier GetCardPlayer()
    {
        var agent = AgentCharaCard.Instance();
        if (agent == null || agent->Data == null)
            return ActorIdentifier.Invalid;

        var worldId = *(ushort*)((byte*)agent->Data + Offsets.AgentCharaCardDataWorldId);
        return CreatePlayer(new ByteString(agent->Data->Name.StringPtr), worldId);
    }

    public ActorIdentifier GetGlamourPlayer()
    {
        var addon = _gameGui.GetAddonByName("MiragePrismMiragePlate");
        return addon == IntPtr.Zero ? ActorIdentifier.Invalid : GetCurrentPlayer();
    }

    public void Dispose()
    {
        Data.Dispose();
        GC.SuppressFinalize(this);
        if (ActorIdentifier.Manager == this)
            ActorIdentifier.Manager = null;
    }

    ~ActorManager()
        => Dispose();

    private readonly IFramework   _framework;
    private readonly IObjectTable _objects;
    private readonly IClientState _clientState;
    private readonly IGameGui     _gameGui;

    private readonly Func<ushort, short> _toParentIdx;

    public short ToCutsceneParent(ushort index)
        => _toParentIdx(index);

    private static unsafe ByteString InspectName
        => new(UIState.Instance()->Inspect.Name);

    private static unsafe ushort InspectWorldId
        => (ushort) UIState.Instance()->Inspect.WorldId;

    public static readonly IReadOnlySet<EnpcId> MannequinIds = new EnpcId[]
    {
        1026228u,
        1026229u,
        1026986u,
        1026987u,
        1026988u,
        1026989u,
        1032291u,
        1032292u,
        1032293u,
        1032294u,
        1033046u,
        1033047u,
        1033658u,
        1033659u,
        1007137u,
        // TODO: Female Hrothgar
    }.ToFrozenSet();
}
