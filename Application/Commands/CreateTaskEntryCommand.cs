using Core.Entities;

namespace Application.Commands;

public class CreateTaskEntryCommandParams
{
    public required string Title { get; set; }

    public required DateTime StartTime { get; set; }

    public required DateTime EndTime { get; set; }

    public required long ProjectId { get; set; }

    public required string TaskId { get; set; }

    public required string Description { get; set; }
}

public class CreateTaskEntryCommand : DbValidationEntryCommandBase<CreateTaskEntryCommandParams>
{
    private readonly TenantAppDbContext _context;
    private readonly IClaimsProvider _claimsProvider;

    public CreateTaskEntryCommand(
        TenantAppDbContext context,
        IClaimsProvider claimsProvider
    )
    {
        _context = context;
        _claimsProvider = claimsProvider;
    }

    public async Task<long> ExecuteAsync(CreateTaskEntryCommandParams createTaskEntryCommandParams)
    {
        return await MakeChangesInDbAsync(createTaskEntryCommandParams);
    }

    protected override async Task<long> MakeChangesToEntryAsync(CreateTaskEntryCommandParams commandParams)
    {
        var taskEntry = new TaskEntry
        {
            TenantId = _claimsProvider.TenantId,
            EmployeeId = _claimsProvider.EmployeeId,
            Title = commandParams.Title,
            StartTime = commandParams.StartTime,
            EndTime = commandParams.EndTime,
            ProjectId = commandParams.ProjectId,
            TaskId = commandParams.TaskId,
            Description = commandParams.Description,
        };

        await _context
            .TaskEntries
            .AddAsync(taskEntry);

        await _context.SaveChangesAsync();

        return taskEntry.Id;
    }
}
