using Core.Entities;

namespace Application.Features.Tracking.GetVacationEntry;

public class GetVacationEntryResponse
{
    public required long Id { get; set; }

    public required EntryType EntryType { get; set; }

    public required PeriodDto Period { get; set; }

    public required bool IsUnpaid { get; set; }
}
