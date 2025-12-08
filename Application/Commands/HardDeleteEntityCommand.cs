using Core;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands;

public class HardDeleteEntityCommand
{
    private readonly TenantAppDbContext _context;

    public HardDeleteEntityCommand(TenantAppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExecuteAsync<TEntity>(long entityId)
        where TEntity : EntityBase
    {
        var entity = await _context
            .QueryableWithinTenant<TEntity>()
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
