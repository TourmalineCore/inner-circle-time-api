using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ToDos.Handlers.GetToDos;

public class GetToDosQuery
{
    private readonly TenantAppDbContext _context;
    private readonly IClaimsProvider _claimsProvider;

    public GetToDosQuery(
        TenantAppDbContext context,
        IClaimsProvider claimsProvider
    )
    {
        _context = context;
        _claimsProvider = claimsProvider;
    }

    public Task<List<ToDo>> GetAsync()
    {
        return _context
            .QueryableWithinTenantAsNoTracking<ToDo>()
            .ToListAsync();
    }
}
