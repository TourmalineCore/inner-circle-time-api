using Core;
using Microsoft.EntityFrameworkCore;

namespace Application;

public class TenantAppDbContext : AppDbContext
{
    private readonly long _tenantId;

    public TenantAppDbContext(
        DbContextOptions<AppDbContext> options,
        IClaimsProvider claimsProvider
    )
    : base(options)
    {
        _tenantId = claimsProvider.TenantId;
    }

    // tests related constructor
    internal TenantAppDbContext(
        DbContextOptions<AppDbContext> options,
        long tenantId
    )
    : base(options)
    {
        _tenantId = tenantId;
    }

    public IQueryable<TEntity> QueryableWithinTenant<TEntity>()
        where TEntity : EntityBase, ICanBeDeleted
    {
        return Set<TEntity>()
            .Where(x => x.TenantId == _tenantId)
            .Where(x => x.DeletedAtUtc == null);
    }

    public IQueryable<TEntity> QueryableWithinTenantAsNoTracking<TEntity>()
        where TEntity : EntityBase, ICanBeDeleted
    {
        return QueryableWithinTenant<TEntity>()
            .AsNoTracking();
    }
}
