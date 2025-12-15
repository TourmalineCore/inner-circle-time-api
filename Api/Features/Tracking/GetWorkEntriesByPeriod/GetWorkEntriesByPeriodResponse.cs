namespace Api.Features.Tracking.GetWorkEntriesByPeriod;

public class GetWorkEntriesByPeriodResponse
{
    public required List<WorkEntryItem> WorkEntries { get; set; }
}

public class WorkEntryItem
{
    public long Id { get; set; }

    public required string Title { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public required string TaskId { get; set; }

    public required string Description { get; set; }
}
