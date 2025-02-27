using Penumbra.GameData.Enums;

namespace Penumbra.GameData.Data;

/// <summary> Functions to parse game paths for certain types of files. </summary>
public static partial class Parser
{
    public static class Groups
    {
        public const string RaceCode             = "race";
        public const string PrimaryId            = "id";
        public const string SecondaryId          = "id2";
        public const string Variant              = "variant";
        public const string Slot                 = "slot";
        public const string BodyType             = "type";
        public const string BodyTypeAbbreviation = "typeAbbr";
        public const string BodyTypeSlot         = "typeSlot";
        public const string Minus                = "minus";
        public const string Catchlight           = "catch";
        public const string Skin                 = "skin";
        public const string ActionKey            = "key";
        public const string Decal                = "decal";
        public const string Suffix               = "suffix";
        public const string FontName             = "font";
        public const string Quality              = "quality";
        public const string Resolution           = "resolution";
        public const string Language             = "language";
    }

    /// <summary> We never want to capture anything not named. </summary>
    public const RegexOptions Flags1 = RegexOptions.ExplicitCapture;

    /// <summary> When possible, we want to use non-backtracking regex. Backreferences are not compatible with that though. </summary>
    public const RegexOptions Flags2 = Flags1 | RegexOptions.NonBacktracking;


    [GeneratedRegex($@"c(?'{Groups.RaceCode}'\d{{4}})", Flags2)]
    private static partial Regex RaceCodeParser();

    public static GenderRace ParseRaceCode(string path)
    {
        var match = RaceCodeParser().Match(path);
        return match.Success
            ? Names.GenderRaceFromCode(match.Groups[Groups.RaceCode].ValueSpan)
            : GenderRace.Unknown;
    }

    public static partial class Monster
    {
        private const string Prefix = $@"chara/monster/m(?'{Groups.PrimaryId}'\d{{4}})/obj/body/b(?'{Groups.SecondaryId}'\d{{4}})";
        private const string Repeat = $@"m\k'{Groups.PrimaryId}'b\k'{Groups.SecondaryId}'";

        [GeneratedRegex($@"{Prefix}/b\k'{Groups.SecondaryId}'\.imc", Flags1)]
        public static partial Regex Imc();

        [GeneratedRegex($@"{Prefix}/model/{Repeat}\.mdl", Flags1)]
        public static partial Regex Mdl();

        [GeneratedRegex($@"{Prefix}/material/v(?'{Groups.Variant}'\d{{4}})/mt_{Repeat}_[a-z]+\.mtrl", Flags1)]
        public static partial Regex Mtrl();

        [GeneratedRegex($@"{Prefix}/texture/v(?'{Groups.Variant}'\d{{2}})_{Repeat}(_[a-z])?_[a-z]\.tex", Flags1)]
        public static partial Regex Tex();

        [GeneratedRegex($@"chara/monster/m(?'{Groups.PrimaryId}'\d{{4}})/skeleton/base/b(?'{Groups.SecondaryId}'\d{{4}})/(eid|skl|phy)_{Repeat}\.(eid|sklb|phyb|skp)", Flags1)]
        public static partial Regex Skeleton();
    }

    public static partial class Weapon
    {
        private const string Prefix = $@"chara/weapon/w(?'{Groups.PrimaryId}'\d{{4}})/obj/body/b(?'{Groups.SecondaryId}'\d{{4}})";
        private const string Repeat = $@"w\k'{Groups.PrimaryId}'b\k'{Groups.SecondaryId}'";

        [GeneratedRegex($@"{Prefix}/b\k'{Groups.SecondaryId}'\.imc", Flags1)]
        public static partial Regex Imc();

        [GeneratedRegex($@"{Prefix}/model/{Repeat}\.mdl", Flags1)]
        public static partial Regex Mdl();

        [GeneratedRegex($@"{Prefix}/material/v(?'{Groups.Variant}'\d{{4}})/mt_{Repeat}_[a-z]+\.mtrl", Flags1)]
        public static partial Regex Mtrl();

        [GeneratedRegex($@"{Prefix}/texture/v(?'{Groups.Variant}'\d{{2}})_{Repeat}(_[a-z])?_[a-z]\.tex", Flags1)]
        public static partial Regex Tex();

        [GeneratedRegex($@"chara/weapon/w(?'{Groups.PrimaryId}'\d{{4}})/skeleton/base/b(?'{Groups.SecondaryId}'\d{{4}})/(eid|skl|phy)_{Repeat}\.(eid|sklb|phyb|skp)", Flags1)]
        public static partial Regex Skeleton();
    }

