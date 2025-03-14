namespace Penumbra.GameData.Files.AtchStructs;

public enum AtchType : uint
{
    Unknown                 = 0,
    GreatAxe                = ((uint)'2' << 16) | ((uint)'a' << 8) | 'x',
    Book                    = ((uint)'2' << 16) | ((uint)'b' << 8) | 'k',
    Bow                     = ((uint)'2' << 16) | ((uint)'b' << 8) | 'w',
    Nouliths                = ((uint)'2' << 16) | ((uint)'f' << 8) | 'f',
    Gunblade                = ((uint)'2' << 16) | ((uint)'g' << 8) | 'b',
    Globe                   = ((uint)'2' << 16) | ((uint)'g' << 8) | 'l',
    Gun                     = ((uint)'2' << 16) | ((uint)'g' << 8) | 'n',
    Scythe                  = ((uint)'2' << 16) | ((uint)'k' << 8) | 'm',
    Katana                  = ((uint)'2' << 16) | ((uint)'k' << 8) | 't',
    Unknown2kz              = ((uint)'2' << 16) | ((uint)'k' << 8) | 'z',
    Rapier                  = ((uint)'2' << 16) | ((uint)'r' << 8) | 'p',
    Spear                   = ((uint)'2' << 16) | ((uint)'s' << 8) | 'p',
    Greatstaff              = ((uint)'2' << 16) | ((uint)'s' << 8) | 't',
    Greatsword              = ((uint)'2' << 16) | ((uint)'s' << 8) | 'w',
    AlchemistAai            = ((uint)'a' << 16) | ((uint)'a' << 8) | 'i',
    AlchemistAal            = ((uint)'a' << 16) | ((uint)'a' << 8) | 'l',
    ArmorerAar              = ((uint)'a' << 16) | ((uint)'a' << 8) | 'r',
    ArmorerAbl              = ((uint)'a' << 16) | ((uint)'b' << 8) | 'l',
    CulinarianAco           = ((uint)'a' << 16) | ((uint)'c' << 8) | 'o',
    GoldsmithAgl            = ((uint)'a' << 16) | ((uint)'g' << 8) | 'l',
    AlchemistAli            = ((uint)'a' << 16) | ((uint)'l' << 8) | 'i',
    AlchemistAlm            = ((uint)'a' << 16) | ((uint)'l' << 8) | 'm',
    LeatherworkerAlt        = ((uint)'a' << 16) | ((uint)'l' << 8) | 't',
    WeaverAse               = ((uint)'a' << 16) | ((uint)'s' << 8) | 'e',
    CarpenterAwo            = ((uint)'a' << 16) | ((uint)'w' << 8) | 'o',
    UnknownAtr              = ((uint)'a' << 16) | ((uint)'t' << 8) | 'r',
    UnknownAvt              = ((uint)'a' << 16) | ((uint)'v' << 8) | 't',
    Aetherotransformer      = ((uint)'b' << 16) | ((uint)'a' << 8) | 'g',
    UnknownBl2              = ((uint)'b' << 16) | ((uint)'l' << 8) | '2',
    UnknownBld              = ((uint)'b' << 16) | ((uint)'l' << 8) | 'd',
    UnknownBll              = ((uint)'b' << 16) | ((uint)'l' << 8) | 'l',
    UnknownBrs              = ((uint)'b' << 16) | ((uint)'r' << 8) | 's',
    Chakrams                = ((uint)'c' << 16) | ((uint)'h' << 8) | 'k',
    UnknownClb              = ((uint)'c' << 16) | ((uint)'l' << 8) | 'b',
    Gloves                  = ((uint)'c' << 16) | ((uint)'l' << 8) | 'g',
    UnknownCls              = ((uint)'c' << 16) | ((uint)'l' << 8) | 's', // Linked to axes
    Claws                   = ((uint)'c' << 16) | ((uint)'l' << 8) | 'w',
    UnknownCol              = ((uint)'c' << 16) | ((uint)'o' << 8) | 'l',
    UnknownCor              = ((uint)'c' << 16) | ((uint)'o' << 8) | 'r',
    UnknownCos              = ((uint)'c' << 16) | ((uint)'o' << 8) | 's',
    Cardholder              = ((uint)'c' << 16) | ((uint)'r' << 8) | 'd',
    UnknownCrr              = ((uint)'c' << 16) | ((uint)'r' << 8) | 'r',
    UnknownCrt              = ((uint)'c' << 16) | ((uint)'r' << 8) | 't',
    CarpenterCsl            = ((uint)'c' << 16) | ((uint)'s' << 8) | 'l',
    CarpenterCsr            = ((uint)'c' << 16) | ((uint)'s' << 8) | 'r',
    Daggers                 = ((uint)'d' << 16) | ((uint)'g' << 8) | 'r',
    Drums                   = ((uint)'d' << 16) | ((uint)'r' << 8) | 'm',
    ReaperShroud            = ((uint)'e' << 16) | ((uint)'b' << 8) | 'z',
    UnknownEgp              = ((uint)'e' << 16) | ((uint)'g' << 8) | 'p',
    UnknownElg              = ((uint)'e' << 16) | ((uint)'l' << 8) | 'g',
    UnknownFcb              = ((uint)'f' << 16) | ((uint)'c' << 8) | 'b',
    UnknownFch              = ((uint)'f' << 16) | ((uint)'c' << 8) | 'h',
    UnknownFdr              = ((uint)'f' << 16) | ((uint)'d' << 8) | 'r',
    FisherFha               = ((uint)'f' << 16) | ((uint)'h' << 8) | 'a',
    Harp                    = ((uint)'f' << 16) | ((uint)'l' << 8) | '2',
    Flute                   = ((uint)'f' << 16) | ((uint)'l' << 8) | 't',
    NinjaFrog               = ((uint)'f' << 16) | ((uint)'r' << 8) | 'g',
    LeatherworkerCulinarian = ((uint)'f' << 16) | ((uint)'r' << 8) | 'y',
    FisherFsh               = ((uint)'f' << 16) | ((uint)'s' << 8) | 'h',
    FistWeapons             = ((uint)'f' << 16) | ((uint)'s' << 8) | 'w',
    UnknownFud              = ((uint)'f' << 16) | ((uint)'u' << 8) | 'd',
    UnknownGdb              = ((uint)'g' << 16) | ((uint)'d' << 8) | 'b',
    UnknownGdh              = ((uint)'g' << 16) | ((uint)'d' << 8) | 'h',
    UnknownGdl              = ((uint)'g' << 16) | ((uint)'d' << 8) | 'l',
    UnknownGdr              = ((uint)'g' << 16) | ((uint)'d' << 8) | 'r',
    UnknownGdt              = ((uint)'g' << 16) | ((uint)'d' << 8) | 't',
    UnknownGdw              = ((uint)'g' << 16) | ((uint)'d' << 8) | 'w',
    MachinistDeployable     = ((uint)'g' << 16) | ((uint)'s' << 8) | 'l',
    DiademCannon            = ((uint)'g' << 16) | ((uint)'s' << 8) | 'r', // unsure
    UnknownGun              = ((uint)'g' << 16) | ((uint)'u' << 8) | 'n',
    UnknownHel              = ((uint)'h' << 16) | ((uint)'e' << 8) | 'l',
    BlacksmithArmorer       = ((uint)'h' << 16) | ((uint)'m' << 8) | 'm',
    BardHarp                = ((uint)'h' << 16) | ((uint)'r' << 8) | 'p',
    Hatchet                 = ((uint)'h' << 16) | ((uint)'t' << 8) | 'c',
    KatanaSheathe           = ((uint)'k' << 16) | ((uint)'s' << 8) | 'h',
    UnknownLet              = ((uint)'l' << 16) | ((uint)'e' << 8) | 't',
    UnknownLpr              = ((uint)'l' << 16) | ((uint)'p' << 8) | 'r', // Linked to 1923
    GoldsmithMlt            = ((uint)'m' << 16) | ((uint)'l' << 8) | 't',
    UnknownMmc              = ((uint)'m' << 16) | ((uint)'m' << 8) | 'c',
    AlchemistMrb            = ((uint)'m' << 16) | ((uint)'r' << 8) | 'b',
    AlchemistMrh            = ((uint)'m' << 16) | ((uint)'r' << 8) | 'h',
    Shotgun                 = ((uint)'m' << 16) | ((uint)'s' << 8) | 'g',
    Cannon                  = ((uint)'m' << 16) | ((uint)'w' << 8) | 'p',
    WeaverNdl               = ((uint)'n' << 16) | ((uint)'d' << 8) | 'l',
    UnknownNik              = ((uint)'n' << 16) | ((uint)'i' << 8) | 'k', // Linked to Nier pod
    UnknownNjd              = ((uint)'n' << 16) | ((uint)'j' << 8) | 'd',
    BotanistNph             = ((uint)'n' << 16) | ((uint)'p' << 8) | 'h',
    Focus                   = ((uint)'o' << 16) | ((uint)'r' << 8) | 'b',
    UnknownOum              = ((uint)'o' << 16) | ((uint)'u' << 8) | 'm',
    UnknownPen              = ((uint)'p' << 16) | ((uint)'e' << 8) | 'n', // Linked to daggers
    Pickaxe                 = ((uint)'p' << 16) | ((uint)'i' << 8) | 'c',
    UnknownPlt              = ((uint)'p' << 16) | ((uint)'l' << 8) | 't',
    UnknownPra              = ((uint)'p' << 16) | ((uint)'r' << 8) | 'a',
    UnknownPrc              = ((uint)'p' << 16) | ((uint)'r' << 8) | 'c',
    LeatherworkerPrf        = ((uint)'p' << 16) | ((uint)'r' << 8) | 'f',
    Quiver                  = ((uint)'q' << 16) | ((uint)'v' << 8) | 'r',
    UnknownRap              = ((uint)'r' << 16) | ((uint)'a' << 8) | 'p',
    NinjaRabbit             = ((uint)'r' << 16) | ((uint)'b' << 8) | 't',
    UnknownRec              = ((uint)'r' << 16) | ((uint)'e' << 8) | 'c',
    Cane                    = ((uint)'r' << 16) | ((uint)'o' << 8) | 'd',
    UnknownRop              = ((uint)'r' << 16) | ((uint)'o' << 8) | 'p',
    UnknownRp1              = ((uint)'r' << 16) | ((uint)'o' << 8) | '1',
    Saw                     = ((uint)'s' << 16) | ((uint)'a' << 8) | 'w',
    UnknownSbt              = ((uint)'s' << 16) | ((uint)'b' << 8) | 't',
    UnknownSht              = ((uint)'s' << 16) | ((uint)'h' << 8) | 't',
    FisherSic               = ((uint)'s' << 16) | ((uint)'i' << 8) | 'c',
    Shield                  = ((uint)'s' << 16) | ((uint)'l' << 8) | 'd',
    Staff                   = ((uint)'s' << 16) | ((uint)'t' << 8) | 'f',
    CulinarianStv           = ((uint)'s' << 16) | ((uint)'t' << 8) | 'v',
    Sword                   = ((uint)'s' << 16) | ((uint)'w' << 8) | 'd',
    UnknownSxs              = ((uint)'s' << 16) | ((uint)'x' << 8) | 's',
    UnknownSxw              = ((uint)'s' << 16) | ((uint)'x' << 8) | 'w',
    SniperRifle             = ((uint)'s' << 16) | ((uint)'y' << 8) | 'l',
    UnknownSyr              = ((uint)'s' << 16) | ((uint)'y' << 8) | 'r',
    UnknownSyu              = ((uint)'s' << 16) | ((uint)'y' << 8) | 'u',
    UnknownSyw              = ((uint)'s' << 16) | ((uint)'y' << 8) | 'w',
    UnknownTan              = ((uint)'t' << 16) | ((uint)'a' << 8) | 'n',
    GoldsmithTbl            = ((uint)'t' << 16) | ((uint)'b' << 8) | 'l',
    UnknownTcs              = ((uint)'t' << 16) | ((uint)'c' << 8) | 's',
    GoldsmithTgn            = ((uint)'t' << 16) | ((uint)'g' << 8) | 'n',
    WeaverTmb               = ((uint)'t' << 16) | ((uint)'m' << 8) | 'b',
    UnknownTms              = ((uint)'t' << 16) | ((uint)'m' << 8) | 's',
    UnknownTrm              = ((uint)'t' << 16) | ((uint)'r' << 8) | 'm', // Linked to Flutes
    UnkownnTrr              = ((uint)'t' << 16) | ((uint)'r' << 8) | 'r',
    UnknownTrw              = ((uint)'t' << 16) | ((uint)'r' << 8) | 'w', // Linked to Greatswords
    UnknownVln              = ((uint)'t' << 16) | ((uint)'l' << 8) | 'n',
    WeaverWhl               = ((uint)'w' << 16) | ((uint)'h' << 8) | 'l',
    UnknownWng              = ((uint)'w' << 16) | ((uint)'n' << 8) | 'g',
    UnknownYpd              = ((uint)'y' << 16) | ((uint)'p' << 8) | 'd',
    ArmorerYtk              = ((uint)'y' << 16) | ((uint)'t' << 8) | 'k',
}

