using Application.Commands;
using Application.ExternalDeps.AssignmentsApi;

namespace Api.Features.Tracking.UpdateTaskEntry;

public class UpdateTaskEntryHandler
{
    private readonly UpdateTaskEntryCommand _updateTaskEntryCommand;
    private readonly IAssignmentsApi _assignmentsApi;

    public UpdateTaskEntryHandler(
        UpdateTaskEntryCommand updateTaskEntryCommand,
        IAssignmentsApi assignmentsApi
    )
    {
        _updateTaskEntryCommand = updateTaskEntryCommand;
        _assignmentsApi = assignmentsApi;
    }

    public async Task HandleAsync(
        long taskEntryId,
        UpdateTaskEntryRequest updateTaskEntryRequest
    )
    {
        var project = await _assignmentsApi.GetEmployeeProjectAsync(updateTaskEntryRequest.ProjectId);

        var updateTaskEntryCommandParams = new UpdateTaskEntryCommandParams
        {
            Id = taskEntryId,
            Title = updateTaskEntryRequest.Title,
            StartTime = updateTaskEntryRequest.StartTime,
            EndTime = updateTaskEntryRequest.EndTime,
            ProjectId = project.Id,
            TaskId = updateTaskEntryRequest.TaskId,
            Description = updateTaskEntryRequest.Description,
        };

        await _updateTaskEntryCommand.ExecuteAsync(updateTaskEntryCommandParams);
    }
}
