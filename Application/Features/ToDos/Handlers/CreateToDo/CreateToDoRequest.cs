using System.ComponentModel.DataAnnotations;

namespace Application.Features.ToDos.Handlers.CreateToDo;

public class CreateToDoRequest
{
    [Required]
    public required string Name { get; set; }
}
