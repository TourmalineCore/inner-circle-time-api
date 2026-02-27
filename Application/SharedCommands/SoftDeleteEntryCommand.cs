using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.SharedCommands;

public class SoftDeleteEntryCommandParams
{
    public required string DeletionReason { get; set; }
}

public class SoftDeleteEntryCommand
{
    private readonly TenantAppDbContext _context;
    private readonly IClaimsProvider _claimsProvider;

    public SoftDeleteEntryCommand(TenantAppDbContext context, IClaimsProvider claimsProvider)
    {
        _context = context;
        _claimsProvider = claimsProvider;
    }

    public async Task<bool> ExecuteAsync(
        long entryId,
        SoftDeleteEntryCommandParams softDeleteEntryCommandParams
    )
    {
        var entry = await _context
            .QueryableWithinTenant<TrackedEntryBase>()
            .Where(x => x.EmployeeId == _claimsProvider.EmployeeId)
            .SingleOrDefaultAsync(x => x.Id == entryId);

        if (entry == null)
        {
            return false;
        }

        entry.DeletedAtUtc = DateTime.UtcNow;
        entry.DeletionReason = softDeleteEntryCommandParams.DeletionReason;

        _context
            .Set<TrackedEntryBase>()
            .Update(entry);

        await _context.SaveChangesAsync();

        return true;
    }
}
