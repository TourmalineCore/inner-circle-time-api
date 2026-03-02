namespace Application.Features.Tracking.CreateUnwellEntry;

public class CreateUnwellEntryHandler
{
    private readonly CreateUnwellEntryCommand _createUnwellEntryCommand;


    public CreateUnwellEntryHandler(
        CreateUnwellEntryCommand createUnwellEntryCommand
    )
    {
        _createUnwellEntryCommand = createUnwellEntryCommand;
    }

    public async Task<CreateUnwellResponse> HandleAsync(
        CreateUnwellEntryRequest createUnwellEntryRequest
    )
    {
        var newUnwellEntryId = await _createUnwellEntryCommand.ExecuteAsync(createUnwellEntryRequest);

        return new CreateUnwellResponse
        {
            NewUnwellEntryId = newUnwellEntryId
        };
    }
}
