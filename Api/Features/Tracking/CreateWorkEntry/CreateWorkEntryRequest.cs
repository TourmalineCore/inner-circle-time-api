using System.ComponentModel.DataAnnotations;

namespace Api.Features.Tracking.CreateWorkEntry;

public class CreateWorkEntryRequest
{
    [Required]
    public required string Title { get; set; }

    [Required]
    public required DateTime StartTime { get; set; }

    [Required]
    public required DateTime EndTime { get; set; }

    public string? TaskId { get; set; }
}
