using Application.Commands;
namespace Api.Features.Tracking.SoftDeleteEntry;

public class SoftDeleteEntryHandler
{
    private readonly SoftDeleteEntryCommand _softDeleteEntryCommand;

    public SoftDeleteEntryHandler(
        SoftDeleteEntryCommand softDeleteEntityCommand
    )
    {
        _softDeleteEntryCommand = softDeleteEntityCommand;
    }

    public async Task<object> HandleAsync(
        long entryId,
        SoftDeleteEntryRequest softDeleteEntryRequest
    )
    {
        var softDeleteEntryCommandParams = new SoftDeleteEntryCommandParams
        {
            DeletionReason = softDeleteEntryRequest.DeletionReason,
        };

        return new
        {
            isDeleted = await _softDeleteEntryCommand.ExecuteAsync(entryId, softDeleteEntryCommandParams)
        };
    }
}
