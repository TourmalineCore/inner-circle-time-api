using Application.SharedCommands;
using Core.Features.Tracking.Entities;

namespace Application.Features.Tracking.Handlers.HardDeleteEntry;

public class HardDeleteEntryHandler
{
    private readonly HardDeleteEntityCommand _hardDeleteEntityCommand;

    public HardDeleteEntryHandler(TenantAppDbContext context, IClaimsProvider claimsProvider)
    {
        _hardDeleteEntityCommand = new HardDeleteEntityCommand(context, claimsProvider);
    }
    public async Task<object> HandleAsync(long entryId)
    {
        return new
        {
            isDeleted = await _hardDeleteEntityCommand.ExecuteAsync<TrackedEntryBase>(entryId)
        };
    }
}
