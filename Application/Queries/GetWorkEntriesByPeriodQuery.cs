using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries;

public class GetWorkEntriesByPeriodQuery
{
    private readonly TenantAppDbContext _context;
    private readonly IClaimsProvider _claimsProvider;

    public GetWorkEntriesByPeriodQuery(
        TenantAppDbContext context,
        IClaimsProvider claimsProvider
    )
    {
        _context = context;
        _claimsProvider = claimsProvider;
    }

    public Task<List<WorkEntry>> GetByPeriodAsync(
        DateOnly startDate,
        DateOnly endDate
    )
    {
        return _context
            .QueryableWithinTenantAsNoTracking<WorkEntry>()
            .Where(x => x.EmployeeId == _claimsProvider.EmployeeId)
            .Where(x => x.IsDeleted == false)
            .Where(x => x.StartTime >= startDate.ToDateTime(TimeOnly.MinValue) && x.EndTime <= endDate.ToDateTime(TimeOnly.MaxValue))
            .ToListAsync();
    }
}
