namespace Application.Features.Tracking.CreateSickLeaveEntry;

public class CreateSickLeaveEntryHandler
{
    private readonly CreateSickLeaveEntryCommand _createSickLeaveCommand;

    public CreateSickLeaveEntryHandler(
        CreateSickLeaveEntryCommand createSickLeaveCommand
    )
    {
        _createSickLeaveCommand = createSickLeaveCommand;
    }

    public async Task<CreateSickLeaveEntryResponse> HandleAsync(
        CreateSickLeaveEntryRequest createSickLeaveEntryRequest
    )
    {
        var newSickLeaveEntryId = await _createSickLeaveCommand.ExecuteAsync(createSickLeaveEntryRequest);

        return new CreateSickLeaveEntryResponse
        {
            NewSickLeaveEntryId = newSickLeaveEntryId
        };
    }
}
