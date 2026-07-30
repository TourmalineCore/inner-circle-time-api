using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ToDos.Handlers.GetToDos;

public class GetToDosQuery
{
    private readonly TenantAppDbContext _context;

    public GetToDosQuery(
        TenantAppDbContext context
    )
    {
        _context = context;
    }

    public Task<List<ToDo>> GetAsync()
    {
        return _context
            .QueryableWithinTenantAsNoTracking<ToDo>()
            .OrderByDescending(x => x.Id)
            .ToListAsync();
    }
}
