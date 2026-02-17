using Core.Entities;

namespace Api.Features.Tracking.GetWorkEntriesByPeriod;

public class GetWorkEntriesByPeriodResponse
{
    public required List<TrackingEntryDto> WorkEntries { get; set; }
}

public class TrackingEntryDto
{
    public required long Id { get; set; }
    public required DateTime StartTime { get; set; }
    public required DateTime EndTime { get; set; }
    public required EventType Type { get; set; }

    public string? Title { get; set; }
    public long? ProjectId { get; set; }
    public string? TaskId { get; set; }
    public string? Description { get; set; }
}
