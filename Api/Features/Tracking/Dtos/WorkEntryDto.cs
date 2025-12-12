using System.ComponentModel.DataAnnotations;

namespace Api.Features.Tracking.Dtos;

public abstract class WorkEntryDto
{
    [Required]
    public required string Title { get; set; }

    [Required]
    public required DateTime StartTime { get; set; }

    [Required]
    public required DateTime EndTime { get; set; }

    public string? TaskId { get; set; }
}
