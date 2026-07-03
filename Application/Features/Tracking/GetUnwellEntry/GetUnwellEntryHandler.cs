using Application.SharedQueries;
using Core.Entities;

namespace Application.Features.Tracking.GetUnwellEntry;

public class GetUnwellEntryHandler
{
    private readonly IGetEntryByIdQuery _getEntryByIdQuery;

    public GetUnwellEntryHandler(
        IGetEntryByIdQuery getEntryByIdQuery
    )
    {
        _getEntryByIdQuery = getEntryByIdQuery;
    }

    public async Task<GetUnwellEntryResponse> HandleAsync(long unwellEntryId)
    {
        var unwellEntry = await _getEntryByIdQuery.GetAsync<UnwellEntry>(unwellEntryId);

        return new GetUnwellEntryResponse
        {
            Id = unwellEntry.Id,
            StartTime = unwellEntry.StartTime,
            EndTime = unwellEntry.EndTime,
            Type = unwellEntry.EntryType,
            EntryType = unwellEntry.EntryType,
        };
    }
}
