using System.ComponentModel.DataAnnotations;

namespace Application.Features.Tracking.SoftDeleteEntry;

public class SoftDeleteEntryRequest
{
    public long Id { get; set; }

    [Required]
    public required string DeletionReason { get; set; }
}