    public static partial class DemiHuman
    {
        private const string Prefix = $@"chara/demihuman/d(?'{Groups.PrimaryId}'\d{{4}})/obj/equipment/e(?'{Groups.SecondaryId}'\d{{4}})";
        private const string Repeat = $@"d\k'{Groups.PrimaryId}'b\k'{Groups.SecondaryId}'";
        private const string Slot   = $"(?'{Groups.Slot}'[a-z]{{3}});";

        [GeneratedRegex($@"{Prefix}/e\k'{Groups.SecondaryId}'\.imc", Flags1)]
        public static partial Regex Imc();

        [GeneratedRegex($@"{Prefix}/model/{Repeat}_{Slot}\.mdl", Flags1)]
        public static partial Regex Mdl();

        [GeneratedRegex($@"{Prefix}/material/v(?'{Groups.Variant}'\d{{4}})/mt_{Repeat}_{Slot}_[a-z]+\.mtrl", Flags1)]
        public static partial Regex Mtrl();

        [GeneratedRegex($@"{Prefix}/texture/v(?'{Groups.Variant}'\d{{2}})_{Repeat}_{Slot}(_[a-z])?_[a-z]\.tex", Flags1)]
        public static partial Regex Tex();

        [GeneratedRegex($@"chara/demihuman/d(?'{Groups.PrimaryId}'\d{{4}})/skeleton/base/b(?'{Groups.SecondaryId}'\d{{4}})/(eid|skl|phy)_{Repeat}\.(eid|sklb|phyb|skp)", Flags1)]
        public static partial Regex Skeleton();
    }


    public static partial class Equipment
    {
        private const string Prefix = $@"chara/equipment/e(?'{Groups.PrimaryId}'\d{{4}})";
        private const string Repeat = $@"c(?'{Groups.RaceCode}'\d{{4}})e\k'{Groups.PrimaryId}'";
        private const string Slot   = $"(?'{Groups.Slot}'[a-z]{{3}})";

        [GeneratedRegex($@"chara/equipment/e(?'{Groups.PrimaryId}'\d{{4}})/e\k'{Groups.PrimaryId}'\.imc", Flags1)]
        public static partial Regex Imc();

        [GeneratedRegex($@"{Prefix}/model/{Repeat}_{Slot}\.mdl", Flags1)]
        public static partial Regex Mdl();

        [GeneratedRegex($@"{Prefix}/material/v(?'{Groups.Variant}'\d{{4}})/mt_{Repeat}_{Slot}_[a-z]+\.mtrl", Flags1)]
        public static partial Regex Mtrl();

        [GeneratedRegex($@"{Prefix}/texture/v(?'{Groups.Variant}'\d{{2}})_{Repeat}_{Slot}(_[a-z])?_[a-z]\.tex", Flags1)]
        public static partial Regex Tex();

        [GeneratedRegex($@"{Prefix}/vfx/eff/ve(?'{Groups.Variant}'\d{{4}})\.avfx", Flags2)]
        public static partial Regex Avfx();
    }

    public static partial class Accessory
    {
        private const string Prefix = $@"chara/equipment/a(?'{Groups.PrimaryId}'\d{{4}})";
        private const string Repeat = $@"c(?'{Groups.RaceCode}'\d{{4}})a\k'{Groups.PrimaryId}'";
        private const string Slot   = $"(?'{Groups.Slot}'[a-z]{{3}})";

        [GeneratedRegex($@"chara/accessory/a(?'{Groups.PrimaryId}'\d{{4}})/e\k'{Groups.PrimaryId}'\.imc", Flags1)]
        public static partial Regex Imc();

        [GeneratedRegex($@"{Prefix}/model/{Repeat}_{Slot}\.mdl", Flags1)]
        public static partial Regex Mdl();

        [GeneratedRegex($@"{Prefix}/material/v(?'{Groups.Variant}'\d{{4}})/mt_{Repeat}_{Slot}_[a-z]+\.mtrl", Flags1)]
        public static partial Regex Mtrl();

        [GeneratedRegex($@"{Prefix}/texture/v(?'{Groups.Variant}'\d{{2}})_{Repeat}_{Slot}(_[a-z])?_[a-z]\.tex", Flags1)]
        public static partial Regex Tex();

