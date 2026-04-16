using Application.Extensions;
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
        return _context
            .QueryableWithinTenantAsNoTracking<TaskEntry>()
            .Where(x => x.ProjectId == projectId)
            .FilterByPeriod(startDate, endDate)
            .ToListAsync();
    }
}
