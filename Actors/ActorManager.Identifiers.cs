using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Newtonsoft.Json.Linq;
using Penumbra.GameData.Structs;
using Penumbra.String;
using Character = FFXIVClientStructs.FFXIV.Client.Game.Character.Character;

namespace Penumbra.GameData.Actors;

public partial class ActorManager
{
    /// <summary>
    /// Try to create an ActorIdentifier from a already parsed JObject <paramref name="data"/>.
    /// </summary>
    /// <param name="data">A parsed JObject</param>
    /// <returns>ActorIdentifier.Invalid if the JObject can not be converted, a valid ActorIdentifier otherwise.</returns>
    public ActorIdentifier FromJson(JObject? data)
    {
        if (data == null)
            return ActorIdentifier.Invalid;

        var type = data[nameof(ActorIdentifier.Type)]?.ToObject<IdentifierType>() ?? IdentifierType.Invalid;
        switch (type)
        {
            case IdentifierType.Player:
            {
                var name      = ByteString.FromStringUnsafe(data[nameof(ActorIdentifier.PlayerName)]?.ToObject<string>(), false);
                var homeWorld = data[nameof(ActorIdentifier.HomeWorld)]?.ToObject<ushort>() ?? 0;
                return CreatePlayer(name, homeWorld);
            }
            case IdentifierType.Retainer:
            {
                var name = ByteString.FromStringUnsafe(data[nameof(ActorIdentifier.PlayerName)]?.ToObject<string>(), false);
                var retainerType = data[nameof(ActorIdentifier.Retainer)]?.ToObject<ActorIdentifier.RetainerType>()
                 ?? ActorIdentifier.RetainerType.Both;
                return CreateRetainer(name, retainerType);
            }
            case IdentifierType.Owned:
            {
                var name      = ByteString.FromStringUnsafe(data[nameof(ActorIdentifier.PlayerName)]?.ToObject<string>(), false);
                var homeWorld = data[nameof(ActorIdentifier.HomeWorld)]?.ToObject<ushort>() ?? 0;
                var kind      = data[nameof(ActorIdentifier.Kind)]?.ToObject<ObjectKind>() ?? ObjectKind.CardStand;
                var dataId    = data[nameof(ActorIdentifier.DataId)]?.ToObject<uint>() ?? 0;
                return CreateOwned(name, homeWorld, kind, dataId);
            }
            case IdentifierType.Special:
            {
                var special = data["Special"]?.ToObject<ScreenActor>() ?? 0;
                return CreateSpecial(special);
            }
            case IdentifierType.Npc:
            {
                var index  = data[nameof(ActorIdentifier.Index)]?.ToObject<ushort>() ?? ushort.MaxValue;
                var kind   = data[nameof(ActorIdentifier.Kind)]?.ToObject<ObjectKind>() ?? ObjectKind.CardStand;
                var dataId = data[nameof(ActorIdentifier.DataId)]?.ToObject<uint>() ?? 0;
                return CreateNpc(kind, dataId, index);
            }
            case IdentifierType.UnkObject:
            {
                var index = data[nameof(ActorIdentifier.Index)]?.ToObject<ushort>() ?? ushort.MaxValue;
                var name  = ByteString.FromStringUnsafe(data[nameof(ActorIdentifier.PlayerName)]?.ToObject<string>(), false);
                return CreateIndividualUnchecked(IdentifierType.UnkObject, name, index, ObjectKind.None, 0);
            }
            default: return ActorIdentifier.Invalid;
        }
    }

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

    private unsafe FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject* HandleCutscene(
        FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject* main)
    {
        if (main == null)
            return null;

        if (main->ObjectIndex is >= (ushort)ScreenActor.CutsceneStart and < (ushort)ScreenActor.CutsceneEnd)
        {
            var parentIdx = _toParentIdx(main->ObjectIndex);
            if (parentIdx >= 0)
                return (FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject*)_objects.GetObjectAddress(parentIdx);
        }

        return main;
    }

    public class IdentifierParseError : Exception
    {
        public IdentifierParseError(string reason)
            : base(reason)
        { }
    }

