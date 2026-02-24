using Application.Queries;
using Core.Entities;

namespace Api.Features.Tracking.GetEntriesByPeriod;

public class GetEntriesByPeriodHandler
{
    private readonly GetEntriesByPeriodQuery _getEntriesByPeriodQuery;

    public GetEntriesByPeriodHandler(
        GetEntriesByPeriodQuery getEntriesByPeriodQuery
    )
    {
        _getEntriesByPeriodQuery = getEntriesByPeriodQuery;
    }

    public async Task<GetEntriesByPeriodResponse> HandleAsync(
        DateOnly startDate,
        DateOnly endDate
    )
    {
        var entriesByPeriod = await _getEntriesByPeriodQuery.GetByPeriodAsync<TrackedEntryBase>(
            startDate,
            endDate
        );

        var taskEntries = entriesByPeriod
            .OfType<TaskEntry>()
            .Select(
                x => new TaskEntryDto
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

        var unwellEntries = entriesByPeriod
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

        return new GetEntriesByPeriodResponse
        {
            WorkEntries = taskEntries,
            TaskEntries = taskEntries,
            UnwellEntries = unwellEntries
        };
    }
}
