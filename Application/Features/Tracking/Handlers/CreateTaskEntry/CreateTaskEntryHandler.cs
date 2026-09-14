namespace Application.Features.Tracking.Handlers.CreateTaskEntry;

public class CreateTaskEntryHandler
{
    private readonly CreateTaskEntryCommand _createTaskEntryCommand;

    public CreateTaskEntryHandler(
        CreateTaskEntryCommand createTaskEntryCommand
    )
    {
        _createTaskEntryCommand = createTaskEntryCommand;
    }

    public async Task<CreateTaskEntryResponse> HandleAsync(
        CreateTaskEntryRequest createTaskEntryRequest
    )
    {
        var newTaskEntryId = await _createTaskEntryCommand.ExecuteAsync(createTaskEntryRequest);

        return new CreateTaskEntryResponse
        {
            NewTaskEntryId = newTaskEntryId
        };
    }
}
