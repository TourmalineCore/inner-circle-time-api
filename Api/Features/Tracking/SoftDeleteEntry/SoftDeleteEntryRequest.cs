using System.ComponentModel.DataAnnotations;

namespace Api.Features.Tracking.SoftDeleteEntry;

public class SoftDeleteEntryRequest
{
    [Required]
    public required string DeletionReason { get; set; }
}
