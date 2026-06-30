using Core.Entities;

public class GetAwayWithMakeUpTimeEntryDto
{
    public required long Id { get; set; }

    public required DateTime StartTime { get; set; }

    public required DateTime EndTime { get; set; }

    public required EntryType Type { get; set; }

    public required string Description { get; set; }

    public required List<MakeUpTimeEntryWithIdDto> MakeUpTimeList { get; set; }
}

public class MakeUpTimeEntryWithIdDto
{
    public required long Id { get; set; }

    public required DateTime StartTime { get; set; }

    public required DateTime EndTime { get; set; }
}
