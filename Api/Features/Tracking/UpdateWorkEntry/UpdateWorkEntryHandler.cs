using Application.Commands;
using Application.ExternalDeps.AssignmentsApi;
using Core.Entities;

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
        var project = _assignmentsApi.FindEmployeeProjectAsync(updateWorkEntryRequest.ProjectId);

        if (project == null)
        {
            throw new ArgumentException("This project doesn't exist or is not available");
        }

        var updateWorkEntryCommandParams = new UpdateWorkEntryCommandParams
        {
            Id = workEntryId,
            Title = updateWorkEntryRequest.Title,
            StartTime = updateWorkEntryRequest.StartTime,
            EndTime = updateWorkEntryRequest.EndTime,
            ProjectId = updateWorkEntryRequest.ProjectId,
            TaskId = updateWorkEntryRequest.TaskId,
            Description = updateWorkEntryRequest.Description,
            Type = EventType.Task, // TODO: after add other event types remove hardcode
        };

        await _updateWorkEntryCommand.ExecuteAsync(updateWorkEntryCommandParams);
    }
}
