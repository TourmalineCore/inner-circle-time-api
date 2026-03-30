using Core;
using Core.Entities;

namespace Application.Features.Internal.GetEmployeesTrackedTaskHoursByProject;

public class GetEmployeesTrackedTaskHoursByProjectHandler
{
    private readonly GetTaskEntriesQuery _getTaskEntriesQuery;

    public GetEmployeesTrackedTaskHoursByProjectHandler(
        GetTaskEntriesQuery getTaskEntriesQuery
    )
    {
        _getTaskEntriesQuery = getTaskEntriesQuery;
    }

    public async Task<GetEmployeesTrackedTaskHoursByProjectResponse> HandleAsync(
        long projectId,
        DateOnly startDate,
        DateOnly endDate
    )
    {
        var taskEntries = await _getTaskEntriesQuery.GetAsync(
            projectId,
            startDate,
            endDate
        );

        var calculatedEmployeesTotalTrackedTaskHours = CalculateTotalTrackedTaskHours.Calculate(taskEntries);

        var employeesEntries = calculatedEmployeesTotalTrackedTaskHours
            .Select(
                x => new EmployeeTrackedTaskHours
                {
                    EmployeeId = x.EmployeeId,
                    TrackedHours = x.TrackedHours,
                })
                .ToList();

        return new GetEmployeesTrackedTaskHoursByProjectResponse
        {
            EmployeesTrackedTaskHours = employeesEntries
        };
    }
}
