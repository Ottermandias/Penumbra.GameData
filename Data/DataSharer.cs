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
            return PluginInterface.GetOrCreateData(GetVersionedTag(tag, Language, Version), func);
        }
        catch (Exception ex)
        {
            Log.Error($"Error creating shared data for {tag}:\n{ex}");
            return func();
        }
    }

    protected Task<T> TryCatchDataAsync<T>(string tag, Action<T> fill) where T : class, new()
    {
        tag = GetVersionedTag(tag, Language, Version);
        T   ret  = new();
        var ret2 = PluginInterface.GetOrCreateData(tag, () => ret);
        if (!ReferenceEquals(ret, ret2))
            return Task.FromResult(ret2);

        return Task.Run(() =>
        {
            fill(ret2);
            return ret2;
        });
    }

    public static void DisposeTag(DalamudPluginInterface pi, string tag, ClientLanguage language, int version)
        => pi.RelinquishData(GetVersionedTag(tag, language, version));

    public static T TryCatchData<T>(DalamudPluginInterface pi, string tag, ClientLanguage language, int version, Func<T> func, IPluginLog log)
        where T : class
    {
        try
        {
            return pi.GetOrCreateData(GetVersionedTag(tag, language, version), func);
        }
        catch (Exception ex)
        {
            log.Error($"Error creating shared actor data for {tag}:\n{ex}");
            return func();
        }
    }

    private static string GetVersionedTag(string tag, ClientLanguage language, int version)
        => $"Penumbra.GameData.{tag}.{language}.V{version}";
}
