using System.ComponentModel.DataAnnotations;

namespace Application.Features.Tracking.CreateAwayWithMakeUpTimeEntry;

public class CreateAwayWithMakeUpTimeEntryRequest
{
    [Required]
    public required DateTime StartTime { get; set; }

    [Required]
    public required DateTime EndTime { get; set; }

    [Required]
    public required string Description { get; set; }

    [Required]
    public required List<MakeUpTimeEntry> MakeUpTimeList { get; set; }
}
