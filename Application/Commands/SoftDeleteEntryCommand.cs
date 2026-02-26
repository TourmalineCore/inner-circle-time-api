using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands;

public class SoftDeleteEntryCommand
{
    private readonly TenantAppDbContext _context;
    private readonly IClaimsProvider _claimsProvider;

    public SoftDeleteEntryCommand(TenantAppDbContext context, IClaimsProvider claimsProvider)
    {
        _context = context;
        _claimsProvider = claimsProvider;
    }

    public async Task<bool> ExecuteAsync(long entryId)
    {
        var entry = await _context
            .QueryableWithinTenant<TrackedEntryBase>()
            .Where(x => x.EmployeeId == _claimsProvider.EmployeeId)
            .Where(x => x.DeletedAtUtc == null)
            .SingleOrDefaultAsync(x => x.Id == entryId);

        if (entry == null)
        {
            return false;
        }

        entry.DeletedAtUtc = DateTime.UtcNow;

        _context
            .Set<TrackedEntryBase>()
            .Update(entry);

        await _context.SaveChangesAsync();

        return true;
    }
}
