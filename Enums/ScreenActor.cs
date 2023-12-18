namespace Penumbra.GameData.Enums;

/// <summary> Special game object indices that represent specific boundaries in the object table or certain actors. </summary>
public enum ScreenActor : ushort
{
    CutsceneStart   = 200,
    GPosePlayer     = 201,
    CutsceneEnd     = 240,
    CharacterScreen = CutsceneEnd,
    ExamineScreen   = 241,
    FittingRoom     = 242,
    DyePreview      = 243,
    Portrait        = 244,
    Card6           = 245,
    Card7           = 246,
    Card8           = 247,
    ScreenEnd       = Card8 + 1,
}
