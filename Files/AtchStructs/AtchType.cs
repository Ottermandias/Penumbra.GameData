using Luna.Generators;

namespace Penumbra.GameData.Files.AtchStructs;

[NamedEnum("ToKnownName", Unknown: "_")]
public enum AtchType : uint
{
    Unknown = 0,

    [Name("Greataxe")]
    GreatAxe = ((uint)'2' << 16) | ((uint)'a' << 8) | 'x',

    [Name("Book")]
    Book = ((uint)'2' << 16) | ((uint)'b' << 8) | 'k',

    [Name("Bow")]
    Bow = ((uint)'2' << 16) | ((uint)'b' << 8) | 'w',

    [Name("Nouliths")]
    Nouliths = ((uint)'2' << 16) | ((uint)'f' << 8) | 'f',

    [Name("Gunblade")]
    Gunblade = ((uint)'2' << 16) | ((uint)'g' << 8) | 'b',

    [Name("Globe")]
    Globe = ((uint)'2' << 16) | ((uint)'g' << 8) | 'l',

    [Name("Gun")]
    Gun = ((uint)'2' << 16) | ((uint)'g' << 8) | 'n',

    [Name("Scythe")]
    Scythe = ((uint)'2' << 16) | ((uint)'k' << 8) | 'm',

    [Name("Katana")]
    Katana = ((uint)'2' << 16) | ((uint)'k' << 8) | 't',

    [Name(Omit: true)]
    Unknown2kz = ((uint)'2' << 16) | ((uint)'k' << 8) | 'z',

    [Name("Rapier")]
    Rapier = ((uint)'2' << 16) | ((uint)'r' << 8) | 'p',

    [Name("Spear")]
    Spear = ((uint)'2' << 16) | ((uint)'s' << 8) | 'p',

    [Name("Greatstaff")]
    Greatstaff = ((uint)'2' << 16) | ((uint)'s' << 8) | 't',

    [Name("Greatsword")]
    Greatsword = ((uint)'2' << 16) | ((uint)'s' << 8) | 'w',

    [Name("Alchemist (AAI)")]
    AlchemistAai = ((uint)'a' << 16) | ((uint)'a' << 8) | 'i',

    [Name("Alchemist (AAL)")]
    AlchemistAal = ((uint)'a' << 16) | ((uint)'a' << 8) | 'l',

    [Name("Armorer (AAR)")]
    ArmorerAar = ((uint)'a' << 16) | ((uint)'a' << 8) | 'r',

    [Name("Armorer (ABL)")]
    ArmorerAbl = ((uint)'a' << 16) | ((uint)'b' << 8) | 'l',

    [Name("Culinarian (ACO)")]
    CulinarianAco = ((uint)'a' << 16) | ((uint)'c' << 8) | 'o',

    [Name("Goldsmith (AGL)")]
    GoldsmithAgl = ((uint)'a' << 16) | ((uint)'g' << 8) | 'l',

    [Name("Alchemist (ALI)")]
    AlchemistAli = ((uint)'a' << 16) | ((uint)'l' << 8) | 'i',

    [Name("Alchemist (ALM)")]
    AlchemistAlm = ((uint)'a' << 16) | ((uint)'l' << 8) | 'm',

    [Name("Leatherworker (ALT)")]
    LeatherworkerAlt = ((uint)'a' << 16) | ((uint)'l' << 8) | 't',

    [Name("Weaver (ASE)")]
    WeaverAse = ((uint)'a' << 16) | ((uint)'s' << 8) | 'e',

    [Name("Carpenter (AWO)")]
    CarpenterAwo = ((uint)'a' << 16) | ((uint)'w' << 8) | 'o',

    [Name(Omit: true)]
    UnknownAtr = ((uint)'a' << 16) | ((uint)'t' << 8) | 'r',

    [Name(Omit: true)]
    UnknownAvt = ((uint)'a' << 16) | ((uint)'v' << 8) | 't',

    [Name("Aetherotransformer (Machinist)")]
    Aetherotransformer = ((uint)'b' << 16) | ((uint)'a' << 8) | 'g',

