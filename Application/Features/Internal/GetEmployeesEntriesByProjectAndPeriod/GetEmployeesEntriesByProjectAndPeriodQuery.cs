using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Internal.GetEmployeesEntriesByProjectAndPeriod;

public class GetEmployeesEntriesByProjectAndPeriodQuery
{
    private readonly TenantAppDbContext _context;

    public GetEmployeesEntriesByProjectAndPeriodQuery(
        TenantAppDbContext context
    )
    {
        _context = context;
    }

    public Task<List<TEntity>> GetByProjectAndPeriodAsync<TEntity>(
        long projectId,
        DateOnly startDate,
        DateOnly endDate
    )
    where TEntity : TaskEntry
    {
        return _context
            .QueryableWithinTenantAsNoTracking<TEntity>()
            .Where(x => x.ProjectId == projectId)
            .Where(x => x.StartTime >= startDate.ToDateTime(TimeOnly.MinValue) && x.EndTime <= endDate.ToDateTime(TimeOnly.MaxValue))
            .ToListAsync();
    }
}
