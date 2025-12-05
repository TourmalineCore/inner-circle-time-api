using Microsoft.VisualBasic;

namespace Core.Entities;

public class WorkEntry : EntityBase
{
    // EntityFrameworkCore related empty default constructor
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public WorkEntry()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    public long EmployeeId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public string TimeZoneId { get; set; }

    public TimeSpan Duration { get; set; }

    public EventType Type { get; set; }

    public string Title { get; set; }

    public string? TaskId { get; set; } = null;

    // TODO: make it required when we add this prop to frontend
    public string? Description { get; set; } = null;

    public bool? IsDeleted { get; set; } = false;
}
