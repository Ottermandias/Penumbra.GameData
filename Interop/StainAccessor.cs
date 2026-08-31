using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using Luna;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Penumbra.GameData.Files;
using Penumbra.GameData.Files.StainMapStructs;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Penumbra.GameData.Interop;

public partial class StainAccessor : IService
{
    public const int GudStmIndex    = 95;
    public const int LegacyStmIndex = 96;

    public readonly StmFile<LegacyDyePack> LegacyStmFile;
    public readonly StmFile<DyePack>       GudStmFile;

    public unsafe StainAccessor(ILogger? log, IDataManager dataManager)
    {
        var characterUtility = CharacterUtility.Instance();

        log           ??= NullLogger.Instance;
        LegacyStmFile =   LoadStmFile<LegacyDyePack>(log, characterUtility, dataManager);
        GudStmFile    =   LoadStmFile<DyePack>(log, characterUtility, dataManager);
    }

    /// <summary> Loads an STM file. Opportunistically attempts to re-use the file already read by the game, with Lumina fallback. </summary>
    private static unsafe StmFile<TDyePack> LoadStmFile<TDyePack>(ILogger log, CharacterUtility* characterUtility, IDataManager dataManager)
        where TDyePack : unmanaged, IDyePack
        => LoadStmFile<TDyePack>(log, characterUtility) ?? LoadStmFile<TDyePack>(log, dataManager);

    private static unsafe StmFile<TDyePack>? LoadStmFile<TDyePack>(ILogger log, CharacterUtility* characterUtility)
        where TDyePack : unmanaged, IDyePack
    {
        if (characterUtility is null)
            return null;

        var stmResourceHandle = characterUtility->ResourceHandles[TDyePack.DefaultStmIndex].Value;
        if (stmResourceHandle is null)
            return null;

        var stmPath = stmResourceHandle->FileName.ToString();
        if (!string.Equals(stmPath, TDyePack.DefaultStmPath, StringComparison.OrdinalIgnoreCase))
        {
            LogLoadFailure(log, typeof(TDyePack), TDyePack.DefaultStmPath, (nint)stmResourceHandle, stmPath);
            return null;
        }

        var stmData = stmResourceHandle->GetDataSpan();
        if (stmData.Length is 0)
            return null;

        LogResourceHandleLoad(log, typeof(TDyePack), (nint)stmResourceHandle);
        return new StmFile<TDyePack>(stmData);
    }

    private static StmFile<TDyePack> LoadStmFile<TDyePack>(ILogger log, IDataManager dataManager) where TDyePack : unmanaged, IDyePack
    {
        LogLuminaLoad(log, typeof(TDyePack));
        return new StmFile<TDyePack>(dataManager);
    }

    [LoggerMessage(LogLevel.Warning,
        "[StainAccessor] Could not load StmFile<{Type}> ({DefaultPath}) from ResourceHandle 0x{ResourceHandle:X} ({Path})")]
    static partial void LogLoadFailure(ILogger logger, Type type, string defaultPath, nint resourceHandle, string path);

    [LoggerMessage(LogLevel.Trace, "[StainAccessor] Loading StmFile<{Type}> from ResourceHandle 0x{ResourceHandle:X}")]
    static partial void LogResourceHandleLoad(ILogger logger, Type type, nint resourceHandle);

    [LoggerMessage(LogLevel.Trace, "[StainAccessor] Loading StmFile<{Type}> from Lumina")]
    static partial void LogLuminaLoad(ILogger logger, Type type);
}
