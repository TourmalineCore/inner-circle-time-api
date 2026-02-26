using Application.Commands;
namespace Api.Features.Tracking.SoftDeleteEntry;

public class SoftEntryDeleteHandler
{
    private readonly SoftDeleteEntryCommand _softDeleteEntryCommand;


    public SoftEntryDeleteHandler(
        SoftDeleteEntryCommand softDeleteEntityCommand
    )
    {
        _softDeleteEntryCommand = softDeleteEntityCommand;
    }

    public async Task<object> HandleAsync(long entryId)
    {
        return new
        {
            isDeleted = await _softDeleteEntryCommand.ExecuteAsync(entryId)
        };
    }
}
