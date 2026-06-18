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
        updateAwayWithMakeUpTimeEntryRequest.Id = awayWithMakeUpTimeEntryId;

        await _updateAwayWithMakeUpTimeEntryCommand.ExecuteAsync(updateAwayWithMakeUpTimeEntryRequest);
    }
}
