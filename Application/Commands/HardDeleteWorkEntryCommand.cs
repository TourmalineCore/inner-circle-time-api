using Core.Entities;

namespace Application.Commands;

public class HardDeleteWorkEntryCommand
{
    private readonly HardDeleteEntityCommand _hardDeleteEntityCommand;

    public HardDeleteWorkEntryCommand(TenantAppDbContext context)
    {
        _hardDeleteEntityCommand = new HardDeleteEntityCommand(context);
    }

    public Task<bool> ExecuteAsync(long workEntryId)
    {
        return _hardDeleteEntityCommand.ExecuteAsync<WorkEntry>(workEntryId);
    }
}
