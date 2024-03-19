using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using OtterGui.Services;
using Penumbra.GameData.Structs;
using GameObject = Dalamud.Game.ClientState.Objects.Types.GameObject;

namespace Penumbra.GameData.Interop;

public unsafe class ObjectManager(IObjectTable objects) : IService
{
    public readonly int Count = objects.Count;

    public readonly  IObjectTable Objects  = objects;
    private readonly Actor*       _address = (Actor*)objects.Address;

    public Actor this[ObjectIndex index]
        => this[(int)index.Index];

    public Actor this[int index]
        => index < 0 || index >= Count ? Actor.Null : _address[index];

    public Actor ById(GameObjectID id)
        => Objects.SearchById(id)?.Address ?? Actor.Null;

    public Actor CompanionParent(Actor companion)
        => this[companion.Index.Index - 1];

    public IEnumerable<Actor> BattleNpcs()
    {
        for (var i = 0; i < ObjectIndex.CutsceneStart.Index; i += 2)
            yield return this[i];
    }

    public GameObject? GetDalamudObject(int index)
        => Objects[index];

    public GameObject? GetDalamudObject(ObjectIndex index)
        => Objects[index.Index];

    public Character? GetDalamudCharacter(int index)
        => Objects[index] as Character;

    public Character? GetDalamudCharacter(ObjectIndex index)
        => Objects[index.Index] as Character;
}
