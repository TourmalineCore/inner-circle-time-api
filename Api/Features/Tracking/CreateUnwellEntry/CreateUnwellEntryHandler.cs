using Application.Commands;

namespace Api.Features.Tracking.CreateUnwellEntry;

public class CreateUnwellEntryHandler
{
    private readonly CreateUnwellEntryCommand _createWorkEntryCommand;


    public CreateUnwellEntryHandler(
        CreateUnwellEntryCommand createWorkEntryCommand
    )
    {
        _createWorkEntryCommand = createWorkEntryCommand;
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

        var newUnwellEntryId = await _createWorkEntryCommand.ExecuteAsync(createUnwellEntryCommandParams);

        return new CreateUnwellResponse
        {
            NewUnwellEntryId = newUnwellEntryId
        };
    }
}
