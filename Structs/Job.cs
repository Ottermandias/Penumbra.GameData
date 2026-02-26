using ImSharp;
using Lumina.Excel.Sheets;

namespace Penumbra.GameData.Structs;

/// <summary>
/// A struct containing the different jobs the game supports.
/// Also contains the jobs Name and Abbreviation as strings.
/// </summary>
public sealed class Job
{
    internal Job(ClassJob job)
    {
        Id = (JobId)job.RowId;
        Role = job.Role switch
        {
            1                           => JobRole.Tank,
            2                           => JobRole.Melee,
            3 when job.PrimaryStat is 4 => JobRole.RangedMagical,
            3 when job.PrimaryStat is 2 => JobRole.RangedPhysical,
            4                           => JobRole.Healer,
            0 when job.PartyBonus is 6  => JobRole.Gatherer,
            0 when job.PartyBonus is 7  => JobRole.Crafter,
            _                           => JobRole.Unknown,
        };
        Name         = job.RowId is 0 ? new StringU8("Adventurer"u8) : new StringU8(job.Name.Data);
        Abbreviation = job.RowId is 0 ? new StringU8("ADV"u8) : new StringU8(job.Abbreviation.Data);
    }

    public enum JobRole : byte
    {
        Unknown        = 0,
        Tank           = 1,
        Melee          = 2,
        RangedPhysical = 3,
        RangedMagical  = 4,
        Healer         = 5,
        Crafter        = 6,
        Gatherer       = 7,
    }

    /// <summary> The name of the job. </summary>
    /// <remarks> They removed the name and abbreviation for Adventurer in 7.2 for some reason, so those are custom. </remarks>
    public readonly StringU8 Name;

    /// <summary> The 3-letter abbreviation of the job. </summary>
    /// <remarks> They removed the name and abbreviation for Adventurer in 7.2 for some reason, so those are custom. </remarks>
    public readonly StringU8 Abbreviation;

    /// <summary> The ID of the job. </summary>
    public readonly JobId Id;

    /// <summary> The role of the job. </summary>
    public readonly JobRole Role;

    /// <summary> The flag corresponding to the job's ID. </summary>
    public JobFlag Flag
        => (JobFlag)(1ul << Id.Id);

    /// <inheritdoc/>
    public override string ToString()
        => Name.ToString();
}
