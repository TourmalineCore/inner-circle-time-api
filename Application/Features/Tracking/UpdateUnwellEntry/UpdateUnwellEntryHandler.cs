namespace Application.Features.Tracking.UpdateUnwellEntry;

public class UpdateUnwellEntryHandler
{
    private readonly UpdateUnwellEntryCommand _updateUnwellEntryCommand;

    public UpdateUnwellEntryHandler(
        UpdateUnwellEntryCommand updateUnwellEntryCommand
    )
    {
        _updateUnwellEntryCommand = updateUnwellEntryCommand;
    }

    public async Task HandleAsync(
        long unwellEntryId,
        UpdateUnwellEntryRequest updateUnwellEntryRequest)
    {
        updateUnwellEntryRequest.Id = unwellEntryId;

        await _updateUnwellEntryCommand.ExecuteAsync(updateUnwellEntryRequest);
    }
}
