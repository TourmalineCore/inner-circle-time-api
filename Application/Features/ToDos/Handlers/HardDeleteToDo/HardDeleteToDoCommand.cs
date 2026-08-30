using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ToDos.Handlers.HardDeleteToDo;

public class DeleteToDoCommand(
    AppDbContext context
)
{
    public async Task<bool> ExecuteAsync(
        long toDoId
    )
    {
        var toDoToDelete = await context
            .Queryable<ToDo>()
            .SingleAsync(x => x.Id == toDoId);

        context
            .Set<ToDo>()
            .Remove(toDoToDelete);

        await context.SaveChangesAsync();

        return true;
    }
}
