using Dalamud.Utility;
using Lumina.Excel.GeneratedSheets;

namespace Penumbra.GameData.Structs;

/// <summary>
/// A struct containing the different jobs the game supports.
/// Also contains the jobs Name and Abbreviation as strings.
/// </summary>
public readonly struct Job(ClassJob job)
{
    /// <summary> The name of the job. </summary>
    public readonly string Name = string.Intern(job.Name.ToDalamudString().ToString());

    /// <summary> The 3-letter abbreviation of the job. </summary>
    public readonly string Abbreviation = string.Intern(job.Abbreviation.ToDalamudString().ToString());

    /// <summary> The ID of the job. </summary>
    public readonly JobId Id = (JobId)job.RowId;

    /// <summary> The flag corresponding to the job's ID. </summary>
    public JobFlag Flag
        => (JobFlag)(1ul << Id.Id);

    /// <inheritdoc/>
    public override string ToString()
        => Name;
}
