using System.ComponentModel.DataAnnotations;

namespace Api.Features.Tracking.UpdateWorkEntry;

public class UpdateWorkEntryRequest
{
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
