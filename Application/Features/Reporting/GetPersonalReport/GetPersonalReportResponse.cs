using Core.Entities;

namespace Application.Features.Reporting.GetPersonalReport;

public class GetPersonalReportResponse
{
    public required List<TrackedEntryDto> TrackedEntries { get; set; }

    public required decimal TaskHours { get; set; }

    public required decimal UnwellHours { get; set; }
}

public class TrackedEntryDto
{
    public required long Id { get; set; }

    public required decimal TrackedHoursPerDay { get; set; }

    public required DateTime StartTime { get; set; }

    public required DateTime EndTime { get; set; }

    public required decimal Hours { get; set; }

    public required EntryType EntryType { get; set; }

    public required ProjectDto Project { get; set; }

    public required TaskDto Task { get; set; }

    public string? Description { get; set; }
}

public class TaskDto
{
    public required string Id { get; set; }

    public required string Title { get; set; }
}
