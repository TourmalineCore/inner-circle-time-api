using Application.Queries;

namespace Api.Features.Tracking.GetWorkEntriesByPeriod;

public class GetWorkEntriesByPeriodHandler
{
    private readonly GetWorkEntriesByPeriodQuery _getWorkEntriesByPeriodQuery;

    public GetWorkEntriesByPeriodHandler(
        GetWorkEntriesByPeriodQuery getWorkEntriesByPeriodQuery
    )
    {
        _getWorkEntriesByPeriodQuery = getWorkEntriesByPeriodQuery;
    }

    public async Task<GetWorkEntriesByPeriodResponse> HandleAsync(
        DateTime startTime,
        DateTime endTime
    )
    {
        var workEntriesByPeriod = await _getWorkEntriesByPeriodQuery.GetByPeriodAsync(
            startTime,
            endTime
        );

        return new GetWorkEntriesByPeriodResponse
        {
            WorkEntries = workEntriesByPeriod
                .Select(workEntry =>
                    new WorkEntryItem
                    {
                        Id = workEntry.Id,
                        Title = workEntry.Title,
                        StartTime = workEntry.StartTime,
                        EndTime = workEntry.EndTime,
                        TaskId = workEntry.TaskId,
                    }
                )
                .ToList()
        };
    }
}
