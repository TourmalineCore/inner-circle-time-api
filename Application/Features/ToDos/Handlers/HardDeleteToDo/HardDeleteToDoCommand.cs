using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ToDos.Handlers.HardDeleteToDo;

public class HardDeleteToDoCommand
{
    private readonly TenantAppDbContext _context;
    private readonly IClaimsProvider _claimsProvider;

    public HardDeleteToDoCommand(
        TenantAppDbContext context,
        IClaimsProvider claimsProvider
    )
    {
        _context = context;
        _claimsProvider = claimsProvider;
    }

    public async Task<bool> ExecuteAsync(long toDoId)
    {
        var toDoToDelete = await _context
            .DeletedAndNotDeletedQueryableWithinTenant<ToDo>()
            .SingleAsync(x => x.Id == toDoId);

        _context
            .Set<ToDo>()
            .Remove(toDoToDelete);

        await _context.SaveChangesAsync();

        return true;
    }
}
