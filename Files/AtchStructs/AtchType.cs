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
    UnknownAtr = ((uint)'a' << 16) | ((uint)'t' << 8) | 'r',
    UnknownAvt = ((uint)'a' << 16) | ((uint)'v' << 8) | 't',

    [Name("Aetherotransformer (Machinist)")]
    Aetherotransformer = ((uint)'b' << 16) | ((uint)'a' << 8) | 'g',
    UnknownBl2 = ((uint)'b' << 16) | ((uint)'l' << 8) | '2',
    UnknownBld = ((uint)'b' << 16) | ((uint)'l' << 8) | 'd',
    UnknownBll = ((uint)'b' << 16) | ((uint)'l' << 8) | 'l',
    UnknownBrs = ((uint)'b' << 16) | ((uint)'r' << 8) | 's',

    [Name("Chakrams")]
    Chakrams = ((uint)'c' << 16) | ((uint)'h' << 8) | 'k',
    UnknownClb = ((uint)'c' << 16) | ((uint)'l' << 8) | 'b',

    [Name("Gloves")]
    Gloves = ((uint)'c' << 16) | ((uint)'l' << 8) | 'g',
    UnknownCls = ((uint)'c' << 16) | ((uint)'l' << 8) | 's', // Linked to axes

    [Name("Claws")]
    Claws = ((uint)'c' << 16) | ((uint)'l' << 8) | 'w',
    UnknownCol = ((uint)'c' << 16) | ((uint)'o' << 8) | 'l',
    UnknownCor = ((uint)'c' << 16) | ((uint)'o' << 8) | 'r',
    UnknownCos = ((uint)'c' << 16) | ((uint)'o' << 8) | 's',

    [Name("Cardholder")]
    Cardholder = ((uint)'c' << 16) | ((uint)'r' << 8) | 'd',
    UnknownCrr = ((uint)'c' << 16) | ((uint)'r' << 8) | 'r',
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
    UnknownEgp = ((uint)'e' << 16) | ((uint)'g' << 8) | 'p',
    UnknownElg = ((uint)'e' << 16) | ((uint)'l' << 8) | 'g',
    UnknownFcb = ((uint)'f' << 16) | ((uint)'c' << 8) | 'b',
    UnknownFch = ((uint)'f' << 16) | ((uint)'c' << 8) | 'h',
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
    UnknownFud = ((uint)'f' << 16) | ((uint)'u' << 8) | 'd',
    UnknownGdb = ((uint)'g' << 16) | ((uint)'d' << 8) | 'b',
    UnknownGdh = ((uint)'g' << 16) | ((uint)'d' << 8) | 'h',
    UnknownGdl = ((uint)'g' << 16) | ((uint)'d' << 8) | 'l',
    UnknownGdr = ((uint)'g' << 16) | ((uint)'d' << 8) | 'r',
    UnknownGdt = ((uint)'g' << 16) | ((uint)'d' << 8) | 't',
    UnknownGdw = ((uint)'g' << 16) | ((uint)'d' << 8) | 'w',

    [Name("Deployable (Machinist)")]
    MachinistDeployable = ((uint)'g' << 16) | ((uint)'s' << 8) | 'l',

    [Name("Diadem Cannon")]
    DiademCannon = ((uint)'g' << 16) | ((uint)'s' << 8) | 'r', // unsure
    UnknownGun = ((uint)'g' << 16) | ((uint)'u' << 8) | 'n',

    [Name("")]
    UnknownHel = ((uint)'h' << 16) | ((uint)'e' << 8) | 'l',

    [Name("Blacksmith / Armorer")]
    BlacksmithArmorer = ((uint)'h' << 16) | ((uint)'m' << 8) | 'm',

    [Name("Harp (Bard)")]
    BardHarp = ((uint)'h' << 16) | ((uint)'r' << 8) | 'p',

    [Name("Hatchet")]
    Hatchet = ((uint)'h' << 16) | ((uint)'t' << 8) | 'c',

    [Name("Katana Sheathe")]
    KatanaSheathe = ((uint)'k' << 16) | ((uint)'s' << 8) | 'h',
    UnknownLet = ((uint)'l' << 16) | ((uint)'e' << 8) | 't',
    UnknownLpr = ((uint)'l' << 16) | ((uint)'p' << 8) | 'r', // Linked to 1923

    [Name("Goldsmith (MLT)")]
    GoldsmithMlt = ((uint)'m' << 16) | ((uint)'l' << 8) | 't',
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
    UnknownNik = ((uint)'n' << 16) | ((uint)'i' << 8) | 'k', // Linked to Nier pod
    UnknownNjd = ((uint)'n' << 16) | ((uint)'j' << 8) | 'd',

    [Name("Botanist (NPH)")]
    BotanistNph = ((uint)'n' << 16) | ((uint)'p' << 8) | 'h',

    [Name("Focus (Redmage)")]
    Focus = ((uint)'o' << 16) | ((uint)'r' << 8) | 'b',
    UnknownOum = ((uint)'o' << 16) | ((uint)'u' << 8) | 'm',
    UnknownPen = ((uint)'p' << 16) | ((uint)'e' << 8) | 'n', // Linked to daggers

    [Name("Pickaxe")]
    Pickaxe = ((uint)'p' << 16) | ((uint)'i' << 8) | 'c',
    UnknownPlt = ((uint)'p' << 16) | ((uint)'l' << 8) | 't',
    UnknownPra = ((uint)'p' << 16) | ((uint)'r' << 8) | 'a',
    UnknownPrc = ((uint)'p' << 16) | ((uint)'r' << 8) | 'c',

    [Name("Leatherworker (PRF)")]
    LeatherworkerPrf = ((uint)'p' << 16) | ((uint)'r' << 8) | 'f',

    [Name("Quiver (Bard)")]
    Quiver = ((uint)'q' << 16) | ((uint)'v' << 8) | 'r',
    UnknownRap = ((uint)'r' << 16) | ((uint)'a' << 8) | 'p',

    [Name("Rabbit Medium (Ninja)")]
    NinjaRabbit = ((uint)'r' << 16) | ((uint)'b' << 8) | 't',
    UnknownRec = ((uint)'r' << 16) | ((uint)'e' << 8) | 'c',

    [Name("Cane")]
    Cane = ((uint)'r' << 16) | ((uint)'o' << 8) | 'd',
    UnknownRop = ((uint)'r' << 16) | ((uint)'o' << 8) | 'p',
    UnknownRp1 = ((uint)'r' << 16) | ((uint)'o' << 8) | '1',

    [Name("Saw")]
    Saw = ((uint)'s' << 16) | ((uint)'a' << 8) | 'w',
    UnknownSbt = ((uint)'s' << 16) | ((uint)'b' << 8) | 't',
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
    UnknownSxs = ((uint)'s' << 16) | ((uint)'x' << 8) | 's',
    UnknownSxw = ((uint)'s' << 16) | ((uint)'x' << 8) | 'w',

    [Name("Sniper Rifle (Machinist)")]
    SniperRifle = ((uint)'s' << 16) | ((uint)'y' << 8) | 'l',
    UnknownSyr = ((uint)'s' << 16) | ((uint)'y' << 8) | 'r',
    UnknownSyu = ((uint)'s' << 16) | ((uint)'y' << 8) | 'u',
    UnknownSyw = ((uint)'s' << 16) | ((uint)'y' << 8) | 'w',
    UnknownTan = ((uint)'t' << 16) | ((uint)'a' << 8) | 'n',

    [Name("Goldsmith (TBL)")]
    GoldsmithTbl = ((uint)'t' << 16) | ((uint)'b' << 8) | 'l',
    UnknownTcs = ((uint)'t' << 16) | ((uint)'c' << 8) | 's',

    [Name("Goldsmith (TGN)")]
    GoldsmithTgn = ((uint)'t' << 16) | ((uint)'g' << 8) | 'n',

    [Name("Weaver (TMB)")]
    WeaverTmb = ((uint)'t' << 16) | ((uint)'m' << 8) | 'b',
    UnknownTms = ((uint)'t' << 16) | ((uint)'m' << 8) | 's',
    UnknownTrm = ((uint)'t' << 16) | ((uint)'r' << 8) | 'm', // Linked to Flutes
    UnkownnTrr = ((uint)'t' << 16) | ((uint)'r' << 8) | 'r',
    UnknownTrw = ((uint)'t' << 16) | ((uint)'r' << 8) | 'w', // Linked to Greatswords
    UnknownVln = ((uint)'t' << 16) | ((uint)'l' << 8) | 'n',

    [Name("Weaver (WHL)")]
    WeaverWhl = ((uint)'w' << 16) | ((uint)'h' << 8) | 'l',
    UnknownWng = ((uint)'w' << 16) | ((uint)'n' << 8) | 'g',
    UnknownYpd = ((uint)'y' << 16) | ((uint)'p' << 8) | 'd',

    [Name("Armorer (YTK)")]
    ArmorerYtk = ((uint)'y' << 16) | ((uint)'t' << 8) | 'k',
}

