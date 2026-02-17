using Application.Queries;
using Core.Entities;

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
        var workEntriesByPeriod = await _getWorkEntriesByPeriodQuery.GetByPeriodAsync<TaskEntry>(
            startDate,
            endDate
        );

        var unwellEntriesByPeriod = await _getWorkEntriesByPeriodQuery.GetByPeriodAsync<UnwellEntry>(
            startDate,
            endDate
        );

        var workEntries = workEntriesByPeriod
            .Select(
                x => new TrackingEntryDto
                {
                    Id = x.Id,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    Type = x.Type,

                    Title = x.Title,
                    ProjectId = x.ProjectId,
                    TaskId = x.TaskId,
                    Description = x.Description
                });

        var unwellEntries = unwellEntriesByPeriod
            .Select(
                x => new TrackingEntryDto
                {
                    Id = x.Id,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    Type = x.Type,
                    Title = null,
                    ProjectId = null,
                    TaskId = null,
                    Description = null
                });

        return new GetWorkEntriesByPeriodResponse
        {
            WorkEntries = workEntries
                .Concat(unwellEntries)
                .ToList()
        };
    }
}
