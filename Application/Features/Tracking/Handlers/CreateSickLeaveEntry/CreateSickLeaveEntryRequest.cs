using System.ComponentModel.DataAnnotations;

namespace Application.Features.Tracking.Handlers.CreateSickLeaveEntry;

public class CreateSickLeaveEntryRequest
{
    [Required]
    public required PeriodDto Period { get; set; }
}