    [Name("Twinblade (Left Hand)")]
    Bl2 = ((uint)'b' << 16) | ((uint)'l' << 8) | '2',

    [Name("Twinblade (Right Hand)")]
    Bld = ((uint)'b' << 16) | ((uint)'l' << 8) | 'd',

    [Name(Omit: true)]
    UnknownBll = ((uint)'b' << 16) | ((uint)'l' << 8) | 'l',

    [Name(Omit: true)]
    UnknownBrs = ((uint)'b' << 16) | ((uint)'r' << 8) | 's',

    [Name("Chakrams")]
    Chakrams = ((uint)'c' << 16) | ((uint)'h' << 8) | 'k',

    [Name(Omit: true)]
    UnknownClb = ((uint)'c' << 16) | ((uint)'l' << 8) | 'b',

    [Name("Gloves")]
    Gloves = ((uint)'c' << 16) | ((uint)'l' << 8) | 'g',

    [Name(Omit: true)]
    UnknownCls = ((uint)'c' << 16) | ((uint)'l' << 8) | 's', // Linked to axes

    [Name("Claws")]
    Claws = ((uint)'c' << 16) | ((uint)'l' << 8) | 'w',

    [Name(Omit: true)]
    UnknownCol = ((uint)'c' << 16) | ((uint)'o' << 8) | 'l',

    [Name(Omit: true)]
    UnknownCor = ((uint)'c' << 16) | ((uint)'o' << 8) | 'r',

    [Name(Omit: true)]
    UnknownCos = ((uint)'c' << 16) | ((uint)'o' << 8) | 's',

    [Name("Cardholder")]
    Cardholder = ((uint)'c' << 16) | ((uint)'r' << 8) | 'd',

    [Name(Omit: true)]
    UnknownCrr = ((uint)'c' << 16) | ((uint)'r' << 8) | 'r',

    [Name(Omit: true)]
    UnknownCrt = ((uint)'c' << 16) | ((uint)'r' << 8) | 't',

    [Name("Carpenter (CSL)")]
    CarpenterCsl = ((uint)'c' << 16) | ((uint)'s' << 8) | 'l',

    [Name("Carpenter (CSR)")]
    CarpenterCsr = ((uint)'c' << 16) | ((uint)'s' << 8) | 'r',

    [Name("Daggers")]
    Daggers = ((uint)'d' << 16) | ((uint)'g' << 8) | 'r',

    [Name("Drums")]
    Drums = ((uint)'d' << 16) | ((uint)'r' << 8) | 'm',

    [Name("Shroud (Reaper)")]
    ReaperShroud = ((uint)'e' << 16) | ((uint)'b' << 8) | 'z',

    [Name(Omit: true)]
    UnknownEgp = ((uint)'e' << 16) | ((uint)'g' << 8) | 'p',

    [Name(Omit: true)]
    UnknownElg = ((uint)'e' << 16) | ((uint)'l' << 8) | 'g',

    [Name(Omit: true)]
    UnknownFcb = ((uint)'f' << 16) | ((uint)'c' << 8) | 'b',

    [Name(Omit: true)]
    UnknownFch = ((uint)'f' << 16) | ((uint)'c' << 8) | 'h',

    [Name(Omit: true)]
    UnknownFdr = ((uint)'f' << 16) | ((uint)'d' << 8) | 'r',

    [Name("Fisher (FHA)")]
    FisherFha = ((uint)'f' << 16) | ((uint)'h' << 8) | 'a',

    [Name("Harp")]
    Harp = ((uint)'f' << 16) | ((uint)'l' << 8) | '2',

    [Name("Flute")]
    Flute = ((uint)'f' << 16) | ((uint)'l' << 8) | 't',

    [Name("Hellfrog Medium (Ninja)")]
    NinjaFrog = ((uint)'f' << 16) | ((uint)'r' << 8) | 'g',

    [Name("Leatherworker / Culinarian")]
    LeatherworkerCulinarian = ((uint)'f' << 16) | ((uint)'r' << 8) | 'y',

