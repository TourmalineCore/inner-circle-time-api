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
        UpdateWorkEntryRequest updateEntryRequest
    )
    {
        var project = await _assignmentsApi.GetEmployeeProjectAsync(updateEntryRequest.ProjectId);

        var updateWorkEntryCommandParams = new UpdateWorkEntryCommandParams
        {
            Id = workEntryId,
            Title = updateEntryRequest.Title,
            StartTime = updateEntryRequest.StartTime,
            EndTime = updateEntryRequest.EndTime,
            ProjectId = project.Id,
            TaskId = updateEntryRequest.TaskId,
            Description = updateEntryRequest.Description,
            Type = EventType.Task, // TODO: after add other event types remove hardcode
        };

        await _updateWorkEntryCommand.ExecuteAsync(updateWorkEntryCommandParams);
    }
}