        [GeneratedRegex($@"{Prefix}/vfx/eff/ve(?'{Groups.Variant}'\d{{4}})\.avfx", Flags2)]
        public static partial Regex Avfx();
    }

    public static partial class Character
    {
        private const string Prefix =
            $@"chara/human/c(?'{Groups.RaceCode}'\d{{4}})/obj/(?'{Groups.BodyType}'[a-z]+)/(?'{Groups.BodyTypeAbbreviation}'[a-z])(?'{Groups.PrimaryId}'\d{{4}})";

        private const string Repeat =
            $@"c\k'{Groups.RaceCode}'\k'{Groups.BodyTypeAbbreviation}'\k'{Groups.PrimaryId}'";

        private const string Slot    = $"(?'{Groups.BodyTypeSlot}'[a-z]{{3}})";
        private const string Variant = $"(?'{Groups.Variant}'\\d{{2}})";

        [GeneratedRegex($@"{Prefix}/model/{Repeat}_{Slot}\.mdl", Flags1)]
        public static partial Regex Mdl();

        [GeneratedRegex($@"{Prefix}/material(/v(?'{Groups.Variant}'\d{{4}}))?/mt_{Repeat}(.*?_ {Slot})?_[a-z]+(?:_[^.]*)?\.mtrl", Flags1)]
        public static partial Regex Mtrl();

        [GeneratedRegex($@"{Prefix}/texture/(?'{Groups.Minus}'(--)?)(v_{Variant})?{Repeat}(_{Slot})?(_[a-z])?_[a-z]\.tex", Flags1)]
        public static partial Regex Tex();


        [GeneratedRegex($@"chara/common/texture/(?'{Groups.Catchlight}'catchlight)(.*)\.tex", Flags2)]
        public static partial Regex Catchlight();

        [GeneratedRegex($@"chara/common/texture/skin(?'{Groups.Skin}'.*)\.tex", Flags2)]
        public static partial Regex Skin();

        [GeneratedRegex($@"chara/common/texture/decal_(?'{Groups.Decal}'[a-z]+)/[-_]?decal_(?'{Groups.PrimaryId}'\d+).tex", Flags2)]
        public static partial Regex Decal();

        [GeneratedRegex($"{Prefix}/texture", Flags2)]
        public static partial Regex Folder();

        [GeneratedRegex($@"chara/human/c(?'{Groups.RaceCode}'\d{{4}})/skeleton/(?'{Groups.BodyType}'[a-z]+)/(?'{Groups.BodyTypeAbbreviation}'[a-z])(?'{Groups.PrimaryId}'\d{{4}})/(eid|skl|phy)_{Repeat}\.(eid|sklb|phyb|skp)", Flags1)]
        public static partial Regex Skeleton();

        [GeneratedRegex($@"chara[\/]action[\/](?'{Groups.ActionKey}'[^\s]+?)\.tmb", RegexOptions.IgnoreCase | Flags2)]
        public static partial Regex Tmb();

        [GeneratedRegex($@"chara[\/]human[\/]c0101[\/]animation[\/]a0001[\/][^\s]+?[\/](?'{Groups.ActionKey}'[^\s]+?)\.pap",
            RegexOptions.IgnoreCase | Flags2)]
        public static partial Regex Pap();

        [GeneratedRegex($@"chara[\/]xls[\/]attachOffset[\/]c(?'{Groups.RaceCode}'\d{{4}})\.atch", RegexOptions.IgnoreCase | Flags2)]
        public static partial Regex Atch();
    }

    [GeneratedRegex(
        $@"ui/icon/(\d+)(/(?'{Groups.Language}'[a-z]{{2}}))?(/(?'{Groups.Quality}'hq))?/(?'{Groups.PrimaryId}'\d*)(?'{Groups.Resolution}'_hr1)?\.tex",
        Flags2)]
    public static partial Regex Icon();

    [GeneratedRegex(
        $@"ui/map/(?'{Groups.PrimaryId}'[a-z0-9]{{4}})/(?'{Groups.Variant}'\d{{2}})/\k'{Groups.PrimaryId}'\k'{Groups.Variant}'(?'{Groups.Suffix}'[a-z])?(_[a-z])?\.tex",
        Flags1)]
    public static partial Regex Map();

    [GeneratedRegex($@"common/font/(?'{Groups.FontName}'.*)_(?'{Groups.PrimaryId}'\d{{2}})(_lobby)?\.fdt", Flags2)]
    public static partial Regex Font();
}
