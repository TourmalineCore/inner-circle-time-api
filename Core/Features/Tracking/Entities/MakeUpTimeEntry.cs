using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Features.Tracking.Entities;

public class MakeUpTimeEntry : TrackedEntryBase
{
    // EntityFrameworkCore related empty default constructor
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public MakeUpTimeEntry()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
        Type = EntryType.MakeUpTime;
    }

    public long RelatedEntryId { get; set; }

    [ForeignKey(nameof(RelatedEntryId))]
    public TrackedEntryBase RelatedEntry { get; set; }

    public EntryType RelatedEntryType { get; set; }
}
