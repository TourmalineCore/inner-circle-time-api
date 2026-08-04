using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ToDos.Handlers.HardDeleteToDo;

public class HardDeleteToDoCommand(
    TenantAppDbContext context
    )
{
    public async Task<bool> ExecuteAsync(long toDoId)
    {
        var toDoToDelete = await context
            .DeletedAndNotDeletedQueryableWithinTenant<ToDo>()
            .SingleAsync(x => x.Id == toDoId);

        context
            .Set<ToDo>()
            .Remove(toDoToDelete);

        await context.SaveChangesAsync();

        return true;
    }
}
