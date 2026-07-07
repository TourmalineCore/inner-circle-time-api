using System.ComponentModel.DataAnnotations;

namespace Application.Features.Tracking.UpdateSickLeaveEntry;

public class UpdateSickLeaveEntryRequest
{
    public long Id { get; set; }

    [Required]
    public required PeriodDto Period { get; set; }
}
