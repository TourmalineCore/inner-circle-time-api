using Application.SharedQueries;
using Core.Entities;

namespace Application.Features.Tracking.GetSickLeaveEntry;

public class GetSickLeaveEntryHandler
{
    private readonly IGetEntryByIdQuery _getEntryByIdQuery;

    public GetSickLeaveEntryHandler(
        IGetEntryByIdQuery getEntryByIdQuery
    )
    {
        _getEntryByIdQuery = getEntryByIdQuery;
    }

    public async Task<GetSickLeaveEntryResponse> HandleAsync(long sickLeaveEntryId)
    {
        var sickLeave = await _getEntryByIdQuery.GetAsync<SickLeaveEntry>(sickLeaveEntryId);

        return new GetSickLeaveEntryResponse
        {
            Id = sickLeave.Id,
            Period = new PeriodDto
            {
                StartDate = DateOnly.FromDateTime(sickLeave.StartTime),
                EndDate = DateOnly.FromDateTime(sickLeave.EndTime),
            },
            EntryType = sickLeave.Type,
        };
    }
}
