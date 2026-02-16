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
        var project = await _assignmentsApi.GetEmployeeProjectAsync(createWorkEntryRequest.ProjectId);

        var createWorkEntryCommandParams = new CreateWorkEntryCommandParams
        {
            Title = createWorkEntryRequest.Title,
            StartTime = createWorkEntryRequest.StartTime,
            EndTime = createWorkEntryRequest.EndTime,
            ProjectId = project.Id,
            TaskId = createWorkEntryRequest.TaskId,
            Description = createWorkEntryRequest.Description,
        };

        var newWorkEntryId = await _createWorkEntryCommand.ExecuteAsync(createWorkEntryCommandParams);

        return new CreateWorkEntryResponse
        {
            NewWorkEntryId = newWorkEntryId
        };
    }
}
