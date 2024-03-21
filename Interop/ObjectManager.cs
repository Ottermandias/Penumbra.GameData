using Dalamud;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using OtterGui.Log;
using Penumbra.GameData.Data;
using Penumbra.GameData.DataContainers.Bases;
using Penumbra.GameData.Enums;
using Penumbra.GameData.Structs;
using GameObject = Dalamud.Game.ClientState.Objects.Types.GameObject;

namespace Penumbra.GameData.Interop;

public unsafe class ObjectManager(DalamudPluginInterface pi, Logger log, IFramework framework, IObjectTable objects)
    : DataSharer<Tuple<DateTime[], List<nint>, Dictionary<GameObjectID, nint>, int[]>>(pi, log, "Penumbra.ObjectManager",
        ClientLanguage.English, 1,
        () => new Tuple<DateTime[], List<nint>, Dictionary<GameObjectID, nint>, int[]>(
            [DateTime.UnixEpoch],
            new List<IntPtr>(objects.Count),
            new Dictionary<GameObjectID, IntPtr>(objects.Count), new int[4])), IReadOnlyCollection<Actor>
{
    public readonly  IObjectTable Objects  = objects;
    private readonly Actor*       _address = (Actor*)objects.Address;

    public virtual bool Update()
    {
        var frame = framework.LastUpdateUTC;
        if (LastFrame == frame)
            return false;

        LastFrame = frame;
        InternalIdDict.Clear();
        InternalAvailable.Clear();

        for (var i = 0; i < ObjectIndex.CutsceneStart.Index; ++i)
            AddActor(i);
        BnpcEnd = InternalAvailable.Count;

        for (var i = ObjectIndex.CutsceneStart.Index; i < ObjectIndex.CharacterScreen.Index; ++i)
            AddActor(i);
        CutsceneEnd = InternalAvailable.Count;

        for (var i = ObjectIndex.CharacterScreen.Index; i < (int)ScreenActor.ScreenEnd; ++i)
            AddActor(i);
        SpecialEnd = InternalAvailable.Count;

        for (var i = (int)ScreenActor.ScreenEnd; i < ObjectIndex.IslandStart.Index; ++i)
            AddActor(i);
        EnpcEnd = InternalAvailable.Count;

        for (var i = ObjectIndex.IslandStart.Index; i < TotalCount; ++i)
            AddActor(i);

        return true;

        void AddActor(int index)
        {
            var actor = this[index];
            if (!actor.Valid)
                return;

            InternalAvailable.Add(actor);
            var id = actor.AsObject->GetObjectID();
            InternalIdDict.TryAdd(id, actor);
        }
    }

    public IEnumerable<Actor> BattleNpcs
    {
        get
        {
            for (var i = 0; i < BnpcEnd; ++i)
                yield return InternalAvailable[i];
        }
    }

    public IEnumerable<Actor> CutsceneCharacters
    {
        get
        {
            for (var i = BnpcEnd; i < CutsceneEnd; ++i)
                yield return InternalAvailable[i];
        }
    }

    public IEnumerable<Actor> SpecialCharacters
    {
        get
        {
            for (var i = CutsceneEnd; i < SpecialEnd; ++i)
                yield return InternalAvailable[i];
        }
    }

    public IEnumerable<Actor> EventNpcs
    {
        get
        {
            for (var i = SpecialEnd; i < EnpcEnd; ++i)
                yield return InternalAvailable[i];
        }
    }

    public IEnumerable<Actor> IslandNpcs
    {
        get
        {
            for (var i = EnpcEnd; i < InternalAvailable.Count; ++i)
                yield return InternalAvailable[i];
        }
    }


    protected int BnpcEnd
    {
        get => Value.Item4[0];
        set => Value.Item4[0] = value;
    }

    protected int CutsceneEnd
    {
        get => Value.Item4[1];
        set => Value.Item4[1] = value;
    }

    protected int SpecialEnd
    {
        get => Value.Item4[2];
        set => Value.Item4[2] = value;
    }

    protected int EnpcEnd
    {
        get => Value.Item4[3];
        set => Value.Item4[3] = value;
    }

    protected DateTime LastFrame
    {
        get => Value.Item1[0];
        private set => Value.Item1[0] = value;
    }

    protected List<nint> InternalAvailable
        => Value.Item2;

    protected Dictionary<GameObjectID, nint> InternalIdDict
        => Value.Item3;

    public Actor this[ObjectIndex index]
        => this[(int)index.Index];

    public Actor this[int index]
        => index < 0 || index >= TotalCount ? Actor.Null : _address[index];

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Actor ById(GameObjectID id)
    {
        Update();
        return ByIdWithoutUpdate(id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Actor ByIdWithoutUpdate(GameObjectID id)
        => InternalIdDict.GetValueOrDefault(id, nint.Zero);

    public Actor CompanionParent(Actor companion)
        => this[companion.Index.Index - 1];

    public GameObject? GetDalamudObject(int index)
        => Objects[index];

    public GameObject? GetDalamudObject(ObjectIndex index)
        => Objects[index.Index];

    public Character? GetDalamudCharacter(int index)
        => Objects[index] as Character;

    public Character? GetDalamudCharacter(ObjectIndex index)
        => Objects[index.Index] as Character;

    protected override long ComputeMemory()
        => DataUtility.DictionaryMemory(16,  Objects.Count)
          + DataUtility.DictionaryMemory(16, Objects.Count / 2)
          + DataUtility.ListMemory(8, Objects.Count);

    protected override int ComputeTotalCount()
        => Objects.Count;

    public IEnumerator<Actor> GetEnumerator()
    {
        Update();
        return InternalAvailable.Select(p => (Actor)p).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public int Count
        => InternalAvailable.Count;
}
