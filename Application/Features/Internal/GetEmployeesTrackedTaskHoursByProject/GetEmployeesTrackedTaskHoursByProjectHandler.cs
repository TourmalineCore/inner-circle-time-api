using Core;
using Core.Entities;

namespace Application.Features.Internal.GetEmployeesTrackedTaskHoursByProject;

public class GetEmployeesTrackedTaskHoursByProjectHandler
{
    private readonly GetEmployeesTrackedTaskHoursByProjectQuery _getEmployeesTrackedTaskHoursByProjectQuery;

    public GetEmployeesTrackedTaskHoursByProjectHandler(
        GetEmployeesTrackedTaskHoursByProjectQuery getEmployeesTrackedTaskHoursByProjectQuery
    )
    {
        _getEmployeesTrackedTaskHoursByProjectQuery = getEmployeesTrackedTaskHoursByProjectQuery;
    }

    public async Task<GetEmployeesTrackedTaskHoursByProjectResponse> HandleAsync(
        long projectId,
        DateOnly startDate,
        DateOnly endDate
    )
    {
        var employeesEntriesByProjectAndPeriod = await _getEmployeesTrackedTaskHoursByProjectQuery.GetByProjectAndPeriodAsync<TaskEntry>(
            projectId,
            startDate,
            endDate
        );

        var calculatedEmployeesTotalTrackedTaskHours = CalculateTotalTrackedTaskHours.Calculate(employeesEntriesByProjectAndPeriod);

        var employeesEntries = calculatedEmployeesTotalTrackedTaskHours
            .OfType<EmployeeTrackedTaskHours>()
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
