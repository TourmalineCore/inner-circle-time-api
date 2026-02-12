using System.ComponentModel.DataAnnotations;
using Core.Entities;

namespace Api.Features.Tracking.CreateAdjustment;

public class CreateAdjustmentRequest
{
    [Required]
    public required EventType Type { get; set; }

    [Required]
    public required DateTime StartTime { get; set; }

    [Required]
    public required DateTime EndTime { get; set; }
}
