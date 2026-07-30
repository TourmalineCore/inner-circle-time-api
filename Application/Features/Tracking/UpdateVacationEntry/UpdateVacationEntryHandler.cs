namespace Application.Features.Tracking.UpdateVacationEntry;

public class UpdateVacationEntryHandler
{
    private readonly UpdateVacationEntryCommand _updateVacationEntryCommand;

    public UpdateVacationEntryHandler(
        UpdateVacationEntryCommand updateVacationEntryCommand
    )
    {
        _updateVacationEntryCommand = updateVacationEntryCommand;
    }

    public async Task HandleAsync(
        long vacationEntryId,
        UpdateVacationEntryRequest updateVacationEntryRequest
    )
    {
        updateVacationEntryRequest.Id = vacationEntryId;

        await _updateVacationEntryCommand.ExecuteAsync(updateVacationEntryRequest);
    }
}
