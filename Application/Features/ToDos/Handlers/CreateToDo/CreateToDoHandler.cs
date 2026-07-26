namespace Application.Features.ToDos.Handlers.CreateToDo;

public class CreateToDoHandler
{
    private readonly CreateToDoCommand _createToDoCommand;

    public CreateToDoHandler(
        CreateToDoCommand createToDoCommand
    )
    {
        _createToDoCommand = createToDoCommand;
    }

    public async Task<CreateToDoResponse> HandleAsync(
        CreateToDoRequest createToDoRequest
    )
    {
        var newToDoId = await _createToDoCommand.ExecuteAsync(createToDoRequest);

        return new CreateToDoResponse
        {
            NewToDoId = newToDoId,
        };
    }
}
