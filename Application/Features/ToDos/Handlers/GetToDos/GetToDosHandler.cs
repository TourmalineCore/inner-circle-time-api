namespace Application.Features.ToDos.Handlers.GetToDos;

public class GetToDosHandler(
    GetToDosQuery getEntryByIdQuery
)
{
    public async Task<GetToDosResponse> HandleAsync()
    {
        var toDos = await getEntryByIdQuery.GetAsync();

        return new GetToDosResponse
        {
            ToDos = toDos
                .Select(x => new ToDoDto
                {
                    Id = x.Id,
                    Name = x.Name,
                })
                .ToList(),
        };
    }
}
