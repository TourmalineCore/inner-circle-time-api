using System.ComponentModel.DataAnnotations;

namespace Application.Features.Tracking.Handlers.CreateVacationEntry;

public class CreateVacationEntryRequest
{
    [Required]
    public required PeriodDto Period { get; set; }

    [Required]
    public required bool IsUnpaid { get; set; }
}