    [Name("Fisher (FSH)")]
    FisherFsh = ((uint)'f' << 16) | ((uint)'s' << 8) | 'h',

    [Name("Fist Weapons")]
    FistWeapons = ((uint)'f' << 16) | ((uint)'s' << 8) | 'w',

    [Name(Omit: true)]
    UnknownFud = ((uint)'f' << 16) | ((uint)'u' << 8) | 'd',

    [Name(Omit: true)]
    UnknownGdb = ((uint)'g' << 16) | ((uint)'d' << 8) | 'b',

    [Name(Omit: true)]
    UnknownGdh = ((uint)'g' << 16) | ((uint)'d' << 8) | 'h',

    [Name(Omit: true)]
    UnknownGdl = ((uint)'g' << 16) | ((uint)'d' << 8) | 'l',

    [Name(Omit: true)]
    UnknownGdr = ((uint)'g' << 16) | ((uint)'d' << 8) | 'r',

    [Name(Omit: true)]
    UnknownGdt = ((uint)'g' << 16) | ((uint)'d' << 8) | 't',

    [Name(Omit: true)]
    UnknownGdw = ((uint)'g' << 16) | ((uint)'d' << 8) | 'w',

    [Name("Deployable (Machinist)")]
    MachinistDeployable = ((uint)'g' << 16) | ((uint)'s' << 8) | 'l',

    [Name("Diadem Cannon")]
    DiademCannon = ((uint)'g' << 16) | ((uint)'s' << 8) | 'r', // unsure

    [Name(Omit: true)]
    UnknownGun = ((uint)'g' << 16) | ((uint)'u' << 8) | 'n',

    [Name(Omit: true)]
    UnknownHel = ((uint)'h' << 16) | ((uint)'e' << 8) | 'l',

    [Name("Blacksmith / Armorer")]
    BlacksmithArmorer = ((uint)'h' << 16) | ((uint)'m' << 8) | 'm',

    [Name("Harp (Bard)")]
    BardHarp = ((uint)'h' << 16) | ((uint)'r' << 8) | 'p',

    [Name("Hatchet")]
    Hatchet = ((uint)'h' << 16) | ((uint)'t' << 8) | 'c',

    [Name("Katana Sheathe")]
    KatanaSheathe = ((uint)'k' << 16) | ((uint)'s' << 8) | 'h',

    [Name(Omit: true)]
    UnknownLet = ((uint)'l' << 16) | ((uint)'e' << 8) | 't',

    [Name(Omit: true)]
    UnknownLpr = ((uint)'l' << 16) | ((uint)'p' << 8) | 'r', // Linked to 1923

    [Name("Goldsmith (MLT)")]
    GoldsmithMlt = ((uint)'m' << 16) | ((uint)'l' << 8) | 't',

    [Name(Omit: true)]
    UnknownMmc = ((uint)'m' << 16) | ((uint)'m' << 8) | 'c',

    [Name("Alchemist (MRB)")]
    AlchemistMrb = ((uint)'m' << 16) | ((uint)'r' << 8) | 'b',

    [Name("Alchemist (MRH)")]
    AlchemistMrh = ((uint)'m' << 16) | ((uint)'r' << 8) | 'h',

    [Name("Shotgun (Machinist)")]
    Shotgun = ((uint)'m' << 16) | ((uint)'s' << 8) | 'g',

    [Name("Cannon (Machinist)")]
    Cannon = ((uint)'m' << 16) | ((uint)'w' << 8) | 'p',

    [Name("Weaver (NDL)")]
    WeaverNdl = ((uint)'n' << 16) | ((uint)'d' << 8) | 'l',

    [Name(Omit: true)]
    UnknownNik = ((uint)'n' << 16) | ((uint)'i' << 8) | 'k', // Linked to Nier pod

    [Name(Omit: true)]
    UnknownNjd = ((uint)'n' << 16) | ((uint)'j' << 8) | 'd',

