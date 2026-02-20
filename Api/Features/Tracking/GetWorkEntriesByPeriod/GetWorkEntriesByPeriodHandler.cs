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
        var workEntriesByPeriod = await _getWorkEntriesByPeriodQuery.GetByPeriodAsync<TrackedEntryBase>(
            startDate,
            endDate
        );

        var workEntries = workEntriesByPeriod
            .OfType<TaskEntry>()
            .Select(
                x => new WorkEntryDto
                {
                    Id = x.Id,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    Type = x.Type,
                    Title = x.Title,
                    ProjectId = x.ProjectId,
                    TaskId = x.TaskId,
                    Description = x.Description
                })
                .ToList();

        var unwellEntries = workEntriesByPeriod
            .OfType<UnwellEntry>()
            .Select(
                x => new UnwellEntryDto
                {
                    Id = x.Id,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    Type = x.Type,
                })
                .ToList();

        return new GetWorkEntriesByPeriodResponse
        {
            WorkEntries = workEntries,
            UnwellEntries = unwellEntries
        };
    }
}
