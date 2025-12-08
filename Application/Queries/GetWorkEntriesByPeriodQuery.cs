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
        DateTime startTime,
        DateTime endTime
    )
    {
        return _context
            .QueryableWithinTenantAsNoTracking<WorkEntry>()
            .Where(x => x.EmployeeId == _claimsProvider.EmployeeId)
            .Where(x => x.StartTime >= startTime && x.EndTime <= endTime)
            .ToListAsync();
    }
}
