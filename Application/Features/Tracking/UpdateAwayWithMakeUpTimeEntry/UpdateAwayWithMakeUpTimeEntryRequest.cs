using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.Features.Tracking.UpdateAwayWithMakeUpTimeEntry;

public class UpdateAwayWithMakeUpTimeEntryRequest
{
    [JsonIgnore]
    public long Id { get; set; }

    [Required]
    public required DateTime StartTime { get; set; }

    [Required]
    public required DateTime EndTime { get; set; }

    [Required]
    public required string Description { get; set; }

    [Required]
    public required List<CreateOrUpdateMakeUpTimeEntryDto> MakeUpTimeList { get; set; }
}
