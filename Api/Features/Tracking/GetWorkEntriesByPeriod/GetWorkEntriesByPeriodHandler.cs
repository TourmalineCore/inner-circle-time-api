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
        DateOnly startDate,
        DateOnly endDate
    )
    {
        var workEntriesByPeriod = await _getWorkEntriesByPeriodQuery.GetByPeriodAsync(
            startDate,
            endDate
        );

        return new GetWorkEntriesByPeriodResponse
        {
            WorkEntries = workEntriesByPeriod
                .Select(workEntry =>
                    new WorkEntryDto
                    {
                        Id = workEntry.Id,
                        Title = workEntry.Title,
                        StartTime = workEntry.StartTime,
                        EndTime = workEntry.EndTime,
                        ProjectId = workEntry.ProjectId,
                        TaskId = workEntry.TaskId,
                        Description = workEntry.Description,
                    }
                )
                .ToList()
        };
    }
}
