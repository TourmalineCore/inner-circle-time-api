using Core.Entities;

namespace Application.Features.Tracking.GetEntriesByPeriod;

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

        var awayWithMakeUpTimeEntries = entriesByPeriod
            .OfType<AwayWithMakeUpTimeEntry>()
            .Select(
                x => new AwayWithMakeUpTimeEntryDto
                {
                    Id = x.Id,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    Type = x.Type,
                    Description = x.Description,
                    MakeUpTimeList = x.MakeUpTimeList
                        .Select(x => new MakeUpTimeEntryDto
                        {
                            Id = x.Id,
                            StartTime = x.StartTime,
                            EndTime = x.EndTime
                        })
                        .ToList()
                })
                .ToList();

        var makeUpTimeEntries = entriesByPeriod
           .OfType<MakeUpTimeEntry>()
           .Select(
               x => new MakeUpTimeEntryWithRelatedEntryDto
               {
                   RelatedEntryId = x.RelatedEntryId,
                   RelatedEntryType = x.RelatedEntryType,
                   StartTime = x.StartTime,
                   EndTime = x.EndTime,
                   Type = x.Type,
               })
               .ToList();

        return new GetEntriesByPeriodResponse
        {
            TaskEntries = taskEntries,
            UnwellEntries = unwellEntries,
            AwayWithMakeUpTimeEntries = awayWithMakeUpTimeEntries,
            MakeUpTimeEntries = makeUpTimeEntries
        };
    }
}
