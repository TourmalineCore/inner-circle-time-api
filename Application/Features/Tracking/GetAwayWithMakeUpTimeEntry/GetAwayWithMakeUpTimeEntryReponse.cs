using Application.Features.Tracking.GetEntriesByPeriod;
using Core.Entities;

namespace Application.Features.Tracking.GetAwayWithMakeUpTimeEntry;

public class GetAwayWithMakeUpTimeEntryResponse
{
    public required long Id { get; set; }

    public required DateTime StartTime { get; set; }

    public required DateTime EndTime { get; set; }

    public required EntryType Type { get; set; }

    public required string Description { get; set; }

    public required List<MakeUpTimeEntryWithIdDto> MakeUpTimeList { get; set; }
}
