namespace Application.Features.Tracking.Handlers.UpdateSickLeaveEntry;

public class UpdateSickLeaveEntryHandler
{
    private readonly UpdateSickLeaveEntryCommand _updateSickLeaveEntryCommand;

    public UpdateSickLeaveEntryHandler(
        UpdateSickLeaveEntryCommand updateSickLeaveEntryCommand
    )
    {
        _updateSickLeaveEntryCommand = updateSickLeaveEntryCommand;
    }

    public async Task HandleAsync(
        long sickLeaveEntryId,
        UpdateSickLeaveEntryRequest updateSickLeaveEntryRequest
    )
    {
        updateSickLeaveEntryRequest.Id = sickLeaveEntryId;

        await _updateSickLeaveEntryCommand.ExecuteAsync(updateSickLeaveEntryRequest);
    }
}
