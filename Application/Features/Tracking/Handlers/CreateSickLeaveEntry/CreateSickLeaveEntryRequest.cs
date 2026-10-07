using System.ComponentModel.DataAnnotations;
using Application.SharedDtos;

namespace Application.Features.Tracking.Handlers.CreateSickLeaveEntry;

public class CreateSickLeaveEntryRequest
{
    [Required]
    public required PeriodDto Period { get; set; }
}
