using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ToDos.Handlers.GetToDos;

public class GetToDosQuery(
    TenantAppDbContext context
)
{
    public Task<List<ToDo>> GetAsync()
    {
        return context
            .QueryableWithinTenantAsNoTracking<ToDo>()
            .OrderByDescending(x => x.Id)
            .ToListAsync();
    }
}
