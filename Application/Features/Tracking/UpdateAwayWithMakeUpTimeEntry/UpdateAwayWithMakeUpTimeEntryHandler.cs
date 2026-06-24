using Application.Validators;

namespace Application.Features.Tracking.UpdateAwayWithMakeUpTimeEntry;

public class UpdateAwayWithMakeUpTimeEntryHandler
{
    private readonly UpdateAwayWithMakeUpTimeEntryCommand _updateAwayWithMakeUpTimeEntryCommand;

    public UpdateAwayWithMakeUpTimeEntryHandler(
        UpdateAwayWithMakeUpTimeEntryCommand updateAwayWithMakeUpTimeEntryCommand
    )
    {
        _updateAwayWithMakeUpTimeEntryCommand = updateAwayWithMakeUpTimeEntryCommand;
    }

    public async Task HandleAsync(
        long awayWithMakeUpTimeEntryId,
        UpdateAwayWithMakeUpTimeEntryRequest updateAwayWithMakeUpTimeEntryRequest
    )
    {
        var isTimeConvering = MakeUpTimeValidator.IsMakeUpTotalTimeConvergingWithPeriod(
            updateAwayWithMakeUpTimeEntryRequest.StartTime,
            updateAwayWithMakeUpTimeEntryRequest.EndTime,
            updateAwayWithMakeUpTimeEntryRequest.MakeUpTimeList
        );

        if (!isTimeConvering)
        {
            throw new ArgumentException("Total make-up time must equal your away time. Please check and adjust your entries.");
        }

        updateAwayWithMakeUpTimeEntryRequest.Id = awayWithMakeUpTimeEntryId;

        await _updateAwayWithMakeUpTimeEntryCommand.ExecuteAsync(updateAwayWithMakeUpTimeEntryRequest);
    }
}
