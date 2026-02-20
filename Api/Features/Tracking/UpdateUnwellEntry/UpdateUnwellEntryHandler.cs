using Application.Commands;

namespace Api.Features.Tracking.UpdateUnwellEntry;

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
        UpdateUnwellEntryRequest updateUnwellEntryRequest
    )
    {
        var updateUnwellEntryCommandParams = new UpdateUnwellEntryCommandParams
        {
            Id = unwellEntryId,
            StartTime = updateUnwellEntryRequest.StartTime,
            EndTime = updateUnwellEntryRequest.EndTime,
        };

        await _updateUnwellEntryCommand.ExecuteAsync(updateUnwellEntryCommandParams);
    }
}
