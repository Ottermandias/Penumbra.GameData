using Dalamud.Game.ClientState.Objects.Enums;
using Penumbra.GameData.Enums;

namespace Penumbra.GameData.Actors;

public static class ActorIdentifierExtensions
{
    public static ActorManager? Manager;

    public static bool DataIdEquals(this ActorManager? manager, ActorIdentifier lhs, ActorIdentifier rhs)
    {
        if (lhs.Kind != rhs.Kind)
            return false;

        if (lhs.DataId == rhs.DataId)
            return true;

        if (manager == null)
            return lhs.Kind == rhs.Kind && lhs.DataId == rhs.DataId || lhs.DataId == uint.MaxValue || rhs.DataId == uint.MaxValue;

        return manager.Data.TryGetName(lhs.Kind, lhs.DataId, out var lhsName)
         && manager.Data.TryGetName(rhs.Kind,    rhs.DataId, out var rhsName)
         && lhsName.Equals(rhsName, StringComparison.OrdinalIgnoreCase);
    }

    public static string ToName(this ObjectKind kind)
        => kind switch
        {
            ObjectKind.None      => "Unknown",
            ObjectKind.BattleNpc => "Battle NPC",
            ObjectKind.EventNpc  => "Event NPC",
            ObjectKind.MountType => "Mount",
            ObjectKind.Companion => "Companion",
            ObjectKind.Ornament  => "Accessory",
            _                    => kind.ToString(),
        };

    public static string ToName(this IdentifierType type)
        => type switch
        {
            IdentifierType.Player    => "Player",
            IdentifierType.Retainer  => "Retainer (Bell)",
            IdentifierType.Owned     => "Owned NPC",
            IdentifierType.Special   => "Special Actor",
            IdentifierType.Npc       => "NPC",
            IdentifierType.UnkObject => "Unknown Object",
            _                        => "Invalid",
        };

    /// <summary> Fixed names for special actors. </summary>
    public static string ToName(this ScreenActor actor)
        => actor switch
        {
            ScreenActor.CharacterScreen => "Character Screen Actor",
            ScreenActor.ExamineScreen   => "Examine Screen Actor",
            ScreenActor.FittingRoom     => "Fitting Room Actor",
            ScreenActor.DyePreview      => "Dye Preview Actor",
            ScreenActor.Portrait        => "Portrait Actor",
            _                           => "Invalid",
        };
}
