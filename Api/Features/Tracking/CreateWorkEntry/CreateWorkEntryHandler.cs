using Application.Commands;
using Application.ExternalDeps.AssignmentsApi;
using Core.Entities;

namespace Api.Features.Tracking.CreateWorkEntry;

public class CreateWorkEntryHandler
{
    private readonly CreateWorkEntryCommand _createWorkEntryCommand;
    private readonly IAssignmentsApi _assignmentsApi;

    public CreateWorkEntryHandler(
        CreateWorkEntryCommand createWorkEntryCommand,
        IAssignmentsApi assignmentsApi
    )
    {
        _createWorkEntryCommand = createWorkEntryCommand;
        _assignmentsApi = assignmentsApi;
    }

    public async Task<CreateWorkEntryResponse> HandleAsync(
        CreateWorkEntryRequest createWorkEntryRequest
    )
    {
        var project = _assignmentsApi.FindEmployeeProjectAsync(createWorkEntryRequest.ProjectId);

        if (project == null)
        {
            throw new ArgumentException("This project doesn't exist or is not available");
        }

        var createWorkEntryCommandParams = new CreateWorkEntryCommandParams
        {
            Title = createWorkEntryRequest.Title,
            StartTime = createWorkEntryRequest.StartTime,
            EndTime = createWorkEntryRequest.EndTime,
            ProjectId = createWorkEntryRequest.ProjectId,
            TaskId = createWorkEntryRequest.TaskId,
            Description = createWorkEntryRequest.Description,
            Type = EventType.Task, // TODO: after add other event types remove hardcode
        };

        var newWorkEntryId = await _createWorkEntryCommand.ExecuteAsync(createWorkEntryCommandParams);

        return new CreateWorkEntryResponse
        {
            NewWorkEntryId = newWorkEntryId
        };
    }
}
