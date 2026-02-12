using Application.Commands;

namespace Api.Features.Tracking.CreateAdjustment;

public class CreateAdjustmentHandler
{
    private readonly CreateAdjustmentCommand _createAdjustmentCommand;

    public CreateAdjustmentHandler(
        CreateAdjustmentCommand createAdjustmentCommand
    )
    {
        _createAdjustmentCommand = createAdjustmentCommand;
    }

    public async Task<CreateAdjustmentResponse> HandleAsync(
        CreateAdjustmentRequest createAdjustmentRequest
    )
    {
        var createAdjustmentCommandParams = new CreateAdjustmentCommandParams
        {
            Type = createAdjustmentRequest.Type,
            StartTime = createAdjustmentRequest.StartTime,
            EndTime = createAdjustmentRequest.EndTime,
        };

        var newAdjustmentId = await _createAdjustmentCommand.ExecuteAsync(createAdjustmentCommandParams);

        return new CreateAdjustmentResponse
        {
            NewAdjustmentId = newAdjustmentId
        };
    }
}
