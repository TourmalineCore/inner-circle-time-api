using Core.Entities;

namespace Application.Commands;

public class CreateWorkEntryCommandParams
{
    public required long EmployeeId { get; set; }

    public required string Title { get; set; }

    public required DateTime StartTime { get; set; }

    public required DateTime EndTime { get; set; }

    public string? TaskId { get; set; }

    public required EventType Type { get; set; }
}

public class CreateWorkEntryCommand
{
    private readonly TenantAppDbContext _context;

    public CreateWorkEntryCommand(
        TenantAppDbContext context
    )
    {
        _context = context;
    }

    public async Task<long> ExecuteAsync(CreateWorkEntryCommandParams createWorkEntryCommandParams)
    {
        if (createWorkEntryCommandParams.Title == "" || createWorkEntryCommandParams.Type == EventType.Default)
        {
            throw new ArgumentException("Fill in all fields");
        }

        var workEntry = new WorkEntry
        {
            EmployeeId = createWorkEntryCommandParams.EmployeeId,
            Title = createWorkEntryCommandParams.Title,
            StartTime = createWorkEntryCommandParams.StartTime,
            EndTime = createWorkEntryCommandParams.EndTime,
            TaskId = createWorkEntryCommandParams.TaskId,
            Type = createWorkEntryCommandParams.Type,
        };

        await _context
            .WorkEntries
            .AddAsync(workEntry);

        await _context.SaveChangesAsync();

        return workEntry.Id;
    }
}
