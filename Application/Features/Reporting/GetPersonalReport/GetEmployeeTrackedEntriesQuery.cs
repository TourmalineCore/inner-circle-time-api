using Application.Extensions;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Reporting.GetPersonalReport;

public class GetEmployeeTrackedEntriesQuery
{
    private readonly TenantAppDbContext _context;

    public GetEmployeeTrackedEntriesQuery(
        TenantAppDbContext context
    )
    {
        _context = context;
    }

    public Task<List<TrackedEntryBase>> GetAsync(
        long employeeId,
        DateOnly startDate,
        DateOnly endDate
    )
    {
        return _context
            .QueryableWithinTenantAsNoTracking<TrackedEntryBase>()
            .Where(x => x.EmployeeId == employeeId)
            .FilterByPeriod(startDate, endDate)
            .ToListAsync();
    }
}
