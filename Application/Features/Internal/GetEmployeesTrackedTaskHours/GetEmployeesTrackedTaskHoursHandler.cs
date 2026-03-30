using Core;

namespace Application.Features.Internal.GetEmployeesTrackedTaskHours;

public class GetEmployeesTrackedTaskHoursHandler
{
    private readonly GetTaskEntriesQuery _getTaskEntriesQuery;

    public GetEmployeesTrackedTaskHoursHandler(
        GetTaskEntriesQuery getTaskEntriesQuery
    )
    {
        _getTaskEntriesQuery = getTaskEntriesQuery;
    }

    public async Task<GetEmployeesTrackedTaskHoursResponse> HandleAsync(
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

        var calculatedEmployeesTotalTrackedTaskHours = TotalTrackedTaskHoursCalculator.Calculate(taskEntries);

        var employeesEntries = calculatedEmployeesTotalTrackedTaskHours
            .Select(
                x => new EmployeeTrackedTaskHourDto
                {
                    EmployeeId = x.EmployeeId,
                    TrackedHours = x.TrackedHours,
                })
                .ToList();

        return new GetEmployeesTrackedTaskHoursResponse
        {
            EmployeesTrackedTaskHours = employeesEntries
        };
    }
}
