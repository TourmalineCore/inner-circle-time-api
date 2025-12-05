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
        where TEntity : EntityBase
    {
        return Set<TEntity>()
            .Where(x => x.TenantId == _tenantId);
    }

    public IQueryable<TEntity> QueryableWithinTenantAsNoTracking<TEntity>()
        where TEntity : EntityBase
    {
        return QueryableWithinTenant<TEntity>()
            .AsNoTracking();
    }
}
