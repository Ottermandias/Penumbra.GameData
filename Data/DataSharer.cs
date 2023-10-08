using Dalamud;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace Penumbra.GameData.Data;

/// <summary>
/// A container base class that shares data through Dalamud but cares about the used language and version.
/// Inheritors should dispose their Dalamud Shares in DisposeInternal via DisposeTag and add them in their constructor via TryCatchData.
/// </summary>
public abstract class DataSharer : IDisposable
{
    protected readonly DalamudPluginInterface PluginInterface;
    protected readonly IPluginLog             Log;
    protected readonly int                    Version;
    protected readonly ClientLanguage         Language;
    private            bool                   _disposed;

    protected DataSharer(DalamudPluginInterface pluginInterface, ClientLanguage language, int version, IPluginLog log)
    {
        PluginInterface = pluginInterface;
        Language        = language;
        Version         = version;
        Log             = log;
    }

    protected virtual void DisposeInternal()
    { }

    public void Dispose()
    {
        if (_disposed)
            return;

        DisposeInternal();
        GC.SuppressFinalize(this);
        _disposed = true;
    }

    ~DataSharer()
        => Dispose();

    protected void DisposeTag(string tag)
        => PluginInterface.RelinquishData(GetVersionedTag(tag, Language, Version));

    protected T TryCatchData<T>(string tag, Func<T> func) where T : class
    {
        try
        {
            Log.Debug($"[DataShare] Creating or obtaining shared data for {tag}.");
            return PluginInterface.GetOrCreateData(GetVersionedTag(tag, Language, Version), func);
        }
        catch (Exception ex)
        {
            Log.Error($"[DataShare] Error creating shared data for {tag}:\n{ex}");
            return func();
        }
    }

    public static void DisposeTag(DalamudPluginInterface pi, string tag, ClientLanguage language, int version)
        => pi.RelinquishData(GetVersionedTag(tag, language, version));

    public static T TryCatchData<T>(DalamudPluginInterface pi, string tag, ClientLanguage language, int version, Func<T> func, IPluginLog log)
        where T : class
    {
        try
        {
            log.Debug($"[DataShare] Creating or obtaining shared data for {tag}.");
            return pi.GetOrCreateData(GetVersionedTag(tag, language, version), func);
        }
        catch (Exception ex)
        {
            log.Error($"[DataShare] Error creating shared actor data for {tag}:\n{ex}");
            return func();
        }
    }

    private static string GetVersionedTag(string tag, ClientLanguage language, int version)
        => $"Penumbra.GameData.{tag}.{language}.V{version}";
}
