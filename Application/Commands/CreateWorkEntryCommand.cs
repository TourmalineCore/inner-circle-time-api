using Core.Entities;

namespace Application.Commands;

public class CreateWorkEntryCommandParams
{
    public required string Title { get; set; }

    public required DateTime StartTime { get; set; }

    public required DateTime EndTime { get; set; }

    public string? TaskId { get; set; }
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
        var workEntry = new WorkEntry
        {
            Title = createWorkEntryCommandParams.Title,
            StartTime = createWorkEntryCommandParams.StartTime,
            EndTime = createWorkEntryCommandParams.EndTime,
            TaskId = createWorkEntryCommandParams.TaskId
        };

        await _context
            .WorkEntries
            .AddAsync(workEntry);

        await _context.SaveChangesAsync();

        return workEntry.Id;
    }
}
