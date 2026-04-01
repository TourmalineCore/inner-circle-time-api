using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Internal.GetEmployeesTrackedTaskHours;

public class GetTaskEntriesQuery
{
    private readonly TenantAppDbContext _context;

    public GetTaskEntriesQuery(
        TenantAppDbContext context
    )
    {
        _context = context;
    }

    public Task<List<TaskEntry>> GetAsync(
        long projectId,
        DateOnly startDate,
        DateOnly endDate
    )
    {
        // Todo: Think about how to test date filtering
        // Issue: https://github.com/orgs/TourmalineCore/projects/5/views/1?pane=issue&itemId=171292110&issue=TourmalineCore%7Cinner-circle-time-api%7C69
        return _context
            .QueryableWithinTenantAsNoTracking<TaskEntry>()
            .Where(x => x.ProjectId == projectId)
            .Where(x => x.StartTime >= startDate.ToDateTime(TimeOnly.MinValue) && x.EndTime <= endDate.ToDateTime(TimeOnly.MaxValue))
            .ToListAsync();
    }
}
