using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.Features.Tracking.UpdateVacationEntry;

public class UpdateVacationEntryRequest
{
    [JsonIgnore]
    public long Id { get; set; }

    [Required]
    public required PeriodDto Period { get; set; }

    [Required]
    public required bool IsUnpaid { get; set; }
}
