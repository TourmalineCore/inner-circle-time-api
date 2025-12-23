namespace Api.Features.Tracking.GetWorkEntriesByPeriod;

public class GetWorkEntriesByPeriodResponse
{
    public required List<WorkEntryItem> WorkEntries { get; set; }
}

public class WorkEntryItem
{
    public required long Id { get; set; }

    public required string Title { get; set; }

    public required DateTime StartTime { get; set; }

    public required DateTime EndTime { get; set; }

    public required string ProjectName { get; set; }

    public required string TaskId { get; set; }

    public required string Description { get; set; }
}