    public ActorIdentifier[] FromUserString(string userString, bool allowIndex)
    {
        if (userString.Length == 0)
            throw new IdentifierParseError("The identifier string was empty.");

        var split = userString.Split('|', 3, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (split.Length < 2)
            throw new IdentifierParseError($"The identifier string {userString} does not contain a type and a value.");

        (ByteString, WorldId) ParsePlayer(string player)
        {
            var parts = player.Split('@', 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (!VerifyPlayerName(parts[0]))
                throw new IdentifierParseError($"{parts[0]} is not a valid player name.");
            if (!ByteString.FromString(parts[0], out var p))
                throw new IdentifierParseError($"The player string {parts[0]} contains invalid symbols.");

            var world = parts.Length == 2
                ? Data.ToWorldId(parts[1])
                : ushort.MaxValue;

            if (!VerifyWorld(world))
                throw new IdentifierParseError($"{parts[1]} is not a valid world name.");

            return (p, world);
        }

        (ObjectKind, uint[], WorldId) ParseNpc(string npc)
        {
            var indexString = allowIndex ? "@<Index>" : string.Empty;
            var split2      = npc.Split(':', 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (split2.Length != 2)
                throw new IdentifierParseError($"NPCs need to be specified by '[Object Type]:[NPC Name]{indexString}'.");

            var split3 = allowIndex
                ? split2[1].Split('@', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                : [ split2[1], ];

            WorldId GetIndex()
            {
                var idx = WorldId.AnyWorld;
                if (split3.Length == 2)
                {
                    if (ushort.TryParse(split3[1], out var intIdx) && intIdx < _objects.Length)
                        idx = (WorldId)intIdx;
                    else
                        throw new IdentifierParseError($"Could not parse index {split3[1]} to valid Index.");
                }

                return idx;
            }

            static bool FindDataId(string name, IReadOnlyDictionary<uint, string> data, out uint[] dataIds)
            {
                dataIds = data.Where(kvp => kvp.Value.Equals(name, StringComparison.OrdinalIgnoreCase)).Select(kvp => kvp.Key).ToArray();
                return dataIds.Length > 0;
            }

            switch (split2[0].ToLowerInvariant())
            {
                case "m":
                case "mount":
                    return FindDataId(split3[0], Data.Mounts, out var id)
                        ? (ObjectKind.MountType, id, GetIndex())
                        : throw new IdentifierParseError($"Could not identify a Mount named {split2[1]}.");
                case "c":
                case "companion":
                case "minion":
                case "mini":
                    return FindDataId(split3[0], Data.Companions, out id)
                        ? (ObjectKind.Companion, id, GetIndex())
                        : throw new IdentifierParseError($"Could not identify a Minion named {split2[1]}.");
                case "a":
                case "o":
                case "accessory":
                case "ornament":
                    return FindDataId(split3[0], Data.Ornaments, out id)
                        ? (ObjectKind.Ornament, id, GetIndex())
                        : throw new IdentifierParseError($"Could not identify an Accessory named {split2[1]}.");
                case "e":
                case "enpc":
                case "eventnpc":
                case "event npc":
                    return FindDataId(split3[0], Data.ENpcs, out id)
                        ? (ObjectKind.EventNpc, id, GetIndex())
                        : throw new IdentifierParseError($"Could not identify an Event NPC named {split2[1]}.");
                case "b":
                case "bnpc":
                case "battlenpc":
                case "battle npc":
                    return FindDataId(split3[0], Data.BNpcs, out id)
                        ? (ObjectKind.BattleNpc, id, GetIndex())
                        : throw new IdentifierParseError($"Could not identify a Battle NPC named {split2[1]}.");
                default: throw new IdentifierParseError($"The argument {split2[0]} is not a valid NPC Type.");
            }
        }

        switch (split[0].ToLowerInvariant())
        {
            case "p":
            case "player":
            {
                var type = IdentifierType.Player;
                var (playerName, worldId) = ParsePlayer(split[1]);
                return [ CreateIndividualUnchecked(type, playerName, worldId.Id, 0, 0) ];
            }
            case "r":
            case "retainer":
            {
                var type = IdentifierType.Retainer;
                if (!VerifyRetainerName(split[1]))
                    throw new IdentifierParseError($"{split[1]} is not a valid player name.");
                if (!ByteString.FromString(split[1], out var playerName))
                    throw new IdentifierParseError($"The retainer string {split[1]} contains invalid symbols.");

                return [ CreateIndividualUnchecked(type, playerName, 0, 0, 0) ] ;
            }
            case "n":
            case "npc":
            {
                var type = IdentifierType.Npc;
                var (kind, objectIds, worldId) = ParseNpc(split[1]);
                return objectIds.Select(i => CreateIndividualUnchecked(type, ByteString.Empty, worldId.Id, kind, i)).ToArray();
            }
            case "o":
            case "owned":
            {
                if (split.Length < 3)
                    throw new IdentifierParseError(
                        "Owned NPCs need a NPC and a player, separated by '|', but only one was provided.");

                var type = IdentifierType.Owned;
                var (kind, objectIds, _)  = ParseNpc(split[1]);
                var (playerName, worldId) = ParsePlayer(split[2]);
                return objectIds.Select(i => CreateIndividualUnchecked(type, playerName, worldId.Id, kind, i)).ToArray();
            }
            default:
                throw new IdentifierParseError(
                    $"{split[0]} is not a valid identifier type. Valid types are [P]layer, [R]etainer, [N]PC, or [O]wned");
        }
    }

    /// <summary>
    /// Compute an ActorIdentifier from a GameObject. If check is true, the values are checked for validity.
    /// </summary>
    public unsafe ActorIdentifier FromObject(FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject* actor,
        out FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject* owner, bool allowPlayerNpc, bool check, bool withoutIndex)
    {
        owner = null;
        if (actor == null)
            return ActorIdentifier.Invalid;

        actor = HandleCutscene(actor);
        ObjectIndex idx = actor->ObjectIndex;
        if (idx.Index is >= (ushort)ScreenActor.CharacterScreen and <= (ushort)ScreenActor.Card8)
            return CreateIndividualUnchecked(IdentifierType.Special, ByteString.Empty, idx.Index, ObjectKind.None, uint.MaxValue);

        var kind = (ObjectKind)actor->ObjectKind;
        switch (kind)
        {
            case ObjectKind.Player:
            {
                var name      = new ByteString(actor->Name);
                var homeWorld = ((Character*)actor)->HomeWorld;
                return check
                    ? CreatePlayer(name, homeWorld)
                    : CreateIndividualUnchecked(IdentifierType.Player, name, homeWorld, ObjectKind.None, uint.MaxValue);
            }
            case ObjectKind.BattleNpc:
            {
                var ownerId = actor->OwnerID;
                // 952 -> 780 is a special case for chocobos because they have NameId == 0 otherwise.
                var nameId = actor->DataID == 952 ? 780 : ((Character*)actor)->NameID;
                if (ownerId != 0xE0000000)
                {
                    owner = HandleCutscene(
                        (FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject*)(_objects.SearchById(ownerId)?.Address ?? IntPtr.Zero));
                    if (owner == null)
                        return ActorIdentifier.Invalid;

                    var name      = new ByteString(owner->Name);
                    var homeWorld = ((Character*)owner)->HomeWorld;
                    return check
                        ? CreateOwned(name, homeWorld, ObjectKind.BattleNpc, nameId)
                        : CreateIndividualUnchecked(IdentifierType.Owned, name, homeWorld, ObjectKind.BattleNpc, nameId);
                }

                // Hack to support Anamnesis changing ObjectKind for NPC faces.
                if (nameId == 0 && allowPlayerNpc)
                {
                    var name = new ByteString(actor->Name);
                    if (!name.IsEmpty)
                    {
                        var homeWorld = ((Character*)actor)->HomeWorld;
                        return check
                            ? CreatePlayer(name, homeWorld)
                            : CreateIndividualUnchecked(IdentifierType.Player, name, homeWorld, ObjectKind.None, uint.MaxValue);
                    }
                }

                var index = withoutIndex ? ushort.MaxValue : actor->ObjectIndex;
                return check
                    ? CreateNpc(ObjectKind.BattleNpc, nameId, index)
                    : CreateIndividualUnchecked(IdentifierType.Npc, ByteString.Empty, index, ObjectKind.BattleNpc, nameId);
            }
            case ObjectKind.EventNpc:
            {
                var dataId = actor->DataID;
                // Special case for squadron that is also in the game functions, cf. E8 ?? ?? ?? ?? 89 87 ?? ?? ?? ?? 4C 89 BF
                if (dataId == 0xF845D)
                    dataId = actor->GetNpcID();
                if (MannequinIds.Contains(dataId))
                {
                    static ByteString Get(byte* ptr)
                        => ptr == null ? ByteString.Empty : new ByteString(ptr);

                    var retainerName = Get(actor->Name);
                    var actualName   = _framework.IsInFrameworkUpdateThread ? Get(actor->GetName()) : ByteString.Empty;
                    if (!actualName.Equals(retainerName))
                    {
                        var ident = check
                            ? CreateRetainer(retainerName, ActorIdentifier.RetainerType.Mannequin)
                            : CreateIndividualUnchecked(IdentifierType.Retainer, retainerName, (ushort)ActorIdentifier.RetainerType.Mannequin,
                                ObjectKind.EventNpc,                             dataId);
                        if (ident.IsValid)
                            return ident;
                    }
                }

                var index = withoutIndex ? ushort.MaxValue : actor->ObjectIndex;
                return check
                    ? CreateNpc(ObjectKind.EventNpc, dataId, index)
                    : CreateIndividualUnchecked(IdentifierType.Npc, ByteString.Empty, index, ObjectKind.EventNpc, dataId);
            }
            case ObjectKind.MountType:
            case ObjectKind.Companion:
            case ObjectKind.Ornament:
            {
                owner = HandleCutscene(
                    (FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject*)_objects.GetObjectAddress(actor->ObjectIndex - 1));
                if (owner == null)
                    return ActorIdentifier.Invalid;

                var dataId    = GetCompanionId(actor, (Character*)owner);
                var name      = new ByteString(owner->Name);
                var homeWorld = ((Character*)owner)->HomeWorld;
                return check
                    ? CreateOwned(name, homeWorld, kind, dataId)
                    : CreateIndividualUnchecked(IdentifierType.Owned, name, homeWorld, kind, dataId);
            }
            case ObjectKind.Retainer:
            {
                var name = new ByteString(actor->Name);
                return check
                    ? CreateRetainer(name, ActorIdentifier.RetainerType.Bell)
                    : CreateIndividualUnchecked(IdentifierType.Retainer, name, (ushort)ActorIdentifier.RetainerType.Bell, ObjectKind.None,
                        uint.MaxValue);
            }
            default:
            {
                var name  = new ByteString(actor->Name);
                var index = withoutIndex ? ushort.MaxValue : actor->ObjectIndex;
                return CreateIndividualUnchecked(IdentifierType.UnkObject, name, index, ObjectKind.None, 0);
            }
        }
    }

    /// <summary>
    /// Obtain the current companion ID for an object by its actor and owner.
    /// </summary>
    private unsafe NpcId GetCompanionId(FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject* actor,
        Character* owner)
    {
        return (ObjectKind)actor->ObjectKind switch
        {
            ObjectKind.MountType => owner->Mount.MountId,
            ObjectKind.Ornament  => owner->Ornament.OrnamentId,
            ObjectKind.Companion => actor->DataID,
            _                    => actor->DataID,
        };
    }

    public unsafe ActorIdentifier FromObject(GameObject? actor, out FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject* owner,
        bool allowPlayerNpc, bool check, bool withoutIndex)
        => FromObject((FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject*)(actor?.Address ?? IntPtr.Zero), out owner, allowPlayerNpc,
            check,                                                                                              withoutIndex);

    public unsafe ActorIdentifier FromObject(GameObject? actor, bool allowPlayerNpc, bool check, bool withoutIndex)
        => FromObject(actor, out _, allowPlayerNpc, check, withoutIndex);

    /// <summary> value can be WorldId, ScreenActor, ObjectIndex or RetainerType. </summary>
    public ActorIdentifier CreateIndividual(IdentifierType type, ByteString name, ushort value, ObjectKind kind, NpcId dataId)
        => type switch
        {
            IdentifierType.Player    => CreatePlayer(name, value),
            IdentifierType.Retainer  => CreateRetainer(name, (ActorIdentifier.RetainerType)value),
            IdentifierType.Owned     => CreateOwned(name, value, kind, dataId),
            IdentifierType.Special   => CreateSpecial((ScreenActor)value),
            IdentifierType.Npc       => CreateNpc(kind, dataId, value),
            IdentifierType.UnkObject => CreateIndividualUnchecked(IdentifierType.UnkObject, name, value, ObjectKind.None, 0),
            _                        => ActorIdentifier.Invalid,
        };

    /// <summary>
    /// Only use this if you are sure the input is valid.
    /// </summary>
    public ActorIdentifier CreateIndividualUnchecked(IdentifierType type, ByteString name, ushort value, ObjectKind kind, NpcId dataId)
        => new(type, kind, (ObjectIndex)value, dataId, name);

    public ActorIdentifier CreatePlayer(ByteString name, WorldId homeWorld)
    {
        if (!VerifyWorld(homeWorld) || !VerifyPlayerName(name.Span))
            return ActorIdentifier.Invalid;

        return new ActorIdentifier(IdentifierType.Player, ObjectKind.Player, homeWorld, 0, name);
    }

    public ActorIdentifier CreateRetainer(ByteString name, ActorIdentifier.RetainerType type)
    {
        if (!VerifyRetainerName(name.Span))
            return ActorIdentifier.Invalid;

        return new ActorIdentifier(IdentifierType.Retainer, ObjectKind.Retainer, type, 0, name);
    }

    public ActorIdentifier CreateSpecial(ScreenActor actor)
    {
        if (!VerifySpecial(actor))
            return ActorIdentifier.Invalid;

        return new ActorIdentifier(IdentifierType.Special, ObjectKind.Player, (ObjectIndex)(uint)actor, 0, ByteString.Empty);
    }

    public ActorIdentifier CreateNpc(ObjectKind kind, NpcId data)
        => CreateNpc(kind, data, ObjectIndex.AnyIndex);

    public ActorIdentifier CreateNpc(ObjectKind kind, NpcId data, ObjectIndex index)
    {
        if (!VerifyIndex(index) || !VerifyNpcData(kind, data))
            return ActorIdentifier.Invalid;

        return new ActorIdentifier(IdentifierType.Npc, kind, index, data, ByteString.Empty);
    }

    public ActorIdentifier CreateOwned(ByteString ownerName, WorldId homeWorld, ObjectKind kind, NpcId dataId)
    {
        if (!VerifyWorld(homeWorld) || !VerifyPlayerName(ownerName.Span) || !VerifyOwnedData(kind, dataId))
            return ActorIdentifier.Invalid;

        return new ActorIdentifier(IdentifierType.Owned, kind, homeWorld, dataId, ownerName);
    }

    /// <summary> Checks SE naming rules. </summary>
    public static bool VerifyPlayerName(ReadOnlySpan<byte> name)
    {
        // Total no more than 20 characters + space.
        if (name.Length is < 5 or > 21)
            return false;

        // Forename and surname, no more spaces.
        var splitIndex = name.IndexOf((byte)' ');
        if (splitIndex < 0 || name[(splitIndex + 1)..].IndexOf((byte)' ') >= 0)
            return false;

        return CheckNamePart(name[..splitIndex], 2, 15) && CheckNamePart(name[(splitIndex + 1)..], 2, 15);
    }

    /// <summary> Checks SE naming rules. </summary>
    public static bool VerifyPlayerName(ReadOnlySpan<char> name)
    {
        // Total no more than 20 characters + space.
        if (name.Length is < 5 or > 21)
            return false;

        // Forename and surname, no more spaces.
        var splitIndex = name.IndexOf(' ');
        if (splitIndex < 0 || name[(splitIndex + 1)..].IndexOf(' ') >= 0)
            return false;

        return CheckNamePart(name[..splitIndex], 2, 15) && CheckNamePart(name[(splitIndex + 1)..], 2, 15);
    }

    /// <summary> Checks SE naming rules. </summary>
    public static bool VerifyRetainerName(ReadOnlySpan<byte> name)
        => CheckNamePart(name, 3, 20);

    /// <summary> Checks SE naming rules. </summary>
    public static bool VerifyRetainerName(ReadOnlySpan<char> name)
        => CheckNamePart(name, 3, 20);

    private static bool CheckNamePart(ReadOnlySpan<char> part, int minLength, int maxLength)
    {
        // Each name part at least 2 and at most 15 characters for players, and at least 3 and at most 20 characters for retainers.
        if (part.Length < minLength || part.Length > maxLength)
            return false;

        // Each part starting with capitalized letter.
        if (part[0] is < 'A' or > 'Z')
            return false;

        // Every other symbol needs to be lowercase letter, hyphen or apostrophe.
        var last = '\0';
        for (var i = 1; i < part.Length; ++i)
        {
            var current = part[i];
            if (current is not ('\'' or '-' or (>= 'a' and <= 'z')))
                return false;

            // Hyphens can not be used in succession, after or before apostrophes or as the last symbol.
            if (last is '\'' && current is '-')
                return false;
            if (last is '-' && current is '-' or '\'')
                return false;

            last = current;
        }

        return true;
    }

    private static bool CheckNamePart(ReadOnlySpan<byte> part, int minLength, int maxLength)
    {
        // Each name part at least 2 and at most 15 characters for players, and at least 3 and at most 20 characters for retainers.
        if (part.Length < minLength || part.Length > maxLength)
            return false;

        // Each part starting with capitalized letter.
        if (part[0] is < (byte)'A' or > (byte)'Z')
            return false;

        // Every other symbol needs to be lowercase letter, hyphen or apostrophe.
        var last = (byte)'\0';
        for (var i = 1; i < part.Length; ++i)
        {
            var current = part[i];
            if (current is not ((byte)'\'' or (byte)'-' or (>= (byte)'a' and <= (byte)'z')))
                return false;

            // Hyphens can not be used in succession, after or before apostrophes or as the last symbol.
            if (last is (byte)'\'' && current is (byte)'-')
                return false;
            if (last is (byte)'-' && current is (byte)'-' or (byte)'\'')
                return false;

            last = current;
        }

        return true;
    }

    /// <summary> Checks if the world is a valid public world or ushort.MaxValue (any world). </summary>
    public bool VerifyWorld(WorldId worldId)
        => worldId == WorldId.AnyWorld || Data.Worlds.ContainsKey(worldId.Id);

    /// <summary> Verify that the enum value is a specific actor and return the name if it is. </summary>
    public static bool VerifySpecial(ScreenActor actor)
        => actor is >= ScreenActor.CharacterScreen and <= ScreenActor.Card8;

    /// <summary> Verify that the object index is a valid index for an NPC. </summary>
    public bool VerifyIndex(ObjectIndex index)
    {
        if (index == ObjectIndex.AnyIndex)
            return true;
        if (index < ObjectIndex.GPosePlayer)
            return (index.Index & 1) == 0;
        if (index > ObjectIndex.Card8)
            return index.Index < _objects.Length;

        return index < ObjectIndex.CharacterScreen;
    }

    /// <summary> Verify that the object kind is a valid owned object, and the corresponding data Id. </summary>
    public bool VerifyOwnedData(ObjectKind kind, NpcId dataId)
    {
        return kind switch
        {
            ObjectKind.MountType => Data.Mounts.ContainsKey(dataId.Id),
            ObjectKind.Companion => Data.Companions.ContainsKey(dataId.Id),
            ObjectKind.Ornament  => Data.Ornaments.ContainsKey(dataId.Id),
            ObjectKind.BattleNpc => Data.BNpcs.ContainsKey(dataId.Id),
            _                    => false,
        };
    }

    public bool VerifyNpcData(ObjectKind kind, NpcId dataId)
        => kind switch
        {
            ObjectKind.MountType => Data.Mounts.ContainsKey(dataId.Id),
            ObjectKind.Companion => Data.Companions.ContainsKey(dataId.Id),
            ObjectKind.Ornament  => Data.Ornaments.ContainsKey(dataId.Id),
            ObjectKind.BattleNpc => Data.BNpcs.ContainsKey(dataId.Id),
            ObjectKind.EventNpc  => Data.ENpcs.ContainsKey(dataId.Id),
            _                    => false,
        };
}
