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
    public async Task<object> HandleAsync(long entryId)
    {
        return new
        {
            isDeleted = await _hardDeleteEntityCommand.ExecuteAsync<TrackedEntryBase>(entryId)
        };
    }
}
