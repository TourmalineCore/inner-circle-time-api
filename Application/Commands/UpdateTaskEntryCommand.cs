using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands;

public class UpdateTaskEntryCommandParams
{
    public required long Id { get; set; }

    public required string Title { get; set; }

    public required DateTime StartTime { get; set; }

    public required DateTime EndTime { get; set; }

    public required string TaskId { get; set; }

    public required long ProjectId { get; set; }

    public required string Description { get; set; }
}

public class UpdateTaskEntryCommand : DbValidationEntryCommandBase<UpdateTaskEntryCommandParams>
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

    public async Task<long> ExecuteAsync(UpdateTaskEntryCommandParams updateTaskEntryCommandParams)
    {
        return await MakeChangesInDbAsync(updateTaskEntryCommandParams);
    }

    protected override async Task<long> MakeChangesToEntryAsync(UpdateTaskEntryCommandParams commandParams)
    {
        await _context
            .QueryableWithinTenant<TaskEntry>()
            .Where(x => x.EmployeeId == _claimsProvider.EmployeeId)
            .Where(x => x.Id == commandParams.Id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.Title, commandParams.Title)
                .SetProperty(x => x.StartTime, commandParams.StartTime)
                .SetProperty(x => x.EndTime, commandParams.EndTime)
                .SetProperty(x => x.TaskId, commandParams.TaskId)
                .SetProperty(x => x.ProjectId, commandParams.ProjectId)
                .SetProperty(x => x.Description, commandParams.Description)
            );

        return commandParams.Id;
    }
}
