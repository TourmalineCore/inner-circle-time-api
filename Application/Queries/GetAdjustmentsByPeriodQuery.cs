using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries;

public class GetAdjustmentsByPeriodQuery
{
    private readonly TenantAppDbContext _context;
    private readonly IClaimsProvider _claimsProvider;

    public GetAdjustmentsByPeriodQuery(
        TenantAppDbContext context,
        IClaimsProvider claimsProvider
    )
    {
        _context = context;
        _claimsProvider = claimsProvider;
    }

    public Task<List<Adjustment>> GetByPeriodAsync(
        DateOnly startDate,
        DateOnly endDate
    )
    {
        return _context
            .QueryableWithinTenantAsNoTracking<Adjustment>()
            .Where(x => x.EmployeeId == _claimsProvider.EmployeeId)
            .Where(x => x.IsDeleted == false)
            .Where(x => x.StartTime >= startDate.ToDateTime(TimeOnly.MinValue) && x.EndTime <= endDate.ToDateTime(TimeOnly.MaxValue))
            .ToListAsync();
    }
}
