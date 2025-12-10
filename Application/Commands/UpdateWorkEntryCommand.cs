using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands;

public class UpdateWorkEntryCommandParams
{
    public required long Id { get; set; }

    public required string Title { get; set; }

    public required DateTime StartTime { get; set; }

    public required DateTime EndTime { get; set; }

    public string? TaskId { get; set; }

    public required EventType Type { get; set; }
}

public class UpdateWorkEntryCommand
{
    private readonly TenantAppDbContext _context;
    private readonly IClaimsProvider _claimsProvider;

    public UpdateWorkEntryCommand(
        TenantAppDbContext context,
        IClaimsProvider claimsProvider
    )
    {
        _context = context;
        _claimsProvider = claimsProvider;
    }

    public async Task ExecuteAsync(UpdateWorkEntryCommandParams updateWorkEntryCommandParams)
    {
        var workEntry = await _context
            .QueryableWithinTenant<WorkEntry>()
            .Where(x => x.EmployeeId == _claimsProvider.EmployeeId)
            .Where(x => x.Id == updateWorkEntryCommandParams.Id)
            .SingleAsync();

        workEntry.Title = updateWorkEntryCommandParams.Title;
        workEntry.StartTime = updateWorkEntryCommandParams.StartTime;
        workEntry.EndTime = updateWorkEntryCommandParams.EndTime;
        workEntry.TaskId = updateWorkEntryCommandParams.TaskId;
        workEntry.Type = updateWorkEntryCommandParams.Type;

        _context
            .WorkEntries
            .Update(workEntry);

        await _context.SaveChangesAsync();
    }
}
