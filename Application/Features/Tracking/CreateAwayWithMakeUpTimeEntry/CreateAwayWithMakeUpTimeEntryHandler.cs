using Application.Validators;
using Core;

namespace Application.Features.Tracking.CreateAwayWithMakeUpTimeEntry;

[UnitTest]
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
        var doesTimeMatch = MakeUpTimeValidator.DoesMakeUpTotalTimeMatchWithRelatedEntryPeriod(
            createAwayWithMakeUpTimeEntryRequest.StartTime,
            createAwayWithMakeUpTimeEntryRequest.EndTime,
            createAwayWithMakeUpTimeEntryRequest.MakeUpTimeList
        );

        if (!doesTimeMatch)
        {
            throw new TimeDoesNotMatchException("Total make-up time must equal your away time. Please check and adjust your entries.");
        }

        var newAwayWithMakeUpTimeEntryId = await _createAwayWithMakeUpTimeEntryCommand.ExecuteAsync(createAwayWithMakeUpTimeEntryRequest);

        return new CreateAwayWithMakeUpTimeEntryResponse
        {
            NewAwayWithMakeUpTimeEntryId = newAwayWithMakeUpTimeEntryId
        };
    }
}
