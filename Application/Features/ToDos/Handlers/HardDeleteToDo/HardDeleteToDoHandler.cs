namespace Application.Features.ToDos.Handlers.HardDeleteToDo;

public class HardDeleteToDoHandler(
    HardDeleteToDoCommand hardDeleteToDoCommand
)
{
    public async Task<DeleteToDoResponse> HandleAsync(
        long toDoId
    )
    {
        return new DeleteToDoResponse
        {
            IsDeleted = await hardDeleteToDoCommand.ExecuteAsync(toDoId)
        };
    }
}
