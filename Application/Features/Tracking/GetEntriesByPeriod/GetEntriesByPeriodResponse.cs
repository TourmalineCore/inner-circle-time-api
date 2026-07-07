using Core.Entities;

namespace Application.Features.Tracking.GetEntriesByPeriod;

public class GetEntriesByPeriodResponse
{
    public required List<TaskEntryDto> TaskEntries { get; set; }

    public required List<UnwellEntryDto> UnwellEntries { get; set; }

    public required List<AwayWithMakeUpTimeEntryDto> AwayWithMakeUpTimeEntries { get; set; }

    public required List<MakeUpTimeEntryWithRelatedEntryDto> MakeUpTimeEntries { get; set; }

    public required List<SickLeaveEntryDto> SickLeaveEntries { get; set; }
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

public class AwayWithMakeUpTimeEntryDto
{
    public required long Id { get; set; }

    public required DateTime StartTime { get; set; }

    public required DateTime EndTime { get; set; }

    public required EntryType Type { get; set; }

    public required string Description { get; set; }

    public required List<MakeUpTimeEntryDto> MakeUpTimeList { get; set; }
}

public class MakeUpTimeEntryWithRelatedEntryDto
{
    public required long RelatedEntryId { get; set; }

    public required EntryType RelatedEntryType { get; set; }

    public required EntryType Type { get; set; }

    public required DateTime StartTime { get; set; }

    public required DateTime EndTime { get; set; }
}

public class SickLeaveEntryDto
{
    public required long Id { get; set; }

    public required EntryType EntryType { get; set; }

    public required DateOnly StartDate { get; set; }

    public required DateOnly EndDate { get; set; }
}

