using Core;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands;

public class HardDeleteEntityCommand
{
    private readonly TenantAppDbContext _context;
    private readonly IClaimsProvider _claimsProvider;

    public HardDeleteEntityCommand(TenantAppDbContext context, IClaimsProvider claimsProvider)
    {
        _context = context;
        _claimsProvider = claimsProvider;
    }

    public async Task<bool> ExecuteAsync<TEntity>(long entityId)
        where TEntity : EntityBase
    {
        var currentEmployeeId = _claimsProvider.EmployeeId;

        var entity = await _context
            .QueryableWithinTenant<TEntity>()
            .Where(x => x.EmployeeId == currentEmployeeId)
            .SingleOrDefaultAsync(x => x.Id == entityId);

        if (entity == null)
        {
            return false;
        }

        _context
            .Set<TEntity>()
            .Remove(entity);

        await _context.SaveChangesAsync();

        return true;
    }
}
