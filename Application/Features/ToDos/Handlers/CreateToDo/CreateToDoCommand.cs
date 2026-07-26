using Core.Entities;

namespace Application.Features.ToDos.Handlers.CreateToDo;

public class CreateToDoCommand
{
    private readonly TenantAppDbContext _context;
    private readonly IClaimsProvider _claimsProvider;

    public CreateToDoCommand(
        TenantAppDbContext context,
        IClaimsProvider claimsProvider
    )
    {
        _context = context;
        _claimsProvider = claimsProvider;
    }

    public async Task<long> ExecuteAsync(CreateToDoRequest createToDoRequest)
    {
        var newToDo = new ToDo
        {
            TenantId = _claimsProvider.TenantId,
            Name = createToDoRequest.Name,
        };

        await _context
            .ToDos
            .AddAsync(newToDo);

        await _context.SaveChangesAsync();

        return newToDo.Id;
    }
}
