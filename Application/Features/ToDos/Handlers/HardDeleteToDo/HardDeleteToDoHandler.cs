namespace Application.Features.ToDos.Handlers.HardDeleteToDo;

public class DeleteToDoHandler(
    DeleteToDoCommand deleteToDoCommand
)
{
    public async Task<DeleteToDoResponse> HandleAsync(
        long toDoId
    )
    {
        return new DeleteToDoResponse
        {
            IsDeleted = await deleteToDoCommand.ExecuteAsync(toDoId),
        };
    }
}
