using Dalamud.Utility;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;

namespace Penumbra.GameData.Structs;

/// <summary>
/// A generic enum for job flags.
/// We do not need to name these since they are just bit-shifted by job IDs.
/// </summary>
[Flags]
public enum JobFlag : ulong;

/// <summary> The game specifies different job groups that can contain specific jobs or not. </summary>
public readonly struct JobGroup
{
    /// <summary> The name of the group. </summary>
    public readonly string Name;

    /// <summary> The number of jobs contained in the group. </summary>
    public readonly int Count;

    /// <summary> The ID of the group. </summary>
    public readonly JobGroupId Id;

    /// <summary> The contained jobs as bit set. </summary>
    private readonly JobFlag _flags;

    /// <summary>
    /// Create a job group from a given category and the ClassJob sheet.
    /// It looks up the different jobs contained in the category and sets the flags appropriately.
    /// </summary>
    public JobGroup(ClassJobCategory group, ExcelSheet<ClassJob> jobs)
    {
        Count  = 0;
        _flags = 0ul;
        Id     = (JobGroupId)group.RowId;
        Name   = group.Name.ToDalamudString().ToString();

        Debug.Assert(jobs.RowCount < 64, $"Number of Jobs exceeded 63 ({jobs.RowCount}).");
        foreach (var job in jobs)
        {
            var abbr = job.Abbreviation.ToString();
            if (abbr.Length == 0)
                continue;

            var prop = group.GetType().GetProperty(abbr);
            Debug.Assert(prop != null, $"Could not get job abbreviation {abbr} property.");

            if (!(bool)prop.GetValue(group)!)
                continue;

            ++Count;
            _flags |= (JobFlag)(1ul << (int)job.RowId);
        }
    }

    /// <summary> Check if a job is contained inside this group. </summary>
    public bool Fits(Job job)
        => _flags.HasFlag(job.Flag);

    /// <summary> Check if a job is contained inside this group. </summary>
    public bool Fits(JobId jobId)
    {
        var flag = (JobFlag)(1ul << jobId.Id);
        return _flags.HasFlag(flag);
    }

    /// <summary> Check if any of the jobs in the given flags fit this group. </summary>
    public bool Fits(JobFlag flag)
        => (_flags & flag) != 0;

    /// <inheritdoc/>
    public override string ToString()
        => Name;

    /// <summary> Iterate over all jobs set in this group. </summary>
    public IEnumerable<JobId> Iterate()
    {
        var minSet = BitOperations.TrailingZeroCount((ulong)_flags);
        var maxSet = 64 - BitOperations.LeadingZeroCount((ulong)_flags);
        for (var i = minSet; i < maxSet; ++i)
        {
            if (_flags.HasFlag((JobFlag)(1ul << i)))
                yield return (JobId)i;
        }
    }
}
