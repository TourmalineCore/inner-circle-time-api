namespace Core.Entities;

public class WorkEntry : TrackingEntryBase
{
    // EntityFrameworkCore related empty default constructor
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public WorkEntry()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
        Type = EntryType.Task;
    }

    public string Title { get; set; }

    public long ProjectId { get; set; }

    public string TaskId { get; set; }

    public string Description { get; set; }
}
