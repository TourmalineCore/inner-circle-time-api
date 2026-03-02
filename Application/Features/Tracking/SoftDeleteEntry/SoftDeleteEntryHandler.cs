namespace Application.Features.Tracking.SoftDeleteEntry;

public class SoftDeleteEntryHandler
{
    private readonly SoftDeleteEntryCommand _softDeleteEntryCommand;

    public SoftDeleteEntryHandler(
        SoftDeleteEntryCommand softDeleteEntryCommand
    )
    {
        _softDeleteEntryCommand = softDeleteEntryCommand;
    }

    public async Task<object> HandleAsync(
        long entryId,
        SoftDeleteEntryRequest softDeleteEntryRequest
    )
    {
        softDeleteEntryRequest.Id = entryId;

        return new
        {
            isDeleted = await _softDeleteEntryCommand.ExecuteAsync(softDeleteEntryRequest)
        };
    }
}
