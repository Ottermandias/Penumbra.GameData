using System.Collections.Frozen;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;
using Luna;
using Penumbra.GameData.Data;
using Penumbra.GameData.Structs;

namespace Penumbra.GameData.DataContainers;

/// <summary> A dictionary containing all the jobs. This is rather small and simple so not shared via data share. </summary>
public sealed class DictJob : IDataContainer, IReadOnlyDictionary<JobId, Job>
{
    /// <summary> Create jobs. </summary>
    public DictJob(IDataManager gameData)
    {
        var stopwatch = Stopwatch.StartNew();
        var sheet     = gameData.GetExcelSheet<ClassJob>();
        _jobs = sheet.Where(j => j.Abbreviation.ByteLength > 0)
            .ToFrozenDictionary(j => (JobId)j.RowId, j => new Job(j));

        Ordered = _jobs.Select(kvp => (sheet.GetRow(kvp.Key.Id), kvp.Value))
            .OrderBy(j => j.Item1.JobIndex == 0)
            .ThenBy(j => j.Item1.IsLimitedJob)
            .ThenBy(j => j.Item2.Role)
            .Select(j => j.Item2)
            .ToArray();

        Memory = DataUtility.DictionaryMemory(32, Count)
          + _jobs.Sum(kvp => kvp.Value.Name.Length + kvp.Value.Abbreviation.Length) * 2
          + DataUtility.ArrayMemory(32, Count);
        Time = stopwatch.ElapsedMilliseconds;
    }

    public readonly IReadOnlyList<Job> Ordered;

    /// <summary> The jobs. </summary>
    private readonly IReadOnlyDictionary<JobId, Job> _jobs;

    /// <inheritdoc/>
    public long Time { get; }

    /// <inheritdoc/>
    public long Memory { get; }

    /// <inheritdoc/>
    public string Name
        => nameof(DictJob);

    /// <inheritdoc/>
    public int TotalCount
        => _jobs.Count;

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<JobId, Job>> GetEnumerator()
        => _jobs.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    /// <inheritdoc/>
    public int Count
        => _jobs.Count;

    /// <inheritdoc/>
    public bool ContainsKey(JobId key)
        => _jobs.ContainsKey(key);

    /// <inheritdoc/>
    public bool TryGetValue(JobId key, [NotNullWhen(true)] out Job? value)
        => _jobs.TryGetValue(key, out value);

    /// <inheritdoc/>
    public Job this[JobId key]
        => _jobs[key];

    /// <inheritdoc/>
    public IEnumerable<JobId> Keys
        => _jobs.Keys;

    /// <inheritdoc/>
    public IEnumerable<Job> Values
        => _jobs.Values;
}
