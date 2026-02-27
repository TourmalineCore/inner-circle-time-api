using System.ComponentModel.DataAnnotations;

namespace Application.Features.Tracking.SoftDeleteEntry;

public class SoftDeleteEntryRequest
{
    [Required]
    public required string DeletionReason { get; set; }
}
