namespace Application.Features.Tracking.CreateAwayWithMakeUpTimeEntry;

public class CreateAwayWithMakeUpTimeEntryHandler
{
    private readonly CreateAwayWithMakeUpTimeEntryCommand _createAwayWithMakeUpTimeEntryCommand;

    public CreateAwayWithMakeUpTimeEntryHandler(
        CreateAwayWithMakeUpTimeEntryCommand createAwayWithMakeUpTimeEntryCommand
    )
    {
        _createAwayWithMakeUpTimeEntryCommand = createAwayWithMakeUpTimeEntryCommand;
    }

    public async Task<CreateAwayWithMakeUpTimeEntryResponse> HandleAsync(
        CreateAwayWithMakeUpTimeEntryRequest createAwayWithMakeUpTimeEntryRequest
    )
    {
        var newAwayWithMakeUpTimeEntryId = await _createAwayWithMakeUpTimeEntryCommand.ExecuteAsync(createAwayWithMakeUpTimeEntryRequest);

        return new CreateAwayWithMakeUpTimeEntryResponse
        {
            NewAwayWithMakeUpTimeEntryId = newAwayWithMakeUpTimeEntryId
        };
    }
}
