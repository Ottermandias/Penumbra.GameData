using System.Threading;
using Dalamud;
using Dalamud.Plugin;
using OtterGui.Log;
using OtterGui.Services;

namespace Penumbra.GameData.DataContainers.Bases;

/// <summary> A service interacting with the Dalamud Data Share. </summary>
/// <typeparam name="T"> The type to be constructed in the Data Share. This can only contain types that Dalamud itself knows. </typeparam>
/// <param name="pluginInterface"> The plugin interface. </param>
/// <param name="log"> A logger. </param>
/// <param name="name"></param>
/// <param name="language"></param>
/// <param name="version"></param>
public abstract class DataSharer<T>(DalamudPluginInterface pluginInterface, Logger log, string name, ClientLanguage language, int version)
    : IDisposable, IAsyncDataContainer
{
    private            T?        _value;
    protected readonly Task<T>   Task        = null!;
    private readonly   Stopwatch _timer      = null!;
    private            int       _totalCount = -1;
    private            long      _memory     = -1;

    public bool Finished
        => _value != null;

    protected DataSharer(DalamudPluginInterface pluginInterface, Logger log, string name, ClientLanguage language, int version,
        Func<T> factory, Task? continuation = null)
        : this(pluginInterface, log, name, language, version)
        => (Task, _timer) = CreateTask(factory, continuation);

    public T Value
        => _value ?? Task.Result;

    public Task Awaiter
        => Task;

    private string Label
        => $"Penumbra.GameData.{Name}.{language}.V{version}";

    public long Time
        => _timer.ElapsedMilliseconds;

    public string Name
        => name;

    public int TotalCount
        => _totalCount < 0 ? _totalCount = ComputeTotalCount() : _totalCount;

    public long Memory
        => _memory < 0 ? _memory = ComputeMemory() : _memory;

    public abstract long ComputeMemory();
    public abstract int  ComputeTotalCount();

    public void Dispose()
    {
        pluginInterface.RelinquishData(Label);
        GC.SuppressFinalize(this);
        Dispose(true);
    }

    protected virtual void Dispose(bool _)
    { }

    private Tuple<Task<T>, Stopwatch> CreateTask(Func<T> factory, Task? continuation = null)
    {
        return pluginInterface.GetOrCreateData(Label, Create);

        Tuple<Task<T>, Stopwatch> Create()
        {
            var stopwatch = new Stopwatch();
            var task = continuation == null
                ? System.Threading.Tasks.Task.Run(() =>
                {
                    log.Debug($"[{Name}] Creating v{version} for {language} on thread {Thread.CurrentThread.ManagedThreadId}...");
                    stopwatch.Start();
                    var ret = factory();
                    stopwatch.Stop();
                    log.Debug($"[{Name}] Created v{version} for {language} in {stopwatch.ElapsedMilliseconds} ms.");
                    _value = ret;
                    return ret;
                })
                : continuation.ContinueWith(_ =>
                {
                    log.Debug($"[{Name}] Creating v{version} for {language} on thread {Thread.CurrentThread.ManagedThreadId}...");
                    stopwatch.Start();
                    var ret = factory();
                    stopwatch.Stop();
                    log.Debug($"[{Name}] Created v{version} for {language} in {stopwatch.ElapsedMilliseconds} ms.");
                    _value = ret;
                    return ret;
                });
            return new Tuple<Task<T>, Stopwatch>(task, stopwatch);
        }
    }
}
