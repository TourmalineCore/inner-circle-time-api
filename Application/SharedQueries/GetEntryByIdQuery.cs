using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.SharedQueries;

public interface IGetEntryByIdQuery
{
    Task<TEntity?> GetAsync<TEntity>(long entryId) where TEntity : TrackedEntryBase;
}

public class GetEntryByIdQuery : IGetEntryByIdQuery
{
    private readonly TenantAppDbContext _context;
    private readonly IClaimsProvider _claimsProvider;

    public GetEntryByIdQuery(
        TenantAppDbContext context,
        IClaimsProvider claimsProvider
    )
    {
        _context = context;
        _claimsProvider = claimsProvider;
    }

    public Task<TEntity?> GetAsync<TEntity>(long entryId)
    where TEntity : TrackedEntryBase
    {
        return _context
            .QueryableWithinTenantAsNoTracking<TEntity>()
            .Where(x => x.EmployeeId == _claimsProvider.EmployeeId)
            .Where(x => x.Id == entryId)
            .Include(x => x.MakeUpTimeList)
            .SingleOrDefaultAsync();
    }
}
