using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using Luna;
using Microsoft.Extensions.Logging;
using Penumbra.GameData.Files;
using Penumbra.GameData.Files.StainMapStructs;

namespace Penumbra.GameData.Interop;

public class StainAccessor : IService
{
    public const int GudStmIndex    = 95;
    public const int LegacyStmIndex = 96;

    public readonly StmFile<LegacyDyePack> LegacyStmFile;
    public readonly StmFile<DyePack>       GudStmFile;

    public unsafe StainAccessor(ILogger? log, IDataManager dataManager)
    {
        var characterUtility = CharacterUtility.Instance();

        LegacyStmFile = LoadStmFile<LegacyDyePack>(log, characterUtility, dataManager);
        GudStmFile    = LoadStmFile<DyePack>(log, characterUtility, dataManager);
    }

    /// <summary> Loads a STM file. Opportunistically attempts to re-use the file already read by the game, with Lumina fallback. </summary>
    private static unsafe StmFile<TDyePack> LoadStmFile<TDyePack>(ILogger? log, CharacterUtility* characterUtility, IDataManager dataManager)
        where TDyePack : unmanaged, IDyePack
        => LoadStmFile<TDyePack>(log, characterUtility) ?? LoadStmFile<TDyePack>(log, dataManager);

    private static unsafe StmFile<TDyePack>? LoadStmFile<TDyePack>(ILogger? log, CharacterUtility* characterUtility)
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
            log?.LogWarning(
                "[StainAccessor] Cannot load StmFile<{Type}> ({DefaultStmPath}) from ResourceHandle 0x{StmResourceHandle:X} ({StmPath})",
                typeof(TDyePack), TDyePack.DefaultStmPath, (nint)stmResourceHandle, stmPath);
            return null;
        }

        var stmData = stmResourceHandle->GetDataSpan();
        if (stmData.Length is 0)
            return null;

        log?.LogDebug("[StainAccessor] Loading StmFile<{Type}> from ResourceHandle 0x{StmResourceHandle:X}", typeof(TDyePack),
            (nint)stmResourceHandle);
        return new StmFile<TDyePack>(stmData);
    }

    private static StmFile<TDyePack> LoadStmFile<TDyePack>(ILogger? log, IDataManager dataManager) where TDyePack : unmanaged, IDyePack
    {
        log?.LogDebug("[StainAccessor] Loading StmFile<{Type}> from Lumina", typeof(TDyePack));
        return new StmFile<TDyePack>(dataManager);
    }
}