public static partial class AtchExtensions
{
    public static string ToAbbreviation(this AtchType type)
        => (uint)type > 0x00FFFFFF
            ? $"{(char)(((uint)type >> 24) & 0xFF)}{(char)(((uint)type >> 16) & 0xFF)}{(char)(((uint)type >> 8) & 0xFF)}{(char)((uint)type & 0xFF)}"
            : $"{(char)(((uint)type >> 16) & 0xFF)}{(char)(((uint)type >> 8) & 0xFF)}{(char)((uint)type & 0xFF)}";

    public static string ToName(this AtchType type)
    {
        var ret = type.ToKnownName();
        return ret[0] is '_' ? type.ToAbbreviation() : ret;
    }

    public static ReadOnlySpan<byte> ToNameU8(this AtchType type)
    {
        var ret = type.ToKnownNameU8();
        return ret[0] is (byte)'_' ? Encoding.UTF8.GetBytes(type.ToAbbreviation()) : ret;
    }

    public static AtchType FromChars(char first, char second, char third, char fourth = '\0')
        => (AtchType)(((uint)first & 0xFF) | (((uint)second & 0xFF) << 8) | (((uint)third & 0xFF) << 16) | (((uint)fourth & 0xFF) << 24));

    public static AtchType FromString(string text)
        => text.Length switch
        {
            1 => FromChars(text[0], '\0',    '\0'),
            2 => FromChars(text[1], text[0], '\0'),
            3 => FromChars(text[2], text[1], text[0]),
            4 => FromChars(text[3], text[2], text[1], text[0]),
            _ => AtchType.Unknown,
        };
}
