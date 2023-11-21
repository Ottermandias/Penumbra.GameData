using System.Collections.Frozen;
using Dalamud;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Lumina.Excel.GeneratedSheets;
using Penumbra.GameData.Enums;
using Penumbra.GameData.Structs;
using PseudoEquipItem = System.ValueTuple<string, ulong, ushort, ushort, ushort, byte, uint>;

namespace Penumbra.GameData.Data;

public sealed class ItemData : DataSharer, IReadOnlyDictionary<FullEquipType, IReadOnlyList<EquipItem>>
{
    private readonly IReadOnlyDictionary<uint, PseudoEquipItem>    _mainItems;
    private readonly IReadOnlyDictionary<uint, PseudoEquipItem>    _offItems;
    private readonly IReadOnlyDictionary<uint, PseudoEquipItem>    _gauntlets;
    private readonly IReadOnlyList<IReadOnlyList<PseudoEquipItem>> _byType;

    private static IReadOnlyList<IReadOnlyList<PseudoEquipItem>> CreateItems(IDataManager dataManager, ClientLanguage language)
    {
        var tmp = Enum.GetValues<FullEquipType>().Select(_ => new List<EquipItem>(1024)).ToArray();

        var itemSheet = dataManager.GetExcelSheet<Item>(language)!;
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

    private static Tuple<IReadOnlyDictionary<uint, PseudoEquipItem>, IReadOnlyDictionary<uint, PseudoEquipItem>> CreateMainItems(
        IReadOnlyList<IReadOnlyList<PseudoEquipItem>> items)
    {
        var dict = new Dictionary<uint, PseudoEquipItem>(1024 * 4);
        foreach (var fistWeapon in items[(int)FullEquipType.Fists])
            dict.TryAdd((uint)fistWeapon.Item2, fistWeapon);

        var gauntlets = items[(int)FullEquipType.Hands].Where(g => dict.ContainsKey((uint)g.Item2)).ToDictionary(g => (uint)g.Item2, g => g);

        foreach (var type in Enum.GetValues<FullEquipType>().Where(v => !FullEquipTypeExtensions.OffhandTypes.Contains(v)))
        {
            var list = items[(int)type];
            foreach (var item in list)
                dict.TryAdd((uint)item.Item2, item);
        }

        return new Tuple<IReadOnlyDictionary<uint, (string, ulong, ushort, ushort, ushort, byte, uint)>,
            IReadOnlyDictionary<uint, (string, ulong, ushort, ushort, ushort, byte, uint)>>(dict.ToFrozenDictionary(), gauntlets.ToFrozenDictionary());
    }

    private static IReadOnlyDictionary<uint, PseudoEquipItem> CreateOffItems(IReadOnlyList<IReadOnlyList<PseudoEquipItem>> items)
    {
        var dict = new Dictionary<uint, PseudoEquipItem>(128);
        foreach (var type in FullEquipTypeExtensions.OffhandTypes)
        {
            var list = items[(int)type];
            foreach (var item in list)
                dict.TryAdd((uint)item.Item2, item);
        }

        return dict.ToFrozenDictionary();
    }

    public ItemData(DalamudPluginInterface pluginInterface, IDataManager dataManager, ClientLanguage language, IPluginLog log)
        : base(pluginInterface, language, 5, log)
    {
        _byType                  = TryCatchData("ItemList",     () => CreateItems(dataManager, language));
        (_mainItems, _gauntlets) = TryCatchData("ItemDictMain", () => CreateMainItems(_byType));
        _offItems                = TryCatchData("ItemDictOff",  () => CreateOffItems(_byType));
    }

    protected override void DisposeInternal()
    {
        DisposeTag("ItemList");
        DisposeTag("ItemDictMain");
        DisposeTag("ItemDictOff");
    }

    public IEnumerator<KeyValuePair<FullEquipType, IReadOnlyList<EquipItem>>> GetEnumerator()
    {
        for (var i = 1; i < _byType.Count; ++i)
            yield return new KeyValuePair<FullEquipType, IReadOnlyList<EquipItem>>((FullEquipType)i, new EquipItemList(_byType[i]));
    }

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public int Count
        => _byType.Count - 1;

    public bool ContainsKey(FullEquipType key)
        => (int)key < _byType.Count && key != FullEquipType.Unknown;

    public bool TryGetValue(FullEquipType key, out IReadOnlyList<EquipItem> value)
    {
        if (ContainsKey(key))
        {
            value = new EquipItemList(_byType[(int)key]);
            return true;
        }

        value = Array.Empty<EquipItem>();
        return false;
    }

    public IReadOnlyList<EquipItem> this[FullEquipType key]
        => TryGetValue(key, out var ret) ? ret : throw new IndexOutOfRangeException();

    public IEnumerable<(ItemId, EquipItem)> AllItems(bool main)
        => (main ? _mainItems : _offItems).Select(i => ((ItemId)i.Key, (EquipItem)i.Value));

    public int TotalItemCount(bool main)
        => main ? _mainItems.Count : _offItems.Count;

    public bool TryGetValue(ItemId key, EquipSlot slot, out EquipItem value)
    {
        var dict = slot is EquipSlot.OffHand ? _offItems : _mainItems;
        if (slot is EquipSlot.Hands && _gauntlets.TryGetValue(key.Id, out var v) || dict.TryGetValue(key.Id, out v))
        {
            value = v;
            return true;
        }

        value = default;
        return false;
    }

    public IEnumerable<FullEquipType> Keys
        => Enum.GetValues<FullEquipType>().Skip(1);

    public IEnumerable<IReadOnlyList<EquipItem>> Values
        => _byType.Skip(1).Select(l => (IReadOnlyList<EquipItem>)new EquipItemList(l));

    private readonly struct EquipItemList : IReadOnlyList<EquipItem>
    {
        private readonly IReadOnlyList<PseudoEquipItem> _items;

        public EquipItemList(IReadOnlyList<PseudoEquipItem> items)
            => _items = items;

        public IEnumerator<EquipItem> GetEnumerator()
            => _items.Select(i => (EquipItem)i).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public int Count
            => _items.Count;

        public EquipItem this[int index]
            => _items[index];
    }

    private static readonly (FullEquipType Main, FullEquipType Off, short Offset)[] WeaponIdTable =
    {
        (FullEquipType.Unknown, FullEquipType.Unknown, 100),
        (FullEquipType.Shield, FullEquipType.Unknown, 100),
        (FullEquipType.Sword, FullEquipType.Unknown, 100),
        (FullEquipType.Fists, FullEquipType.FistsOff, 50),
        (FullEquipType.Axe, FullEquipType.Unknown, 100),
        (FullEquipType.Lance, FullEquipType.Unknown, 100),
        (FullEquipType.Bow, FullEquipType.BowOff, 97),
        (FullEquipType.Wand, FullEquipType.Unknown, 100),
        (FullEquipType.Staff, FullEquipType.Unknown, 100),
        (FullEquipType.Wand, FullEquipType.Unknown, 100),
        (FullEquipType.Staff, FullEquipType.Unknown, 100),
        (FullEquipType.Unknown, FullEquipType.Unknown, 100),
        (FullEquipType.Unknown, FullEquipType.Unknown, 100),
        (FullEquipType.Unknown, FullEquipType.Unknown, 100),
        (FullEquipType.Unknown, FullEquipType.Unknown, 100),
        (FullEquipType.Broadsword, FullEquipType.Unknown, 100),
        (FullEquipType.Fists, FullEquipType.FistsOff, 50),
        (FullEquipType.Book, FullEquipType.Unknown, 100),
        (FullEquipType.Daggers, FullEquipType.DaggersOff, 50),
        (FullEquipType.Unknown, FullEquipType.Unknown, 100),
        (FullEquipType.Gun, FullEquipType.GunOff, 98),
        (FullEquipType.Orrery, FullEquipType.OrreryOff, 98),
        (FullEquipType.Katana, FullEquipType.KatanaOff, 50),
        (FullEquipType.Rapier, FullEquipType.RapierOff, 50),
        (FullEquipType.Cane, FullEquipType.Unknown, 100),
        (FullEquipType.Gunblade, FullEquipType.Unknown, 100),
        (FullEquipType.Glaives, FullEquipType.GlaivesOff, 50),
        (FullEquipType.Nouliths, FullEquipType.Unknown, 100),
        (FullEquipType.Scythe, FullEquipType.Unknown, 100),
        (FullEquipType.Unknown, FullEquipType.Unknown, 100),
        (FullEquipType.Unknown, FullEquipType.Unknown, 100),
        (FullEquipType.Unknown, FullEquipType.Unknown, 100),
        (FullEquipType.Unknown, FullEquipType.Unknown, 100),
        (FullEquipType.Unknown, FullEquipType.Unknown, 100),
        (FullEquipType.Unknown, FullEquipType.Unknown, 100),
        (FullEquipType.Unknown, FullEquipType.Unknown, 100),
        (FullEquipType.Unknown, FullEquipType.Unknown, 100),
        (FullEquipType.Unknown, FullEquipType.Unknown, 100),
        (FullEquipType.Unknown, FullEquipType.Unknown, 100),
        (FullEquipType.Unknown, FullEquipType.Unknown, 100),
        (FullEquipType.Unknown, FullEquipType.Unknown, 100),
        (FullEquipType.Unknown, FullEquipType.Unknown, 100),
        (FullEquipType.Unknown, FullEquipType.Unknown, 100),
        (FullEquipType.Unknown, FullEquipType.Unknown, 100),
        (FullEquipType.Unknown, FullEquipType.Unknown, 100),
        (FullEquipType.Unknown, FullEquipType.Unknown, 100),
        (FullEquipType.Unknown, FullEquipType.Unknown, 100),
        (FullEquipType.Unknown, FullEquipType.Unknown, 100),
        (FullEquipType.Unknown, FullEquipType.Unknown, 100),
        (FullEquipType.Unknown, FullEquipType.Unknown, 100),
        (FullEquipType.Saw, FullEquipType.ClawHammer, 40),
        (FullEquipType.CrossPeinHammer, FullEquipType.File, 40),
        (FullEquipType.RaisingHammer, FullEquipType.Pliers, 40),
        (FullEquipType.LapidaryHammer, FullEquipType.GrindingWheel, 40),
        (FullEquipType.Knife, FullEquipType.Awl, 40),
        (FullEquipType.Needle, FullEquipType.SpinningWheel, 40),
        (FullEquipType.Alembic, FullEquipType.Mortar, 40),
        (FullEquipType.Frypan, FullEquipType.CulinaryKnife, 40),
        (FullEquipType.Unknown, FullEquipType.Unknown, 100),
        (FullEquipType.Unknown, FullEquipType.Unknown, 100),
        (FullEquipType.Unknown, FullEquipType.Unknown, 100),
        (FullEquipType.Unknown, FullEquipType.Unknown, 100),
        (FullEquipType.Unknown, FullEquipType.Unknown, 100),
        (FullEquipType.Unknown, FullEquipType.Unknown, 100),
        (FullEquipType.Unknown, FullEquipType.Unknown, 100),
        (FullEquipType.Unknown, FullEquipType.Unknown, 100),
        (FullEquipType.Unknown, FullEquipType.Unknown, 100),
        (FullEquipType.Unknown, FullEquipType.Unknown, 100),
        (FullEquipType.Unknown, FullEquipType.Unknown, 100),
        (FullEquipType.Unknown, FullEquipType.Unknown, 100),
        (FullEquipType.Pickaxe, FullEquipType.Sledgehammer, 50),
        (FullEquipType.Hatchet, FullEquipType.GardenScythe, 50),
        (FullEquipType.FishingRod, FullEquipType.Gig, 50),
    };

    public static FullEquipType ConvertWeaponId(SetId id)
    {
        var quotient = Math.DivRem(id.Id - 1, 100, out var remainder);
        if (quotient > WeaponIdTable.Length)
            return FullEquipType.Unknown;

        var (primary, secondary, offset) = WeaponIdTable[quotient];
        return remainder >= offset ? secondary : primary;

        // return id.Id switch
        // {
        //     > 0100 and <= 0200 => FullEquipType.Shield,
        //     > 0200 and <= 0300 => FullEquipType.Sword,
        //     > 0300 and <= 0350 => FullEquipType.Fists,
        //     > 0350 and <= 0400 => FullEquipType.FistsOff,
        //     > 0400 and <= 0500 => FullEquipType.Axe,
        //     > 0500 and <= 0600 => FullEquipType.Lance,
        //     > 0600 and <= 0650 => FullEquipType.Bow,
        //     > 0650 and <= 0700 => FullEquipType.BowOff,
        //     > 0700 and <= 0800 => FullEquipType.Wand,
        //     > 0800 and <= 0900 => FullEquipType.Staff,
        //     > 0900 and <= 1000 => FullEquipType.Wand,
        //     > 1000 and <= 1100 => FullEquipType.Staff,
        //     > 1500 and <= 1600 => FullEquipType.Broadsword,
        //     > 1600 and <= 1650 => FullEquipType.Fists,
        //     > 1650 and <= 1700 => FullEquipType.FistsOff,
        //     > 1700 and <= 1800 => FullEquipType.Book,
        //     > 1800 and <= 1850 => FullEquipType.Daggers,
        //     > 1850 and <= 1900 => FullEquipType.DaggersOff,
        //     > 2000 and <= 2050 => FullEquipType.Gun,
        //     > 2050 and <= 2100 => FullEquipType.GunOff,
        //     > 2100 and <= 2150 => FullEquipType.Orrery,
        //     > 2150 and <= 2200 => FullEquipType.OrreryOff,
        //     > 2200 and <= 2250 => FullEquipType.Katana,
        //     > 2250 and <= 2300 => FullEquipType.KatanaOff,
        //     > 2300 and <= 2350 => FullEquipType.Rapier,
        //     > 2350 and <= 2400 => FullEquipType.RapierOff,
        //     > 2400 and <= 2500 => FullEquipType.Cane,
        //     > 2500 and <= 2600 => FullEquipType.Gunblade,
        //     > 2600 and <= 2650 => FullEquipType.Glaives,
        //     > 2650 and <= 2700 => FullEquipType.GlaivesOff,
        //     > 2700 and <= 2800 => FullEquipType.Nouliths,
        //     > 2800 and <= 2900 => FullEquipType.Scythe,
        //     > 5040 and <= 5100 => FullEquipType.ClawHammer,
        //     > 5100 and <= 5140 => FullEquipType.CrossPeinHammer,
        //     > 5140 and <= 5200 => FullEquipType.File,
        //     > 5200 and <= 5240 => FullEquipType.RaisingHammer,
        //     > 5240 and <= 5300 => FullEquipType.Pliers,
        //     > 5300 and <= 5340 => FullEquipType.LapidaryHammer,
        //     > 5340 and <= 5400 => FullEquipType.GrindingWheel,
        //     > 5400 and <= 5440 => FullEquipType.Knife,
        //     > 5440 and <= 5500 => FullEquipType.Awl,
        //     > 5500 and <= 5540 => FullEquipType.Needle,
        //     > 5540 and <= 5600 => FullEquipType.SpinningWheel,
        //     > 5600 and <= 5640 => FullEquipType.Alembic,
        //     > 5640 and <= 5700 => FullEquipType.Mortar,
        //     > 5700 and <= 5740 => FullEquipType.Frypan,
        //     > 5740 and <= 5800 => FullEquipType.CulinaryKnife,
        //     > 7000 and <= 7050 => FullEquipType.Pickaxe,
        //     > 7050 and <= 7100 => FullEquipType.Sledgehammer,
        //     > 7100 and <= 7150 => FullEquipType.Hatchet,
        //     > 7150 and <= 7200 => FullEquipType.GardenScythe,
        //     > 7200 and <= 7250 => FullEquipType.FishingRod,
        //     > 7250 and <= 7300 => FullEquipType.Gig,
        //     _ => FullEquipType.Unknown,
        // };
    }
}