    [Name("Botanist (NPH)")]
    BotanistNph = ((uint)'n' << 16) | ((uint)'p' << 8) | 'h',

    [Name("Focus (Redmage)")]
    Focus = ((uint)'o' << 16) | ((uint)'r' << 8) | 'b',

    [Name(Omit: true)]
    UnknownOum = ((uint)'o' << 16) | ((uint)'u' << 8) | 'm',

    [Name(Omit: true)]
    UnknownPen = ((uint)'p' << 16) | ((uint)'e' << 8) | 'n', // Linked to daggers

    [Name("Pickaxe")]
    Pickaxe = ((uint)'p' << 16) | ((uint)'i' << 8) | 'c',

    [Name(Omit: true)]
    UnknownPlt = ((uint)'p' << 16) | ((uint)'l' << 8) | 't',

    [Name(Omit: true)]
    UnknownPra = ((uint)'p' << 16) | ((uint)'r' << 8) | 'a',

    [Name(Omit: true)]
    UnknownPrc = ((uint)'p' << 16) | ((uint)'r' << 8) | 'c',

    [Name("Leatherworker (PRF)")]
    LeatherworkerPrf = ((uint)'p' << 16) | ((uint)'r' << 8) | 'f',

    [Name("Quiver (Bard)")]
    Quiver = ((uint)'q' << 16) | ((uint)'v' << 8) | 'r',

    [Name(Omit: true)]
    UnknownRap = ((uint)'r' << 16) | ((uint)'a' << 8) | 'p',

    [Name("Rabbit Medium (Ninja)")]
    NinjaRabbit = ((uint)'r' << 16) | ((uint)'b' << 8) | 't',

    [Name(Omit: true)]
    UnknownRec = ((uint)'r' << 16) | ((uint)'e' << 8) | 'c',

    [Name("Cane")]
    Cane = ((uint)'r' << 16) | ((uint)'o' << 8) | 'd',

    [Name(Omit: true)]
    UnknownRop = ((uint)'r' << 16) | ((uint)'o' << 8) | 'p',

    [Name(Omit: true)]
    UnknownRp1 = ((uint)'r' << 16) | ((uint)'o' << 8) | '1',

    [Name("Saw")]
    Saw = ((uint)'s' << 16) | ((uint)'a' << 8) | 'w',

    [Name(Omit: true)]
    UnknownSbt = ((uint)'s' << 16) | ((uint)'b' << 8) | 't',

    [Name(Omit: true)]
    UnknownSht = ((uint)'s' << 16) | ((uint)'h' << 8) | 't',

    [Name("Fisher (SIC)")]
    FisherSic = ((uint)'s' << 16) | ((uint)'i' << 8) | 'c',

    [Name("Shield")]
    Shield = ((uint)'s' << 16) | ((uint)'l' << 8) | 'd',

    [Name("Staff")]
    Staff = ((uint)'s' << 16) | ((uint)'t' << 8) | 'f',

    [Name("Culinarian (STV)")]
    CulinarianStv = ((uint)'s' << 16) | ((uint)'t' << 8) | 'v',

    [Name("Sword")]
    Sword = ((uint)'s' << 16) | ((uint)'w' << 8) | 'd',

    [Name(Omit: true)]
    UnknownSxs = ((uint)'s' << 16) | ((uint)'x' << 8) | 's',

    [Name(Omit: true)]
    UnknownSxw = ((uint)'s' << 16) | ((uint)'x' << 8) | 'w',

    [Name("Sniper Rifle (Machinist)")]
    SniperRifle = ((uint)'s' << 16) | ((uint)'y' << 8) | 'l',

    [Name(Omit: true)]
    UnknownSyr = ((uint)'s' << 16) | ((uint)'y' << 8) | 'r',

    [Name(Omit: true)]
    UnknownSyu = ((uint)'s' << 16) | ((uint)'y' << 8) | 'u',

    [Name(Omit: true)]
    UnknownSyw = ((uint)'s' << 16) | ((uint)'y' << 8) | 'w',

