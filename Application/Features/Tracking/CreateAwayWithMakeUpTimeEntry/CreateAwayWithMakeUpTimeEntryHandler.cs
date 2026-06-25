using Application.Validators;

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
        var isTimeConvering = MakeUpTimeValidator.DoesMakeUpTotalTimeMatchWithRelatedEntryPeriod(
            createAwayWithMakeUpTimeEntryRequest.StartTime,
            createAwayWithMakeUpTimeEntryRequest.EndTime,
            createAwayWithMakeUpTimeEntryRequest.MakeUpTimeList
        );

        if (!isTimeConvering)
        {
            throw new ArgumentException("Total make-up time must equal your away time. Please check and adjust your entries.");
        }

        var newAwayWithMakeUpTimeEntryId = await _createAwayWithMakeUpTimeEntryCommand.ExecuteAsync(createAwayWithMakeUpTimeEntryRequest);

        return new CreateAwayWithMakeUpTimeEntryResponse
        {
            NewAwayWithMakeUpTimeEntryId = newAwayWithMakeUpTimeEntryId
        };
    }
}
