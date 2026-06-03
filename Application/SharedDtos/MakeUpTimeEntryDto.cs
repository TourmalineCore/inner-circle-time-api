using System.ComponentModel.DataAnnotations;

public class MakeUpTimeEntryDto
{
    [Required]
    public required DateTime StartTime { get; set; }

    [Required]
    public required DateTime EndTime { get; set; }
}
