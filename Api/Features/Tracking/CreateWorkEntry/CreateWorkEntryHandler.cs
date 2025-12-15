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
        CreateWorkEntryRequest createWorkEntryRequest
    )
    {
        var createWorkEntryCommandParams = new CreateWorkEntryCommandParams
        {
            Title = createWorkEntryRequest.Title,
            StartTime = createWorkEntryRequest.StartTime,
            EndTime = createWorkEntryRequest.EndTime,
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
