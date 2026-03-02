using Core.Entities;

namespace Application.Features.Tracking.CreateTaskEntry;

public class CreateTaskEntryCommand : DbValidationEntryCommandBase<CreateTaskEntryRequest>
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

    public async Task<long> ExecuteAsync(CreateTaskEntryRequest createTaskEntryRequest)
    {
        return await MakeChangesInDbAsync(createTaskEntryRequest);
    }

    protected override async Task<long> MakeChangesToEntryAsync(CreateTaskEntryRequest createTaskEntryRequest)
    {
        var taskEntry = new TaskEntry
        {
            TenantId = _claimsProvider.TenantId,
            EmployeeId = _claimsProvider.EmployeeId,
            Title = createTaskEntryRequest.Title,
            StartTime = createTaskEntryRequest.StartTime,
            EndTime = createTaskEntryRequest.EndTime,
            ProjectId = createTaskEntryRequest.ProjectId,
            TaskId = createTaskEntryRequest.TaskId,
            Description = createTaskEntryRequest.Description,
        };

        await _context
            .TaskEntries
            .AddAsync(taskEntry);

        await _context.SaveChangesAsync();

        return taskEntry.Id;
    }
}
