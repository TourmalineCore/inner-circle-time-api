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
        var isTimeConvering = MakeUpTimeValidator.IsMakeUpTotalTimeConvergingWithPeriod(
            createAwayWithMakeUpTimeEntryRequest.StartTime,
            createAwayWithMakeUpTimeEntryRequest.EndTime,
            createAwayWithMakeUpTimeEntryRequest.MakeUpTimeList
        );

        if (!isTimeConvering)
        {
            throw new ArgumentException("The time doesn't match, please update your make-up or away time.");
        }

        var newAwayWithMakeUpTimeEntryId = await _createAwayWithMakeUpTimeEntryCommand.ExecuteAsync(createAwayWithMakeUpTimeEntryRequest);

        return new CreateAwayWithMakeUpTimeEntryResponse
        {
            NewAwayWithMakeUpTimeEntryId = newAwayWithMakeUpTimeEntryId
        };
    }
}
