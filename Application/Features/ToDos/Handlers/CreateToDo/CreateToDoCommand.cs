using Core;
using Core.Entities;

namespace Application.Features.ToDos.Handlers.CreateToDo;

public class CreateToDoCommand(
    AppDbContext context,
    IDateTimeProvider dateTimeProvider
)
{
    public async Task<long> ExecuteAsync(
        CreateToDoRequest createToDoRequest
    )
    {
        var newToDo = new ToDo
        {
            Name = createToDoRequest.Name,
            CreatedAtUtc = dateTimeProvider.UtcNow,
        };

        await context
            .ToDos
            .AddAsync(newToDo);

        await context.SaveChangesAsync();

        return newToDo.Id;
    }
}
