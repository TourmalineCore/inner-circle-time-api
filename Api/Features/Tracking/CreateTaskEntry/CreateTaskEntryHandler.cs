using Application.Commands;
using Application.ExternalDeps.AssignmentsApi;

namespace Api.Features.Tracking.CreateTaskEntry;

public class CreateTaskEntryHandler
{
    private readonly CreateTaskEntryCommand _createTaskEntryCommand;

    private readonly IAssignmentsApi _assignmentsApi;

    public CreateTaskEntryHandler(
        CreateTaskEntryCommand createTaskEntryCommand,
        IAssignmentsApi assignmentsApi
    )
    {
        _createTaskEntryCommand = createTaskEntryCommand;
        _assignmentsApi = assignmentsApi;
    }

    public async Task<CreateTaskEntryResponse> HandleAsync(
        CreateTaskEntryRequest createTaskEntryRequest
    )
    {
        var project = await _assignmentsApi.GetEmployeeProjectAsync(createTaskEntryRequest.ProjectId);

        var createTaskEntryCommandParams = new CreateTaskEntryCommandParams
        {
            Title = createTaskEntryRequest.Title,
            StartTime = createTaskEntryRequest.StartTime,
            EndTime = createTaskEntryRequest.EndTime,
            ProjectId = project.Id,
            TaskId = createTaskEntryRequest.TaskId,
            Description = createTaskEntryRequest.Description,
        };

        var newTaskEntryId = await _createTaskEntryCommand.ExecuteAsync(createTaskEntryCommandParams);

        return new CreateTaskEntryResponse
        {
            NewTaskEntryId = newTaskEntryId
        };
    }
}
