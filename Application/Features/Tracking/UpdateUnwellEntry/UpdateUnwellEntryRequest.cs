using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.Features.Tracking.UpdateUnwellEntry;

public class UpdateUnwellEntryRequest
{
    [JsonIgnore]
    public long Id { get; set; }

    [Required]
    public required DateTime StartTime { get; set; }

    [Required]
    public required DateTime EndTime { get; set; }
}
