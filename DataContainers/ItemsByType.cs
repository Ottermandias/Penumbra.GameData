using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Lumina.Excel.GeneratedSheets;
using OtterGui.Log;
using Penumbra.GameData.DataContainers.Bases;
using Penumbra.GameData.Enums;
using Penumbra.GameData.Structs;

namespace Penumbra.GameData.DataContainers;

public sealed class ItemsByType(DalamudPluginInterface pi, Logger log, IDataManager dataManager)
    : DataSharer<IReadOnlyList<IReadOnlyList<PseudoEquipItem>>>(pi, log, "ItemsByType", dataManager.Language, 1,
            () => CreateItems(dataManager)),
        IReadOnlyDictionary<FullEquipType, IReadOnlyList<EquipItem>>
{
    private static IReadOnlyList<IReadOnlyList<PseudoEquipItem>> CreateItems(IDataManager dataManager)
    {
        var tmp = Enum.GetValues<FullEquipType>().Select(_ => new List<EquipItem>(1024)).ToArray();

        var itemSheet = dataManager.GetExcelSheet<Item>(dataManager.Language)!;
        foreach (var item in itemSheet.Where(i => i.Name.RawData.Length > 1))
        {
            var type = item.ToEquipType();
            if (type.IsWeapon() || type.IsTool())
            {
                var mh = EquipItem.FromMainhand(item);
                if (item.ModelMain != 0)
                    tmp[(int)type].Add(mh);
                if (item.ModelSub != 0)
                {
                    if (type is FullEquipType.Fists && item.ModelSub < 0x100000000)
                    {
                        tmp[(int)FullEquipType.Hands].Add(new EquipItem(mh.Name + " (Gauntlets)", mh.Id, mh.IconId, (SetId)item.ModelSub, 0,
                            (byte)(item.ModelSub >> 16), FullEquipType.Hands, mh.Flags, mh.Level, mh.JobRestrictions));
                        tmp[(int)FullEquipType.FistsOff].Add(new EquipItem(mh.Name + FullEquipType.FistsOff.OffhandTypeSuffix(), mh.Id,
                            mh.IconId, (SetId)(mh.ModelId.Id + 50), mh.WeaponType, mh.Variant, FullEquipType.FistsOff, mh.Flags, mh.Level,
                            mh.JobRestrictions));
                    }
                    else
                    {
                        tmp[(int)type.ValidOffhand()].Add(EquipItem.FromOffhand(item));
                    }
                }
            }
            else if (type != FullEquipType.Unknown)
            {
                tmp[(int)type].Add(EquipItem.FromArmor(item));
            }
        }

        var ret = new IReadOnlyList<PseudoEquipItem>[tmp.Length];
        ret[0] = Array.Empty<PseudoEquipItem>();
        for (var i = 1; i < tmp.Length; ++i)
            ret[i] = tmp[i].OrderBy(item => item.Name).Select(s => (PseudoEquipItem)s).ToArray();

        return ret;
    }

    public IEnumerator<KeyValuePair<FullEquipType, IReadOnlyList<EquipItem>>> GetEnumerator()
    {
        for (var i = 1; i < Value.Count; ++i)
            yield return new KeyValuePair<FullEquipType, IReadOnlyList<EquipItem>>((FullEquipType)i, new EquipItemList(Value[i]));
    }

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public int Count
        => Value.Count - 1;

    public bool ContainsKey(FullEquipType key)
        => (int)key < Value.Count && key != FullEquipType.Unknown;

    public bool TryGetValue(FullEquipType key, out IReadOnlyList<EquipItem> value)
    {
        if (ContainsKey(key))
        {
            value = new EquipItemList(Value[(int)key]);
            return true;
        }

        value = Array.Empty<EquipItem>();
        return false;
    }

    public IReadOnlyList<EquipItem> this[FullEquipType key]
        => TryGetValue(key, out var ret) ? ret : throw new IndexOutOfRangeException();

    public IEnumerable<FullEquipType> Keys
        => Enum.GetValues<FullEquipType>().Skip(1);

    public IEnumerable<IReadOnlyList<EquipItem>> Values
        => Value.Skip(1).Select(l => (IReadOnlyList<EquipItem>)new EquipItemList(l));

    public override long ComputeMemory()
        => 16 + TotalCount * 32 + Count * 16;

    public override int ComputeTotalCount()
        => Value.Sum(row => row.Count);
}
