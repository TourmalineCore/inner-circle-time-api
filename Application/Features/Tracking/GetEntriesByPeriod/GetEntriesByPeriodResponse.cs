using Core.Entities;

namespace Application.Features.Tracking.GetEntriesByPeriod;

public class GetEntriesByPeriodResponse
{
    public required List<TaskEntryDto> TaskEntries { get; set; }

    public required List<UnwellEntryDto> UnwellEntries { get; set; }
}

public class TaskEntryDto
{
    public required long Id { get; set; }

    public required string Title { get; set; }

    public required DateTime StartTime { get; set; }

    public required DateTime EndTime { get; set; }

    public required EntryType Type { get; set; }

    public required long ProjectId { get; set; }

    public required string TaskId { get; set; }

    public required string Description { get; set; }
}

public class UnwellEntryDto
{
    public required long Id { get; set; }

    public required DateTime StartTime { get; set; }

    public required DateTime EndTime { get; set; }

    public required EntryType Type { get; set; }
}
