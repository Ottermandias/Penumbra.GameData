using Dalamud.Game;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Hooking;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using OtterGui.Log;
using Penumbra.GameData.Data;
using Penumbra.GameData.DataContainers.Bases;
using Penumbra.GameData.Enums;
using Penumbra.GameData.Structs;

namespace Penumbra.GameData.Interop;

public unsafe class ObjectManager(
    IDalamudPluginInterface pi,
    IGameInteropProvider interop,
    Logger log,
    IFramework framework,
    IObjectTable objects)
    : DataSharer<Tuple<object?[], bool[], List<nint>, Dictionary<GameObjectId, nint>, int[]>>(pi, log, "ObjectManager",
        ClientLanguage.English, 2,
        () => new Tuple<object?[], bool[], List<nint>, Dictionary<GameObjectId, nint>, int[]>(
            [null],
            [true],
            new List<nint>(objects.Length),
            new Dictionary<GameObjectId, nint>(objects.Length), new int[4])), IReadOnlyCollection<Actor>
{
    public readonly  IObjectTable                      Objects  = objects;
    private readonly Actor*                            _address = (Actor*)Unsafe.AsPointer(ref GameObjectManager.Instance()->Objects.IndexSorted[0]);
    private          Hook<UpdateObjectArraysDelegate>? _updateHook;
    private readonly Logger                            _log = log;

    private void UpdateHooks()
    {
        if (HookOwner is not null)
            return;

        NeedsUpdate = true;
        HookOwner   = this;
        _updateHook?.Dispose();
        _log.Debug("[ObjectManager] Moving object table hook owner to this.");
        _updateHook = interop.HookFromSignature<UpdateObjectArraysDelegate>("40 57 48 83 EC ?? 48 89 5C 24 ?? 33 DB", UpdateObjectArraysDetour);
        _updateHook.Enable();
    }

    private delegate void UpdateObjectArraysDelegate(GameObjectManager* manager);

    private void UpdateObjectArraysDetour(GameObjectManager* manager)
    {
        _updateHook!.Original(manager);
        NeedsUpdate = true;
    }

    public virtual bool Update()
    {
        if (!framework.IsInFrameworkUpdateThread)
            return false;

        UpdateHooks();
        if (!NeedsUpdate)
            return false;

        _log.Verbose("[ObjectManager] Updating object manager.");
        NeedsUpdate = false;
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
            var id = actor.AsObject->GetGameObjectId();
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
        get => Value.Item5[0];
        set => Value.Item5[0] = value;
    }

    protected int CutsceneEnd
    {
        get => Value.Item5[1];
        set => Value.Item5[1] = value;
    }

    protected int SpecialEnd
    {
        get => Value.Item5[2];
        set => Value.Item5[2] = value;
    }

    protected int EnpcEnd
    {
        get => Value.Item5[3];
        set => Value.Item5[3] = value;
    }

    protected object? HookOwner
    {
        get => Value.Item1[0];
        private set => Value.Item1[0] = value;
    }

    protected bool NeedsUpdate
    {
        get => Value.Item2[0];
        private set => Value.Item2[0] = value;
    }

    protected List<nint> InternalAvailable
        => Value.Item3;

    protected Dictionary<GameObjectId, nint> InternalIdDict
        => Value.Item4;

    public Actor this[ObjectIndex index]
        => this[(int)index.Index];

    public Actor this[int index]
        => index < 0 || index >= TotalCount ? Actor.Null : _address[index];

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Actor ById(GameObjectId id)
    {
        Update();
        return ByIdWithoutUpdate(id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Actor ByIdWithoutUpdate(GameObjectId id)
        => InternalIdDict.GetValueOrDefault(id, nint.Zero);

    public Actor CompanionParent(Actor companion)
        => this[companion.Index.Index - 1];

    public IGameObject? GetDalamudObject(int index)
        => Objects[index];

    public IGameObject? GetDalamudObject(ObjectIndex index)
        => Objects[index.Index];

    public ICharacter? GetDalamudCharacter(int index)
        => Objects[index] as ICharacter;

    public ICharacter? GetDalamudCharacter(ObjectIndex index)
        => Objects[index.Index] as ICharacter;

    protected override long ComputeMemory()
        => DataUtility.DictionaryMemory(16,  Objects.Length)
          + DataUtility.DictionaryMemory(16, Objects.Length / 2)
          + DataUtility.ListMemory(8, Objects.Length);

    protected override int ComputeTotalCount()
        => Objects.Length;

    public IEnumerator<Actor> GetEnumerator()
    {
        Update();
        return InternalAvailable.Select(p => (Actor)p).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public int Count
        => InternalAvailable.Count;

    protected override void Dispose(bool _)
    {
        base.Dispose(_);
        _updateHook?.Dispose();
        HookOwner = null;
    }
}
