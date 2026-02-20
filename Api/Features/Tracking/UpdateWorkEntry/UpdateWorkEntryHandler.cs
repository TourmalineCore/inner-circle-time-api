using Application.Commands;
using Application.ExternalDeps.AssignmentsApi;

namespace Api.Features.Tracking.UpdateWorkEntry;

public class UpdateWorkEntryHandler
{
    private readonly UpdateWorkEntryCommand _updateWorkEntryCommand;
    private readonly IAssignmentsApi _assignmentsApi;

    public UpdateWorkEntryHandler(
        UpdateWorkEntryCommand updateWorkEntryCommand,
        IAssignmentsApi assignmentsApi
    )
    {
        _updateWorkEntryCommand = updateWorkEntryCommand;
        _assignmentsApi = assignmentsApi;
    }

    public async Task HandleAsync(
        long workEntryId,
        UpdateWorkEntryRequest updateWorkEntryRequest
    )
    {
        var project = await _assignmentsApi.GetEmployeeProjectAsync(updateWorkEntryRequest.ProjectId);

        var updateWorkEntryCommandParams = new UpdateWorkEntryCommandParams
        {
            Id = workEntryId,
            Title = updateWorkEntryRequest.Title,
            StartTime = updateWorkEntryRequest.StartTime,
            EndTime = updateWorkEntryRequest.EndTime,
            ProjectId = project.Id,
            TaskId = updateWorkEntryRequest.TaskId,
            Description = updateWorkEntryRequest.Description,
        };

        await _updateWorkEntryCommand.ExecuteAsync(updateWorkEntryCommandParams);
    }
}
