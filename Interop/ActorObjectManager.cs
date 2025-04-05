using Dalamud.Game.ClientState.Objects;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Penumbra.GameData.Actors;
using Penumbra.GameData.Enums;
using Penumbra.String;

namespace Penumbra.GameData.Interop;

public unsafe class ActorObjectManager : IDisposable, IReadOnlyDictionary<ActorIdentifier, ActorData>
{
    public readonly  ObjectManager  Objects;
    public readonly  ActorManager   Actors;
    private readonly IClientState   _clientState;
    private readonly ITargetManager _targets;

    public bool IsInGPose
        => _clientState.IsGPosing;

    public bool   IsInLobby { get; private set; } = false;
    public ushort World     { get; private set; }

    private bool _needsIdentifierUpdate = true;

    private readonly Dictionary<ActorIdentifier, ActorData> _identifiers         = new(200);
    private readonly Dictionary<ActorIdentifier, ActorData> _allWorldIdentifiers = new(200);
    private readonly Dictionary<ActorIdentifier, ActorData> _nonOwnedIdentifiers = new(200);

    public ActorObjectManager(ObjectManager objects, ActorManager actors, IClientState clientState, ITargetManager targets)
    {
        Objects                  =  objects;
        Actors                   =  actors;
        _clientState             =  clientState;
        _targets                 =  targets;
        Objects.OnUpdateRequired += OnUpdateRequired;
    }

    public void Dispose()
    {
        Objects.OnUpdateRequired -= OnUpdateRequired;
    }

    public Actor GPosePlayer
        => Objects[(int)ScreenActor.GPosePlayer];

    public Actor Player
        => Objects[0];

    public unsafe Actor Target
        => _clientState.IsGPosing ? TargetSystem.Instance()->GPoseTarget : TargetSystem.Instance()->Target;

    public Actor Focus
        => _targets.FocusTarget?.Address ?? nint.Zero;

    public Actor MouseOver
        => _targets.MouseOverTarget?.Address ?? nint.Zero;

    public (ActorIdentifier Identifier, ActorData Data) PlayerData
    {
        get
        {
            Update();
            return Player.Identifier(Actors, out var ident) && _identifiers.TryGetValue(ident, out var data)
                ? (ident, data)
                : (ident, ActorData.Invalid);
        }
    }

    public (ActorIdentifier Identifier, ActorData Data) TargetData
    {
        get
        {
            Update();
            return Target.Identifier(Actors, out var ident) && _identifiers.TryGetValue(ident, out var data)
                ? (ident, data)
                : (ident, ActorData.Invalid);
        }
    }

    public IEnumerator<KeyValuePair<ActorIdentifier, ActorData>> GetEnumerator()
        => Identifiers.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public int Count
        => Identifiers.Count;

    /// <summary> Also handles All Worlds players and non-owned NPCs. </summary>
    public bool ContainsKey(ActorIdentifier key)
        => Identifiers.ContainsKey(key) || _allWorldIdentifiers.ContainsKey(key) || _nonOwnedIdentifiers.ContainsKey(key);

    public bool TryGetValue(ActorIdentifier key, out ActorData value)
        => Identifiers.TryGetValue(key, out value);

    public bool TryGetValueAllWorld(ActorIdentifier key, out ActorData value)
    {
        Update();
        return _allWorldIdentifiers.TryGetValue(key, out value);
    }

    public bool TryGetValueNonOwned(ActorIdentifier key, out ActorData value)
    {
        Update();
        return _nonOwnedIdentifiers.TryGetValue(key, out value);
    }

    public ActorData this[ActorIdentifier key]
        => Identifiers[key];

    public IEnumerable<ActorIdentifier> Keys
        => Identifiers.Keys;

    public IEnumerable<ActorData> Values
        => Identifiers.Values;

    public bool GetName(string lowerName, out Actor actor)
    {
        (actor, var ret) = lowerName switch
        {
            ""          => (Actor.Null, true),
            "<me>"      => (Player, true),
            "self"      => (Player, true),
            "<t>"       => (Target, true),
            "target"    => (Target, true),
            "<f>"       => (Focus, true),
            "focus"     => (Focus, true),
            "<mo>"      => (MouseOver, true),
            "mouseover" => (MouseOver, true),
            _           => (Actor.Null, false),
        };
        return ret;
    }

