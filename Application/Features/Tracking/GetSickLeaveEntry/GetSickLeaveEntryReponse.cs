using Core.Entities;

namespace Application.Features.Tracking.GetSickLeaveEntry;

public class GetSickLeaveEntryResponse
{
    public required long Id { get; set; }

    public required PeriodDto Period { get; set; }

    public required EntryType EntryType { get; set; }
}
