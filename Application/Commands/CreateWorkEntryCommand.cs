using Core.Entities;

namespace Application.Commands;

public class CreateWorkEntryCommandParams
{
    public required string Title { get; set; }

    public required DateTime StartTime { get; set; }

    public required DateTime EndTime { get; set; }

    public required long ProjectId { get; set; }

    public required string TaskId { get; set; }

    public required string Description { get; set; }
}

public class CreateWorkEntryCommand : DbValidationWorkEntryCommandBase<CreateWorkEntryCommandParams>
{
    private readonly TenantAppDbContext _context;
    private readonly IClaimsProvider _claimsProvider;

    public CreateWorkEntryCommand(
        TenantAppDbContext context,
        IClaimsProvider claimsProvider
    )
    {
        _context = context;
        _claimsProvider = claimsProvider;
    }

    public async Task<long> ExecuteAsync(CreateWorkEntryCommandParams createWorkEntryCommandParams)
    {
        return await MakeChangesInDbAsync(createWorkEntryCommandParams);
    }

    protected override async Task<long> MakeChangesToWorkEntryAsync(CreateWorkEntryCommandParams commandParams)
    {
        var workEntry = new WorkEntry
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
            .WorkEntries
            .AddAsync(workEntry);

        await _context.SaveChangesAsync();

        return workEntry.Id;
    }
}
