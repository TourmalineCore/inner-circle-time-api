using Application.SharedMappers;
using Application.SharedQueries;
using Core.Entities;

namespace Application.Features.Tracking.GetVacationEntry;

public class GetVacationEntryHandler
{
    private readonly IGetEntryByIdQuery _getEntryByIdQuery;

    public GetVacationEntryHandler(
        IGetEntryByIdQuery getEntryByIdQuery
    )
    {
        _getEntryByIdQuery = getEntryByIdQuery;
    }

    public async Task<GetVacationEntryResponse> HandleAsync(long vacationEntryId)
    {
        var vacationEntry = await _getEntryByIdQuery.GetAsync<VacationEntry>(vacationEntryId);

        return new GetVacationEntryResponse
        {
            Id = vacationEntry.Id,
            Period = PeriodMapper.MapToPeriodDto(
                vacationEntry.StartTime,
                vacationEntry.EndTime
            ),
            EntryType = vacationEntry.Type,
            IsUnpaid = vacationEntry.IsUnpaid
        };
    }
}
