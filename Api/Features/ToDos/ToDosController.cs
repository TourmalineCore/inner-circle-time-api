using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using Application.Features.ToDos.Handlers.CreateToDo;
using Application.Features.ToDos.Handlers.GetToDos;
using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.ToDos;

[Authorize]
[ApiController]
[Route("api/to-dos")]
public class ToDosController : ControllerBase
{
    private static long _nextToDoId = 0;
    private static readonly ConcurrentDictionary<long, ToDo> _toDos = new();

    [HttpGet]
    public Task<GetToDosResponse> GetToDosAsync()
    {
        return Task.FromResult(
            new GetToDosResponse
            {
                ToDos = _toDos
                    .Values
                    .Select(x => new ToDoDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                    })
                    .ToList(),
            }
        );
    }

    [HttpPost]
    public Task<CreateToDoResponse> CreatToDoAsync(
        [Required][FromBody] CreateToDoRequest createToDoRequest
    )
    {
        var newToDo = new ToDo
        {
            Id = Interlocked.Increment(ref _nextToDoId),
            Name = createToDoRequest.Name,
        };

        _toDos[newToDo.Id] = newToDo;

        return Task.FromResult(
            new CreateToDoResponse()
            {
                NewToDoId = newToDo.Id,
            }
        );
    }

    [HttpDelete]
    public object DeleteToDoAsync(
        [Required][FromQuery] long toDoId
    )
    {
        return new
        {
            isDeleted = _toDos.Remove(toDoId, out _)
        };
    }
}
