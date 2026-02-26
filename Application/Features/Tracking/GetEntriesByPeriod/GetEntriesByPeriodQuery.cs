using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Tracking.GetEntriesByPeriod;

public class GetEntriesByPeriodQuery
{
    private readonly TenantAppDbContext _context;
    private readonly IClaimsProvider _claimsProvider;

    public GetEntriesByPeriodQuery(
        TenantAppDbContext context,
        IClaimsProvider claimsProvider
    )
    {
        _context = context;
        _claimsProvider = claimsProvider;
    }

    public Task<List<TEntity>> GetByPeriodAsync<TEntity>(
        DateOnly startDate,
        DateOnly endDate
    )
    where TEntity : TrackedEntryBase
    {
        return _context
            .QueryableWithinTenantAsNoTracking<TEntity>()
            .Where(x => x.EmployeeId == _claimsProvider.EmployeeId)
            .Where(x => x.IsDeleted == false)
            .Where(x => x.StartTime >= startDate.ToDateTime(TimeOnly.MinValue) && x.EndTime <= endDate.ToDateTime(TimeOnly.MaxValue))
            .ToListAsync();
    }
}
