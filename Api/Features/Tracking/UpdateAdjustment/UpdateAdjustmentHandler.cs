using Application.Commands;

namespace Api.Features.Tracking.UpdateAdjustment;

public class UpdateAdjustmentHandler
{
    private readonly UpdateAdjustmentCommand _updateAdjustmentCommand;

    public UpdateAdjustmentHandler(
        UpdateAdjustmentCommand updateAdjustmentCommand
    )
    {
        _updateAdjustmentCommand = updateAdjustmentCommand;
    }

    public async Task HandleAsync(
        long adjustmentId,
        UpdateAdjustmentRequest updateEntryRequest
    )
    {
        var updateAdjustmentCommandParams = new UpdateAdjustmentCommandParams
        {
            Id = adjustmentId,
            Type = updateEntryRequest.Type,
            StartTime = updateEntryRequest.StartTime,
            EndTime = updateEntryRequest.EndTime,
        };

        await _updateAdjustmentCommand.ExecuteAsync(updateAdjustmentCommandParams);
    }
}
