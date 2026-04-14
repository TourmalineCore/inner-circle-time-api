using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Reporting.GetPersonalReport;

public class GetTrackedEntriesQuery
{
    private readonly TenantAppDbContext _context;

    public GetTrackedEntriesQuery(
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
        // Todo: Think about how to test date filtering
        // Issue: https://github.com/orgs/TourmalineCore/projects/5/views/1?pane=issue&itemId=171292110&issue=TourmalineCore%7Cinner-circle-time-api%7C69
        return _context
            .QueryableWithinTenantAsNoTracking<TrackedEntryBase>()
            .Where(x => x.EmployeeId == employeeId)
            .Where(x => x.StartTime >= startDate.ToDateTime(TimeOnly.MinValue) && x.EndTime <= endDate.ToDateTime(TimeOnly.MaxValue))
            .ToListAsync();
    }
}
