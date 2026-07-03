using Core.Entities;

namespace Application.Features.Tracking.GetUnwellEntry;

public class GetUnwellEntryResponse
{
    public required long Id { get; set; }

    public required DateTime StartTime { get; set; }

    public required DateTime EndTime { get; set; }

    public required EntryType Type { get; set; }

    public required EntryType EntryType { get; set; }
}
