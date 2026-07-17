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
                // DB stores end_time as the start of the next day (see ADR #008)
                // Subtract 1 day when displaying to show the correct end date on UI
                EndDate = DateOnly.FromDateTime(sickLeave.EndTime.AddDays(-1)),
            },
            EntryType = sickLeave.Type,
        };
    }
}
