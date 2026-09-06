using Dalamud.Plugin.Services;
using Luna;
using Penumbra.GameData.Files;

namespace Penumbra.GameData.Interop;

public class ShaderParameterAccessor(IDataManager gameData) : IService
{
    private readonly Lazy<SpmFile> _bgSpmFile     = new(() => new SpmFile(gameData, SpmFile.Table.Bg));
    private readonly Lazy<SpmFile> _charaSpmFile  = new(() => new SpmFile(gameData, SpmFile.Table.Chara));
    private readonly Lazy<SpmFile> _commonSpmFile = new(() => new SpmFile(gameData, SpmFile.Table.Common));

    public SpmFile BgSpmFile
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _bgSpmFile.Value;
    }

    public SpmFile CharaSpmFile
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _charaSpmFile.Value;
    }

    public SpmFile CommonSpmFile
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _commonSpmFile.Value;
    }

    public SpmFile GetSpmFile(SpmFile.Table table)
        => table switch
        {
            SpmFile.Table.Bg     => _bgSpmFile.Value,
            SpmFile.Table.Chara  => _charaSpmFile.Value,
            SpmFile.Table.Common => _commonSpmFile.Value,
            _                    => throw new ArgumentException($"Invalid SPM table 0x{(uint)table:X8}", nameof(table)),
        };
}
