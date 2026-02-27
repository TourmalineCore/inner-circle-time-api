using Application.SharedCommands;
using Core.Entities;

namespace Application.Features.Tracking.HardDeleteEntry;

public class HardDeleteEntryHandler
{
    private readonly HardDeleteEntityCommand _hardDeleteEntityCommand;

    public HardDeleteEntryHandler(TenantAppDbContext context, IClaimsProvider claimsProvider)
    {
        _hardDeleteEntityCommand = new HardDeleteEntityCommand(context, claimsProvider);
    }
    public Task<bool> HandleAsync(long entryId)
    {
        return _hardDeleteEntityCommand.ExecuteAsync<TrackedEntryBase>(entryId);
    }
}
