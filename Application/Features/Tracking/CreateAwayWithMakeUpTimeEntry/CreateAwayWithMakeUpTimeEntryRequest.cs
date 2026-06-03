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
    public required List<MakeUpTimeEntryDto> MakeUpTimeList { get; set; }
}


public class MakeUpTimeEntryDto
{
    [Required]
    public required DateTime StartTime { get; set; }

    [Required]
    public required DateTime EndTime { get; set; }
}
