using Application.ExternalDeps.AssignmentsApi;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands;

public class UpdateWorkEntryCommandParams
{
    public required long Id { get; set; }

    public required string Title { get; set; }

    public required DateTime StartTime { get; set; }

    public required DateTime EndTime { get; set; }

    public required string TaskId { get; set; }

    public required long ProjectId { get; set; }

    public required string Description { get; set; }

    public required EventType Type { get; set; }
}

public class UpdateWorkEntryCommand
{
    private readonly TenantAppDbContext _context;
    private readonly IClaimsProvider _claimsProvider;
    private readonly IAssignmentsApi _assignmentsApi;

    public UpdateWorkEntryCommand(
        TenantAppDbContext context,
        IClaimsProvider claimsProvider,
        IAssignmentsApi assignmentsApi
    )
    {
        _context = context;
        _claimsProvider = claimsProvider;
        _assignmentsApi = assignmentsApi;
    }

    public async Task ExecuteAsync(UpdateWorkEntryCommandParams updateWorkEntryCommandParams)
    {
        var project = _assignmentsApi.FindEmployeeProjectAsync(updateWorkEntryCommandParams.ProjectId);

        if (project == null)
        {
            throw new ArgumentException("This project doesn't exist or is not available");
        }

        await _context
            .QueryableWithinTenant<WorkEntry>()
            .Where(x => x.EmployeeId == _claimsProvider.EmployeeId)
            .Where(x => x.Id == updateWorkEntryCommandParams.Id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.Title, updateWorkEntryCommandParams.Title)
                .SetProperty(x => x.StartTime, updateWorkEntryCommandParams.StartTime)
                .SetProperty(x => x.EndTime, updateWorkEntryCommandParams.EndTime)
                .SetProperty(x => x.ProjectId, updateWorkEntryCommandParams.ProjectId)
                .SetProperty(x => x.TaskId, updateWorkEntryCommandParams.TaskId)
                .SetProperty(x => x.Description, updateWorkEntryCommandParams.Description)
                .SetProperty(x => x.Type, updateWorkEntryCommandParams.Type)
            );
    }
}
