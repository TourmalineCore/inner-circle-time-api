namespace Application.Features.ToDos.Handlers.HardDeleteToDo;

public class HardDeleteToDoHandler
{
    private readonly HardDeleteToDoCommand _hardDeleteToDoCommand;

    public HardDeleteToDoHandler(
        HardDeleteToDoCommand hardDeleteToDoCommand
    )
    {
        _hardDeleteToDoCommand = hardDeleteToDoCommand;
    }

    public async Task<object> HandleAsync(long toDoId)
    {
        return new
        {
            isDeleted = await _hardDeleteToDoCommand.ExecuteAsync(toDoId)
        };
    }
}
