using Core.Features.Tracking.Entities;

namespace Application.Features.Tracking.Handlers.GetSickLeaveEntry;

public class GetSickLeaveEntryResponse
{
    public required long Id { get; set; }

    public required PeriodDto Period { get; set; }

    public required EntryType EntryType { get; set; }
}
