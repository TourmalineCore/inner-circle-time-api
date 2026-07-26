namespace Application.Features.ToDos.Handlers.GetToDos;

public class GetToDosHandler
{
    private readonly GetToDosQuery _getToDosQuery;

    public GetToDosHandler(
        GetToDosQuery getEntryByIdQuery
    )
    {
        _getToDosQuery = getEntryByIdQuery;
    }

    public async Task<GetToDosResponse> HandleAsync()
    {
        var toDos = await _getToDosQuery.GetAsync();

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
