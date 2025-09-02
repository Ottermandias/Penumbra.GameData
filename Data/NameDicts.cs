using Dalamud.Game.ClientState.Objects.Enums;
using Luna;
using Penumbra.GameData.DataContainers;
using Penumbra.GameData.Structs;

namespace Penumbra.GameData.Data;

/// <summary> A collection service for all the name dictionaries required for actor identification. </summary>
public sealed class NameDicts(
    DictWorld worlds,
    DictMount mounts,
    DictCompanion companions,
    DictOrnament ornaments,
    DictBNpc bNpcs,
    DictENpc eNpcs)
    : IAsyncService
{
    /// <summary> Worlds available for players. </summary>
    public readonly DictWorld Worlds = worlds;

    /// <summary> Valid Mount names in title case by mount id. </summary>
    public readonly DictMount Mounts = mounts;

    /// <summary> Valid Companion names in title case by companion id. </summary>
    public readonly DictCompanion Companions = companions;

    /// <summary> Valid ornament names by id. </summary>
    public readonly DictOrnament Ornaments = ornaments;

    /// <summary> Valid BNPC names in title case by BNPC Name id. </summary>
    public readonly DictBNpc BNpcs = bNpcs;

    /// <summary> Valid ENPC names in title case by ENPC id. </summary>
    public readonly DictENpc ENpcs = eNpcs;

    /// <summary> Finished when all name dictionaries are finished. </summary>
    public Task Awaiter { get; } =
        Task.WhenAll(worlds.Awaiter, mounts.Awaiter, companions.Awaiter, ornaments.Awaiter, bNpcs.Awaiter, eNpcs.Awaiter);

    /// <inheritdoc/>
    public bool Finished
        => Awaiter.IsCompletedSuccessfully;

    /// <summary> Return the world name including the Any World option. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public string ToWorldName(WorldId worldId)
        => worldId == WorldId.AnyWorld ? "Any World" : Worlds.GetValueOrDefault(worldId, "Invalid");

    /// <summary> Return the world id corresponding to the given name. </summary>
    /// <returns> ushort.MaxValue if the name is empty, 0 if it is not a valid world, or the worlds' id. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public WorldId ToWorldId(string worldName)
        => worldName.Length != 0
            ? Worlds.FirstOrDefault(kvp => string.Equals(kvp.Value, worldName, StringComparison.OrdinalIgnoreCase), default).Key
            : WorldId.AnyWorld;

    /// <summary> Convert a given ID for a certain ObjectKind to a name. </summary>
    /// <returns> Invalid or a valid name. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public string ToName(ObjectKind kind, NpcId dataId)
        => TryGetName(kind, dataId, out var ret) ? ret : "Invalid";

    /// <summary> Convert a given ID for a certain ObjectKind to a name. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryGetName(ObjectKind kind, NpcId dataId, [NotNullWhen(true)] out string? name)
    {
        name = null;
        return kind switch
        {
            ObjectKind.MountType => Mounts.TryGetValue(dataId.MountId, out name),
            ObjectKind.Companion => Companions.TryGetValue(dataId.CompanionId, out name),
            ObjectKind.Ornament  => Ornaments.TryGetValue(dataId.OrnamentId, out name),
            ObjectKind.BattleNpc => BNpcs.TryGetValue(dataId.BNpcNameId, out name),
            ObjectKind.EventNpc  => ENpcs.TryGetValue(dataId.ENpcId, out name),
            _                    => false,
        };
    }
}
