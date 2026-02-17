using System.ComponentModel.DataAnnotations;

namespace Api.Features.Tracking.UpdateUnwellEntry;

public class UpdateUnwellEntryRequest
{
    [Required]
    public required DateTime StartTime { get; set; }

    [Required]
    public required DateTime EndTime { get; set; }
}
