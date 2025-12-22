using Application.Commands;
using Core.Entities;

namespace Api.Features.Tracking.UpdateWorkEntry;

public class UpdateWorkEntryHandler
{
    private readonly UpdateWorkEntryCommand _updateWorkEntryCommand;

    public UpdateWorkEntryHandler(
        UpdateWorkEntryCommand updateWorkEntryCommand
    )
    {
        _updateWorkEntryCommand = updateWorkEntryCommand;
    }

    public async Task HandleAsync(
        long workEntryId,
        UpdateWorkEntryRequest updateEntryRequest
    )
    {
        var updateWorkEntryCommandParams = new UpdateWorkEntryCommandParams
        {
            Id = workEntryId,
            Title = updateEntryRequest.Title,
            StartTime = updateEntryRequest.StartTime,
            EndTime = updateEntryRequest.EndTime,
            ProjectId = updateEntryRequest.ProjectId,
            TaskId = updateEntryRequest.TaskId,
            Description = updateEntryRequest.Description,
            Type = EventType.Task, // TODO: after add other event types remove hardcode
        };

        await _updateWorkEntryCommand.ExecuteAsync(updateWorkEntryCommandParams);
    }
}
