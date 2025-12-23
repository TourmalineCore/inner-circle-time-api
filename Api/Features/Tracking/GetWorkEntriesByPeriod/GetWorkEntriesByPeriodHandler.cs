using Application.ExternalDeps.AssignmentsApi;
using Application.Queries;

namespace Api.Features.Tracking.GetWorkEntriesByPeriod;

public class GetWorkEntriesByPeriodHandler
{
    private readonly GetWorkEntriesByPeriodQuery _getWorkEntriesByPeriodQuery;

    private readonly IAssignmentsApi _assignmentsApi;

    public GetWorkEntriesByPeriodHandler(
        GetWorkEntriesByPeriodQuery getWorkEntriesByPeriodQuery,
        IAssignmentsApi assignmentsApi
    )
    {
        _getWorkEntriesByPeriodQuery = getWorkEntriesByPeriodQuery;
        _assignmentsApi = assignmentsApi;
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
                {
                    var project = _assignmentsApi.FindEmployeeProjectAsync(workEntry.ProjectId);

                    return new WorkEntryItem
                    {
                        Id = workEntry.Id,
                        Title = workEntry.Title,
                        StartTime = workEntry.StartTime,
                        EndTime = workEntry.EndTime,
                        ProjectName = project!.Name,
                        TaskId = workEntry.TaskId,
                        Description = workEntry.Description,
                    };
                })
                .ToList()
        };
    }
}
