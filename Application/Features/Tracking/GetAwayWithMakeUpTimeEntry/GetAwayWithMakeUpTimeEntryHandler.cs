using Application.SharedQueries;

namespace Application.Features.Tracking.GetAwayWithMakeUpTimeEntry;

public class GetAwayWithMakeUpTimeEntryHandler
{
    private readonly IGetEntryByIdQuery _getEntryByIdQuery;

    public GetAwayWithMakeUpTimeEntryHandler(
        IGetEntryByIdQuery getEntryByIdQuery
    )
    {
        _getEntryByIdQuery = getEntryByIdQuery;
    }

    public async Task<GetAwayWithMakeUpTimeEntryResponse> HandleAsync(long awayWithMakeUpTimeEntryId)
    {
        var awayWithMakeUpTimeEntry = await _getEntryByIdQuery.GetAsync<AwayWithMakeUpTimeEntry>(awayWithMakeUpTimeEntryId);

        return new GetAwayWithMakeUpTimeEntryResponse
        {
            Id = awayWithMakeUpTimeEntry.Id,
            StartTime = awayWithMakeUpTimeEntry.StartTime,
            EndTime = awayWithMakeUpTimeEntry.EndTime,
            Type = awayWithMakeUpTimeEntry.Type,
            Description = awayWithMakeUpTimeEntry.Description,
            MakeUpTimeList = awayWithMakeUpTimeEntry.MakeUpTimeList
                .Select(x => new MakeUpTimeEntryDto
                {
                    Id = x.Id,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime
                })
                .ToList()
        };
    }
}

