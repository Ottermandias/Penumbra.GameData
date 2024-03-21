using Dalamud;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using OtterGui.Log;
using Penumbra.GameData.Data;
using Penumbra.GameData.DataContainers.Bases;
using Penumbra.GameData.Structs;
using GameObject = Dalamud.Game.ClientState.Objects.Types.GameObject;

namespace Penumbra.GameData.Interop;

public unsafe class ObjectManager(DalamudPluginInterface pi, Logger log, IFramework framework, IObjectTable objects)
    : DataSharer<Tuple<DateTime[], List<nint>, Dictionary<GameObjectID, nint>>>(pi, log, "Penumbra.ObjectManager",
        ClientLanguage.English, 1,
        () => new Tuple<DateTime[], List<nint>, Dictionary<GameObjectID, nint>>(
            [DateTime.UnixEpoch],
            new List<IntPtr>(objects.Count),
            new Dictionary<GameObjectID, IntPtr>(objects.Count))), IReadOnlyCollection<Actor>
{
    public readonly  IObjectTable Objects  = objects;
    private readonly Actor*       _address = (Actor*)objects.Address;

    public virtual void Update()
    {
        var frame = framework.LastUpdateUTC;
        if (LastFrame == frame)
            return;

        LastFrame = frame;
        InternalIdDict.Clear();
        InternalAvailable.Clear();
        for (var i = 0; i < TotalCount; ++i)
        {
            var actor = this[i];
            if (!actor.Valid)
                continue;

            InternalAvailable.Add(actor);
            var id = actor.AsObject->GetObjectID();
            InternalIdDict.Add(id, actor);
        }
    }

    protected DateTime LastFrame
    {
        get => Value.Item1[0];
        set => Value.Item1[0] = value;
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