public static class AtchExtensions
{
    public static string ToAbbreviation(this AtchType type)
        => (uint)type > 0x00FFFFFF
            ? $"{(char)(((uint)type >> 24) & 0xFF)}{(char)(((uint)type >> 16) & 0xFF)}{(char)(((uint)type >> 8) & 0xFF)}{(char)((uint)type & 0xFF)}"
            : $"{(char)(((uint)type >> 16) & 0xFF)}{(char)(((uint)type >> 8) & 0xFF)}{(char)((uint)type & 0xFF)}";

    public static string ToName(this AtchType type)
        => type switch
        {
            AtchType.GreatAxe                => "Greataxe",
            AtchType.Book                    => "Book",
            AtchType.Bow                     => "Bow",
            AtchType.Nouliths                => "Nouliths",
            AtchType.Gunblade                => "Gunblade",
            AtchType.Globe                   => "Globe",
            AtchType.Gun                     => "Gun",
            AtchType.Scythe                  => "Scythe",
            AtchType.Katana                  => "Katana",
            AtchType.Rapier                  => "Rapier",
            AtchType.Spear                   => "Spear",
            AtchType.Greatstaff              => "Greatstaff",
            AtchType.Greatsword              => "Greatsword",
            AtchType.AlchemistAai            => "Alchemist (AAI)",
            AtchType.AlchemistAal            => "Alchemist (AAL)",
            AtchType.ArmorerAar              => "Armorer (AAR)",
            AtchType.ArmorerAbl              => "Armorer (ABL)",
            AtchType.CulinarianAco           => "Culinarian (ACO)",
            AtchType.GoldsmithAgl            => "Goldsmith (AGL)",
            AtchType.AlchemistAli            => "Alchemist (ALI)",
            AtchType.AlchemistAlm            => "Alchemist (ALM)",
            AtchType.LeatherworkerAlt        => "Leatherworker (ALT)",
            AtchType.WeaverAse               => "Weaver (ASE)",
            AtchType.CarpenterAwo            => "Carpenter (AWO)",
            AtchType.Aetherotransformer      => "Aetherotransformer (Machinist)",
            AtchType.Chakrams                => "Chakrams",
            AtchType.Gloves                  => "Gloves",
            AtchType.Claws                   => "Claws",
            AtchType.Cardholder              => "Cardholder",
            AtchType.CarpenterCsl            => "Carpenter (CSL)",
            AtchType.CarpenterCsr            => "Carpenter (CSR)",
            AtchType.Daggers                 => "Daggers",
            AtchType.Drums                   => "Drums",
            AtchType.ReaperShroud            => "Shroud (Reaper)",
            AtchType.FisherFha               => "Fisher (FHA)",
            AtchType.Harp                    => "Harp",
            AtchType.Flute                   => "Flute",
            AtchType.NinjaFrog               => "Hellfrog Medium (Ninja)",
            AtchType.LeatherworkerCulinarian => "Leatheworker / Culinarian",
            AtchType.FisherFsh               => "Fisher (FSH)",
            AtchType.FistWeapons             => "Fist Weapons",
            AtchType.MachinistDeployable     => "Deployable (Machinist)",
            AtchType.DiademCannon            => "Diadem Cannon?",
            AtchType.BlacksmithArmorer       => "Blacksmith / Armorer",
            AtchType.BardHarp                => "Harp (Bard)",
            AtchType.Hatchet                 => "Hatchet",
            AtchType.KatanaSheathe           => "Katana Sheathe",
            AtchType.GoldsmithMlt            => "Goldsmith (MLT)",
            AtchType.AlchemistMrb            => "Alchemist (MRB)",
            AtchType.AlchemistMrh            => "Alchemist (MRH)",
            AtchType.Shotgun                 => "Shotgun (Machinist)",
            AtchType.Cannon                  => "Cannon (Machinist)",
            AtchType.WeaverNdl               => "Weaver (NDL)",
            AtchType.BotanistNph             => "Botanist (NPH)",
            AtchType.Focus                   => "Focus (Redmage)",
            AtchType.Pickaxe                 => "Pickaxe",
            AtchType.LeatherworkerPrf        => "Leatherworker (PRF)",
            AtchType.Quiver                  => "Quiver (Bard)",
            AtchType.NinjaRabbit             => "Rabbit Medium (Ninja)",
            AtchType.Cane                    => "Cane",
            AtchType.Saw                     => "Saw",
            AtchType.FisherSic               => "Fisher (SIC)",
            AtchType.Shield                  => "Shield",
            AtchType.Staff                   => "Staff",
            AtchType.CulinarianStv           => "Culinarian (STV)",
            AtchType.Sword                   => "Sword",
            AtchType.SniperRifle             => "Sniper Rifle (Machinist)",
            AtchType.GoldsmithTbl            => "Goldsmith (TBL)",
            AtchType.GoldsmithTgn            => "Goldsmith (TGN)",
            AtchType.WeaverTmb               => "Weaver (TMB)",
            AtchType.WeaverWhl               => "Weaver (WHL)",
            AtchType.ArmorerYtk              => "Armorer (YTK)",
            _                                => type.ToAbbreviation(),
        };

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
