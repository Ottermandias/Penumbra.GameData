namespace Penumbra.GameData.Enums;

/// <summary> Special game object indices that represent specific boundaries in the object table or certain actors. </summary>
public enum ScreenActor : ushort
{
    CutsceneStart   = 200,
    GPosePlayer     = 201,
    CutsceneEnd     = 250,
    CharacterScreen = CutsceneEnd,
    ExamineScreen   = 251,
    FittingRoom     = 252,
    DyePreview      = 253,
    Portrait        = 254,
    Card6           = 255,
    Card7           = 256,
    Card8           = 257,
    ScreenEnd       = Card8 + 1,
}
