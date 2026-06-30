using Core.Entities;

public class GetUnwellEntryDto
{
    public required long Id { get; set; }

    public required DateTime StartTime { get; set; }

    public required DateTime EndTime { get; set; }

    public required EntryType Type { get; set; }
}
