using System.ComponentModel.DataAnnotations;
using Core.Entities;

public class MakeUpTimeEntry : TrackedEntryBase
{
    // EntityFrameworkCore related empty default constructor
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public MakeUpTimeEntry()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
        Type = EntryType.MakeUpTime;
    }

    [Required]
    public long RelatedEntryId { get; set; }
}
