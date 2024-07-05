namespace Penumbra.GameData;

/// <summary> Offsets in game data in use by Penumbra. </summary>
public static class Offsets
{
    // ActorManager.Data
    public const int AgentCharaCardDataWorldId = 0xC0;

    // ObjectIdentification
    public const  int DrawObjectGetModelTypeVfunc = 50;
    private const int DrawObjectModelBase         = 0x8E8;
    public const  int DrawObjectModelUnk1         = DrawObjectModelBase;
    public const  int DrawObjectModelUnk2         = DrawObjectModelBase + 0x08;
    public const  int DrawObjectModelUnk3         = DrawObjectModelBase + 0x20;
    public const  int DrawObjectModelUnk4         = DrawObjectModelBase + 0x28;

    // PathResolver.AnimationState
    public const int GetGameObjectIdxVfunc = 28;
    public const int TimeLinePtr           = 0x50;

    // PathResolver.Meta
    public const int UpdateModelSkip     = 0x9EC;
    public const int GetEqpIndirectSkip1 = 0xBF8;
    public const int GetEqpIndirectSkip2 = 0xBF0;
    public const int GetEqpIndirect2Skip = 0x90;

    // FontReloader
    public const int ReloadFontsVfunc = 43;

    // ObjectReloader
    public const int EnableDrawVfunc  = 16;
    public const int DisableDrawVfunc = 17;
}
