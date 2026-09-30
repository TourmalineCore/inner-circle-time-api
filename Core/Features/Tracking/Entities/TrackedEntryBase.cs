namespace Core.Features.Tracking.Entities;

public class TrackedEntryBase : EntityBase, IOwnedByEmployee, ICanBeDeleted
{
    private DateTime _startTime;

    private DateTime _endTime;

    // EntityFrameworkCore related empty default constructor
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public TrackedEntryBase()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    public long EmployeeId { get; set; }

    // The time tracker stores the time exactly to the minute.
    // Seconds are discarded when assigning
    public DateTime StartTime
    {
        get => _startTime;
        set => _startTime = TrimToMinutes(value);
    }

    public DateTime EndTime
    {
        get => _endTime;
        set => _endTime = TrimToMinutes(value);
    }

    // TODO: make it required when we add this prop to frontend
    public string? TimeZoneId { get; set; }

    public TimeSpan Duration { get; set; }

    public EntryType Type { get; set; }

    public DateTime? DeletedAtUtc { get; set; }

    public string? DeletionReason { get; set; }

    public List<MakeUpTimeEntry> MakeUpTimeList { get; set; }

    public int GetDurationInMinutes()
    {
        return (int)(EndTime - StartTime).TotalMinutes;
    }

    public decimal GetDurationInHours()
    {
        return GetDurationInMinutes().ToHoursWithoutRounding();
    }

    private static DateTime TrimToMinutes(DateTime dateTime)
    {
        return new DateTime(
            dateTime.Year,
            dateTime.Month,
            dateTime.Day,
            dateTime.Hour,
            dateTime.Minute,
            0,
            dateTime.Kind
        );
    }
}
