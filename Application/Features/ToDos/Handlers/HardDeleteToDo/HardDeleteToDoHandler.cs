namespace Application.Features.ToDos.Handlers.HardDeleteToDo;

public class HardDeleteToDoHandler(
    HardDeleteToDoCommand hardDeleteToDoCommand
)
{
    public async Task<object> HandleAsync(long toDoId)
    {
        return new
        {
            isDeleted = await hardDeleteToDoCommand.ExecuteAsync(toDoId)
        };
    }
}
