using System.ComponentModel.DataAnnotations;

namespace Application.Features.Tracking.UpdateUnwellEntry;

public class UpdateUnwellEntryRequest
{
    public long Id { get; set; }

    [Required]
    public required DateTime StartTime { get; set; }

    [Required]
    public required DateTime EndTime { get; set; }
}
