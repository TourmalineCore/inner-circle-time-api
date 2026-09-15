using Core.Features.Tracking.Entities;

namespace Application.Features.Tracking.Handlers.GetTaskEntry;

public class GetTaskEntryResponse
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
