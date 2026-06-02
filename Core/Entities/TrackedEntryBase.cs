namespace Core.Entities;

public class TrackedEntryBase : EntityBase, IOwnedByEmployee, ICanBeDeleted
{
    // EntityFrameworkCore related empty default constructor
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public TrackedEntryBase()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    public required long EmployeeId { get; set; }

    public required DateTime StartTime { get; set; }

    public required DateTime EndTime { get; set; }

    // TODO: make it required when we add this prop to frontend
    public string? TimeZoneId { get; set; }

    public TimeSpan Duration { get; set; }

    public EntryType Type { get; set; }

    public DateTime? DeletedAtUtc { get; set; }

    public string? DeletionReason { get; set; }

    public int GetDurationInMinutes()
    {
        return (int)(EndTime - StartTime).TotalMinutes;
    }

    public decimal GetDurationInHours()
    {
        return GetDurationInMinutes().ToHoursWithoutRounding();
    }
}
