using System.ComponentModel.DataAnnotations;

namespace Application.Features.Tracking.CreateSickLeaveEntry;

public class CreateSickLeaveEntryRequest
{
    [Required]
    public required PeriodDto Period { get; set; }
}
