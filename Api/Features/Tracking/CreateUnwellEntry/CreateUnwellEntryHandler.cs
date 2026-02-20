using Application.Commands;

namespace Api.Features.Tracking.CreateUnwellEntry;

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
        var createUnwellEntryCommandParams = new CreateUnwellEntryCommandParams
        {
            StartTime = createUnwellEntryRequest.StartTime,
            EndTime = createUnwellEntryRequest.EndTime,
        };

        var newUnwellEntryId = await _createUnwellEntryCommand.ExecuteAsync(createUnwellEntryCommandParams);

        return new CreateUnwellResponse
        {
            NewUnwellEntryId = newUnwellEntryId
        };
    }
}
