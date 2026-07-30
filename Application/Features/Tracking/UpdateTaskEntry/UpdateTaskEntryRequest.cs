using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.Features.Tracking.UpdateTaskEntry;

public class UpdateTaskEntryRequest
{
    [JsonIgnore]
    public long Id { get; set; }

    [Required]
    public required string Title { get; set; }

    [Required]
    public required DateTime StartTime { get; set; }

    [Required]
    public required DateTime EndTime { get; set; }

    [Required]
    public required long ProjectId { get; set; }

    [Required]
    public required string TaskId { get; set; }

    [Required]
    public required string Description { get; set; }
}
