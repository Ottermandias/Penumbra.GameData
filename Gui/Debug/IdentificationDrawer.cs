using Dalamud.Interface.Utility;
using ImSharp;
using Penumbra.GameData.Data;
using Penumbra.GameData.DataContainers;
using Penumbra.GameData.DataContainers.Bases;
using Penumbra.GameData.Enums;
using Penumbra.GameData.Structs;

namespace Penumbra.GameData.Gui.Debug;

/// <summary> Draw all data associated to identification and resolvers. </summary>
public class IdentificationDrawer(
    ObjectIdentification identifier,
    GamePathParser gamePathParser,
    DictAction actions,
    DictEmote emotes,
    DictModelChara modelCharas,
    IdentificationListEquipment equipment,
    IdentificationListWeapons weapons,
    IdentificationListModels models) : IGameDataDrawer
{
    /// <inheritdoc/>
    public ReadOnlySpan<byte> Label
        => "Object Identification"u8;

    /// <inheritdoc/>
    public bool Disabled
        => !identifier.Finished;


    /// <inheritdoc/>
    public void Draw()
    {
        DrawParser();
        DrawIdentifier();
        DrawDictModelChara();
        DrawIdentificationList("Equipment List"u8, ref _equipmentFrom, ref _equipmentTo, ref _equipmentFilter, equipment, t => t.Item1);
        DrawIdentificationList("Weapon List"u8,    ref _weaponsFrom,   ref _weaponsTo,   ref _weaponsFilter,   weapons,   t => t.Item1);
        DrawIdentificationList("Model List"u8, ref _modelsFrom, ref _modelsTo, ref _modelFilter, models,
            t => modelCharas.TryGetValue(t.RowId, out var names)
                ? string.Join(", ", names.Select(n => n.Name).Distinct())
                : t.RowId.ToString());
        DrawLuminaDict("Emotes"u8,  ref _emoteFilter,  emotes,  e => e.Name.ExtractTextExtended());
        DrawLuminaDict("Actions"u8, ref _actionFilter, actions, a => a.Name.ExtractTextExtended());
    }

    // Input
    private string          _gamePath = string.Empty;
    private EquipSlot       _slot     = EquipSlot.Head;
    private CharacterWeapon _weapon;

    // Filters
    private ulong  _equipmentFrom;
    private ulong  _weaponsFrom;
    private ulong  _modelsFrom;
    private ulong  _equipmentTo      = ulong.MaxValue;
    private ulong  _weaponsTo        = ulong.MaxValue;
    private ulong  _modelsTo         = ulong.MaxValue;
    private string _equipmentFilter  = string.Empty;
    private string _weaponsFilter    = string.Empty;
    private string _modelFilter      = string.Empty;
    private string _modelCharaFilter = string.Empty;
    private string _actionFilter     = string.Empty;
    private string _emoteFilter      = string.Empty;

    /// <summary> Draw a game path parser. </summary>
    private void DrawParser()
    {
        ImEx.TextFrameAligned("Parse Game Path"u8);
        Im.Line.SameInner();
        Im.Item.SetNextWidth(300 * Im.Style.GlobalScale);
        Im.Input.Text("##gamePath"u8, ref _gamePath, "Enter game path..."u8);
        var fileInfo = gamePathParser.GetFileInfo(_gamePath);
        Im.Text(
            $"{fileInfo.ObjectType} {fileInfo.EquipSlot} {fileInfo.PrimaryId} {fileInfo.SecondaryId} {fileInfo.Variant} {fileInfo.BodySlot} {fileInfo.CustomizationType}");
        Text(string.Join("\n", identifier.Identify(_gamePath).Keys));
    }

    /// <summary> Draw an equip item identifier. </summary>
    private void DrawIdentifier()
    {
        ImEx.TextFrameAligned("Identify Model"u8);
        Im.Line.SameInner();
        EquipSlotCombo.Draw("##Slot"u8, ""u8, ref _slot);
        Im.Line.SameInner();
        ModelInput.DrawWeaponInput(ref _weapon);

        var items = identifier.Identify(_weapon.Skeleton, _weapon.Weapon, _weapon.Variant, _slot);
        foreach (var item in items)
            Text(item.Name);
    }

    /// <summary> Draw a list of keys to items optimized for identification. </summary>
    private static void DrawIdentificationList<T>(ReadOnlySpan<byte> name, ref ulong from, ref ulong to, ref string filter, KeyList<T> keyList,
        Func<T, string> drawValue)
    {
        using var tree = Im.Tree.Node(name);
        if (!tree)
            return;

        var resetScroll = Im.Input.Text("##filter"u8, ref filter, "Filter..."u8);
        ImEx.TextFrameAligned("From"u8);
        Im.Line.SameInner();
        float width;
        using (_ = Im.Font.PushMono())
        {
            width = Im.Font.CalculateSize("0000000000000000"u8).X + 2 * Im.Style.FramePadding.X;
            Im.Item.SetNextWidth(width);
            resetScroll |= Im.Input.Scalar("##From"u8, ref from, "%016llx"u8, flags: InputTextFlags.CharsHexadecimal);
        }

        Im.Line.SameInner();
        ImEx.TextFrameAligned("to"u8);
        Im.Line.SameInner();
        using (_ = Im.Font.PushMono())
        {
            Im.Item.SetNextWidth(width);
            resetScroll |= Im.Input.Scalar("##To"u8, ref to, "%016llx"u8, flags: InputTextFlags.CharsHexadecimal);
        }

        var height = Im.Style.TextHeightWithSpacing + 2 * Im.Style.CellPadding.Y;
        using var table = Im.Table.Begin("##table"u8, 2, TableFlags.RowBackground | TableFlags.ScrollY | TableFlags.BordersOuter,
            new Vector2(-1, 10 * height));
        if (!table)
            return;

        if (resetScroll)
            Im.Scroll.Y = 0;
        table.SetupColumn("1"u8, TableColumnFlags.WidthFixed, width);
        table.SetupColumn("2"u8, TableColumnFlags.WidthStretch);
        table.NextColumn();
        var skips = ImGuiClip.GetNecessarySkips(height);
        table.NextColumn();
        var f       = filter;
        var tmpFrom = from;
        var tmpTo   = to;
        var remainder = ImGuiClip.FilteredClippedDraw(keyList.Value.Select(p => (p.Key, p.Key.ToString("X16"), drawValue(p.Data))), skips,
            p => p.Key >= tmpFrom
             && p.Key <= tmpTo
             && (p.Item2.Contains(f, StringComparison.OrdinalIgnoreCase) || p.Item3.Contains(f, StringComparison.OrdinalIgnoreCase)),
            p =>
            {
                using (_ = Im.Font.PushMono())
                {
                    Im.Table.DrawColumn(p.Item2);
                }

                Im.Table.DrawColumn(p.Item3);
            });
        ImGuiClip.DrawEndDummy(remainder, height);
    }

    /// <summary> Draw a dict associating name parts with objects. </summary>
    private static void DrawLuminaDict<T>(ReadOnlySpan<byte> name, ref string filter, DictLuminaName<T> dict,
        Func<T, string> drawValue)
    {
        using var tree = Im.Tree.Node(name);
        if (!tree)
            return;

        var resetScroll = Im.Input.Text("##filter"u8, ref filter, "Filter..."u8);
        var height      = Im.Style.TextHeightWithSpacing + 2 * Im.Style.CellPadding.Y;
        using var table = Im.Table.Begin("##table"u8, 2, TableFlags.RowBackground | TableFlags.ScrollY | TableFlags.BordersOuter,
            new Vector2(-1, 10 * height));
        if (!table)
            return;

        if (resetScroll)
            Im.Scroll.Y = 0;
        table.SetupColumn("1"u8, TableColumnFlags.WidthStretch, 0.3f);
        table.SetupColumn("2"u8, TableColumnFlags.WidthStretch, 0.7f);
        table.NextColumn();
        var skips = ImGuiClip.GetNecessarySkips(height);
        table.NextColumn();
        var f = filter;
        var remainder = ImGuiClip.FilteredClippedDraw(dict.Value.Select(p => (p.Key, string.Join(", ", p.Value.Select(drawValue)))), skips,
            p => p.Key.Contains(f, StringComparison.OrdinalIgnoreCase) || p.Item2.Contains(f, StringComparison.OrdinalIgnoreCase),
            p =>
            {
                Im.Table.DrawColumn(p.Key);
                Im.Table.DrawColumn(p.Item2);
            });
        ImGuiClip.DrawEndDummy(remainder, height);
    }

    /// <summary> Draw all collected model character to names. </summary>
    private void DrawDictModelChara()
    {
        DebugUtility.DrawNameTable("Model Chara List", ref _modelCharaFilter, true,
            modelCharas.Select(kvp => ((ulong)kvp.Key.Id, string.Join(", ", kvp.Value.Select(n => n.Name).Distinct()))));
    }

    /// <summary> Draw text only if it is not empty. </summary>
    private static void Text(string text)
    {
        if (text.Length > 0)
            Im.Text(text);
    }
}
