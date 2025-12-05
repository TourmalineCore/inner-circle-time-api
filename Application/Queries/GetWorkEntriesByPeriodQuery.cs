using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries;

public class GetWorkEntriesByPeriodQuery
{
    private readonly TenantAppDbContext _context;

    public GetWorkEntriesByPeriodQuery(TenantAppDbContext context)
    {
        _context = context;
    }

    public Task<List<WorkEntry>> GetByPeriodAsync(
        DateTime startTime,
        DateTime endTime,
        long employeeId
    )
    {
        return _context
            .QueryableWithinTenantAsNoTracking<WorkEntry>()
            .Where(x => x.EmployeeId == employeeId)
            .Where(x => x.StartTime >= startTime && x.EndTime <= endTime)
            .ToListAsync();
    }
}
