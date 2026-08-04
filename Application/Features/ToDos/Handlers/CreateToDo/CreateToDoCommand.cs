using Core.Entities;

namespace Application.Features.ToDos.Handlers.CreateToDo;

public class CreateToDoCommand(
    TenantAppDbContext context,
    IClaimsProvider claimsProvider
)
{
    public async Task<long> ExecuteAsync(CreateToDoRequest createToDoRequest)
    {
        var newToDo = new ToDo
        {
            TenantId = claimsProvider.TenantId,
            Name = createToDoRequest.Name,
        };

        await context
            .ToDos
            .AddAsync(newToDo);

        await context.SaveChangesAsync();

        return newToDo.Id;
    }
}
