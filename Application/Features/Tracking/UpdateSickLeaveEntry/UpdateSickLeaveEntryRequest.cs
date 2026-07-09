using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.Features.Tracking.UpdateSickLeaveEntry;

public class UpdateSickLeaveEntryRequest
{
    [JsonIgnore]
    public long Id { get; set; }

    [Required]
    public required PeriodDto Period { get; set; }
}
