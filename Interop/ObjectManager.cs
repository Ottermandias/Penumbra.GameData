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
using ShareTuple =
    System.Tuple<object?[], bool[], System.Collections.Generic.List<nint>,
        System.Collections.Generic.Dictionary<FFXIVClientStructs.FFXIV.Client.Game.Object.GameObjectId, nint>, int[],
        System.Collections.Concurrent.ConcurrentDictionary<System.Action, byte>,
        System.Collections.Concurrent.ConcurrentDictionary<System.Action, byte>>;

namespace Penumbra.GameData.Interop;

public unsafe class ObjectManager(
    IDalamudPluginInterface pi,
    IGameInteropProvider interop,
    Logger log,
    IFramework framework,
    IObjectTable objects)
    : DataSharer<ShareTuple>(pi, log, "ObjectManager", ClientLanguage.English, 2, () => DefaultShareTuple(objects)), IReadOnlyCollection<Actor>
{
    private static ShareTuple DefaultShareTuple(IObjectTable objects)
        => new([null], [true], new List<nint>(objects.Length), new Dictionary<GameObjectId, nint>(objects.Length), new int[4], [], []);

    public readonly IObjectTable Objects = objects;
    private readonly Actor* _address = (Actor*)Unsafe.AsPointer(ref GameObjectManager.Instance()->Objects.IndexSorted[0]);

    private readonly Logger _log = log;

    private void UpdateHooks()
    {
        if (HookOwner is not null)
            return;

        NeedsUpdate = true;
        HookOwner = this;
        _updateHook?.Dispose();
        _log.Debug("[ObjectManager] Moving object table hook owner to this.");
        _updateHook = interop.HookFromSignature<UpdateObjectArraysDelegate>("40 57 48 83 EC ?? 48 89 5C 24 ?? 33 DB", UpdateObjectArraysDetour);
        _updateHook.Enable();
    }

    private void Update()
    {
        if (!framework.IsInFrameworkUpdateThread)
            return;

        UpdateHooks();
        if (!NeedsUpdate)
            return;

        _log.Verbose("[ObjectManager] Updating object manager.");
        NeedsUpdate = false;

        UpdateAvailable();
        InvokeUpdates();
    }

    private void InvokeUpdates()
    {
        foreach (var ac in Value.Item6.Keys)
        {
            try
            {
                ac.Invoke();
            }
            catch (Exception ex)
            {
                _log.Error($"[ObjectManager] Error during invocation of update subscribers:\n{ex}");
            }
        }
    }

    private void InvokeRequiredUpdates()
    {
        foreach (var ac in Value.Item7.Keys)
        {
            try
            {
                ac.Invoke();
            }
            catch (Exception ex)
            {
                _log.Error($"[ObjectManager] Error during invocation of required update subscribers:\n{ex}");
            }
        }
    }

    private void UpdateAvailable()
    {
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

        return;

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

    public IReadOnlyList<Actor> BattleNpcs
        => new ListSlice(InternalAvailable, 0, BnpcEnd);

    public IReadOnlyList<Actor> CutsceneCharacters
        => new ListSlice(InternalAvailable, BnpcEnd, CutsceneEnd - BnpcEnd);

    public IReadOnlyList<Actor> SpecialCharacters
        => new ListSlice(InternalAvailable, CutsceneEnd, SpecialEnd - CutsceneEnd);

    public IReadOnlyList<Actor> EventNpcs
        => new ListSlice(InternalAvailable, SpecialEnd, EnpcEnd - SpecialEnd);

    public IReadOnlyList<Actor> IslandNpcs
        => new ListSlice(InternalAvailable, EnpcEnd);

    private readonly struct ListSlice : IReadOnlyList<Actor>
    {
        private readonly IReadOnlyList<nint> _list;
        private readonly int _start;
        private readonly int _count;

        public ListSlice(IReadOnlyList<nint> list, int start = 0)
        {
            _list = list;
            _start = start;
            _count = list.Count - start;
            if (_count < 0 || _start < 0)
                throw new IndexOutOfRangeException($"Can not slice list with {_list.Count} elements from {_start}.");
        }

        public ListSlice(IReadOnlyList<nint> list, int start, int count)
        {
            _list = list;
            _start = start;
            _count = count;
            if (_start < 0)
                throw new IndexOutOfRangeException($"Can not slice list with {_list.Count} elements from {_start}.");
            if (_count < 0 || _start + _count > list.Count)
                throw new IndexOutOfRangeException($"Can not slice {_count} elements of list with {_list.Count} elements from {_start}.");
        }

        public IEnumerator<Actor> GetEnumerator()
            => _list.Skip(_start).Take(_count).Select(i => (Actor)i).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public int Count
            => _count;

        public Actor this[int index]
            => _list[index + _start];
    }


    private int BnpcEnd
    {
        get => Value.Item5[0];
        set => Value.Item5[0] = value;
    }

    private int CutsceneEnd
    {
        get => Value.Item5[1];
        set => Value.Item5[1] = value;
    }

    private int SpecialEnd
    {
        get => Value.Item5[2];
        set => Value.Item5[2] = value;
    }

    private int EnpcEnd
    {
        get => Value.Item5[3];
        set => Value.Item5[3] = value;
    }

    private object? HookOwner
    {
        get => Value.Item1[0];
        set => Value.Item1[0] = value;
    }

    private bool NeedsUpdate
    {
        get => Value.Item2[0];
        set => Value.Item2[0] = value;
    }

#pragma warning disable CS8601 // Possible null reference assignment.
    public event Action OnUpdate
    {
        add => Value.Item6.TryAdd(value, 0);
        remove => Value.Item6.Remove(value, out _);
    }

    public event Action OnUpdateRequired
    {
        add => Value.Item7.TryAdd(value, 0);
        remove => Value.Item7.Remove(value, out _);
    }
#pragma warning restore CS8601

    private List<nint> InternalAvailable
    {
        get
        {
            Update();
            return Value.Item3;
        }
    }

    private Dictionary<GameObjectId, nint> InternalIdDict
        => Value.Item4;

    public Actor this[ObjectIndex index]
        => this[(int)index.Index];

    public Actor this[int index]
        => index < 0 || index >= TotalCount ? Actor.Null : _address[index];

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Actor ById(GameObjectId id)
    {
        Update();
        return InternalIdDict.GetValueOrDefault(id, nint.Zero);
    }

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
        => DataUtility.DictionaryMemory(16, Objects.Length)
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
    {
        get
        {
            Update();
            return InternalAvailable.Count;
        }
    }

    protected override void Dispose(bool _)
    {
        base.Dispose(_);
        _updateHook?.Dispose();
        if (HookOwner == this)
            HookOwner = null;
    }

    private delegate void UpdateObjectArraysDelegate(GameObjectManager* manager);

    private Hook<UpdateObjectArraysDelegate>? _updateHook;

    private void UpdateObjectArraysDetour(GameObjectManager* manager)
    {
        _updateHook!.Original(manager);
        NeedsUpdate = true;
        InvokeRequiredUpdates();
    }
}
