using Core.Entities;

namespace Application.Features.Tracking.GetUnwellEntry;

public class GetUnwellEntryResponse
{
    public required GetUnwellEntryDto UnwellEntry { get; set; }
}

public class GetUnwellEntryDto
{
    public required long Id { get; set; }

    public required DateTime StartTime { get; set; }

    public required DateTime EndTime { get; set; }

    public required EntryType Type { get; set; }
}
