using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands;

public class SoftDeleteEntityCommand
{
    private readonly TenantAppDbContext _context;
    private readonly IClaimsProvider _claimsProvider;

    public SoftDeleteEntityCommand(TenantAppDbContext context, IClaimsProvider claimsProvider)
    {
        _context = context;
        _claimsProvider = claimsProvider;
    }

    public async Task ExecuteAsync(long entityId)
    {
        await _context
            .QueryableWithinTenant<TrackedEntryBase>()
            .Where(x => x.EmployeeId == _claimsProvider.EmployeeId)
            .Where(x => x.Id == entityId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.DeletedAtUtc, DateTime.UtcNow)
        );
    }
}
