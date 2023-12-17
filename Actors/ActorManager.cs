using Dalamud.Plugin.Services;
using Penumbra.GameData.Data;
using Penumbra.GameData.DataContainers.Bases;
using Penumbra.GameData.Enums;

namespace Penumbra.GameData.Actors;

public delegate short CutsceneResolver(ushort index);

public sealed class ActorManager : ActorIdentifierFactory, IDisposable, IAsyncService
{
    public readonly  NameDicts     Data;
    private readonly ActorResolver _resolver;
    private readonly IClientState  _clientState;

    public Task Awaiter
        => Data.Awaiter;

    public void Dispose()
    {
        if (ActorIdentifierExtensions.Manager == this)
            ActorIdentifierExtensions.Manager = null;
    }

    public ActorManager(NameDicts data,
        IFramework framework,
        IObjectTable objects,
        IClientState clientState,
        IGameGui gameGui,
        CutsceneResolver toParentIdx)
        : base(objects, framework, data, toParentIdx)
    {
        Data                              =   data;
        _clientState                      =   clientState;
        _resolver                         =   new ActorResolver(gameGui, objects, clientState);
        ActorIdentifierExtensions.Manager ??= this;
    }

    public ActorIdentifier GetCurrentPlayer()
        => _resolver.GetCurrentPlayer(this);

    public ActorIdentifier GetInspectPlayer()
        => _resolver.GetInspectPlayer(this);

    public bool ResolvePartyBannerPlayer(ScreenActor type, out ActorIdentifier id)
        => _resolver.ResolvePartyBannerPlayer(this, type, out id);

    public bool ResolveMahjongPlayer(ScreenActor type, out ActorIdentifier id)
        => _resolver.ResolveMahjongPlayer(this, type, out id);

    public bool ResolvePvPBannerPlayer(ScreenActor type, out ActorIdentifier id)
        => _resolver.ResolvePvPBannerPlayer(this, type, out id);

    public ActorIdentifier GetCardPlayer()
        => _resolver.GetCardPlayer(this);

    public ActorIdentifier GetGlamourPlayer()
        => _resolver.GetGlamourPlayer(this);

    /// <summary>
    /// Use stored data to convert an ActorIdentifier to a string.
    /// </summary>
    public string ToString(ActorIdentifier id)
    {
        return id.Type switch
        {
            IdentifierType.Player => id.HomeWorld.Id != _clientState.LocalPlayer?.HomeWorld.Id
                ? $"{id.PlayerName} ({Data.ToWorldName(id.HomeWorld)})"
                : id.PlayerName.ToString(),
            IdentifierType.Retainer => $"{id.PlayerName}{id.Retainer switch
            {
                ActorIdentifier.RetainerType.Bell      => " (Bell)",
                ActorIdentifier.RetainerType.Mannequin => " (Mannequin)",
                _                                      => " (Retainer)",
            }}",
            IdentifierType.Owned => id.HomeWorld.Id != _clientState.LocalPlayer?.HomeWorld.Id
                ? $"{id.PlayerName} ({Data.ToWorldName(id.HomeWorld)})'s {Data.ToName(id.Kind, id.DataId)}"
                : $"{id.PlayerName}s {Data.ToName(id.Kind,                                     id.DataId)}",
            IdentifierType.Special => ((ScreenActor)id.Index.Index).ToName(),
            IdentifierType.Npc =>
                id.Index == ushort.MaxValue
                    ? Data.ToName(id.Kind, id.DataId)
                    : $"{Data.ToName(id.Kind, id.DataId)} at {id.Index}",
            IdentifierType.UnkObject => id.PlayerName.IsEmpty
                ? $"Unknown Object at {id.Index}"
                : $"{id.PlayerName} at {id.Index}",
            _ => "Invalid",
        };
    }

    /// <summary>
    /// Use stored data to convert an ActorIdentifier to a name only.
    /// </summary>
    public string ToName(ActorIdentifier id)
    {
        return id.Type switch
        {
            IdentifierType.Player    => id.PlayerName.ToString(),
            IdentifierType.Retainer  => id.PlayerName.ToString(),
            IdentifierType.Owned     => $"{id.PlayerName}s {Data.ToName(id.Kind, id.DataId)}",
            IdentifierType.Special   => ((ScreenActor)id.Index.Index).ToName(),
            IdentifierType.Npc       => Data.ToName(id.Kind, id.DataId),
            IdentifierType.UnkObject => id.PlayerName.IsEmpty ? id.PlayerName.ToString() : "Unknown Object",
            _                        => "Invalid",
        };
    }
}
