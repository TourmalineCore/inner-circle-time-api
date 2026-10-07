using System.ComponentModel.DataAnnotations;

namespace Application.SharedDtos;

public class CreateOrUpdateMakeUpTimeEntryDto
{
    [Required]
    public required DateTime StartTime { get; set; }

    [Required]
    public required DateTime EndTime { get; set; }
}
