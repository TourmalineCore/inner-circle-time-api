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
            throw new ArgumentException("The time doesn't match, please update your make-up or away time.");
        }

        updateAwayWithMakeUpTimeEntryRequest.Id = awayWithMakeUpTimeEntryId;

        await _updateAwayWithMakeUpTimeEntryCommand.ExecuteAsync(updateAwayWithMakeUpTimeEntryRequest);
    }
}