    [Name(Omit: true)]
    UnknownTan = ((uint)'t' << 16) | ((uint)'a' << 8) | 'n',

    [Name("Goldsmith (TBL)")]
    GoldsmithTbl = ((uint)'t' << 16) | ((uint)'b' << 8) | 'l',

    [Name(Omit: true)]
    UnknownTcs = ((uint)'t' << 16) | ((uint)'c' << 8) | 's',

    [Name("Goldsmith (TGN)")]
    GoldsmithTgn = ((uint)'t' << 16) | ((uint)'g' << 8) | 'n',

    [Name("Weaver (TMB)")]
    WeaverTmb = ((uint)'t' << 16) | ((uint)'m' << 8) | 'b',

    [Name(Omit: true)]
    UnknownTms = ((uint)'t' << 16) | ((uint)'m' << 8) | 's',

    [Name(Omit: true)]
    UnknownTrm = ((uint)'t' << 16) | ((uint)'r' << 8) | 'm', // Linked to Flutes

    [Name(Omit: true)]
    UnkownnTrr = ((uint)'t' << 16) | ((uint)'r' << 8) | 'r',

    [Name(Omit: true)]
    UnknownTrw = ((uint)'t' << 16) | ((uint)'r' << 8) | 'w', // Linked to Greatswords

    [Name(Omit: true)]
    UnknownVln = ((uint)'t' << 16) | ((uint)'l' << 8) | 'n',

    [Name("Weaver (WHL)")]
    WeaverWhl = ((uint)'w' << 16) | ((uint)'h' << 8) | 'l',

    [Name(Omit: true)]
    UnknownWng = ((uint)'w' << 16) | ((uint)'n' << 8) | 'g',

    [Name(Omit: true)]
    UnknownYpd = ((uint)'y' << 16) | ((uint)'p' << 8) | 'd',

    [Name("Armorer (YTK)")]
    ArmorerYtk = ((uint)'y' << 16) | ((uint)'t' << 8) | 'k',
}

public static partial class AtchExtensions
{
    extension(AtchType type)
    {
        public string ToAbbreviation()
            => (uint)type > 0x00FFFFFF
                ? $"{(char)(((uint)type >> 24) & 0xFF)}{(char)(((uint)type >> 16) & 0xFF)}{(char)(((uint)type >> 8) & 0xFF)}{(char)((uint)type & 0xFF)}"
                : $"{(char)(((uint)type >> 16) & 0xFF)}{(char)(((uint)type >> 8) & 0xFF)}{(char)((uint)type & 0xFF)}";

        public string ToName()
        {
            var ret = type.ToKnownName();
            return ret[0] is '_' ? type.ToAbbreviation() : ret;
        }

        public ReadOnlySpan<byte> ToNameU8()
        {
            var ret = type.ToKnownNameU8();
            return ret[0] is (byte)'_' ? Encoding.UTF8.GetBytes(type.ToAbbreviation()) : ret;
        }

        public static AtchType FromChars(char first, char second, char third, char fourth = '\0')
            => (AtchType)(((uint)first & 0xFF) | (((uint)second & 0xFF) << 8) | (((uint)third & 0xFF) << 16) | (((uint)fourth & 0xFF) << 24));

        public static AtchType FromString(ReadOnlySpan<byte> text)
            => text.Length switch
            {
                1 => FromChars((char)text[0], '\0',          '\0'),
                2 => FromChars((char)text[1], (char)text[0], '\0'),
                3 => FromChars((char)text[2], (char)text[1], (char)text[0]),
                4 => FromChars((char)text[3], (char)text[2], (char)text[1], (char)text[0]),
                _ => AtchType.Unknown,
            };

        public static AtchType FromString(ReadOnlySpan<char> text)
            => text.Length switch
            {
                1 => FromChars(text[0], '\0',    '\0'),
                2 => FromChars(text[1], text[0], '\0'),
                3 => FromChars(text[2], text[1], text[0]),
                4 => FromChars(text[3], text[2], text[1], text[0]),
                _ => AtchType.Unknown,
            };
    }
}
