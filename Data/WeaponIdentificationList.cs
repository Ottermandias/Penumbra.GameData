using Dalamud;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Penumbra.GameData.Enums;
using Penumbra.GameData.Structs;
using PseudoEquipItem = System.ValueTuple<string, ulong, ushort, ushort, ushort, byte, uint>;

namespace Penumbra.GameData.Data;

internal sealed class WeaponIdentificationList : KeyList<PseudoEquipItem>
{
    private const string Tag     = "WeaponIdentification";
    private const int    Version = 3;

    public WeaponIdentificationList(DalamudPluginInterface pi, ClientLanguage language, ItemData data, IPluginLog log)
        : base(pi, Tag, language, Version, CreateWeaponList(data), log)
    { }

    public IEnumerable<EquipItem> Between(SetId modelId)
        => Between(ToKey(modelId, 0, 0), ToKey(modelId, 0xFFFF, 0xFF)).Select(e => (EquipItem)e);

    public IEnumerable<EquipItem> Between(SetId modelId, WeaponType type, Variant variant = default)
    {
        if (type == 0)
            return Between(ToKey(modelId, 0, 0), ToKey(modelId, 0xFFFF, 0xFF)).Select(e => (EquipItem)e);
        if (variant == 0)
            return Between(ToKey(modelId, type, 0), ToKey(modelId, type, 0xFF)).Select(e => (EquipItem)e);

        return Between(ToKey(modelId, type, variant), ToKey(modelId, type, variant)).Select(e => (EquipItem)e);
    }

    public void Dispose(DalamudPluginInterface pi, ClientLanguage language)
        => DataSharer.DisposeTag(pi, Tag, language, Version);

    public static ulong ToKey(SetId modelId, WeaponType type, Variant variant)
        => ((ulong)modelId.Id << 32) | ((ulong)type.Id << 16) | variant.Id;

    public static ulong ToKey(EquipItem i)
        => ToKey(i.ModelId, i.WeaponType, i.Variant);

    protected override IEnumerable<ulong> ToKeys(PseudoEquipItem data)
    {
        yield return ToKey(data);
    }

    protected override bool ValidKey(ulong key)
        => key != 0;

    protected override int ValueKeySelector(PseudoEquipItem data)
        => (int)data.Item2;

    private static IEnumerable<PseudoEquipItem> CreateWeaponList(ItemData data)
        => data.Where(kvp => !kvp.Key.IsEquipment() && !kvp.Key.IsAccessory())
            .SelectMany(kvp => kvp.Value)
            .Select(i => (PseudoEquipItem)i);
}
