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
        var sickLeaveEntry = await _getEntryByIdQuery.GetAsync<SickLeaveEntry>(sickLeaveEntryId);

        return new GetSickLeaveEntryResponse
        {
            Id = sickLeaveEntry.Id,
            Period = new PeriodDto
            {
                StartDate = DateOnly.FromDateTime(sickLeaveEntry.StartTime),
                // DB stores end_time as the start of the next day (see ADR #008 - https://github.com/TourmalineCore/inner-circle-documentation/blob/master/time-tracker/adrs/008-sick-leave-and-vacation-storage.md)
                // Subtract 1 day when displaying to show the correct end date on UI
                EndDate = DateOnly.FromDateTime(sickLeaveEntry.EndTime.AddDays(-1)),
            },
            EntryType = sickLeaveEntry.Type,
        };
    }
}
