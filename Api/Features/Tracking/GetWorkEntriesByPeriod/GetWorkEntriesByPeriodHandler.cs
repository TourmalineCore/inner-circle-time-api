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
        string startTime,
        string endTime
    )
    {
        var workEntriesByPeriod = await _getWorkEntriesByPeriodQuery.GetByPeriodAsync(
            DateTime.Parse(startTime),
            DateTime.Parse(endTime)
        );

        return new GetWorkEntriesByPeriodResponse
        {
            WorkEntries = workEntriesByPeriod
                .Select(workEntry =>
                {
                    return new WorkEntryItem
                    {
                        Id = workEntry.Id,
                        Title = workEntry.Title,
                        StartTime = workEntry.StartTime,
                        EndTime = workEntry.EndTime,
                        TaskId = workEntry.TaskId,
                    };
                })
                .ToList()
        };
    }
}
