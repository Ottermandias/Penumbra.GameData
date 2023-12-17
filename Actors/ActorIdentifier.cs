using Dalamud.Game.ClientState.Objects.Enums;
using Penumbra.GameData.Enums;
using Penumbra.GameData.Structs;
using Penumbra.String;

namespace Penumbra.GameData.Actors;

[StructLayout(LayoutKind.Explicit)]
public readonly struct ActorIdentifier : IEquatable<ActorIdentifier>
{
    public static readonly ActorIdentifier Invalid = new(IdentifierType.Invalid, 0, WorldId.AnyWorld, 0, ByteString.Empty);

    public enum RetainerType : ushort
    {
        Both      = 0,
        Bell      = 1,
        Mannequin = 2,
    }

    // @formatter:off
    [FieldOffset( 0 )] public readonly IdentifierType Type;       // All
    [FieldOffset( 1 )] public readonly ObjectKind     Kind;       // Npc, Owned
    [FieldOffset( 2 )] public readonly WorldId        HomeWorld;  // Player, Owned
    [FieldOffset( 2 )] public readonly ObjectIndex    Index;      // NPC, Special
    [FieldOffset( 2 )] public readonly RetainerType   Retainer;   // Retainer
    [FieldOffset( 4 )] public readonly NpcId          DataId;     // Owned, NPC
    [FieldOffset( 8 )] public readonly ByteString     PlayerName; // Player, Owned
    // @formatter:on

    public ActorIdentifier CreatePermanent()
        => new(Type, Kind, Index, DataId, PlayerName.IsEmpty || PlayerName.IsOwned ? PlayerName : PlayerName.Clone());

    public ScreenActor Special
        => (ScreenActor)Index.Index;

    public bool Equals(ActorIdentifier other)
    {
        if (Type != other.Type)
            return false;

        return Type switch
        {
            IdentifierType.Player => HomeWorld == other.HomeWorld && PlayerName.EqualsCi(other.PlayerName),
            IdentifierType.Retainer => (Retainer == other.Retainer || Retainer == RetainerType.Both || other.Retainer == RetainerType.Both)
             && PlayerName.EqualsCi(other.PlayerName),
            IdentifierType.Owned => HomeWorld == other.HomeWorld
             && PlayerName.EqualsCi(other.PlayerName)
             && ActorIdentifierExtensions.Manager.DataIdEquals(this, other),
            IdentifierType.Special => Index == other.Index,
            IdentifierType.Npc => ActorIdentifierExtensions.Manager.DataIdEquals(this, other)
             && (Index == other.Index || Index == ushort.MaxValue || other.Index == ushort.MaxValue),
            IdentifierType.UnkObject => PlayerName.EqualsCi(other.PlayerName) && Index == other.Index,
            _                        => false,
        };
    }

    public override bool Equals(object? obj)
        => obj is ActorIdentifier other && Equals(other);

    public static bool operator ==(ActorIdentifier lhs, ActorIdentifier rhs)
        => lhs.Equals(rhs);

    public static bool operator !=(ActorIdentifier lhs, ActorIdentifier rhs)
        => !lhs.Equals(rhs);

    public bool IsValid
        => Type is not (IdentifierType.UnkObject or IdentifierType.Invalid);

    public string Incognito(string? name)
    {
        name ??= ToString();
        switch (Type)
        {
            case IdentifierType.Player:
            {
                var parts = name.Split(' ', 3);
                return parts.Length == 2 ? $"{parts[0][0]}. {parts[1][0]}." : $"{parts[0][0]}. {parts[1][0]}. {parts[2]}";
            }
            case IdentifierType.Owned:
            {
                var parts = name.Split(' ', 3);
                return parts[2][0] == '(' ? $"{parts[0][0]}. {parts[1][0]}. {parts[2]}" : $"{parts[0][0]}. {parts[1][0]}.'s {parts[2]}";
            }
            case IdentifierType.Retainer:
            {
                var parts = name.Split(' ', 2);
                return $"{parts[0][0]}. {parts[1]}";
            }
            default: return name;
        }
    }

    public override string ToString()
        => ActorIdentifierExtensions.Manager?.ToString(this)
         ?? Type switch
            {
                IdentifierType.Player => $"{PlayerName} ({HomeWorld})",
                IdentifierType.Retainer =>
                    $"{PlayerName}{Retainer switch
                    {
                        RetainerType.Bell      => " (Bell)",
                        RetainerType.Mannequin => " (Mannequin)",
                        _                      => " (Retainer)",
                    }}",
                IdentifierType.Owned   => $"{PlayerName}s {Kind.ToName()} {DataId} ({HomeWorld})",
                IdentifierType.Special => ((ScreenActor)Index.Index).ToName(),
                IdentifierType.Npc =>
                    Index == ushort.MaxValue
                        ? $"{Kind.ToName()} #{DataId}"
                        : $"{Kind.ToName()} #{DataId} at {Index}",
                IdentifierType.UnkObject => PlayerName.IsEmpty
                    ? $"Unknown Object at {Index}"
                    : $"{PlayerName} at {Index}",
                _ => "Invalid",
            };

    public string ToName()
        => ActorIdentifierExtensions.Manager?.ToName(this) ?? "Unknown Object";

    public override int GetHashCode()
        => Type switch
        {
            IdentifierType.Player    => HashCode.Combine(IdentifierType.Player,    PlayerName, HomeWorld),
            IdentifierType.Retainer  => HashCode.Combine(IdentifierType.Player,    PlayerName),
            IdentifierType.Owned     => HashCode.Combine(IdentifierType.Owned,     Kind, PlayerName, HomeWorld, DataId),
            IdentifierType.Special   => HashCode.Combine(IdentifierType.Special,   Index),
            IdentifierType.Npc       => HashCode.Combine(IdentifierType.Npc,       Kind,       DataId),
            IdentifierType.UnkObject => HashCode.Combine(IdentifierType.UnkObject, PlayerName, Index),
            _                        => 0,
        };

    internal ActorIdentifier(IdentifierType type, ObjectKind kind, ObjectIndex index, NpcId data, ByteString playerName)
    {
        Type       = type;
        Kind       = kind;
        Retainer   = RetainerType.Both;
        HomeWorld  = WorldId.AnyWorld;
        Index      = index;
        DataId     = data;
        PlayerName = playerName;
    }

    internal ActorIdentifier(IdentifierType type, ObjectKind kind, WorldId worldId, NpcId data, ByteString playerName)
    {
        Type       = type;
        Kind       = kind;
        Retainer   = RetainerType.Both;
        Index      = ObjectIndex.AnyIndex;
        HomeWorld  = worldId;
        DataId     = data;
        PlayerName = playerName;
    }

    internal ActorIdentifier(IdentifierType type, ObjectKind kind, RetainerType retainerType, NpcId data, ByteString playerName)
    {
        Type       = type;
        Kind       = kind;
        Index      = ObjectIndex.AnyIndex;
        HomeWorld  = WorldId.AnyWorld;
        Retainer   = retainerType;
        DataId     = data;
        PlayerName = playerName;
    }
}
