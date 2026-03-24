using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Internal.GetEmployeesTrackedTaskHoursByProject;

public class GetEmployeesTrackedTaskHoursByProjectQuery
{
    private readonly AppDbContext _context;

    private readonly IClaimsProvider _claimsProvider;

    public GetEmployeesTrackedTaskHoursByProjectQuery(
        AppDbContext context,
        IClaimsProvider claimsProvider
    )
    {
        _context = context;
        _claimsProvider = claimsProvider;
    }

    public Task<List<TEntity>> GetByProjectAndPeriodAsync<TEntity>(
        long projectId,
        DateOnly startDate,
        DateOnly endDate
    )
    where TEntity : TaskEntry
    {
        return _context.Set<TEntity>()
            .Where(x => x.TenantId == _claimsProvider.TenantId)
            .Where(x => x.DeletedAtUtc == null)
            .Where(x => x.ProjectId == projectId)
            .Where(x => x.StartTime >= startDate.ToDateTime(TimeOnly.MinValue) && x.EndTime <= endDate.ToDateTime(TimeOnly.MaxValue))
            .ToListAsync();
    }
}
