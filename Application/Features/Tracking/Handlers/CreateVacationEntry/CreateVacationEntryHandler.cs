namespace Application.Features.Tracking.Handlers.CreateVacationEntry;

public class CreateVacationEntryHandler
{
    private readonly CreateVacationEntryCommand _createVacationEntryCommand;

    public CreateVacationEntryHandler(
        CreateVacationEntryCommand createVacationEntryCommand
    )
    {
        _createVacationEntryCommand = createVacationEntryCommand;
    }

    public async Task<CreateVacationEntryResponse> HandleAsync(
        CreateVacationEntryRequest createVacationEntryRequest
    )
    {
        var newVacationEntryId = await _createVacationEntryCommand.ExecuteAsync(createVacationEntryRequest);

        return new CreateVacationEntryResponse
        {
            NewVacationEntryId = newVacationEntryId
        };
    }
}
