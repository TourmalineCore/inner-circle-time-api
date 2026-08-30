using System.ComponentModel.DataAnnotations;
using Application.Features.ToDos.Handlers.CreateToDo;
using Application.Features.ToDos.Handlers.GetToDos;
using Application.Features.ToDos.Handlers.HardDeleteToDo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.ToDos;

[Authorize]
[ApiController]
[Route("api/to-dos")]
public class ToDosController : ControllerBase
{
    [HttpGet]
    public Task<GetToDosResponse> GetToDosAsync(
        [FromServices] GetToDosHandler getToDosHandler
    )
    {
        return getToDosHandler.HandleAsync();
    }

    [HttpPost]
    public Task<CreateToDoResponse> CreateToDoAsync(
        [Required][FromBody] CreateToDoRequest createToDoRequest,
        [FromServices] CreateToDoHandler createToDoHandler
    )
    {
        return createToDoHandler.HandleAsync(createToDoRequest);
    }

    [HttpDelete]
    public Task<DeleteToDoResponse> DeleteToDoAsync(
        [Required][FromQuery] long toDoId,
        [FromServices] DeleteToDoHandler deleteToDoHandler
    )
    {
        return deleteToDoHandler.HandleAsync(toDoId);
    }
}
