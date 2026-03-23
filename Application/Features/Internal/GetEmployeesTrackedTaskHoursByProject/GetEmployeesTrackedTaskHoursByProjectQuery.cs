using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Internal.GetEmployeesTrackedTaskHoursByProject;

public class GetEmployeesTrackedTaskHoursByProjectQuery
{
    private readonly AppDbContext _context;

    public GetEmployeesTrackedTaskHoursByProjectQuery(
        AppDbContext context
    )
    {
        _context = context;
    }

    public Task<List<TEntity>> GetByProjectAndPeriodAsync<TEntity>(
        long projectId,
        DateOnly startDate,
        DateOnly endDate,
        long tenantId
    )
    where TEntity : TaskEntry
    {
        return _context.Set<TEntity>()
            .Where(x => x.TenantId == tenantId)
            .Where(x => x.DeletedAtUtc == null)
            .Where(x => x.ProjectId == projectId)
            .Where(x => x.StartTime >= startDate.ToDateTime(TimeOnly.MinValue) && x.EndTime <= endDate.ToDateTime(TimeOnly.MaxValue))
            .ToListAsync();
    }
}
