namespace Application.Features.ToDos.Handlers.CreateToDo;

public class CreateToDoHandler(
    CreateToDoCommand createToDoCommand
)
{
    public async Task<CreateToDoResponse> HandleAsync(
        CreateToDoRequest createToDoRequest
    )
    {
        var newToDoId = await createToDoCommand.ExecuteAsync(createToDoRequest);

        return new CreateToDoResponse
        {
            NewToDoId = newToDoId,
        };
    }
}
