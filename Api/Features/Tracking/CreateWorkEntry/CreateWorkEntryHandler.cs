using Application.Commands;
using Core.Entities;

namespace Api.Features.Tracking.CreateWorkEntry;

public class CreateWorkEntryHandler
{
    private readonly CreateWorkEntryCommand _createWorkEntryCommand;

    public CreateWorkEntryHandler(
        CreateWorkEntryCommand createWorkEntryCommand
    )
    {
        _createWorkEntryCommand = createWorkEntryCommand;
    }

    public async Task<CreateWorkEntryResponse> HandleAsync(
        CreateWorkEntryRequest createWorkEntryRequest,
        long employeeId
    )
    {
        var createWorkEntryCommandParams = new CreateWorkEntryCommandParams
        {
            EmployeeId = employeeId,
            Title = createWorkEntryRequest.Title,
            StartTime = createWorkEntryRequest.StartTime,
            EndTime = createWorkEntryRequest.EndTime,
            TaskId = createWorkEntryRequest.TaskId,
            Type = EventType.Task, // TODO: after add other event types remove hardcode
        };

        var newWorkEntryId = await _createWorkEntryCommand.ExecuteAsync(createWorkEntryCommandParams);

        return new CreateWorkEntryResponse
        {
            NewWorkEntryId = newWorkEntryId
        };
    }
}
