using Core.Entities;

namespace Application.Features.Tracking.GetEntriesByPeriod;

public class GetEntriesByPeriodResponse
{
    public required List<GetTaskEntryDto> TaskEntries { get; set; }

    public required List<GetUnwellEntryDto> UnwellEntries { get; set; }

    public required List<GetAwayWithMakeUpTimeEntryDto> AwayWithMakeUpTimeEntries { get; set; }

    public required List<GetMakeUpTimeEntryEntryDto> MakeUpTimeEntries { get; set; }
}


public class GetMakeUpTimeEntryEntryDto
{
    public required long RelatedEntryId { get; set; }

    public required EntryType Type { get; set; }

    public required DateTime StartTime { get; set; }

    public required DateTime EndTime { get; set; }
}

