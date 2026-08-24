using System.ComponentModel.DataAnnotations;

namespace Application.Features.Tracking.CreateVacationEntry;

public class CreateVacationEntryRequest
{
    [Required]
    public required PeriodDto Period { get; set; }

    [Required]
    public required bool IsUnpaid { get; set; }
}
