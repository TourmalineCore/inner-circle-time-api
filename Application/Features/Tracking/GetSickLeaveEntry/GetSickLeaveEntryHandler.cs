using Application.SharedMappers;
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
            Period = PeriodMapper.ToPeriodDto(
                sickLeaveEntry.StartTime,
                sickLeaveEntry.EndTime
            ),
            EntryType = sickLeaveEntry.Type,
        };
    }
}
