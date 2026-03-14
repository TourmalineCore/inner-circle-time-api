using System.ComponentModel.DataAnnotations;

namespace Application.Features.Tracking.CreateUnwellEntry;

public class CreateUnwellEntryRequest
{
    [Required]
    public required DateTime StartTime { get; set; }

    [Required]
    public required DateTime EndTime { get; set; }
}
