using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ToDos.Handlers.GetToDos;

public class GetToDosQuery(
    AppDbContext context
)
{
    public Task<List<ToDo>> GetAsync()
    {
        return context
            .QueryableAsNoTracking<ToDo>()
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync();
    }
}
