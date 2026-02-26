namespace Application.Features.Tracking.UpdateTaskEntry;

public class UpdateTaskEntryHandler
{
    private readonly UpdateTaskEntryCommand _updateTaskEntryCommand;

    public UpdateTaskEntryHandler(
        UpdateTaskEntryCommand updateTaskEntryCommand
    )
    {
        _updateTaskEntryCommand = updateTaskEntryCommand;
    }

    public async Task HandleAsync(
        long taskEntryId,
        UpdateTaskEntryRequest updateTaskEntryRequest
    )
    {
        updateTaskEntryRequest.Id = taskEntryId;
        await _updateTaskEntryCommand.ExecuteAsync(updateTaskEntryRequest);
    }
}