    private void Update()
    {
        if (!_needsIdentifierUpdate)
            return;

        _needsIdentifierUpdate = false;

        World = (ushort)(Player.Valid ? Player.HomeWorld : 0);
        _identifiers.Clear();
        _allWorldIdentifiers.Clear();
        _nonOwnedIdentifiers.Clear();

        IsInLobby = AddLobbyCharacters();
        if (IsInLobby)
            return;

        foreach (var actor in Objects.BattleNpcs.Concat(Objects.CutsceneCharacters))
        {
            if (actor.Identifier(Actors, out var identifier))
                HandleIdentifier(identifier, actor);
        }

        AddSpecial(ScreenActor.CharacterScreen, "Character Screen Actor");
        AddSpecial(ScreenActor.ExamineScreen,   "Examine Screen Actor");
        AddSpecial(ScreenActor.FittingRoom,     "Fitting Room Actor");
        AddSpecial(ScreenActor.DyePreview,      "Dye Preview Actor");
        AddSpecial(ScreenActor.Portrait,        "Portrait Actor");
        AddSpecial(ScreenActor.Card6,           "Card Actor 6");
        AddSpecial(ScreenActor.Card7,           "Card Actor 7");
        AddSpecial(ScreenActor.Card8,           "Card Actor 8");

        foreach (var actor in Objects.EventNpcs)
        {
            if (actor.Identifier(Actors, out var identifier))
                HandleIdentifier(identifier, actor);
        }

        return;

        void AddSpecial(ScreenActor idx, string label)
        {
            var actor = Objects[(int)idx];
            if (actor.Identifier(Actors, out var ident))
            {
                var data = new ActorData(actor, label);
                _identifiers.Add(ident, data);
            }
        }
    }

    private void HandleIdentifier(ActorIdentifier identifier, Actor character)
    {
        if (!identifier.IsValid)
            return;

        if (!_identifiers.TryGetValue(identifier, out var data))
        {
            data                     = new ActorData(character, identifier.ToString());
            _identifiers[identifier] = data;
        }
        else
        {
            data.Objects.Add(character);
        }

        if (identifier.Type is IdentifierType.Player or IdentifierType.Owned)
        {
            var allWorld = Actors.CreateIndividualUnchecked(identifier.Type, identifier.PlayerName, ushort.MaxValue,
                identifier.Kind,
                identifier.DataId);

            if (!_allWorldIdentifiers.TryGetValue(allWorld, out var allWorldData))
            {
                allWorldData                   = new ActorData(character, allWorld.ToString());
                _allWorldIdentifiers[allWorld] = allWorldData;
            }
            else
            {
                allWorldData.Objects.Add(character);
            }
        }

        if (identifier.Type is IdentifierType.Owned)
        {
            var nonOwned = Actors.CreateNpc(identifier.Kind, identifier.DataId);
            if (!_nonOwnedIdentifiers.TryGetValue(nonOwned, out var nonOwnedData))
            {
                nonOwnedData                   = new ActorData(character, nonOwned.ToString());
                _nonOwnedIdentifiers[nonOwned] = nonOwnedData;
            }
            else
            {
                nonOwnedData.Objects.Add(character);
            }
        }
    }

    private void OnUpdateRequired()
        => _needsIdentifierUpdate = true;

    private IReadOnlyDictionary<ActorIdentifier, ActorData> Identifiers
    {
        get
        {
            Update();
            return _identifiers;
        }
    }

    private bool AddLobbyCharacters()
    {
        if (_clientState.IsLoggedIn)
            return false;

        var agent = AgentLobby.Instance();
        if (agent == null)
            return false;

        var span = agent->LobbyData.CharaSelectEntries.AsSpan();

        // The lobby uses the first 8 cutscene actors.
        var cnt = 0;
        foreach (var actor in Objects.CutsceneCharacters.Take(8))
        {
            if (!actor.Valid) //shouldn't happen so should be safe to break?
                break;

            if (cnt >= span.Length)
                break;

            if (span[cnt].Value == null) //should mean the end of valid actors so should be safe to break?
                break;

            var chara = span[cnt].Value;
            HandleIdentifier(Actors.CreatePlayer(new ByteString(chara->Name), chara->HomeWorldId), actor);

            cnt++;
        }

        return true;
    }
}
