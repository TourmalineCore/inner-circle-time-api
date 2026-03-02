using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Tracking.UpdateTaskEntry;

public class UpdateTaskEntryCommand : DbValidationEntryCommandBase<UpdateTaskEntryRequest>
{
    private readonly TenantAppDbContext _context;
    private readonly IClaimsProvider _claimsProvider;

    public UpdateTaskEntryCommand(
        TenantAppDbContext context,
        IClaimsProvider claimsProvider
    )
    {
        _context = context;
        _claimsProvider = claimsProvider;
    }

    public async Task<long> ExecuteAsync(UpdateTaskEntryRequest updateTaskEntryRequest)
    {
        return await MakeChangesInDbAsync(updateTaskEntryRequest);
    }

    protected override async Task<long> MakeChangesToEntryAsync(UpdateTaskEntryRequest updateTaskEntryRequest)
    {
        await _context
            .QueryableWithinTenant<TaskEntry>()
            .Where(x => x.EmployeeId == _claimsProvider.EmployeeId)
            .Where(x => x.Id == updateTaskEntryRequest.Id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.Title, updateTaskEntryRequest.Title)
                .SetProperty(x => x.StartTime, updateTaskEntryRequest.StartTime)
                .SetProperty(x => x.EndTime, updateTaskEntryRequest.EndTime)
                .SetProperty(x => x.TaskId, updateTaskEntryRequest.TaskId)
                .SetProperty(x => x.ProjectId, updateTaskEntryRequest.ProjectId)
                .SetProperty(x => x.Description, updateTaskEntryRequest.Description)
            );

        return updateTaskEntryRequest.Id;
    }
}
