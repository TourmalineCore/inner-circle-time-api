using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Application.SharedDtos;

namespace Application.Features.Tracking.Handlers.UpdateSickLeaveEntry;

public class UpdateSickLeaveEntryRequest
{
    [JsonIgnore]
    public long Id { get; set; }

    [Required]
    public required PeriodDto Period { get; set; }
}
